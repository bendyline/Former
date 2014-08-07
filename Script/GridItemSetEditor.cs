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
using BL.UI.KendoControls;
using Kendo.UI;
using kendo.data;

namespace BL.Forms
{

    public class GridItemSetEditor : Control, IItemSetEditor
    {
        [ScriptName("c_grid")]
        private BL.UI.KendoControls.Grid grid;

        private List<IItem> itemsShown;

        private IDataStoreItemSet itemSet;

        private FormSettings formSettings;
        private String itemFormTemplateId;
        private String itemPlacementFieldName;

        [ScriptName("e_addButton")]
        private InputElement addButton;

        private bool showAddButton = true;
        private String addItemCta;

        private ItemSetEditorMode mode;
        private PropertyChangedEventHandler propertyChanged;
        private NotifyCollectionChangedEventHandler collectionChanged;
        public event DataStoreItemEventHandler ItemAdded;

        public FormSettings FormSettings
        {
            get
            {
                return this.formSettings;
            }

            set
            {
                if (this.formSettings == value)
                {
                    return;
                }

                if (this.formSettings != null)
                {
                    this.formSettings.FieldSettingsCollection.CollectionChanged -= this.collectionChanged;
                }

                this.formSettings = value;

                this.formSettings.FieldSettingsCollection.CollectionChanged += this.collectionChanged;
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
                this.itemPlacementFieldName = value;
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
                this.itemFormTemplateId = value;
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
                this.mode = value;
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
                this.addItemCta = value;
            }
        }

        public bool ShowAddButton
        {
            get
            {
                return this.showAddButton;
            }

            set
            {
                this.showAddButton = value;

                this.ApplyAddButtonVisibility();
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
                    this.itemSet.ItemSetChanged -= itemSet_ItemSetChanged;
                }

                this.itemSet = value;

                if (this.itemSet != null)
                {
                    this.itemSet.ItemSetChanged += itemSet_ItemSetChanged;

                    this.itemSet.BeginRetrieve(this.ItemsRetrieved, null);
                }
                else
                {
                    this.Update();
                }
            }
        }

        public GridItemSetEditor()
        {
            this.itemsShown = new List<IItem>();
            this.propertyChanged = fs_PropertyChanged;
            this.collectionChanged = FieldSettingsCollection_CollectionChanged;
        }

        private void ApplyAddButtonVisibility()
        {
            if (this.addButton == null)
            {
                return;
            }

            if (this.showAddButton)
            {
                this.addButton.Style.Display = "block";
            }
            else
            {
                this.addButton.Style.Display = "none";
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.addButton != null)
            {
                this.addButton.AddEventListener("mousedown", this.AddButtonClick, true);
                this.addButton.AddEventListener("touchstart", this.AddButtonClick, true);

                this.ApplyAddButtonVisibility();
            }

            this.grid.Save += this.HandleSave; 
        }

        private void HandleSave(object sender, ObjectEventArgs oe)
        {
            object model = oe.Value;

            if (model != null)
            {
                int? id = null;

                Script.Literal("{0}={1}.Id", id, model);

                if (id != null)
                {
                    IItem item = this.ItemSet.GetItemById(id.ToString());

                    if (item != null)
                    {
                        Item.SetDataObject(this.ItemSet, item, model);
                    }
                }
            }
        }

