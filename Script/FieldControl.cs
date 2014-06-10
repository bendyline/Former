// Forms.cs
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
    public enum FieldMode
    {
        FormDefault = 0,
        Edit = 1,
        View = 2,
        Example = 3
    }

    public class FieldControl : FormControl 
    {
        private IDataStoreField field;
        private String fieldName;
        private FieldMode mode = FieldMode.Edit;
        private PropertyChangedEventHandler propertyChanged;

        public FieldMode EffectiveMode
        {
            get
            {
                if (this.mode == FieldMode.FormDefault)
                {
                    if (this.Form.Mode == FormMode.Example)
                    {
                        return FieldMode.Example;
                    }
                    else if (this.Form.Mode == FormMode.NewForm || this.Form.Mode == FormMode.EditForm)
                    {
                        return FieldMode.Edit;
                    }
                    else
                    {
                        return FieldMode.View;
                    }
                }

                return this.mode;
            }
        }

        [ScriptName("i_mode")]
        public FieldMode Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                this.mode = value;
            }
        }

        public IDataStoreField Field
        {
            get
            {
                if (this.field == null && this.fieldName != null)
                {
                    this.field = this.Item.Type.GetField(this.fieldName);
                }

                return this.field;
            }
        }

        public bool IsReady
        {
            get
            {
                return this.fieldName != null && this.Item != null;
            }
        }

        [ScriptName("s_name")]
        public String FieldName
        {
            get
            {
                return this.fieldName;
            }

            set
            {
                if (this.fieldName == value)
                {
                    return;
                }

                this.fieldName = value;

                this.Update();
            }
        }

        public FieldUserInterfaceOptions EffectiveUserInterfaceOptions
        {
            get
            {
                FieldUserInterfaceOptions efuio = this.Form.GetFieldUserInterfaceOptionsOverride(this.fieldName);

                if (efuio == null)
                {
                    efuio = this.Field.UserInterfaceOptions;
                }

                return efuio;
            }
        }

        public FieldControl()
        {
            this.propertyChanged = fs_PropertyChanged;
        }

        protected override void OnUpdate()
        {
            if (this.Item == null || this.Form == null || this.Field == null)
            {
                return;
            }

            this.UnhookExistingEvents();

            this.Field.PropertyChanged += this.propertyChanged;

            this.Field.UserInterfaceOptions.PropertyChanged += this.propertyChanged;

            FieldSettings fs = this.Form.Settings.FieldSettingsCollection.GetFieldByName(this.fieldName);

            if (fs != null)
            {
                fs.PropertyChanged += this.propertyChanged;

                if (fs.UserInterfaceOptionsOverride != null)
                {
                    fs.UserInterfaceOptionsOverride.PropertyChanged += this.propertyChanged;
                }
            }
        }

        private void fs_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Update();
        }

        public override void Dispose()
        {
            base.Dispose();

            this.UnhookExistingEvents();
        }

        private void UnhookExistingEvents()
        {
            if (this.Item == null || this.Form == null || this.Field == null)
            {
                return;
            }

            if (this.Field != null)
            {
                this.Field.PropertyChanged -= this.propertyChanged;
            }

            if (this.Field.UserInterfaceOptions != null)
            {
                this.Field.UserInterfaceOptions.PropertyChanged -= this.propertyChanged;
            }

            FieldSettings fs = this.Form.Settings.FieldSettingsCollection.GetFieldByName(this.fieldName);

            if (fs != null)
            {
                fs.PropertyChanged -= this.propertyChanged;

                if (fs.UserInterfaceOptionsOverride != null)
                {
                    fs.UserInterfaceOptionsOverride.PropertyChanged -= this.propertyChanged;
                }
            }
        }

        protected void ApplyToControl(FieldControl fc)
        {
            fc.Form = this.Form;
            fc.Item = this.Item;
            fc.Mode = this.Mode;
            fc.FieldName = this.FieldName;
        }
    }
}
