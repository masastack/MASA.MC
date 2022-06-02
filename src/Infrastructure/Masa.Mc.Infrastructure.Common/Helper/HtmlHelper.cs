// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Helper;

public static class HtmlHelper
{
    public static string ReplaceHtmlTag(string html, int length = 0)
    {
        string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
        strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

        if (length > 0 && strText.Length > length)
            return strText.Substring(0, length);

        return strText;
    }

    public static string CutString(string inputString, int len)
    {
        string newString = "";
        if (string.IsNullOrWhiteSpace(inputString))
        {
            newString = "";
        }
        inputString = ReplaceHtmlTag(inputString);
        if (inputString.Length <= len)
        {
            newString = inputString;
        }
        else
        {
            if (inputString.Length > 3)
            {
                newString = inputString.Substring(0, len - 3) + "...";
            }
            else
            {
                newString = inputString.Substring(0, len);
            }

        }
        return newString;
    }
}
