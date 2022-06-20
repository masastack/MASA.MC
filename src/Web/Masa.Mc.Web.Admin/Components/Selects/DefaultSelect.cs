// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Components.Selects;

public class DefaultSelect<TItem, TItemValue, TValue> : MSelect<TItem, TItemValue, TValue>
{
    [Parameter] public Action<DefaultTextfieldAction>? Action { get; set; }

    private DefaultTextfieldAction InternalAction { get; set; } = new();

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Dense = true;
        Height = 48;
        HideDetails = "auto";
        Outlined = true;

        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Dense && Height == 48)
        {
            Class ??= string.Empty;
            if (!Class.Contains("m-input--dense-48"))
            {
                Class += " m-input--dense-48";
            }
        }

        if (Action is not null)
        {
            Action.Invoke(InternalAction);

            AppendContent = builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "d-flex");
                builder.AddAttribute(2, "style", $"margin-top:-7px;margin-right:-12px;height:{Height}px;");
                builder.AddContent(3, subBuilder =>
                {
                    subBuilder.OpenComponent<MDivider>(0);
                    subBuilder.AddAttribute(1, "Vertical", true);
                    subBuilder.CloseComponent();

                    subBuilder.OpenComponent<AutoLoadingButton>(2);
                    subBuilder.AddAttribute(3, "Text", InternalAction.Text);
                    subBuilder.AddAttribute(4, "Disabled", InternalAction.Disabled);
                    subBuilder.AddAttribute(5, "Color", InternalAction.Color);
                    subBuilder.AddAttribute(6, "Style", "border:none;border-bottom-left-radius:0;border-top-left-radius:0;height:100%;");
                    subBuilder.AddAttribute(7, "OnClick", EventCallback.Factory.Create<MouseEventArgs>(this, InternalAction.OnClick));
                    subBuilder.AddAttribute(8, "ChildContent", (RenderFragment)(cb => cb.AddContent(9, InternalAction.Content)));
                    subBuilder.CloseComponent();
                });
                builder.CloseElement();
            };
        }
    }
}
