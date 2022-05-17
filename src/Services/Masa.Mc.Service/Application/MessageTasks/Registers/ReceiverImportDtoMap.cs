// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using CsvHelper.Configuration;

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Registers
{
    public class ReceiverImportDtoMap : ClassMap<ReceiverImportDto>
    {
        public ReceiverImportDtoMap()
        {
            Map(m => m.DisplayName).Index(0).Name("昵称");
            Map(m => m.PhoneNumber).Index(1).Name("手机号");
            Map(m => m.Email).Index(1).Name("邮箱");
        }
    }
}
