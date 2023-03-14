// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.Aggregates;

public class WebsiteMessageTag : ValueObject
{
    public string Tag { get; set; } = string.Empty;

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Tag;
    }

    public WebsiteMessageTag(string tag)
    {
        Tag = tag;
    }
}
