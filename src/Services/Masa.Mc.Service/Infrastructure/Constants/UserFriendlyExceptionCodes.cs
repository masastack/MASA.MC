// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Constants;

public static class UserFriendlyExceptionCodes
{
    public const string CHANNEL_TYPE_CANNOT_BE_MODIFIED = "ChannelTypeCannotBeModified";
    public const string CHANNEL_CODE_CANNOT_BE_MODIFIED = "ChannelCodeCannotBeModified";
    public const string MESSAGE_TASK_TO_BE_SENT_OR_BEING_SENT_CANNOT_BE_DELETED = "MessageTaskToBeSentOrBeingSentCannotBeDeleted";
    public const string CHANNEL_NAME_CANNOT_BE_DUPLICATE = "ChannelNameCannotBeDuplicate";
    public const string MESSAGE_HAS_BEEN_SENT_SUCCESSFULLY_NO_NEED_TO_RESEND = "MessageHasBeenSentSuccessfullyNoNeedToResend";
    public const string UNKNOWN_CHANNEL_TYPE = "UnknownChannelType";
    public const string MESSAGE_TEMPLATE_NOT_EXIST = "MessageTemplateNotExist";
    public const string ENABLED_STATUS_CANNOT_BE_DELETED = "EnabledStatusCannotBeDeleted";
    public const string CHANNEL_REQUIRED = "ChannelRequired";
    public const string SIGN_REQUIRED = "SignRequired";
    public const string TEMPLATE_VARIABLES_REQUIRED = "TemplateVariablesRequired";
    public const string MESSAGE_TASK_DISABLE_HAS_HISTORY = "MessageTaskDisableHasHistory";
    public const string MESSAGE_TASK_HISTORY_WITHDRAWN = "MessageTaskHistoryWithdrawn";
    public const string CAN_BE_MODIFIED_BEFORE_SENDING = "CanBeModifiedBeforeSending";
    public const string MESSAGE_TEMPLATE_CANNOT_DELETE_BY_MESSAGE_TASK = "MessageTemplateCannotDeleteByMessageTask";
    public const string CHANNEL_CANNOT_DELETED = "ChannelCannotDeleted";
    public const string CHANNEL_CANNOT_REPEATED = "ChannelCannotRepeated";
    public const string CHANNEL_TYPE_DOES_NOT_MATCH_CHANNEL = "ChannelTypeDoesNotMatchChannel";
    public const string TEMPLATE_CODE_CANNOT_REPEATED = "TemplateCodeCannotRepeated";
    public const string TEMPLATE_ID_CANNOT_REPEATED = "TemplateIdCannotRepeated";
    public const string RECEIVER_GROUP_NAME_CANNOT_REPEATED = "ReceiverGroupNameCannotRepeated";
    public const string SYNC_SMS_TEMPLATE_ERROR_MESSAGE = "SyncSmsTemplateErrorMessage";
}
