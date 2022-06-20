// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks;

public partial class MessageTaskList : AdminCompontentBase
{
    private TemplateMessageEditModal _templateEditModal = default!;
    private OrdinaryMessageEditModal _ordinaryEditModal = default!;
    private MessageTaskSendModal _sendModal = default!;
    private MessageTaskDetailModal _detailModal = default!;
    private SendTestMessageModal _sendTestModal = default!;
    private GetMessageTaskInputDto _queryParam = new(8) { TimeType = MessageTaskTimeTypes.ModificationTime };
    private PaginatedListDto<MessageTaskDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private bool advanced = true;

    ChannelService ChannelService => McCaller.ChannelService;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadData()
    {
        Loading = true;
        _entities = (await MessageTaskService.GetListAsync(_queryParam));
        _channelItems = (await ChannelService.GetListAsync(new GetChannelInputDto(99))).Result;
        Loading = false;
        StateHasChanged();
    }

    private async Task HandleOk()
    {
        await LoadData();
    }

    private async Task RefreshAsync()
    {
        _queryParam.Page = 1;
        await LoadData();
    }

    private async Task HandlePaginationChange(PaginationEventArgs args)
    {
        _queryParam.Page = args.Page;
        _queryParam.PageSize = args.PageSize;
        await LoadData();
    }

    private async Task HandleClearAsync()
    {
        _queryParam = new(8) { TimeType = MessageTaskTimeTypes.ModificationTime };
        await LoadData();
    }

    private void ToggleAdvanced()
    {
        advanced = !advanced;
    }

    private async Task HandleEditAsync(MessageTaskDto model)
    {
        if (model.EntityType == MessageEntityTypes.Ordinary)
        {
            await _ordinaryEditModal.OpenModalAsync(model);
        }
        if (model.EntityType == MessageEntityTypes.Template)
        {
            await _templateEditModal.OpenModalAsync(model);
        }
    }

    private void NavigateToSend()
    {
        NavigationManager.NavigateTo("/messageTasks/sendMessage");
    }

    private async Task HandleDelAsync(Guid _entityId)
    {
        await ConfirmAsync(T("DeletionConfirmationMessage"), async () => { await DeleteAsync(_entityId); });
    }

    private async Task DeleteAsync(Guid _entityId)
    {
        Loading = true;
        await MessageTaskService.DeleteAsync(_entityId);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskDeleteMessage"));
        await LoadData();
    }

    private async Task HandleIsDraft()
    {
        _queryParam.IsDraft = _queryParam.IsDraft == true ? null : true;
        await LoadData();
    }

    private async Task HandleIsEnabled(MessageTaskDto item)
    {
        if (!item.IsEnabled)
        {
            await MessageTaskService.EnabledAsync(new EnabledMessageTaskInputDto { MessageTaskId = item.Id });
            item.IsEnabled = true;
        }
        else
        {
            await MessageTaskService.DisableAsync(new DisableMessageTaskInputDto { MessageTaskId = item.Id });
            item.IsEnabled = false;
        }
    }
}
