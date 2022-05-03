// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.App.Invoice.Dto
{
    public class InvoiceStateDto
    {
        public string Label { get; set; }

        public int Value { get; set; }

        public InvoiceStateDto(string label, int value)
        {
            Label = label;
            Value = value;
        }
    }
}