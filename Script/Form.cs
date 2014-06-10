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
    public enum FormMode
    {
        EditForm = 0,
        NewForm = 1,
        ViewForm = 2,
        Example = 3
    }

    public enum AdjustedFieldState
    {
        DefaultState = 0,
        Hide = 1,
        Show = 2
    }

    public class Form : ItemControl 
    {
        private FormSettings settings;
        private String iteratorFieldTemplateId;

        [ScriptName("c_fieldIterator")]
        private FieldIterator fieldIterator;

        [ScriptName("s_iteratorFieldTemplateId")]
        public String IteratorFieldTemplateId
        {
            get
            {
                return this.iteratorFieldTemplateId;
            }

            set
            {
                this.iteratorFieldTemplateId = value;

                if (this.fieldIterator != null)
                {
                    ((FieldIterator)this.fieldIterator).FieldTemplateId = this.iteratorFieldTemplateId;
                }
            }
        }

        public FormSettings Settings
        {
            get
            {
                if (this.settings == null)
                {
                    this.settings = new FormSettings();
                    this.settings.PropertyChanged += settings_PropertyChanged;
                    this.settings.FieldSettingsCollection.CollectionChanged += FieldSettingsCollection_CollectionChanged;
                }

                return this.settings;
            }

            set
            {
                if (this.settings == value)
                {
                    return;
                }

                this.settings = value;

                if (this.fieldIterator != null)
                {
                    this.fieldIterator.OnSettingsChange();
                }
            }
        }


        public FormMode Mode
        {
            get
            {
                return this.Settings.Mode;
            }

            set
            {
                this.Settings.Mode = value;
            }
        }

        public bool IsEditing
        {
            get
            {
                FormMode fm = this.Mode;

                return fm == FormMode.EditForm || fm == FormMode.NewForm;
            }
        }

        public Form()
        {
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

        public FieldUserInterfaceOptions GetFieldUserInterfaceOptionsOverride(String fieldName)
        {
            return this.Settings.FieldSettingsCollection.GetFieldUserInterfaceOptionsOverride(fieldName);
        }

        public FieldMode GetFieldModeOverride(String fieldName)
        {
            return this.Settings.FieldSettingsCollection.GetFieldModeOverride(fieldName);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.fieldIterator != null)
            {
                if (this.iteratorFieldTemplateId != null)
                {
                    this.fieldIterator.FieldTemplateId = this.iteratorFieldTemplateId;
                }
            }
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


        private void FieldSettingsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.fieldIterator != null)
            {
                this.fieldIterator.OnSettingsChange();
            }
        }

        private void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.fieldIterator != null)
            {
                this.fieldIterator.OnSettingsChange();
            }
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
