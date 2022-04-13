using System;
namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public static class AliyunSmsSenderExtensions
{
    public static void SetOptions(this ISmsSender smsSender, AliyunSmsOptions options)
    {
        var dic = new Dictionary<string, object>
        {
            ["AccessKeySecret"] = options.AccessKeySecret,
            ["AccessKeyId"] = options.AccessKeyId,
            ["EndPoint"] = options.EndPoint
        };
        smsSender.SetOptions(dic);
    }
}