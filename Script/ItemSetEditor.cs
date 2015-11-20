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
    public enum ItemSetEditorMode
    {
        Rows = 0,
        TemplatePlacedItems = 1,
        Linear = 2
    }

    public class ItemSetEditor : Control, IItemSetEditor
    {
        private DataStoreItemSetEventHandler itemSetChanged;
        
        private IDataStoreItemSet itemSet;
        private ImageBrowserOptions defaultImageBrowserOptions;
        private String[] defaultStylesheets;

        private FormMode formMode = FormMode.EditForm;

        private ItemSetInterface itemSetInterface;
        private String itemFormTemplateId;
        private String itemFormTemplateIdSmall;
        private String itemPlacementFieldName;

        private bool displayAddAndDeleteButtons = true;
        private bool displayPersistButton = true;

        private bool isReadOnly = false;

        private String addItemCta;
        private Dictionary<String, Form> formsByItemId;
        private List<Form> forms;

        private ItemSetEditorMode mode;
        private PropertyChangedEventHandler itemSetInterfacePropertyChanged;
        private PropertyChangedEventHandler fieldPropertyChanged;
        private NotifyCollectionChangedEventHandler fieldInterfaceCollectionChanged;
        private event DataStoreItemChangedEventHandler itemChangedEventHandler;

        public event DataStoreItemEventHandler ItemAdded;
        public event DataStoreItemEventHandler ItemDeleted;

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

        public String[] DefaultStylesheets
        {
            get
            {
                return this.defaultStylesheets;
            }

            set
            {
                this.defaultStylesheets = value;
            }
        }

        protected List<Form> Forms
        {
            get
            {
                if (this.forms == null)
                {
                    this.forms = new List<Form>();
                }

                return this.forms;
            }
        }

        [ScriptName("i_formMode")]
        public FormMode FormMode
        {
            get
            {
                return this.formMode;
            }

            set
            {
                if (this.formMode == value)
                {
                    return;
                }

                this.formMode = value;

                if (this.forms != null)
                {
                    foreach (Form f in this.forms)
                    {
                        f.Mode = this.formMode;
                    }
                }
            }
        }
        
        public ItemSetInterface ItemSetInterface
        {
            get
            {
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
                    this.itemSetInterface.PropertyChanged -= this.itemSetInterfacePropertyChanged;
                    this.itemSetInterface.FieldInterfaces.CollectionChanged -= this.fieldInterfaceCollectionChanged;
                }

                this.itemSetInterface = value;


                if (this.forms != null)
                {
                    foreach (Form f in this.forms)
                    {
                        f.ItemSetInterface = this.itemSetInterface;
                    }
                }

                this.Update();

                this.itemSetInterface.PropertyChanged += this.itemSetInterfacePropertyChanged;
                this.itemSetInterface.FieldInterfaces.CollectionChanged += this.fieldInterfaceCollectionChanged;
            }
        }

        [ScriptName("b_isReadOnly")]
        public bool IsReadOnly
        {
            get
            {
                return this.isReadOnly;
            }

            set
            {
                this.isReadOnly = value;
            }
        }

        public String ItemPlacementFieldName
        {
            get
            {
                return this.itemPlacementFieldName;
            }

            set
            {
                if (this.itemPlacementFieldName == value)
                {
                    return;
                }

                this.itemPlacementFieldName = value;

                this.Update();
            }
        }

        public String ItemFormTemplateId
        {
            get
            {
                return this.itemFormTemplateId;
            }

            set
            {
                if (this.itemFormTemplateId == value)
                {
                    return;
                }

                this.itemFormTemplateId = value;

                if (!Context.Current.IsSmallFormFactor)
                {
                    if (this.forms != null)
                    {
                        foreach (Form f in this.forms)
                        {
                            f.TemplateId = this.itemFormTemplateId;
                        }
                    }
                }

                this.Update();
            }
        }

        public String ItemFormTemplateIdSmall
        {
            get
            {
                return this.itemFormTemplateIdSmall;
            }

            set
            {
                if (this.itemFormTemplateIdSmall == value)
                {
                    return;
                }

                this.itemFormTemplateIdSmall = value;

                if (Context.Current.IsSmallFormFactor)
                {
                    if (this.forms != null)
                    {
                        foreach (Form f in this.forms)
                        {
                            f.TemplateId = this.itemFormTemplateIdSmall;
                        }
                    }
                }

                this.Update();
            }
        }

        public ItemSetEditorMode Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                if (this.mode == value)
                {
                    return;
                }

                this.mode = value;

                this.Update();
            }
        }
        
        public String AddItemCta
        {
            get
            {
                return this.addItemCta;
            }

            set
            {
                if (this.addItemCta == value)
                {
                    return;
                }

                this.addItemCta = value;

                this.OnAddItemCtaChange();
            }
        }

        [ScriptName("b_displayPersistButton")]
        public bool DisplayPersistButton
        {
            get
            {
                return this.displayPersistButton;
            }

            set
            {
                this.displayPersistButton = value;

                this.OnPersistVisibilityChange();
            }
        }

        [ScriptName("b_displayAddAndDeleteButtons")]
        public bool DisplayAddAndDeleteButtons
        {
            get
            {
                return this.displayAddAndDeleteButtons;
            }

            set
            {
                if (this.displayAddAndDeleteButtons == value)
                {
                    return;
                }

                this.displayAddAndDeleteButtons = value;

                OnAddAndDeleteVisibilityChange();
            }
        }

        public IDataStoreItemSet ItemSet
        {
            get
            {
                return this.itemSet;
            }

            set
            {
                if (this.itemSet == value)
                {
                    return;
                }

                if (this.itemSet != null)
                {
                    this.itemSet.ItemSetChanged -= this.itemSetChanged;
                    this.itemSet.ItemInSetChanged -= this.itemChangedEventHandler;
                }

                this.itemSet = value;

                if (this.itemSet != null)
                {
                    this.itemSet.ItemSetChanged += this.itemSetChanged;
                    this.itemSet.ItemInSetChanged += this.itemChangedEventHandler;

                    this.itemSet.BeginRetrieve(this.ItemsRetrieved, null);
                }
                else
                {
                    this.Update();
                }
            }
        }

        public ItemSetEditor()
        {
            this.formsByItemId = new Dictionary<string, Form>();

            this.itemSetChanged = this.itemSet_ItemSetChanged;
            this.itemChangedEventHandler = this.item_ItemChanged;

            this.itemSetInterfacePropertyChanged = itemSetOrFieldInterface_PropertyChanged;
            this.fieldPropertyChanged = itemSetOrFieldInterface_PropertyChanged;
            this.fieldInterfaceCollectionChanged = FieldInterfaceCollection_CollectionChanged;

            this.EnqueueUpdates = true;
        }

        public void SetItemSetInterfaceAndItems(ItemSetInterface isi, IDataStoreItemSet newItemSet)
        {
            if (this.itemSet == newItemSet && this.itemSetInterface == isi)
            {
                return;
            }

            if (this.itemSetInterface != null)
            {
                this.itemSetInterface.PropertyChanged -= this.itemSetInterfacePropertyChanged;
                this.itemSetInterface.FieldInterfaces.CollectionChanged -= this.fieldInterfaceCollectionChanged;
            }

            this.itemSetInterface = isi;
            this.itemSetInterface.PropertyChanged += this.itemSetInterfacePropertyChanged;
            this.itemSetInterface.FieldInterfaces.CollectionChanged += this.fieldInterfaceCollectionChanged;

            if (this.itemSet != null)
            {
                this.itemSet.ItemSetChanged -= this.itemSetChanged;
            }

            this.itemSet = newItemSet;

            if (this.itemSet != null)
            {
                this.itemSet.ItemSetChanged += this.itemSetChanged;

                this.itemSet.BeginRetrieve(this.ItemsRetrieved, null);
            }
            else
            {
                this.Update();
            }
        }

        protected virtual void SetDefaultItemValues(IItem item)
        {
            // set default data values, where applicable
            foreach (IDataStoreField f in this.ItemSet.Type.Fields)
            {
                FieldInterface fi = this.ItemSetInterface.FieldInterfaces.GetFieldByName(f.Name);

                if (f.Type == FieldType.Integer)
                {
                    if (fi != null && fi.InterfaceTypeOptionsOverride != null && fi.InterfaceTypeOptionsOverride.IntDefaultValue != null)
                    {
                        item.SetInt32Value(f.Name, (Int32)fi.InterfaceTypeOptionsOverride.IntDefaultValue);
                    }
                    else if (!Script.IsNullOrUndefined(f.InterfaceTypeOptions) && f.InterfaceTypeOptions.IntDefaultValue != null)
                    {
                        item.SetInt32Value(f.Name, (Int32)f.InterfaceTypeOptions.IntDefaultValue);
                    }
                }
            }
        }

        protected virtual void EnsureItemIsAtEndOfList(IItem item)
        {
            // if the type has an order field, make sure that this new item is at the bottom of the list.
            foreach (FieldInterface fieldInterface in this.ItemSetInterface.FieldInterfaces)
            {
                if (fieldInterface.InterfaceTypeOverride == FieldInterfaceType.Order)
                {
                    int maxOrder = 0;

                    foreach (IItem existingItem in this.ItemSet.Items)
                    {
                        int? itemOrder = existingItem.GetInt32Value(fieldInterface.Name);

                        if (itemOrder != null && itemOrder > maxOrder)
                        {
                            maxOrder = (int)itemOrder;
                        }
                    }

                    item.SetInt32Value(fieldInterface.Name, maxOrder + 10);
                }
            }
        }

        protected virtual void NotifyItemAdded(IItem item)
        {
            if (this.ItemAdded != null)
            {
                DataStoreItemEventArgs dsiea = new DataStoreItemEventArgs(item);

                this.ItemAdded(this, dsiea);
            }
        }

        protected virtual void NotifyItemDeleted(IItem item)
        {
            if (this.ItemDeleted != null)
            {
                DataStoreItemEventArgs dsiea = new DataStoreItemEventArgs(item);

                this.ItemDeleted(this, dsiea);
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.ItemSet != null)
            {
                foreach (Field field in this.ItemSet.Type.Fields)
                {
                    FieldInterface fs = this.ItemSetInterface.FieldInterfaces.GetFieldByName(field.Name);

                    if (fs != null)
                    {
                        fs.PropertyChanged -= this.fieldPropertyChanged;
                        fs.PropertyChanged += this.fieldPropertyChanged;
                    }
                }
            }
        }


        public virtual void DisposeItemInterfaceItems()
        {

        }

        protected virtual void OnAddAndDeleteVisibilityChange()
        {
        }

        protected virtual void OnPersistVisibilityChange()
        {
        }
        protected virtual void OnAddItemCtaChange()
        {
        }

        public virtual void Save()
        {

        }

        public bool? GetFieldRequiredOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldRequiredOverride(fieldName);
        }

        public Nullable<FieldInterfaceType> GetFieldInterfaceTypeOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldInterfaceTypeOverride(fieldName);
        }

        public FieldInterfaceTypeOptions GetFieldInterfaceTypeOptionsOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldInterfaceTypeOptionsOverride(fieldName);
        }

        private void itemSetOrFieldInterface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Update();
        }

        private void FieldInterfaceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Update();
        }
        
        private void ItemsRetrieved(IAsyncResult result)
        {
            this.Update();
        }
        
        private void item_ItemChanged(object sender, DataStoreItemChangedEventArgs e)
        {
            this.OnItemInSetChanged(e);
        }

        protected virtual void OnItemInSetChanged(DataStoreItemChangedEventArgs e)
        {

        }

        private void itemSet_ItemSetChanged(object sender, DataStoreItemSetEventArgs e)
        {
            this.OnItemSetChanged(e);
        }

        protected virtual void OnItemSetChanged(DataStoreItemSetEventArgs e)
        {

        }

        public String GetFieldTitleOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldTitleOverride(fieldName);
        }

        public DisplayState GetAdjustedDisplayState(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetAdjustedDisplayState(fieldName);
        }

        protected Form EnsureForm(IItem item)
        {
            if (this.formsByItemId.ContainsKey(item.Id))
            {
                return this.formsByItemId[item.Id];
            }

            Form f = new Form();

            this.formsByItemId[item.Id] = f;

            return f;
        }
    }
}
