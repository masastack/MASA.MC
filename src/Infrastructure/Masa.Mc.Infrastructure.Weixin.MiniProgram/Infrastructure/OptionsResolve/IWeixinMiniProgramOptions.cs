// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.MiniProgram.Infrastructure.OptionsResolve;

public interface IWeixinMiniProgramOptions : IOptions
{
    string AppId { get; set; }

    string AppSecret { get; set; }

    string Token { get; set; }

    string EncodingAESKey { get; set; }

    string MiniprogramState { get; set; }

    string Lang { get; set; }
}
