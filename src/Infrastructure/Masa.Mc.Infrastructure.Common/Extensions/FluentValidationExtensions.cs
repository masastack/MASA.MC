// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> Chinese<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.CHINESE);
    }

    public static IRuleBuilderOptions<T, string> Number<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.NUMBER);
    }

    public static IRuleBuilderOptions<T, string> Letter<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.LETTER);
    }

    public static IRuleBuilderOptions<T, string> LowerLetter<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.LOWER_LETTER);
    }

    public static IRuleBuilderOptions<T, string> UpperLetter<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.UPPER_LETTER);
    }

    public static IRuleBuilderOptions<T, string> LetterNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.LETTER_NUMBER);
    }

    public static IRuleBuilderOptions<T, string> LetterNumberSymbol<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.LETTER_NUMBER_SYMBOL);
    }

    public static IRuleBuilderOptions<T, string> ChineseLetter<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.CHINESE_LETTER);
    }

    public static IRuleBuilderOptions<T, string> ChineseLetterNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.CHINESE_LETTER_NUMBER);
    }

    public static IRuleBuilderOptions<T, string> ChineseLetterNumberSymbol<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.CHINESE_LETTER_NUMBER_SYMBOL);
    }

    public static IRuleBuilderOptions<T, string> Phone<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches<T>(RegularHelper.PHONE);
    }

    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.EMAIL);
    }

    public static IRuleBuilderOptions<T, string> IdCard<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.IDCARD);
    }

    public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.URL);
    }

    public static IRuleBuilderOptions<T, string> MinLength<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength)
    {
        return ruleBuilder.MinimumLength(minimumLength);
    }

    public static IRuleBuilderOptions<T, string> MaxLength<T>(this IRuleBuilder<T, string> ruleBuilder, int maximumLength)
    {
        return ruleBuilder.MaximumLength(maximumLength);
    }

    public static IRuleBuilderOptions<T, TProperty> Required<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return ruleBuilder.NotNull().NotEmpty();
    }
}
