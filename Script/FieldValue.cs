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
    public class FieldValue : FieldControl
    {
        [ScriptName("e_fieldBin")]
        private Element fieldBin;

        private FieldControl fieldControl;

        public FieldValue()
        {
            this.MonitorItemEvents = false;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.IsReady && this.fieldBin != null)
            {
                FieldChoiceCollection fccAlternates = this.Form.GetFieldChoicesOverride(this.Field.Name);

                if (this.Field.Type == FieldType.Enumeration || (fccAlternates != null && fccAlternates.Choices.Count >0))
                {
                    this.fieldControl = new ChoiceFieldValue();
                    this.ApplyToControl(this.fieldControl);
                    this.fieldControl.EnsureElements();

                    this.fieldBin.AppendChild(this.fieldControl.Element);
                }
                else if (this.Field.Type == FieldType.Text)
                {
                    this.fieldControl = new TextFieldValue();

                    this.ApplyToControl(this.fieldControl);

                    this.fieldControl.EnsureElements();

                    this.fieldBin.AppendChild(this.fieldControl.Element);
                }
            }
        }

        public override void PersistToItem()
        {
            if (this.fieldControl != null)
            {
                this.fieldControl.PersistToItem();
            }
        }
    }
}
