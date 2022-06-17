// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.MessageTasks.Modules;

public partial class MessageReceiversImport
{
    [Parameter]
    public List<MessageTaskReceiverDto> Value { get; set; } = new();

    [Parameter]
    public Guid? MessageTemplatesId { get; set; }

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<List<MessageTaskReceiverDto>> OnAdd { get; set; }

    [Inject]
    IBlazorDownloadFileService BlazorDownloadFileService { get; set; } = default!;

    private ImportResultDto<MessageTaskReceiverDto> _importResult;

    private int _progress = 0;
    private string _fileName = string.Empty;
    private bool _isUpload;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    private async void HandleFileChange(IBrowserFile file)
    {
        _progress = 0;
        await using var memoryStream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(memoryStream);
        var fileContent = memoryStream.ToArray();
        _progress = 100;
        _isUpload = true;
        if (FileEncoderHelper.GetTextFileEncodingType(fileContent) != Encoding.UTF8)
        {
            await WarningAsync(T("Description.MessageReceiversUpload.EncodingTips"));
            return;
        }
        _fileName = file.Name;
        var dto = new ImportReceiversDto
        {
            FileName = _fileName,
            FileContent = fileContent,
            Size = file.Size,
            ContentType = "text/csv",
            MessageTemplatesId = MessageTemplatesId
        };
        _importResult = await MessageTaskService.ImportReceiversAsync(dto);
        Value = _importResult.Data.ToList();
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private async Task Download()
    {
        var contentBytes = await MessageTaskService.GenerateReceiverImportTemplateAsync(MessageTemplatesId);
        await BlazorDownloadFileService.DownloadFile("ReceiverImportTemplate.csv", contentBytes, "text/csv");
    }
}
