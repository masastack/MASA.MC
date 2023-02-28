// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> LetterNumberSymbol<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.LETTER_NUMBER_SYMBOL);
    }

    public static IRuleBuilderOptions<T, string> ChineseLetterNumberSymbol<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.CHINESE_LETTER_NUMBER_SYMBOL);
    }

    public static IRuleBuilderOptions<T, string> MinLength<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength)
    {
        return ruleBuilder.MinimumLength(minimumLength);
    }

    public static IRuleBuilderOptions<T, string> MaxLength<T>(this IRuleBuilder<T, string> ruleBuilder, int maximumLength)
    {
        return ruleBuilder.MaximumLength(maximumLength);
    }

    public static IRuleBuilderOptions<T, TProperty> Required<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string errorMessage = "{PropertyName} is required")
    {
        return ruleBuilder.NotNull().WithMessage(errorMessage)
            .NotEmpty().WithMessage(errorMessage)
            ;
    }
}
