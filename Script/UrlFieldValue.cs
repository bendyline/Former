/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Diagnostics;
using jQueryApi;
using BL.UI;
using BL.Data;
using System.Runtime.CompilerServices;

namespace BL.Forms
{
    public class UrlFieldValue : UrlFieldControl
    {
        [ScriptName("e_textInput")]
        private InputElement textInput;

        [ScriptName("e_textDisplay")]
        private Element textDisplay;

        private String lastPersistedValue = String.Empty;

        private bool commitPending = false;

        public UrlFieldValue()
        {
            this.EnsureScript("kendo.ui.Validator", "js/kendo/kendo.validator.min.js");
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.textInput != null)
            {
                this.textInput.AddEventListener("change", this.HandleTextInputChanged, true);

                ElementUtilities.RegisterTextInputBehaviors(this.textInput);

                jQueryObject jqo = jQuery.FromObject(this.textInput);

                Script.Literal("{0}.kendoValidator().data(\"kendoValidator\")", jqo);
            }
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        private void HandleTextInputKeyUp(ElementEvent e)
        {
            if (!this.commitPending)
            {
                this.commitPending = true;

                Window.SetTimeout(this.SaveValue, 3000);
            }
        }

        private void HandleTextInputChanged(ElementEvent e)
        {
            this.SaveValue();
        }

        private void SaveValue()
        {
            this.commitPending = false;

            this.lastPersistedValue = this.textInput.Value;
            this.Item.SetStringValue(this.FieldName, this.textInput.Value);

            //          this.textInput.Focus();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.textInput == null || !this.IsReady)
            {
                return;
            }

            String val = this.Item.GetStringValue(this.FieldName);

            if (val == null)
            {
                val = String.Empty;
            }

            if (this.EffectiveMode == FieldMode.Example)
            {
                this.textInput.Value = "Example";
                this.textInput.Disabled = true;
                this.textDisplay.Style.Display = "none";
                this.textInput.Style.Display = "block";

                if ((String.IsNullOrEmpty(this.textInput.Value) || this.textInput.Value == lastPersistedValue) && !commitPending)
                {
                    this.lastPersistedValue = val;
                    this.textInput.Value = val;
                }
            }
            else if (this.EffectiveMode == FieldMode.Edit)
            {
                this.textInput.Disabled = false;
                this.textDisplay.Style.Display = "none";
                this.textInput.Style.Display = "block";

                if ((String.IsNullOrEmpty(this.textInput.Value) || this.textInput.Value == lastPersistedValue) && !commitPending)
                {
                    this.lastPersistedValue = val;
                    this.textInput.Value = val;
                }
            }
            else
            {
                this.textDisplay.Style.Display = "block";
                this.textInput.Style.Display = "none";

                ElementUtilities.SetText(this.textDisplay, val);
            }

            if (this.EffectiveUserInterfaceOptions != null)
            {
                Nullable<int> suggestedWidth = this.EffectiveUserInterfaceOptions.SuggestedWidth;

                if (suggestedWidth != null)
                {
                    int widthToUse = (int)suggestedWidth;


                    if (Context.Current.BrowserInnerWidth < 360 && widthToUse > 200)
                    {
                        widthToUse = (widthToUse * 2) / 3;
                    }
                    else if (Context.Current.BrowserInnerWidth < 420 && widthToUse > 200)
                    {
                        widthToUse = (widthToUse * 3) / 4;
                    }

                    this.textDisplay.Style.MinWidth = widthToUse + "px";
                    this.textInput.Style.MinWidth = this.textDisplay.Style.MinWidth;
                    this.textInput.Style.MaxWidth = this.textInput.Style.MinWidth;
                }
                else
                {
                    this.textDisplay.Style.MinWidth = "150px";
                    this.textInput.Style.MinWidth = "150px";
                }

                Nullable<int> fontSize = this.EffectiveUserInterfaceOptions.FontSize;

                if (fontSize != null)
                {
                    this.textDisplay.Style.FontSize = (int)fontSize + "pt";
                    this.textInput.Style.FontSize = (int)fontSize + "pt";
                    this.textInput.Style.MaxHeight = "1.8em";
                    this.textInput.Style.TextIndent = "0.1em";
                }
                else
                {
                    this.textDisplay.Style.FontSize = String.Empty;
                    this.textInput.Style.FontSize = String.Empty;
                    this.textInput.Style.MaxHeight = String.Empty;
                    this.textInput.Style.TextIndent = String.Empty;
                }
            }

            if (this.FieldInterface != null && this.FieldInterface.InterfaceTypeOptionsOverride != null && this.FieldInterface.InterfaceTypeOptionsOverride.Placeholder != null)
            {
                this.textInput.SetAttribute("placeholder", this.FieldInterface.InterfaceTypeOptionsOverride.Placeholder);
            }
            else
            {
                this.textInput.SetAttribute("placeholder", "");
            }
        }

        public override void PersistToItem()
        {

        }
    }
}
