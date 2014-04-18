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
    public class FieldIterator : FormControl
    {
        [ScriptName("e_fieldBin")]
        private Element fieldBin;

        private List<FullField> fields;

        public FieldIterator()
        {
            this.fields = new List<FullField>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.Item == null || this.Form == null)
            {
                return;
            }

            if (this.fieldBin == null)
            {
                return;
            }

            while (this.fieldBin.ChildNodes.Length > 0)
            {
                this.fieldBin.RemoveChild(this.fieldBin.ChildNodes[0]);
            }

            this.fields.Clear();

            foreach (Field field in this.Item.Type.Fields)
            {
                AdjustedFieldState afs = this.Form.GetFieldState(field.Name);

                if (afs != null)
                {
                    FullField ff = new FullField();

                    ff.Form = this.Form;
                    ff.Item = this.Item;
                    ff.FieldName = field.Name;

                    ff.EnsureElements();

                    this.fieldBin.AppendChild(ff.Element);

                    this.fields.Add(ff);
                }
            }
        }
    }
}
