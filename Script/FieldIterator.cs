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
    public class FieldIterator : FormControl
    {
        [ScriptName("e_fieldBin")]
        private Element fieldBin;

        private List<LabeledField> fields;
        private Dictionary<String, LabeledField> fieldsByName;

        public FieldIterator()
        {
            this.fields = new List<LabeledField>();
            this.fieldsByName = new Dictionary<string, LabeledField>();
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

            List<LabeledField> fieldsNotUsed = new List<LabeledField>();

            foreach (LabeledField lf in fields)
            {
                fieldsNotUsed.Add(lf);
            }
            
            foreach (Field field in this.Item.Type.Fields)
            {                
                AdjustedFieldState afs = this.Form.GetAdjustedFieldState(field.Name);

                if (afs != AdjustedFieldState.DefaultState)
                {
                    LabeledField ff = this.fieldsByName[field.Name];

                    if (ff == null)
                    {
                        ff = new LabeledField();

                        FieldMode fm = this.Form.GetFieldModeOverride(field.Name);

                        if (fm != FieldMode.FormDefault)
                        {
                            ff.Mode = fm;
                        }

                        ff.Form = this.Form;
                        ff.Item = this.Item;
                        ff.FieldName = field.Name;

                        ff.EnsureElements();

                        this.fieldBin.AppendChild(ff.Element);

                        this.fields.Add(ff);

                        this.fieldsByName[field.Name] = ff;

                    }
                    else
                    {
                        fieldsNotUsed.Remove(ff);
                    }
                }
            }

            foreach (LabeledField f in fieldsNotUsed)
            {
                this.fieldBin.RemoveChild(f.Element);
                this.fields.Remove(f);
                this.fieldsByName[f.Field.Name] = null;
            }
        }
    }
}
