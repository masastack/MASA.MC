// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos;

public class ImportResultDto<T> where T : class
{
    public bool HasError { get; set; }
    public IList<DataRowErrorInfo> RowErrors { get; set; } = new List<DataRowErrorInfo>();
    public IList<TemplateErrorInfo> TemplateErrors { get; set; } = new List<TemplateErrorInfo>();
    public string ErrorMsg { get; set; } = string.Empty;
    public ICollection<T> Data { get; set; } = new Collection<T>();
}
