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
    public class CheckboxFieldValue : FieldControl
    {
        [ScriptName("e_checkboxInput")]
        private InputElement checkboxInput;

        public CheckboxFieldValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.checkboxInput != null)
            {
                this.checkboxInput.AddEventListener("change", this.HandleTextInputChanged, true);
            }
        }

        private void HandleTextInputChanged(ElementEvent e)
        {
            this.Item.SetStringValue(this.FieldName, ControlUtilities.GetIsChecked(this.checkboxInput).ToString());
        }


        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.checkboxInput == null || !this.IsReady)
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
                this.checkboxInput.Value = "Example";
                this.checkboxInput.Disabled = true;
                this.checkboxInput.Style.Display = "block";
                ControlUtilities.SetIsCheckedFromObject(this.checkboxInput, val);
            }
            else if (this.EffectiveMode == FieldMode.Edit)
            {
                this.checkboxInput.Disabled = false;
                this.checkboxInput.Style.Display = "block";
                ControlUtilities.SetIsCheckedFromObject(this.checkboxInput, val);
            }
            else
            {
                this.checkboxInput.Style.Display = "none";
            }
        }

        public override void PersistToItem()
        {
            
        }       
    }
}
