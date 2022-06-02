// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.Auth.Subjects;

public class UserDataService
{
    static List<UserDataDto> _datas = new()
    {
        new UserDataDto(new Guid(TempCurrentUserConsts.ID), TempCurrentUserConsts.NAME, "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", TempCurrentUserConsts.PHONE_NUMBER, TempCurrentUserConsts.EMAIL),
        new UserDataDto(new Guid("1C97411E-82A8-4AEE-3DBD-08DA1C923854"), "鬼谷子1", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341231", "121@163.com"),
        new UserDataDto(new Guid("0A911818-05D2-4E7F-6884-08DA1D1E7DF3"), "鬼谷子2", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341232", "122@163.com"),
        new UserDataDto(new Guid("FDB58E02-0FD8-4C1D-A5EC-08DA1DD3703C"), "鬼谷子3", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341233", "123@163.com"),
        new UserDataDto(new Guid("8A234711-2E11-4E53-A5ED-08DA1DD3703C"), "鬼谷子4", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341234", "124@163.com"),
        new UserDataDto(new Guid("3CEFCB9B-7486-44BE-3070-08DA1DD90DAE"), "鬼谷子5", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341235", "125@163.com"),
        new UserDataDto(new Guid("7D91D72A-C272-47E9-AE06-08DA1DE9062B"), "鬼谷子6", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341236", "126@163.com"),
        new UserDataDto(new Guid("DD76FB82-6F46-4814-F5A3-08DA21B88B2E"), "鬼谷子7", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341237", "127@163.com"),
    };

    public static List<UserDataDto> GetList() => _datas;
}