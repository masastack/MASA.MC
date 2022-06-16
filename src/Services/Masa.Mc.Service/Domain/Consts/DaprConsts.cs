// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.Consts;

public static class DaprConsts
{
#if DEBUG
    public const string DAPR_PUBSUB_NAME = "pubsub";
#else
    public const string DAPR_PUBSUB_NAME = "pubsub.masa-mc-dev";
#endif
}
