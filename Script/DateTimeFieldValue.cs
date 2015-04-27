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
    public class DateTimeFieldValue : DateTimeFieldControl
    {
        [ScriptName("c_dateTimePicker")]
        private DateTimePicker dateTimePicker;

        public DateTimeFieldValue()
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

            this.dateTimePicker.Changed += mobileSwitch_Changed;
        }

        private void mobileSwitch_Changed(object sender, EventArgs e)
        {
            this.CurrentValue = this.dateTimePicker.Value;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.dateTimePicker == null)
            {
                return;
            }

            this.dateTimePicker.Value = this.CurrentValue;
        }
    }
}
