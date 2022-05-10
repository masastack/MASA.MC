// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.App.ECommerce.Dto
{
    public class AddressTypeDto
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public AddressTypeDto(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }
}