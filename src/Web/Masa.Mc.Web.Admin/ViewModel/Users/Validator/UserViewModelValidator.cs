// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.ViewModel.Users.Validator;

public class UserViewModelValidator : AbstractValidator<UserViewModel>
{
    public UserViewModelValidator()
    {
        RuleFor(input => input.DisplayName).Required().Length(2, 50).ChineseLetterNumber();
        RuleFor(input => input.PhoneNumber).Required().Phone().When(x=>string.IsNullOrEmpty(x.Email));
        RuleFor(input => input.Email).Required().Email().When(x => string.IsNullOrEmpty(x.PhoneNumber));
    }
}
