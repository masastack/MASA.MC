// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Helper;

public class UtilHelper
{
    public static List<string> MidStrEx(string sourse, string startstr, string endstr)
    {
        List<string> paramList = new();
        Regex regex = new Regex("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
        var match = regex.Matches(sourse);
        foreach (var item in match)
        {
            if (item == null) continue;
            paramList.Add(item.ToString());
        }
        return paramList;
    }
}
