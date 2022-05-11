// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Helper;

public class UtilConvert
{
    public static string GetHash<T>(Stream stream) where T : HashAlgorithm
    {
        StringBuilder sb = new StringBuilder();

        MethodInfo create = typeof(T).GetMethod("Create", new Type[] { });
        using (T crypt = (T)create.Invoke(null, null))
        {
            if (crypt != null)
            {
                byte[] hashBytes = crypt.ComputeHash(stream);
                foreach (byte bt in hashBytes)
                {
                    sb.Append(bt.ToString("x2"));
                }
            }
        }
        return sb.ToString();
    }

    public static string GetGuidToString()
    {
        long i = 1;
        foreach (byte b in Guid.NewGuid().ToByteArray())
        {
            i *= ((int)b + 1);
        }
        string tempStr = string.Format("{0:x}", i - DateTime.Now.Ticks);
        if (tempStr.Length != 16)
        {
            tempStr += "0";
        }
        return tempStr;
    }

    public static string GetGuidToNumber()
    {
        byte[] buffer = Guid.NewGuid().ToByteArray();
        return BitConverter.ToInt64(buffer, 0).ToString();
    }

    public static string Number(int Length)
    {
        return Number(Length, false);
    }

    public static string Number(int Length, bool Sleep)
    {
        if (Sleep)
            System.Threading.Thread.Sleep(3);
        string result = "";
        System.Random random = new Random();
        for (int i = 0; i < Length; i++)
        {
            result += random.Next(10).ToString();
        }
        return result;
    }

    public static string GetCheckCode(int codeCount)
    {
        string str = string.Empty;
        int rep = 0;
        long num2 = DateTime.Now.Ticks + rep;
        rep++;
        Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
        for (int i = 0; i < codeCount; i++)
        {
            char ch;
            int num = random.Next();
            if ((num % 2) == 0)
            {
                ch = (char)(0x30 + ((ushort)(num % 10)));
            }
            else
            {
                ch = (char)(0x41 + ((ushort)(num % 0x1a)));
            }
            str = str + ch.ToString();
        }
        return str;
    }
}
