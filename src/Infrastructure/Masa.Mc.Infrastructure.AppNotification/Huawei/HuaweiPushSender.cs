// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Huawei;

public class HuaweiPushSender : IAppNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly HuaweiOAuthService _oauthService;
    private readonly IOptionsResolver<IHuaweiPushOptions> _optionsResolver;

    public HuaweiPushSender(HttpClient httpClient, HuaweiOAuthService oauthService, IOptionsResolver<IHuaweiPushOptions> optionsResolver)
    {
        _httpClient = httpClient;
        _oauthService = oauthService;
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponseBase> SendAsync(SingleAppMessage message, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.PushMessageUrlFormat, options.ProjectId);

        var payload = BuildSingleMessagePayload(message.ClientId, message);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload),
            Headers = { { "Authorization", $"Bearer {accessToken}" } }
        };

        var response = await _httpClient.SendAsync(request, ct);
        return await HandleResponse(response, ct);
    }

    private IEnumerable<string[]> SplitTokens(string[] tokens)
    {
        for (int i = 0; i < tokens.Length; i += 1000)
        {
            yield return tokens.Skip(i).Take(1000).ToArray();
        }
    }

    public async Task<AppNotificationResponseBase> BatchSendAsync(BatchAppMessage message, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();

        var responses = new List<AppNotificationResponseBase>();
        foreach (var batch in SplitTokens(message.ClientIds))
        {
            var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
            var url = string.Format(HuaweiConstants.PushMessageUrlFormat, options.ProjectId);

            var payload = BuildBatchMessagePayload(batch, message);

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload),
                Headers = { { "Authorization", $"Bearer {accessToken}" } }
            };

            var response = await _httpClient.SendAsync(request, ct);
            responses.Add(await HandleResponse(response, ct));
        }

        return responses.All(r => r.Success)
            ? new AppNotificationResponseBase(true, "All sent successfully")
            : new AppNotificationResponseBase(false, "Partial failure");
    }

    public async Task<AppNotificationResponseBase> BroadcastSendAsync(AppMessage message, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.PushMessageUrlFormat, options.ProjectId);

        var payload = BuildBroadcastMessagePayload(message);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload),
            Headers = { { "Authorization", $"Bearer {accessToken}" } }
        };

        var response = await _httpClient.SendAsync(request, ct);
        return await HandleResponse(response, ct);
    }

    public Task<AppNotificationResponseBase> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        // Huawei Push V1/V2 does not support message withdrawal  
        return Task.FromResult(new AppNotificationResponseBase(false, "Withdrawal operation not supported"));
    }

    private HmsPushRequest BuildSingleMessagePayload(string token, AppMessage message)
    {
        return new HmsPushRequest
        {
            Message = new HmsMessage
            {
                Token = new[] { token },
                Notification = new HmsNotification { Title = message.Title, Body = message.Text },
                Data = JsonConvert.SerializeObject(message.TransmissionContent),
                Android = new HmsAndroidConfig
                {
                    Notification = new HmsAndroidNotification
                    {
                        ClickAction = new
                        {
                            type = 1,
                            intent = $"#Intent;action={message.Url};end"
                        }
                    }
                }
            }
        };
    }

    private HmsPushRequest BuildBatchMessagePayload(string[] tokens, AppMessage message)
    {
        return new HmsPushRequest
        {
            Message = new HmsMessage
            {
                Token = tokens,
                Notification = new HmsNotification { Title = message.Title, Body = message.Text },
                Data = JsonConvert.SerializeObject(message.TransmissionContent),
                Android = new HmsAndroidConfig
                {
                    Notification = new HmsAndroidNotification
                    {
                        ClickAction = new
                        {
                            type = 1,
                            intent = $"#Intent;action={message.Url};end"
                        }
                    }
                }
            }
        };
    }

    private HmsPushRequest BuildBroadcastMessagePayload(AppMessage message)
    {
        return new HmsPushRequest
        {
            Message = new HmsMessage
            {
                Topic = "all", // Assume all devices subscribe to the "all" topic  
                Notification = new HmsNotification { Title = message.Title, Body = message.Text },
                Data = JsonConvert.SerializeObject(message.TransmissionContent),
                Android = new HmsAndroidConfig
                {
                    Notification = new HmsAndroidNotification
                    {
                        ClickAction = new
                        {
                            type = 1,
                            intent = $"#Intent;action={message.Url};end"
                        }
                    }
                }
            }
        };
    }

    private async Task<AppNotificationResponseBase> HandleResponse(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<JsonElement>(options: null, cancellationToken: ct);
            var code = result.GetProperty("code").GetString();
            var msg = result.TryGetProperty("msg", out var msgProp) ? msgProp.GetString() : null;
            var requestId = result.TryGetProperty("requestId", out var idProp) ? idProp.GetString() : null;

            if (code == "80000000")
            {
                return new AppNotificationResponseBase(true, "Success", requestId);
            }
            else if (code == "80100000")
            {
                // Some tokens are invalid  
                var illegalTokens = result.GetProperty("msg").GetProperty("illegal_tokens").EnumerateArray().Select(t => t.GetString()).ToList();
                return new AppNotificationResponseBase(false, $"Some tokens are invalid: {string.Join(",", illegalTokens)}", requestId);
            }
            else
            {
                return new AppNotificationResponseBase(false, $"Push failed: {msg}", requestId);
            }
        }

        var error = await response.Content.ReadAsStringAsync(ct);
        return new AppNotificationResponseBase(false, $"Push failed: HTTP {response.StatusCode} - {error}");
    }
}
