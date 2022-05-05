﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageReceivers : AdminCompontentBase
{
    [Parameter]
    public List<MessageTaskReceiverDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> ValueChanged { get; set; }

    [Parameter]
    public ChannelTypes? Type { get; set; }

    private ExternalUserCreateModal _createModal;
    private List<Guid> _userIds = new List<Guid>();
    private List<UserViewModel> _stateUserItems = new List<UserViewModel>()
    {
        new UserViewModel(new Guid("DBF6118B-7DCC-42D7-8A2C-08DA1C8CE8FC"),ReceiverGroupItemTypes.Team,"DBF6118B-7DCC-42D7-8A2C-08DA1C8CE8FC","xx团队","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("3256EBB6-CDAE-4D6E-447B-08DA1C8D8CBB"),ReceiverGroupItemTypes.Role,"3256EBB6-CDAE-4D6E-447B-08DA1C8D8CBB","xx角色","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("BDB3B3C1-30F3-43A2-35B0-08DA1C8E35F7"),ReceiverGroupItemTypes.Organization,"BDB3B3C1-30F3-43A2-35B0-08DA1C8E35F7","xx组织架构","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("1C97411E-82A8-4AEE-3DBD-08DA1C923854"),ReceiverGroupItemTypes.User,"1C97411E-82A8-4AEE-3DBD-08DA1C923854","鬼谷子1","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("0A911818-05D2-4E7F-6884-08DA1D1E7DF3"),ReceiverGroupItemTypes.User,"0A911818-05D2-4E7F-6884-08DA1D1E7DF3","鬼谷子2","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("FDB58E02-0FD8-4C1D-A5EC-08DA1DD3703C"),ReceiverGroupItemTypes.User,"FDB58E02-0FD8-4C1D-A5EC-08DA1DD3703C","鬼谷子3","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("8A234711-2E11-4E53-A5ED-08DA1DD3703C"),ReceiverGroupItemTypes.User,"8A234711-2E11-4E53-A5ED-08DA1DD3703C","鬼谷子4","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("3CEFCB9B-7486-44BE-3070-08DA1DD90DAE"),ReceiverGroupItemTypes.User,"3CEFCB9B-7486-44BE-3070-08DA1DD90DAE","鬼谷子5","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("7D91D72A-C272-47E9-AE06-08DA1DE9062B"),ReceiverGroupItemTypes.User,"7D91D72A-C272-47E9-AE06-08DA1DE9062B","鬼谷子6","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
        new UserViewModel(new Guid("DD76FB82-6F46-4814-F5A3-08DA21B88B2E"),ReceiverGroupItemTypes.User,"DD76FB82-6F46-4814-F5A3-08DA21B88B2E","鬼谷子7","https://cdn.masastack.com/stack/images/website/masa-blazor/doddgu.png","13312341234","123@163.com"),
    };

    public void Remove(UserViewModel item)
    {
        var index = _userIds.IndexOf(item.Id);
        if (index >= 0)
        {
            _userIds.RemoveAt(index);
        }
    }

    public async Task AddAsync()
    {
        var list = _stateUserItems.Where(x => _userIds.Contains(x.Id)).ToList();
        var dtos = list.Adapt<List<MessageTaskReceiverDto>>();
        foreach (var dto in dtos)
        {
            if (!Value.Any(x => x.SubjectId == dto.SubjectId && x.Type == dto.Type))
            {
                Value.Insert(0, dto);
            }
        }
        await ValueChanged.InvokeAsync(Value);
    }

    public async Task RemoveValue(MessageTaskReceiverDto item)
    {
        Value.RemoveAll(x => x.SubjectId == item.SubjectId && x.Type == item.Type);
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task HandleOk(UserViewModel user)
    {
        _stateUserItems.Add(user);
        Value.Add(user.Adapt<MessageTaskReceiverDto>());
        await ValueChanged.InvokeAsync(Value);
    }
}
