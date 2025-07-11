// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.YunMas;

public static class YunMasUtil
{
    public static string ToBase64Json(object obj)
    {
        var settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeNonAscii
        };
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, settings);
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
    }

    public static string Md5Lower32(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    public static string BuildMac(string ecName, string apId, string secretKey, string mobiles, string content, string sign, string addSerial)
    {
        var macRaw = $"{ecName}{apId}{secretKey}{mobiles}{content}{sign}{addSerial}";
        return Md5Lower32(macRaw);
    }
}
