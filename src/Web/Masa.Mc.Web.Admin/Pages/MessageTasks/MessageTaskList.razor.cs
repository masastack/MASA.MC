// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks;

public partial class MessageTaskList : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnNavigateToSend { get; set; }

    private TemplateMessageEditModal _templateEditModal = default!;
    private OrdinaryMessageEditModal _ordinaryEditModal = default!;
    private MessageTaskDetailModal _detailModal = default!;
    private SendTestMessageModal _sendTestModal = default!;
    private GetMessageTaskInputDto _queryParam = new(20) { TimeType = MessageTaskTimeTypes.ModificationTime, Source = MessageTaskSources.Management };
    private PaginatedListDto<MessageTaskDto> _entities = new();
    private List<ChannelDto> _channelItems = new();
    private bool advanced = false;
    private bool isAnimate;

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

    private async Task HandlePageChanged(int page)
    {
        _queryParam.Page = page;
        await LoadData();
    }

    private async Task HandlePageSizeChanged(int pageSize)
    {
        _queryParam.PageSize = pageSize;
        await LoadData();
    }

    private async Task HandleClearAsync()
    {
        _queryParam = new(20) { TimeType = MessageTaskTimeTypes.ModificationTime, Source = MessageTaskSources.Management };
        await LoadData();
    }

    private void ToggleAdvanced()
    {
        advanced = !advanced;
        isAnimate = true;
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

    private async Task NavigateToSend()
    {
        if (OnNavigateToSend.HasDelegate)
        {
            await OnNavigateToSend.InvokeAsync();
        }
        //NavigationManager.NavigateTo("/messageTasks/sendMessage");
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
            await ConfirmAsync(T("EnableConfirmationMessageTask"), async () =>
            {
                await MessageTaskService.EnabledAsync(new EnabledMessageTaskInputDto { MessageTaskId = item.Id });
                item.IsEnabled = true;
            });

        }
        else
        {
            await ConfirmAsync(T("DisableConfirmationMessageTask"), async () =>
            {
                await MessageTaskService.DisableAsync(new DisableMessageTaskInputDto { MessageTaskId = item.Id });
                item.IsEnabled = false;
            });

        }
    }
}
