// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.Others.AccountSettings.Dto
{
    public class CountryDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public CountryDto(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
