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
        private Element specialButtonsCell;
        private InputElement deleteButton;
   
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

            ElementUtilities.ClearChildElements(this.Element);

            List<LabeledField> fieldsNotUsed = new List<LabeledField>();

            foreach (LabeledField lf in fields)
            {
                fieldsNotUsed.Add(lf);
            }

            if (this.specialButtonsCell == null && this.ItemSetInterface.DisplayDeleteItemButton)
            {
                this.specialButtonsCell = this.CreateElement("cell");

                this.deleteButton = (InputElement)this.CreateElementWithTypeAndAdditionalClasses("deleteButton", "BUTTON", "k-button");
                this.deleteButton.AddEventListener("click", this.HandleItemDelete, true);

                Element e = this.CreateElementWithAdditionalClasses("backIcon", "glyphicon glyphicon-remove");

                this.deleteButton.AppendChild(e);

                this.specialButtonsCell.AppendChild(this.deleteButton);
            }
            else if (this.specialButtonsCell != null && !this.ItemSetInterface.DisplayDeleteItemButton)
            {   
                this.specialButtonsCell = null;
            }

            if (this.ItemSetInterface.DisplayDeleteItemButton)
            {
                if (!ElementUtilities.ElementIsChildOf(this.specialButtonsCell, this.Element))
                {
                    this.Element.AppendChild(this.specialButtonsCell);
                }
            }

            List<Field> sortedFields = new List<Field>();

            foreach (Field field in this.Item.Type.Fields)
            {
                sortedFields.Add(field);
            }

            sortedFields.Sort(this.ItemSetInterface.CompareFields);

            foreach (Field field in sortedFields)
            {                
                DisplayState afs = this.GetAdjustedDisplayState(field.Name);

                if (afs == DisplayState.Show)
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
                    Element cellElement = null;


                    if (ff.Element == null || ff.Element.ParentNode == null)
                    {
                        cellElement = this.CreateElement("cell");

                        ff.Form = this;
                        ff.FieldName = field.Name;

                        ff.EnsureElements();

                        cellElement.AppendChild(ff.Element);
                    }
                    else                       
                    {
                        cellElement = ff.Element.ParentNode;
                    }

                    if (this.specialButtonsCell != null)
                    {
                        this.Element.InsertBefore(cellElement, this.specialButtonsCell);
                    }
                    else
                    {
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

            // add a spacer
            Element spacer = this.CreateElement("cell");
            spacer.Style.Width = "100%";
            ElementUtilities.SetHtml(spacer, "&#160;");
            this.Element.AppendChild(spacer);

            foreach (LabeledField f in fieldsNotUsed)
            {
                f.Dispose();

                if (f.Element != null)
                {
                    if (f.Element.ParentNode != null)
                    {
                        try
                        {
                            this.Element.RemoveChild(f.Element.ParentNode);
                        }
                        catch (Exception)
                        { 
                            ;
                        }
                    }
                }

                this.fields.Remove(f);
                this.fieldsByName[f.Field.Name] = null;
            }
        }

        protected override void OnSettingsChange()
        {
            base.OnSettingsChange();

            this.Update();
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

        public String GetFieldTitleOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldTitleOverride(fieldName);
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
