// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using AlibabaCloud.SDK.Dybaseapi20170525;
using AlibabaCloud.SDK.Dybaseapi20170525.Models;
using Aliyun.MNS;
using Aliyun.MNS.Model;

namespace Masa.Mc.Service.Admin.Application.MessageReceipts;

public class AliyunMnsReceiptConsumer : BackgroundService
{
    private readonly IChannelRepository _channelRepository;
    private readonly ILogger<AliyunMnsReceiptConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const int SleepTimeMs = 50;
    private const int NoChannelWaitSeconds = 60;
    private const int ErrorRetrySeconds = 5;
    private const int TokenBufferSeconds = 120; // 过期时间小于2分钟则重新获取，防止服务器时间误差
    private const int BatchSize = 16;
    private const string MnsAccountEndpoint = "https://1943695596114318.mns.cn-hangzhou.aliyuncs.com/";
    private const string MessageType = "SmsReport";
    private readonly object _lockObject = new();
    
    private readonly Dictionary<string, QueryTokenForMnsQueueResponseBody.QueryTokenForMnsQueueResponseBodyMessageTokenDTO> _tokenMap = new();
    private readonly Dictionary<string, Queue> _queueMap = new();
    private readonly Dictionary<string, (string AccessKeyId, string AccessKeySecret)> _credentialsMap = new();

