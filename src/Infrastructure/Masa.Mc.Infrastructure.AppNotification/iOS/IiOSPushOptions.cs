// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.iOS;

public interface IiOSPushOptions : IOptions
{
    string BundleId { get; set; }

    string CertContent{ get; set; }

    string KeyId { get; set; }

    string TeamId { get; set; }
}
