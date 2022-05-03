﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.Authentication.Components
{
    public partial class ForgotPassword
    {
        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public bool HideLogo { get; set; }

        [Parameter]
        public double Width { get; set; } = 410;

        [Parameter]
        public StringNumber? Elevation { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> SendResetClick { get; set; }

        [Parameter]
        public string SignInRoute { get; set; } = $"pages/authentication/login-v1";
    }
}