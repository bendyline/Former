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

        private IDataStoreField lastField;
        private FieldInterface fieldInterface;

        public FieldInterface Settings
        {
            get
            {
                return this.Form.ItemSetInterface.FieldInterfaces.GetFieldByName(this.FieldName);
            }
        }

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

        public FieldInterfaceTypeOptions EffectiveUserInterfaceOptions
        {
            get
            {
                FieldInterfaceTypeOptions efuio = this.Form.GetFieldInterfaceTypeOptionsOverride(this.fieldName);

                if (efuio == null)
                {
                    efuio = this.Field.InterfaceTypeOptions;
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

            if (this.Field != this.lastField && this.Field != null)
            {
                this.UnhookFieldEvents(this.lastField);

                this.lastField = this.Field;

                this.Field.PropertyChanged += this.propertyChanged;

                this.Field.InterfaceTypeOptions.PropertyChanged += this.propertyChanged;
            }

            FieldInterface fs = this.Form.ItemSetInterface.FieldInterfaces.GetFieldByName(this.fieldName);

            if (fs != null && fs != this.fieldInterface)
            {
                this.UnhookFieldInterfaceEvents(this.fieldInterface);

                this.fieldInterface = fs;

                fs.PropertyChanged += this.propertyChanged;

                // Debug.WriteLine("(FieldControl::OnUpdate) - Registering field interface property changed. Events: " + fs.GetPropertyChangedEventCount());

                if (fs.InterfaceTypeOptionsOverride != null)
                {
                    fs.InterfaceTypeOptionsOverride.PropertyChanged += this.propertyChanged;
                }
            }
        }

        private void fs_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Update();
        }

        public override void Dispose()
        {
            this.UnhookFieldEvents(this.Field);
            this.UnhookFieldInterfaceEvents(this.fieldInterface);

            base.Dispose();
        }


        private void UnhookFieldEvents(IDataStoreField lastField)
        {
            if (lastField == null)
            {
                return;
            }

            lastField.PropertyChanged -= this.propertyChanged;

            if (lastField.InterfaceTypeOptions != null)
            {
                lastField.InterfaceTypeOptions.PropertyChanged -= this.propertyChanged;
            }
        }

        private void UnhookFieldInterfaceEvents(FieldInterface lastFieldInterface)
        {
            if (lastFieldInterface == null)
            {
                return;
            }

            lastFieldInterface.PropertyChanged -= this.propertyChanged;
            
            // Debug.WriteLine("(FieldControl::UnhookExistingEvents) - Detaching field events for " + this.fieldName + ". Events: " + lastFieldInterface.GetPropertyChangedEventCount());

            if (lastFieldInterface.InterfaceTypeOptionsOverride != null)
            {
                lastFieldInterface.InterfaceTypeOptionsOverride.PropertyChanged -= this.propertyChanged;
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
