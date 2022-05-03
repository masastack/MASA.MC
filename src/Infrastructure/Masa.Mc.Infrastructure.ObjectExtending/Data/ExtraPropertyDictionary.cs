// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.ObjectExtending;

[Serializable]
public class ExtraPropertyDictionary : ConcurrentDictionary<string, object>
{
    public ExtraPropertyDictionary()
    {

    }

    public ExtraPropertyDictionary(IDictionary<string, object> dictionary)
        : base(dictionary)
    {
    }
}