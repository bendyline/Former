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
    public enum FormMode
    {
        Edit = 0,
        NewForm = 1,
        Display = 2
    }

    public enum AdjustedFieldState
    {
        Hide = 0,
        Show = 1
    }

    public class Form : ItemControl 
    {
        private FormMode formMode = FormMode.Edit;

        private Dictionary<String, String> fieldTitleOverrides;
        private Dictionary<String, AdjustedFieldState> fieldStates;
        private Dictionary<String, FieldChoiceCollection> fieldChoiceOverrides;


        [ScriptName("c_fieldIterator")]
        private Control fieldIterator;

        public bool IsEditing
        {
            get
            {
                return this.formMode == FormMode.Edit || this.formMode == FormMode.NewForm;
            }
        }

        public Form()
        {
            this.fieldTitleOverrides = new Dictionary<string, string>();
            this.fieldStates = new Dictionary<string, AdjustedFieldState>();
            this.fieldChoiceOverrides = new Dictionary<string, FieldChoiceCollection>();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


        public void Save()
        {

            foreach (Control ic in this.TemplateControls)
            {
                if (ic is FieldControl)
                {
                    ((FieldControl)ic).PersistToItem();
                }
            }

            ((ODataEntity)this.Item).Save();
        }

        public FieldChoiceCollection GetFieldChoices(String fieldName)
        {
            return this.fieldChoiceOverrides[fieldName];
        }

        public void AddFieldChoices(String fieldName, FieldChoiceCollection fcc)
        {
            this.fieldChoiceOverrides[fieldName] = fcc;
        }

        public AdjustedFieldState GetFieldState(String fieldName)
        {
            return this.fieldStates[fieldName];
        }

        public void AddFieldState(String fieldName, AdjustedFieldState afs)
        {
            this.fieldStates[fieldName] = afs;
        }

        public String GetFieldTitleOverride(String fieldName)
        {
            return this.fieldTitleOverrides[fieldName];
        }

        public void AddFieldTitleOverride(String fieldName, String newFieldTitle)
        {
            this.fieldTitleOverrides[fieldName] = newFieldTitle;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            foreach (Control c in this.TemplateControls)
            {
                this.ApplyToControl(c);
            }

            if (this.fieldIterator != null)
            {
                this.ApplyToControl(this.fieldIterator);
            }
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
