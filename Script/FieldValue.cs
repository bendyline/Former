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

        private FieldInterfaceType previousInterfaceType = FieldInterfaceType.NoValue;

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

            if (this.IsReady && this.fieldBin != null && this.Field != null)
            {
                String fieldName = this.Field.Name;

                FieldInterfaceType interfaceType = this.Field.InterfaceType;
                Nullable<FieldInterfaceType> altInterfaceType = this.Form.GetFieldInterfaceTypeOverride(fieldName);

                if (altInterfaceType != null && altInterfaceType != FieldInterfaceType.TypeDefault)
                {
                    interfaceType = (FieldInterfaceType)altInterfaceType;
                }

                if (interfaceType != previousInterfaceType || this.fieldControl == null)
                {
                    previousInterfaceType = interfaceType;

                    if (this.fieldControl != null)
                    {
                        this.fieldControl.Dispose();
                    }

                    while (this.fieldBin.ChildNodes.Length > 0)
                    {
                        this.fieldBin.RemoveChild(this.fieldBin.ChildNodes[0]);
                    }

                    if (this.Field.Type == FieldType.Integer && interfaceType == FieldInterfaceType.Scale)
                    {
                        this.fieldControl = new ScaleFieldValue();

                        this.ApplyToControl(this.fieldControl);
                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if (this.Field.Type == FieldType.ShortText && interfaceType == FieldInterfaceType.User)
                    {
                        this.fieldControl = new UserValue();
                        this.ApplyToControl(this.fieldControl);
                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if ((this.Field.Type == FieldType.ShortText || this.Field.Type == FieldType.UnboundedText) && interfaceType == FieldInterfaceType.UserList)
                    {
                        this.fieldControl = new UserList();
                        this.ApplyToControl(this.fieldControl);
                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if ((this.Field.Type == FieldType.ShortText || this.Field.Type == FieldType.Integer) && interfaceType == FieldInterfaceType.Choice)
                    {
                        this.fieldControl = new ChoiceFieldValue();

                        this.ApplyToControl(this.fieldControl);
                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if (this.Field.Type == FieldType.ShortText)
                    {
                        this.fieldControl = new TextFieldValue();

                        if (interfaceType == FieldInterfaceType.Email)
                        {
                            this.fieldControl.TemplateId = "bl-forms-emailtextfieldvalue";
                        }

                        this.ApplyToControl(this.fieldControl);

                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if (this.Field.Type == FieldType.Integer || this.Field.Type == FieldType.BigInteger)
                    {
                        this.fieldControl = new IntegerFieldValue();

                        this.ApplyToControl(this.fieldControl);

                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if (this.Field.Type == FieldType.UnboundedText)
                    {
                        this.fieldControl = new MultilineTextFieldValue();

                        this.ApplyToControl(this.fieldControl);

                        this.fieldControl.EnsureElements();

                        this.fieldBin.AppendChild(this.fieldControl.Element);
                    }
                    else if (this.Field.Type == FieldType.RichContent)
                    {
                        this.fieldControl = new RichContentFieldValue();

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
                else
                {
                    this.ApplyToControl(this.fieldControl);
                }
            }
        }

        protected override void OnTemplateDisposed()
        {
            base.OnTemplateDisposed();

            if (this.fieldControl != null)
            {
                this.fieldControl.Dispose();
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
