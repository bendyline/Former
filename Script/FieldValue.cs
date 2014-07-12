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

        private FieldUserInterfaceType previousUserInterfaceType = FieldUserInterfaceType.NoValue;

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
                String fieldName = this.Field.Name;

                FieldUserInterfaceType userInterfaceType = this.Field.UserInterfaceType;
                FieldUserInterfaceType altUserInterfaceType = this.Form.GetFieldUserInterfaceTypeOverride(fieldName);

                if (altUserInterfaceType != FieldUserInterfaceType.TypeDefault)
                {
                    userInterfaceType = altUserInterfaceType;
                }

                if (userInterfaceType != previousUserInterfaceType)
                {
                    previousUserInterfaceType = userInterfaceType;

                    if (this.fieldControl != null)
                    {
                        this.fieldControl.Dispose();
                    }

                    while (this.fieldBin.ChildNodes.Length > 0)
                    {
                        this.fieldBin.RemoveChild(this.fieldBin.ChildNodes[0]);
                    }

                    if (this.Field.Type == FieldType.Integer && userInterfaceType == FieldUserInterfaceType.Scale)
                    {
                        this.fieldControl = new ScaleFieldValue();

                        this.ApplyToControl(this.fieldControl);
                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if (this.Field.Type == FieldType.ShortText && userInterfaceType == FieldUserInterfaceType.User)
                    {
                        this.fieldControl = new UserValue();
                        this.ApplyToControl(this.fieldControl);
                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if (this.Field.Type == FieldType.ShortText && userInterfaceType == FieldUserInterfaceType.Choice)
                    {
                        this.fieldControl = new ChoiceFieldValue();

                        this.ApplyToControl(this.fieldControl);
                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if (this.Field.Type == FieldType.ShortText)
                    {
                        this.fieldControl = new TextFieldValue();

                        if (userInterfaceType == FieldUserInterfaceType.Email)
                        {
                            this.fieldControl.TemplateId = "bl-forms-emailtextfieldvalue";
                        }

                        this.ApplyToControl(this.fieldControl);

                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if (this.Field.Type == FieldType.BoolChoice)
                    {
                        this.fieldControl = new CheckboxFieldValue();

                        this.ApplyToControl(this.fieldControl);

                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
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