        private void fs_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Update();
        }


        private void FieldSettingsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Update();
        }


        private void AddButtonClick(ElementEvent e)
        {
            IItem item = this.itemSet.Type.CreateItem();

            this.itemSet.Add(item);

            int index = this.itemSet.Items.Count - 1;
 
            if (this.ItemAdded != null)
            {
                DataStoreItemEventArgs dsiea = new DataStoreItemEventArgs(item);

                this.ItemAdded(this, dsiea);
            }
        }

        private void ItemsRetrieved(IAsyncResult result)
        {
            this.Update();
        }

        public void Save()
        {
     
        }

        private void itemSet_ItemSetChanged(object sender, DataStoreItemSetEventArgs e)
        {
            this.Update();
        }


        private int CompareFields(Field fieldA, Field fieldB)
        {
            FieldSettingsCollection fsc = this.FormSettings.FieldSettingsCollection;

            FieldSettings fieldSettingsA = fsc.GetFieldByName(fieldA.Name);
            FieldSettings fieldSettingsB = fsc.GetFieldByName(fieldB.Name);

            if (fieldSettingsA == null && fieldSettingsB == null)
            {
                return fieldA.Name.CompareTo(fieldB.Name);
            }

            int orderA = -1;

            if (fieldSettingsA != null)
            {
                orderA = fieldSettingsA.Order;
            }


            int orderB = -1;

            if (fieldSettingsB != null)
            {
                orderB = fieldSettingsB.Order;
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

        public AdjustedFieldState GetAdjustedFieldState(String fieldName)
        {
            return this.FormSettings.FieldSettingsCollection.GetAdjustedFieldState(fieldName);
        }
 
        protected override void OnUpdate()
        {
            if (this.grid == null)
            {
                return;
            }

            if (this.addItemCta != null)
            {
                this.addButton.Value = this.addItemCta;
            }           

            if (this.itemSet != null)
            {
                GridOptions go = new GridOptions();

                go.Editable = "inline";
                List<GridColumn> columns = new List<GridColumn>();

                go.Columns = columns;

                List<Field> sortedFields = new List<Field>();

                foreach (Field field in this.ItemSet.Type.Fields)
                {
                    sortedFields.Add(field);
                }

                sortedFields.Sort(this.CompareFields);

                ModelOptions m = new ModelOptions();
                m.Fields = new Dictionary<string, ModelField>();
               
                foreach (Field field in sortedFields)
                {
                    FieldSettings fs = this.FormSettings.FieldSettingsCollection.GetFieldByName(field.Name);

                    if (fs != null)
                    {
                        fs.PropertyChanged -= this.propertyChanged;
                        fs.PropertyChanged += this.propertyChanged;
                    }

                    AdjustedFieldState afs = this.GetAdjustedFieldState(field.Name);

                    if (afs == AdjustedFieldState.Show)
                    {
                        ModelField mf = new ModelField();

                       GridColumn gc = new GridColumn();
                       String fieldTitleOverride = fs.TitleOverride;

                       if (fieldTitleOverride != null)
                       {
                           gc.Title = fs.TitleOverride;
                       }
                       else
                       {
                           gc.Title = field.DisplayName;
                       }

                        gc.Field = field.Name;
                        columns.Add(gc);

                        if (field.Type == FieldType.BigInteger || field.Type == FieldType.BigNumber || field.Type == FieldType.Integer)
                        {
                            mf.Type = "number";
                        }
                        else if (field.Type == FieldType.BoolChoice)
                        {
                            mf.Type = "boolean";
                        }
                        else
                        {
                            mf.Type = "string";
                        }

                        if (fs.FieldModeOverride == FieldMode.View)
                        {
                            mf.Editable = false;
                        } 
                        else
                        {
                            mf.Editable = true;
                        }

                        m.Fields[gc.Name] = mf;
                    }
                }

                GridColumn gcCommand = new GridColumn();
                gcCommand.Command = new String[] { "edit", "destroy" };

                go.Columns.Add(gcCommand);

                List<object> objects = new List<object>();

                foreach (IItem item in this.ItemSet.Items)
                {
                    object o = Item.GetDataObject(this.ItemSet, item);
                    objects.Add(o);
                }

                DataSourceOptions dso= new DataSourceOptions();
                dso.Schema = new Schema();
                dso.Schema.Model = m;

                dso.Data = objects;
                DataSource ds = new DataSource(dso);
                
                go.DataSource = ds;
                this.grid.Options = go;


                ds.Read();
            }
        }
    }
}
