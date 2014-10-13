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
        [ScriptName("e_checkboxText")]
        private Element checkboxText;

        public Boolean CurrentValue
        {
            get
            {
               
                String val = this.Item.GetStringValue(this.FieldName);

                if (val == null)
                {
                    return false;
                }

                return Boolean.Parse(val);
            }
        }
        public CheckboxFieldValue()
        {
            this.TrackInteractionEvents = true;
        }

        protected override void OnClick(ElementEvent e)
        {
            base.OnClick(e);

            if (CurrentValue)
            {
                this.Item.SetBooleanValue(this.FieldName, false);
            }
            else
            {
                this.Item.SetBooleanValue(this.FieldName, true);
            }

        }

   

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.checkboxText== null || !this.IsReady)
            {
                return;
            }

            if (this.EffectiveMode == FieldMode.Example)
            {
                this.checkboxText.Style.Display = "";
            }
            else if (this.EffectiveMode == FieldMode.Edit)
            {
                if (this.CurrentValue)
                {
                    this.checkboxText.Style.Display = "";
                }
                else
                {
                    this.checkboxText.Style.Display = "none";
                }
            }
            else
            {
                if (this.CurrentValue)
                {
                    this.checkboxText.Style.Display = "";
                }
                else
                {
                    this.checkboxText.Style.Display = "none";
                }
            }
        }

        public override void PersistToItem()
        {
            
        }       
    }
}
