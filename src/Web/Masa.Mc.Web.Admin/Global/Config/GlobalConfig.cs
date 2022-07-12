// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Mc.Web.Admin.Shared;

namespace Masa.Mc.Web.Admin.Global
{
    public class GlobalConfig
    {
        #region Field

        private bool _Loading;

        #endregion

        #region Property

        public bool Loading
        {
            get => _Loading;
            set
            {
                if (_Loading != value)
                {
                    _Loading = value;
                    OnLoadingChanged?.Invoke(_Loading, LoadingText);
                }
            }
        }

        public string LoadingText = string.Empty;

        public bool ThrottleFlag { get; set; }

        #endregion


        #region event

        public delegate void GlobalConfigChanged();
        public delegate void LoadingChanged(bool loading, string loadingText);

        public event GlobalConfigChanged? OnCurrentNavChanged;
        public event LoadingChanged? OnLoadingChanged;

        #endregion
    }
}