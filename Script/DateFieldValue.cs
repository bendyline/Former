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
using BL.UI.KendoControls;

namespace BL.Forms
{
    public class DateFieldValue : DateTimeFieldControl
    {
        [ScriptName("c_datePicker")]
        private DatePicker datePicker;

        [ScriptName("e_datePickerValue")]
        private Element datePickerValue;

        public DateFieldValue()
        {

        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.datePicker.Changed += date_Changed;
        }

        private void date_Changed(object sender, EventArgs e)
        {
            this.CurrentValue = this.datePicker.Value;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.datePicker == null)
            {
                return;
            }

            if (this.EffectiveMode == FieldMode.View)
            {
                String result = String.Empty;

                if (this.CurrentValue != null)
                {
                    result = Utilities.GetStaticDateValue(this.CurrentValue);
                }

                ElementUtilities.SetText(this.datePickerValue, result);

                this.datePickerValue.Style.Display = "";
                this.datePicker.Visible = false;
            }
            else
            {
                this.datePickerValue.Style.Display = "none";
                this.datePicker.Visible = true;

                this.datePicker.Value = this.CurrentValue;
            }
        }
    }
}
