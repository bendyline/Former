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
    public class LabeledField : FieldControl
    {
        [ScriptName("c_fieldLabel")]
        private FieldLabel fieldLabel;

        [ScriptName("c_fieldValue")]
        private FieldValue fieldValue;

        [ScriptName("c_fieldValidationIndicator")]
        private FieldValidationIndicator fieldValidationIndicator;

        public LabeledField()
        {
            this.MonitorItemEvents = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.IsReady)
            {
                if (this.fieldLabel != null)
                {
                    this.ApplyToControl(this.fieldLabel);
                }

                if (this.fieldValue!= null)
                {
                    this.ApplyToControl(this.fieldValue);
                }

                if (this.fieldValidationIndicator != null)
                {
                    this.ApplyToControl(this.fieldValidationIndicator);
                }
            }
        }

        public override void PersistToItem()
        {
            if (this.fieldLabel != null)
            {
                this.fieldLabel.Update();
            }

            if (this.fieldValue != null)
            {
                this.fieldValue.Update();
            }
        }
    }
}
