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
    public class CheckboxFieldValue : BooleanFieldControl
    {
        [ScriptName("e_checkboxText")]
        private Element checkboxText;

        private long lastClickTime = -1;

        public CheckboxFieldValue()
        {
            this.TrackInteractionEvents = true;
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        protected override void OnClick(ElementEvent e)
        {
            base.OnClick(e);

            int nowTime = Date.Now.GetTime();

            // add a bit of a deadtime to switch around
            if ((nowTime - lastClickTime) > 200)
            {
                lastClickTime = nowTime;

                this.CurrentValue = !this.CurrentValue;
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
