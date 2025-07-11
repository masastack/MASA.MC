// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms;

public interface ISmsTemplateService
{
    Task<SmsResponseBase> GetSmsTemplateAsync(string templateCode);

    Task<SmsResponseBase> GetSmsTemplateListAsync(int page = 1, int pageSize = 50);
}
