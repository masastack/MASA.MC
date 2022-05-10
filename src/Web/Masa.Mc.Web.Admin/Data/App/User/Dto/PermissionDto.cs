// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Data.App.User.Dto
{
    public class PermissionDto
    {
        public string Module { get; set; } = default!;

        public bool Read { get; set; }

        public bool Write { get; set; }

        public bool Create { get; set; }

        public bool Delete { get; set; }
    }
}