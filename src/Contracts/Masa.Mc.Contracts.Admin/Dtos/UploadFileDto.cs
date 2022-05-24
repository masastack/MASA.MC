// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos;

public class UploadFileDto
{
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
    public long Size { get; set; }
    public string ContentType { get; set; }
}
