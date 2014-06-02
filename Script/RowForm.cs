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

    public class RowForm : Form
    {
        private List<LabeledField> fields;
        private Dictionary<String, LabeledField> fieldsByName;
   
        public RowForm()
        {
            this.fields = new List<LabeledField>();
            this.fieldsByName = new Dictionary<string, LabeledField>();
        }
        
        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.Item == null )
            {
                return;
            }

            if (this.Element == null)
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
                AdjustedFieldState afs = this.GetAdjustedFieldState(field.Name);

                if (afs == AdjustedFieldState.Show)
                {
                    LabeledField ff = this.fieldsByName[field.Name];

                    if (ff == null)
                    {
                        ff = new LabeledField();

                        if (this.IteratorFieldTemplateId != null)
                        {
                            ff.TemplateId = this.IteratorFieldTemplateId;
                        }

                        this.fields.Add(ff);
                        this.fieldsByName[field.Name] = ff;

                    }
                    else
                    {
                        fieldsNotUsed.Remove(ff);
                    }

                    if (ff.Element == null || ff.Element.ParentNode == null)
                    {
                        Element cellElement = this.CreateElement("cell");

                        ff.Form = this;
                        ff.FieldName = field.Name;

                        ff.EnsureElements();

                        cellElement.AppendChild(ff.Element);
                        this.Element.AppendChild(cellElement);
                    }

                    FieldMode fm = this.GetFieldModeOverride(field.Name);

                    if (fm != FieldMode.FormDefault)
                    {
                        ff.Mode = fm;
                    }

                    ff.Item = null;
                    ff.Item = this.Item;

                }
            }

            foreach (LabeledField f in fieldsNotUsed)
            {
                this.Element.RemoveChild(f.Element.ParentNode);
                this.fields.Remove(f);
                this.fieldsByName[f.Field.Name] = null;
            }
        }

        public String GetFieldTitleOverride(String fieldName)
        {
            return this.Settings.FieldSettingsCollection.GetFieldTitleOverride(fieldName);
        }

        public FieldChoiceCollection GetFieldChoicesOverride(String fieldName)
        {
            FieldChoiceCollection fcc = this.Settings.FieldSettingsCollection.GetFieldChoicesOverride(fieldName);

            return fcc;
        }

        public AdjustedFieldState GetAdjustedFieldState(String fieldName)
        {
            return this.Settings.FieldSettingsCollection.GetAdjustedFieldState(fieldName);
        }

        public FieldUserInterfaceType GetFieldUserInterfaceTypeOverride(String fieldName)
        {
            return this.Settings.FieldSettingsCollection.GetFieldUserInterfaceTypeOverride(fieldName);
        }

        public FieldMode GetFieldModeOverride(String fieldName)
        {
            return this.Settings.FieldSettingsCollection.GetFieldModeOverride(fieldName);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public void Save(AsyncCallback callback, object state)
        {

            foreach (Control ic in this.TemplateControls)
            {
                if (ic is FieldControl)
                {
                    ((FieldControl)ic).PersistToItem();
                }
            }

            ((ODataEntity)this.Item).Save(callback, state);
        }

        public void ApplyToControl(Control c)
        {
            if (c is ItemControl)
            {
                ((ItemControl)c).Item = this.Item;
            }

            if (c is FormControl)
            {
                ((FormControl)c).Form = this;
            }
    
        }
    }
}
