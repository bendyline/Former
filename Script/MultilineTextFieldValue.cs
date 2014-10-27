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
    public class MultilineTextFieldValue : FieldControl
    {
        [ScriptName("e_textInput")]
        private TextAreaElement textInput;

        [ScriptName("e_textDisplay")]
        private Element textDisplay;

        private bool commitPending = false;

        public MultilineTextFieldValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.textInput != null)
            {
                this.textInput.AddEventListener("change", this.HandleTextInputChanged, true);
                this.textInput.AddEventListener("keyup", this.HandleTextInputKeyPressed, true);
            }
        }

        private void HandleTextInputKeyPressed(ElementEvent e)
        {
            if (!this.commitPending)
            {
                this.commitPending = true;

                Window.SetTimeout(this.SaveValue, 2000);
            }
        }

        private void HandleTextInputChanged(ElementEvent e)
        {
            this.SaveValue();
        }

        private void SaveValue()
        {
            this.commitPending = false;

            this.Item.SetStringValue(this.FieldName, this.textInput.Value);
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
                this.textInput.Value = val;
            }
            else if (this.EffectiveMode == FieldMode.Edit)
            {
                this.textInput.Disabled = false;
                this.textDisplay.Style.Display = "none";
                this.textInput.Style.Display = "block";

                this.textInput.Value = val;
            }
            else
            {
                this.textDisplay.Style.Display = "block";
                this.textInput.Style.Display = "none";

                ElementUtilities.SetText(this.textDisplay, val);
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
