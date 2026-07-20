// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.MiniProgram;

public class WeixinMiniProgramOptions : IWeixinMiniProgramOptions
{
    public string AppId { get; set; } = string.Empty;

    public string AppSecret { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string EncodingAESKey { get; set; } = string.Empty;

    public string MiniprogramState { get; set; } = "formal";

    public string Lang { get; set; } = "zh_CN";
}
