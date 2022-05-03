// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class SendTestMessageModal : AdminCompontentBase
{
    [Parameter]
    public EventCallback OnOk { get; set; }

    private SendTestMessageTaskInput _input = new();
    private bool _visible;
    private ChannelType? _type;
    private List<Guid> _userIds = new List<Guid>();
    private List<UserViewModel> _stateUserItems = new List<UserViewModel>()
    {
        new UserViewModel(new Guid("DBF6118B-7DCC-42D7-8A2C-08DA1C8CE8FC"),ReceiverGroupItemType.Team,"DBF6118B-7DCC-42D7-8A2C-08DA1C8CE8FC","xx团队","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("3256EBB6-CDAE-4D6E-447B-08DA1C8D8CBB"),ReceiverGroupItemType.Role,"3256EBB6-CDAE-4D6E-447B-08DA1C8D8CBB","xx角色","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("BDB3B3C1-30F3-43A2-35B0-08DA1C8E35F7"),ReceiverGroupItemType.Organization,"BDB3B3C1-30F3-43A2-35B0-08DA1C8E35F7","xx组织架构","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("1C97411E-82A8-4AEE-3DBD-08DA1C923854"),ReceiverGroupItemType.User,"1C97411E-82A8-4AEE-3DBD-08DA1C923854","鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("0A911818-05D2-4E7F-6884-08DA1D1E7DF3"),ReceiverGroupItemType.User,"0A911818-05D2-4E7F-6884-08DA1D1E7DF3","鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("FDB58E02-0FD8-4C1D-A5EC-08DA1DD3703C"),ReceiverGroupItemType.User,"FDB58E02-0FD8-4C1D-A5EC-08DA1DD3703C","鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("8A234711-2E11-4E53-A5ED-08DA1DD3703C"),ReceiverGroupItemType.User,"8A234711-2E11-4E53-A5ED-08DA1DD3703C","鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("3CEFCB9B-7486-44BE-3070-08DA1DD90DAE"),ReceiverGroupItemType.User,"3CEFCB9B-7486-44BE-3070-08DA1DD90DAE","鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("7D91D72A-C272-47E9-AE06-08DA1DE9062B"),ReceiverGroupItemType.User,"7D91D72A-C272-47E9-AE06-08DA1DE9062B","鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("DD76FB82-6F46-4814-F5A3-08DA21B88B2E"),ReceiverGroupItemType.User,"DD76FB82-6F46-4814-F5A3-08DA21B88B2E","鬼谷子","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
    };

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    public async Task OpenModalAsync(Guid messageTaskId, ChannelType? type)
    {
        _input.Id = messageTaskId;
        _type = type;
        await InvokeAsync(() =>
        {
            _visible = true;
            StateHasChanged();
        });
    }

    private void HandleCancel()
    {
        _visible = false;
        ResetForm();
    }

    private async Task HandleOkAsync()
    {
        Loading = true;
        //var items = _stateUserItems.Where(x => _userIds.Contains(x.Id)).ToList();
        //var dtos = items.Adapt<List<ReceiverItemDto>>();
        //_input.Receivers.Items = dtos;
        await MessageTaskService.SendTestAsync(_input);
        Loading = false;
        await SuccessMessageAsync(T("MessageTaskSendMessage"));
        _visible = false;

        if (OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
        }
        ResetForm();
    }

    private void ResetForm()
    {
        _input = new();
    }

    public void Remove(UserViewModel item)
    {
        var index = _userIds.IndexOf(item.Id);
        if (index >= 0)
        {
            _userIds.RemoveAt(index);
        }
    }

    private void HandleUserChange()
    {
        var items = _stateUserItems.Where(x => _userIds.Contains(x.Id)).ToList();
        var dtos = items.Adapt<List<ReceiverItemDto>>();
        _input.Receivers.Items = dtos;
    }
}
