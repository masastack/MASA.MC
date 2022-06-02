// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.Auth.Subjects;

public class SubjectDataService
{
    static List<SubjectDataDto> _datas = new()
    {
        new SubjectDataDto(new Guid(TempCurrentUserConsts.ID), MessageTaskReceiverTypes.User, TempCurrentUserConsts.ID, TempCurrentUserConsts.NAME, "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", TempCurrentUserConsts.PHONE_NUMBER, TempCurrentUserConsts.EMAIL),
        new SubjectDataDto(new Guid("1C97411E-82A8-4AEE-3DBD-08DA1C923854"), MessageTaskReceiverTypes.User, "1C97411E-82A8-4AEE-3DBD-08DA1C923854", "鬼谷子1", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341231", "121@163.com"),
        new SubjectDataDto(new Guid("0A911818-05D2-4E7F-6884-08DA1D1E7DF3"), MessageTaskReceiverTypes.User, "0A911818-05D2-4E7F-6884-08DA1D1E7DF3", "鬼谷子2", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341232", "122@163.com"),
        new SubjectDataDto(new Guid("FDB58E02-0FD8-4C1D-A5EC-08DA1DD3703C"), MessageTaskReceiverTypes.User, "FDB58E02-0FD8-4C1D-A5EC-08DA1DD3703C", "鬼谷子3", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341233", "123@163.com"),
        new SubjectDataDto(new Guid("8A234711-2E11-4E53-A5ED-08DA1DD3703C"), MessageTaskReceiverTypes.User, "8A234711-2E11-4E53-A5ED-08DA1DD3703C", "鬼谷子4", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341234", "124@163.com"),
        new SubjectDataDto(new Guid("3CEFCB9B-7486-44BE-3070-08DA1DD90DAE"), MessageTaskReceiverTypes.User, "3CEFCB9B-7486-44BE-3070-08DA1DD90DAE", "鬼谷子5", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341235", "125@163.com"),
        new SubjectDataDto(new Guid("7D91D72A-C272-47E9-AE06-08DA1DE9062B"), MessageTaskReceiverTypes.User, "7D91D72A-C272-47E9-AE06-08DA1DE9062B", "鬼谷子6", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341236", "126@163.com"),
        new SubjectDataDto(new Guid("DD76FB82-6F46-4814-F5A3-08DA21B88B2E"), MessageTaskReceiverTypes.User, "DD76FB82-6F46-4814-F5A3-08DA21B88B2E", "鬼谷子7", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341237", "127@163.com"),
        new SubjectDataDto(new Guid("DBF6118B-7DCC-42D7-8A2C-08DA1C8CE8FC"), MessageTaskReceiverTypes.Team, "DBF6118B-7DCC-42D7-8A2C-08DA1C8CE8FC", "xx团队", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341234", "123@163.com"),
        new SubjectDataDto(new Guid("3256EBB6-CDAE-4D6E-447B-08DA1C8D8CBB"), MessageTaskReceiverTypes.Role, "3256EBB6-CDAE-4D6E-447B-08DA1C8D8CBB", "xx角色", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341234", "123@163.com"),
        new SubjectDataDto(new Guid("BDB3B3C1-30F3-43A2-35B0-08DA1C8E35F7"), MessageTaskReceiverTypes.Organization, "BDB3B3C1-30F3-43A2-35B0-08DA1C8E35F7", "xx组织架构", "https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png", "13312341234", "123@163.com"),
    };

    public static List<SubjectDataDto> GetList() => _datas;
}
