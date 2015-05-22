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
using Kendo.UI;

namespace BL.Forms
{
    public enum FormMode
    {
        EditForm = 0,
        NewForm = 1,
        ViewForm = 2,
        Example = 3
    }

    public enum DisplayState
    {
        DefaultState = 0,
        Hide = 1,
        Show = 2,
        ShowInDetailHideInList = 3,
        ShowInListHideInDetail = 4
    }

    public class Form : ItemControl, IForm
    {
        private ItemSetInterface itemSetInterface;
        private String iteratorFieldTemplateId;

        [ScriptName("c_fieldIterator")]
        private FieldIterator fieldIterator;

        [ScriptName("e_specialButtons")]
        private Element specialButtons;

        private Element deleteButton;

        private NotifyCollectionChangedEventHandler fieldSettingsChangeHandler;
        private PropertyChangedEventHandler formSettingsChangeHandler;

        public event DataStoreItemEventHandler ItemDeleted;

        private ImageBrowserOptions defaultImageBrowserOptions;

        public ImageBrowserOptions DefaultImageBrowserOptions
        {
            get
            {
                return this.defaultImageBrowserOptions;
            }

            set
            {
                this.defaultImageBrowserOptions = value;
            }
        }

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

        public bool IsValid
        {
            get
            {
                foreach (IDataStoreField f in this.Item.Type.Fields)
                {
                    if (!this.IsFieldValidForItem(f, this.Item))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public ItemSetInterface ItemSetInterface
        {
            get
            {
                if (this.itemSetInterface == null)
                {
                    this.itemSetInterface = new ItemSetInterface();
                    this.itemSetInterface.PropertyChanged += formSettingsChangeHandler;
                    this.itemSetInterface.FieldInterfaces.CollectionChanged += fieldSettingsChangeHandler;
                }

                return this.itemSetInterface;
            }

            set
            {
                if (this.itemSetInterface == value)
                {
                    return;
                }
                
                if (this.itemSetInterface != null)
                {
                    this.itemSetInterface.PropertyChanged -= formSettingsChangeHandler;
                    this.itemSetInterface.FieldInterfaces.CollectionChanged -= fieldSettingsChangeHandler;
                }

                this.itemSetInterface = value;

                this.itemSetInterface.PropertyChanged += formSettingsChangeHandler;
                this.itemSetInterface.FieldInterfaces.CollectionChanged += fieldSettingsChangeHandler;

                if (this.fieldIterator != null)
                {
                    this.fieldIterator.OnInterfaceChange();
                }


                this.Update();
            }
        }


        public FormMode Mode
        {
            get
            {
                return this.ItemSetInterface.FormMode;
            }

            set
            {
                this.ItemSetInterface.FormMode = value;


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
            this.fieldSettingsChangeHandler = this.FieldSettingsCollection_CollectionChanged;
            this.formSettingsChangeHandler = this.settings_PropertyChanged;

            this.EnqueueUpdates = true;
        }

        public bool IsFieldValidForItem(IDataStoreField field, IItem item)
        {
            bool? requiredOverride = this.GetFieldRequiredOverride(field.Name);

            if (requiredOverride == true || (requiredOverride == null && field.Required))
            {
                object value = item.GetValue(field.Name);

                if (value == null)
                {
                    return false;
                }

                if ((field.Type == FieldType.ShortText || field.Type == FieldType.UnboundedText || field.Type == FieldType.RichContent) && (String)value == String.Empty)
                {
                    return false;
                }
            }

            FieldInterface fi = this.itemSetInterface[field.Name];

            if (fi != null)
            {
                FieldInterfaceTypeOptions fito = this.GetFieldInterfaceTypeOptionsOverride(field.Name);
                Nullable<FieldInterfaceType> fit = this.GetFieldInterfaceTypeOverride(field.Name);
                
                if (fito == null)
                {
                    fito = fi.InterfaceTypeOptionsOverride;
                }


                if (fit == FieldInterfaceType.Email)
                {
                    String email = item.GetStringValue(field.Name);

                    if (!String.IsNullOrEmpty(email))
                    {
                        if (!Utilities.IsValidEmail(email))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool ContainsTemplateFieldControl(String fieldName)
        {
            foreach (Control c in this.TemplateControls)
            {
                if (c is FieldControl)
                {
                    if ( ((FieldControl)c).FieldName == fieldName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void DeleteItem()
        {
            this.Item.DeleteItem();

            if (this.ItemDeleted != null)
            {
                DataStoreItemEventArgs dsiea = new DataStoreItemEventArgs(this.Item);

                this.ItemDeleted(this, dsiea);
            }
        }

        public String GetFieldDisplayNameOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldTitleOverride(fieldName);
        }

        public bool? GetFieldRequiredOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldRequiredOverride(fieldName);
        }

        public bool? GetFieldAllowNullOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldAllowNullOverride(fieldName);
        }

        public FieldChoiceCollection GetFieldChoicesOverride(String fieldName)
        {
            FieldChoiceCollection fcc = this.ItemSetInterface.FieldInterfaces.GetFieldChoicesOverride(fieldName);

            return fcc;
        }

        public DisplayState GetAdjustedDisplayState(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetAdjustedDisplayState(fieldName);
        }

        public Nullable<FieldInterfaceType> GetFieldInterfaceTypeOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldInterfaceTypeOverride(fieldName);
        }

        public FieldInterfaceTypeOptions GetFieldInterfaceTypeOptionsOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldInterfaceTypeOptionsOverride(fieldName);
        }

        public FieldMode GetFieldModeOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldModeOverride(fieldName);
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

                this.ApplyToControl(this.fieldIterator);
            }
        }

        public virtual void Save(AsyncCallback callback, object state)
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
            if (e.Action != NotifyCollectionChangedAction.ItemStateChanged || (e.Action == NotifyCollectionChangedAction.ItemStateChanged && e.StateChangePropertyName == "Display"))
            {
                if (this.fieldIterator != null)
                {
                    this.fieldIterator.OnInterfaceChange();
                }

                this.OnSettingsChange();
            }
        }

        protected virtual void OnSettingsChange()
        {

        }

        private void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.fieldIterator != null)
            {
                this.fieldIterator.OnInterfaceChange();
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

            if (this.specialButtons != null && this.ItemSetInterface != null && this.ItemSetInterface.DisplayDeleteItemButton && this.Mode != FormMode.ViewForm)
            {
                if (this.deleteButton == null)
                {
                    this.deleteButton = (InputElement)this.CreateElementWithTypeAndAdditionalClasses("deleteButton", "BUTTON", "k-button");
                    this.deleteButton.AddEventListener("click", this.HandleItemDelete, true);

                    ElementUtilities.SetText(this.deleteButton, "");
                    this.specialButtons.AppendChild(this.deleteButton);
                }
                else
                {
                    this.deleteButton.Style.Display = "";
                }
            }
            else if (this.deleteButton != null)
            {
                this.deleteButton.Style.Display = "none";
            }
        }

        protected void HandleItemDelete(ElementEvent eventData)
        {
            this.DeleteItem();
        }

        public void ApplyToControl(Control c)
        {
            if (c is ItemControl)
            {
                ((ItemControl)c).ItemSet = this.ItemSet;
                ((ItemControl)c).Item = this.Item;
            }

            if (c is FormControl)
            {
                ((FormControl)c).Form = this;
            }    
        }
    }
}
