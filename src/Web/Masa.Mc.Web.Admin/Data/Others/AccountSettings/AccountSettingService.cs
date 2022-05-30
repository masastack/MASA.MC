// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.Others.AccountSettings
{
    public static class AccountSettingService
    {
        public static AccountDto GetAccount() => new AccountDto("johndoe", "John Doe", "granger007@hogward.com", "Crystal Technologies");

        public static List<CountryDto> GetCountryList() => new()
        {
            new("1", "USA"),
            new("2", "India"),
            new("3", "Canada"),
        };

        public static InformationDto GetInformation() => new("", DateOnly.FromDateTime(DateTimeOffset.Now.DateTime), "1", "", 6562542568);

        public static SocialDto GetSocial() => new("https://www.twitter.com", "", "", "https://www.linkedin.com", "", "");
    }
}