    public AliyunMnsReceiptConsumer(
        IChannelRepository channelRepository,
        ILogger<AliyunMnsReceiptConsumer> logger,
        IServiceProvider serviceProvider)
    {
        _channelRepository = channelRepository;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var channels = await GetEnabledMnsChannelsAsync(stoppingToken);
                
                if (channels.Count == 0)
                {
                    _logger.LogDebug("No enabled MNS channels found. Waiting {Seconds} seconds...", NoChannelWaitSeconds);
                    await Task.Delay(TimeSpan.FromSeconds(NoChannelWaitSeconds), stoppingToken);
                    continue;
                }

                var queueGroups = channels
                    .Where(c => !string.IsNullOrEmpty(c.GetDataValue<string>(nameof(AliyunSmsOptions.MnsQueueName))))
                    .GroupBy(c => c.GetDataValue<string>(nameof(AliyunSmsOptions.MnsQueueName))!)
                    .ToList();

                foreach (var group in queueGroups)
                {
                    await ProcessMessagesAsync(group.First(), group.Key, stoppingToken);
                }

                await Task.Delay(SleepTimeMs, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AliyunMnsReceiptConsumer");
                await Task.Delay(TimeSpan.FromSeconds(ErrorRetrySeconds), stoppingToken);
            }
        }
    }

    private async Task<List<Channel>> GetEnabledMnsChannelsAsync(CancellationToken cancellationToken)
    {
        var channels = await _channelRepository.AsNoTracking()
            .Where(x => x.Type == ChannelType.Sms && x.Provider == (int)SmsProviders.Aliyun)
            .ToListAsync(cancellationToken);

        return channels
            .Where(c => c.GetDataValue<bool?>(nameof(AliyunSmsOptions.EnableMnsConsumer)) == true 
                     && !string.IsNullOrEmpty(c.GetDataValue<string>(nameof(AliyunSmsOptions.MnsQueueName))))
            .ToList();
    }

    private async Task ProcessMessagesAsync(Channel channel, string queueName, CancellationToken cancellationToken)
    {
        QueryTokenForMnsQueueResponseBody.QueryTokenForMnsQueueResponseBodyMessageTokenDTO? token = null;
        Queue? queue = null;
        string? accessKeyId = null;
        string? accessKeySecret = null;

        lock (_lockObject)
        {
            // 获取凭证
            if (!_credentialsMap.ContainsKey(queueName))
            {
                accessKeyId = channel.GetDataValue<string>(nameof(AliyunSmsOptions.AccessKeyId));
                accessKeySecret = channel.GetDataValue<string>(nameof(AliyunSmsOptions.AccessKeySecret));
                
                if (string.IsNullOrEmpty(accessKeyId) || string.IsNullOrEmpty(accessKeySecret))
                {
                    _logger.LogWarning("Channel {ChannelId} missing AccessKeyId or AccessKeySecret for queue {QueueName}", channel.Id, queueName);
                    return;
                }
                _credentialsMap[queueName] = (accessKeyId, accessKeySecret);
            }
            else
            {
                var creds = _credentialsMap[queueName];
                accessKeyId = creds.AccessKeyId;
                accessKeySecret = creds.AccessKeySecret;
            }

            // 获取Token
            if (_tokenMap.ContainsKey(queueName))
            {
                token = _tokenMap[queueName];
            }

            // 获取Queue
            if (_queueMap.ContainsKey(queueName))
            {
                queue = _queueMap[queueName];
            }

            TimeSpan ts = new TimeSpan(0);

            if (token != null)
            {
                DateTime expireTime = Convert.ToDateTime(token.ExpireTime);
                DateTime now = DateTime.Now;
                ts = expireTime - now;
            }

            // 如果Token过期或Queue不存在，重新获取
            if (token == null || ts.TotalSeconds < TokenBufferSeconds || queue == null)
            {
                token = GetTokenByMessageType(accessKeyId, accessKeySecret, MessageType, queueName);

                // 创建MNS客户端并获取队列
                var client = new MNSClient(token.AccessKeyId, token.AccessKeySecret, MnsAccountEndpoint, token.SecurityToken);
                queue = client.GetNativeQueue(queueName);
                
                if (_tokenMap.ContainsKey(queueName))
                {
                    _tokenMap.Remove(queueName);
                }
                if (_queueMap.ContainsKey(queueName))
                {
                    _queueMap.Remove(queueName);
                }
                _tokenMap[queueName] = token;
                if (queue != null)
                {
                    _queueMap[queueName] = queue;
                }
            }
        }

        try
        {
            if (queue == null)
            {
                _logger.LogError("Queue is null for queue name: {QueueName}", queueName);
                return;
            }

            // 批量接收消息
            var batchReceiveMessageResponse = queue.BatchReceiveMessage(BatchSize);
            var messages = batchReceiveMessageResponse.Messages;

            for (int i = 0; i < messages.Count; i++)
            {
                try
                {
                    var message = messages[i];
                    if (message == null) continue;

                    var body = message.Body;
                    var receiptHandle = message.ReceiptHandle;

                    if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(receiptHandle))
                    {
                        continue;
                    }

                    byte[] outputb = Convert.FromBase64String(body);
                    string orgStr = Encoding.UTF8.GetString(outputb);
                    _logger.LogDebug("Received MNS message from queue {QueueName}: {Message}", queueName, orgStr);

                    // 解析并处理回执消息
                    await ProcessReceiptMessageAsync(orgStr, cancellationToken);

                    // 消费成功的前提下删除消息
                    queue.DeleteMessage(receiptHandle);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing MNS message at index {Index} for queue {QueueName}", i, queueName);
                }
            }
        }
        catch (MessageNotExistException ex)
        {
            // 队列不存在或消息不存在，清理缓存以便下次重新获取
            _logger.LogWarning(ex, "Queue or message not exist for queue {QueueName}, clearing cache", queueName);
            lock (_lockObject)
            {
                _tokenMap.Remove(queueName);
                _queueMap.Remove(queueName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error receiving messages from MNS queue: {QueueName}", queueName);
        }
    }

    private QueryTokenForMnsQueueResponseBody.QueryTokenForMnsQueueResponseBodyMessageTokenDTO GetTokenByMessageType(
        string accessKeyId, string accessKeySecret, string messageType, string queueName)
    {
        var config = new AlibabaCloud.OpenApiClient.Models.Config
        {
            AccessKeyId = accessKeyId,
            AccessKeySecret = accessKeySecret,
            Endpoint = "dybaseapi.aliyuncs.com"
        };
        var client = new AlibabaCloud.SDK.Dybaseapi20170525.Client(config);

        var request = new QueryTokenForMnsQueueRequest
        {
            MessageType = messageType,
            QueueName = queueName
        };
        var response = client.QueryTokenForMnsQueue(request);
        return response.Body.MessageTokenDTO;
    }

    private async Task ProcessReceiptMessageAsync(string messageBody, CancellationToken cancellationToken)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<AliyunReceiptStatusDto> receiptData;

            // MNS消息可能是单个对象或数组，需要兼容两种格式
            if (string.IsNullOrWhiteSpace(messageBody))
            {
                _logger.LogWarning("Received empty message body");
                return;
            }

            var trimmedBody = messageBody.Trim();
            if (trimmedBody.StartsWith('['))
            {
                // 数组格式
                receiptData = JsonSerializer.Deserialize<List<AliyunReceiptStatusDto>>(messageBody, options) ?? new List<AliyunReceiptStatusDto>();
            }
            else if (trimmedBody.StartsWith('{'))
            {
                // 单个对象格式，包装成列表
                var singleItem = JsonSerializer.Deserialize<AliyunReceiptStatusDto>(messageBody, options);
                receiptData = singleItem != null ? new List<AliyunReceiptStatusDto> { singleItem } : new List<AliyunReceiptStatusDto>();
            }
            else
            {
                _logger.LogWarning("Received invalid JSON format: {MessageBody}", messageBody);
                return;
            }

            if (receiptData == null || receiptData.Count == 0)
            {
                _logger.LogWarning("Received empty or invalid receipt data");
                return;
            }

            // 创建回执输入对象
            var receiptInput = new AliyunReceiptInput { Statuses = receiptData };

            // 通过事件总线发布回执处理命令
            await using var scope = _serviceProvider.CreateAsyncScope();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
            var command = new ReceiveAliyunReceiptCommand(receiptInput);
            await eventBus.PublishAsync(command, cancellationToken);

            _logger.LogInformation("Processed {Count} receipt messages from MNS", receiptData.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing receipt message: {MessageBody}", messageBody);
            throw;
        }
    }
}
