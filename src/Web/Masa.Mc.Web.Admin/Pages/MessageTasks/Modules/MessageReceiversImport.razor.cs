// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageReceiversImport
{
    [Parameter]
    public ChannelTypes? Type { get; set; }

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> OnAdd { get; set; }

    private string _downloadUrl;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _downloadUrl = $"{McApiOptions.McServiceBaseAddress}/api/message-task/GenerateReceiverImportTemplate";
    }

    private async void HandleFileChange(IBrowserFile file)
    {
        await using var memoryStream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(memoryStream);
        var dto = new UploadFileDto
        {
            FileName = file.Name,
            FileContent = memoryStream.ToArray(),
            Size = file.Size,
            ContentType = "text/csv"
            //ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };
        var list = await MessageTaskService.ImportReceiversAsync(dto);
        if (list != null && OnAdd.HasDelegate)
        {
            await OnAdd.InvokeAsync(list);
        }
    }
}
