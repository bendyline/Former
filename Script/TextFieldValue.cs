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
    public class TextFieldValue : FieldControl
    {
        [ScriptName("e_textInput")]
        private InputElement textInput;

        public TextFieldValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.textInput != null)
            {
                this.textInput.AddEventListener("change", this.HandleTextInputChanged, true);
            }
        }

        private void HandleTextInputChanged(ElementEvent e)
        {
            this.Item.SetStringValue(this.FieldName, this.textInput.Value);

        }


        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.textInput == null)
            {
                return;
            }

            if (this.EffectiveMode == FieldMode.Example)
            {
                this.textInput.Value = "Example";
                this.textInput.Disabled = true;
            }
            else if (this.EffectiveMode == FieldMode.Edit)
            {
                this.textInput.Disabled = false;
            }

            if (this.IsReady)
            {
                String val = this.Item.GetStringValue(this.FieldName);

                if (val == null)
                {
                    val = String.Empty;
                }

                this.textInput.Value = val;
            }
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
