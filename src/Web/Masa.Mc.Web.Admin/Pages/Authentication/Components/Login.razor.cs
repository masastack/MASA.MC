// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Pages.Authentication.Components
{
    public partial class Login
    {
        private bool _show;

        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public bool HideLogo { get; set; }

        [Parameter]
        public double Width { get; set; } = 410;

        [Parameter]
        public StringNumber? Elevation { get; set; }

        [Parameter]
        public string CreateAccountRoute { get; set; } = $"pages/authentication/register-v1";

        [Parameter]
        public string ForgotPasswordRoute { get; set; } = $"pages/authentication/forgot-password-v1";

        [Parameter]
        public EventCallback<MouseEventArgs> OnLogin { get; set; }
    }
}