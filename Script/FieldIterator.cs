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

        private String fieldTemplateId;

        private Dictionary<FieldInterface, Field> usedFieldInterfaces;
        private List<LabeledField> fields;
        private Dictionary<String, LabeledField> fieldsByName;
        private PropertyChangedEventHandler propertyChanged;

        [ScriptName("s_fieldTemplateId")]
        public String FieldTemplateId
        {
            get
            {
                return this.fieldTemplateId;
            }

            set
            {
                this.fieldTemplateId = value;

                foreach (LabeledField lf in this.fields)
                {
                    lf.TemplateId = this.fieldTemplateId;
                }
            }
        }

        public FieldIterator()
        {
            this.fields = new List<LabeledField>();
            this.fieldsByName = new Dictionary<string, LabeledField>();

            this.usedFieldInterfaces = new Dictionary<FieldInterface, Field>();
            this.propertyChanged = fs_PropertyChanged;
        }

        protected internal override void OnInterfaceChange()
        {
            base.OnInterfaceChange();

            this.OnUpdate();
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

            List<Field> sortedFields = new List<Field>();

            foreach (Field field in this.Item.Type.Fields)
            {
                sortedFields.Add(field);
            }

            sortedFields.Sort(this.CompareFields);

            ElementUtilities.ClearChildElements(this.fieldBin);

            foreach (Field field in sortedFields)
            {
                // only add the field if it's not part of the uber form template.
                if (!((Form)this.Form).ContainsTemplateFieldControl(field.Name))
                {
                    FieldInterface fs = this.Form.ItemSetInterface.FieldInterfaces.GetFieldByName(field.Name);

                    if (fs != null && !this.usedFieldInterfaces.ContainsKey(fs))
                    {
                        this.usedFieldInterfaces[fs] = field;

                        fs.PropertyChanged -= this.propertyChanged;
                        fs.PropertyChanged += this.propertyChanged;
                    }

                    DisplayState afs = this.Form.GetAdjustedDisplayState(field.Name);

                    if (afs == DisplayState.Show || afs == DisplayState.ShowInDetailHideInList)
                    {
                        LabeledField ff = this.fieldsByName[field.Name];

                        if (ff == null)
                        {
                            ff = new LabeledField();

                            ff.Form = this.Form;
                            ff.FieldName = field.Name;

                            if (this.fieldTemplateId != null)
                            {
                                ff.TemplateId = this.fieldTemplateId;
                            }

                            ff.EnsureElements();

                            this.fields.Add(ff);

                            this.fieldsByName[field.Name] = ff;

                        }
                        else
                        {
                            if (fs != ff.FieldInterface)
                            {
                                ff.Update();
                            }

                            fieldsNotUsed.Remove(ff);
                        }

                        this.fieldBin.AppendChild(ff.Element);

                        FieldMode fm = this.Form.GetFieldModeOverride(field.Name);

                        if (fm != FieldMode.FormDefault)
                        {
                            ff.Mode = fm;
                        }

                        ff.ItemSet = this.ItemSet;
                        ff.Item = this.Item;
                    }
                }
            }

            foreach (LabeledField f in fieldsNotUsed)
            {
                ElementUtilities.RemoveIfChildOf(f.Element, this.fieldBin);

                this.fields.Remove(f);
                this.fieldsByName[f.Field.Name] = null;

                f.Dispose();
            }
        }

        protected override void OnTemplateDisposed()
        {
            base.OnTemplateDisposed();

            if (this.fields != null)
            {
                foreach (LabeledField lf in this.fields)
                {
                    lf.Dispose();
                }
            }
        }

        private int CompareFields(Field fieldA, Field fieldB)
        {
            FieldInterfaceCollection fsc = this.Form.ItemSetInterface.FieldInterfaces;

            FieldInterface fieldInterfaceA = fsc.GetFieldByName(fieldA.Name);
            FieldInterface fieldInterfaceB = fsc.GetFieldByName(fieldB.Name);

            if (fieldInterfaceA == null && fieldInterfaceB == null)
            {
                return fieldA.Name.CompareTo(fieldB.Name);
            }

            int orderA = 0;

            if (fieldInterfaceA != null)
            {
                if (fieldInterfaceA.Order != null)
                {
                    orderA = (int)fieldInterfaceA.Order;
                }
            }


            int orderB = 0;

            if (fieldInterfaceB != null)
            {
                if (fieldInterfaceB.Order != null)
                {
                    orderB = (int)fieldInterfaceB.Order;
                }
            }

            if (orderA < 0)
            {
                orderA = 100000;
            }

            if (orderB < 0)
            {
                orderB = 100000;
            }


            if (orderA == orderB)
            {
                return fieldA.Name.CompareTo(fieldB.Name);
            }

            return orderA - orderB;
        }

        private void fs_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Update();
        }
    }
}
