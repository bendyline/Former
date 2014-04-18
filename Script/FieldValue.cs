// Forms.cs
//

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
                FieldChoiceCollection fcc = this.Form.GetFieldChoices(this.Field.Name);

                if (this.Field.Type == FieldType.Enumeration || fcc != null)
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
