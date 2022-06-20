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
    private long maxFileSize = 1024 * 1024 * 15;

    MessageTaskService MessageTaskService => McCaller.MessageTaskService;

    private async void HandleFileChange(IBrowserFile file)
    {
        var fileContent = await ReadFile(file);
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

    private async Task<byte[]> ReadFile(IBrowserFile file)
    {
        await using var memoryStream = new MemoryStream();
        using var readStream = file.OpenReadStream(maxFileSize);
        var bytesRead = 0;
        var totalRead = 0;
        var buffer = new byte[2048];

        while ((bytesRead = await readStream.ReadAsync(buffer)) != 0)
        {
            totalRead += bytesRead;

            await memoryStream.WriteAsync(buffer, 0, bytesRead);

            _progress = (int)(totalRead * 100.0 / file.Size);
        }
        return memoryStream.ToArray();
    }
}
