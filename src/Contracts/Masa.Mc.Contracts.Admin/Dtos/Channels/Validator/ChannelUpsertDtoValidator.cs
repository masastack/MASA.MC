// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Channels.Validator;

public class ChannelUpsertDtoValidator : AbstractValidator<ChannelUpsertDto>
{
    public ChannelUpsertDtoValidator()
    {
        RuleFor(inputDto => inputDto.DisplayName).Required().Length(2, 50);
        RuleFor(inputDto => inputDto.Code).Required().Length(2, 50);
        RuleFor(inputDto => inputDto.Type).IsInEnum();
        RuleFor(inputDto => inputDto.Description).Length(0, 255);
        RuleFor(inputDto => inputDto.ExtraProperties).Must(x => !string.IsNullOrEmpty(x.GetProperty<string>(nameof(SmsChannelOptions.AccessKeyId)))).When(x => x.Type == ChannelTypes.Sms).WithMessage("Please enter accessKeyId");
        RuleFor(inputDto => inputDto.ExtraProperties).Must(x => !string.IsNullOrEmpty(x.GetProperty<string>(nameof(SmsChannelOptions.AccessKeySecret)))).When(x => x.Type == ChannelTypes.Sms).WithMessage("Please enter accessKeySecret");
        RuleFor(inputDto => inputDto.ExtraProperties).Must(x => !string.IsNullOrEmpty(x.GetProperty<string>(nameof(EmailChannelOptions.UserName)))).When(x => x.Type == ChannelTypes.Email).WithMessage("Please enter email account");
        RuleFor(inputDto => inputDto.ExtraProperties).Must(x => !string.IsNullOrEmpty(x.GetProperty<string>(nameof(EmailChannelOptions.Password)))).When(x => x.Type == ChannelTypes.Email).WithMessage("Please enter email password");
        RuleFor(inputDto => inputDto.ExtraProperties).Must(x => !string.IsNullOrEmpty(x.GetProperty<string>(nameof(EmailChannelOptions.Smtp)))).When(x => x.Type == ChannelTypes.Email).WithMessage("Please enter smtp");
    }
}
