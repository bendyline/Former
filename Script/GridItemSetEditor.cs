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
using System.Serialization;

namespace BL.Forms
{

    public class GridItemSetEditor : Control, IItemSetEditor
    {
        [ScriptName("e_gridContainer")]
        private Element gridContainer;

        [ScriptName("c_persist")]
        private PersistButton persist;

        private BL.UI.KendoControls.Grid grid;

        private DataStoreItemEventHandler itemInSetChanged;
        private DataStoreItemSetEventHandler itemSetChanged;

        private ImageBrowserOptions defaultImageBrowserOptions;

        private List<IItem> itemsShown;

        private List<Field> userListFields;
        private IDataStoreItemSet itemSet;
        private DataSource activeDataSource;


        private FormMode formMode = FormMode.EditForm;

        private ItemSetInterface itemSetInterface;
        private String itemFormTemplateId;
        private String itemFormTemplateIdSmall;
        private String itemPlacementFieldName;

        [ScriptName("e_addButton")]
        private InputElement addButton;

        [ScriptName("e_exportButton")]
        private InputElement exportButton;

        private bool displayAddButton = true;
        private bool displayPersistButton = true;

        private bool isEditingRow = false;
        private String addItemCta;

        private ItemSetEditorMode mode;
        private PropertyChangedEventHandler itemSetInterfacePropertyChanged;
        private PropertyChangedEventHandler fieldPropertyChanged;
        private NotifyCollectionChangedEventHandler fieldInterfaceCollectionChanged;
        public event DataStoreItemEventHandler ItemAdded;
        public event DataStoreItemEventHandler ItemDeleted;

        private event ModelEventHandler gridSave;
        private event ModelEventHandler gridEdit;
        private event ModelEventHandler gridRemove;
        private event ModelEventHandler gridCancel;

        private Dictionary<String, Form> formsByItemId;

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
            }
        }

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

                this.Update();

                this.itemSetInterface.PropertyChanged += this.itemSetInterfacePropertyChanged;
                this.itemSetInterface.FieldInterfaces.CollectionChanged += this.fieldInterfaceCollectionChanged;
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

                return;
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

                this.Update();
            }
        }

        public bool DisplayAddButton
        {
            get
            {
                return this.displayAddButton;
            }

            set
            {
                if (this.displayAddButton == value)
                {
                    return;
                }

                this.displayAddButton = value;

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
                    this.itemSet.ItemSetChanged -= this.itemSetChanged;
                }

                this.itemSet = value;

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
        }

        public GridItemSetEditor()
        {
            KendoControlFactory.EnsureKendoBaseUx(this);
            KendoControlFactory.EnsureKendoData(this);

            this.itemsShown = new List<IItem>();
            this.formsByItemId = new Dictionary<string, Form>();

            this.itemInSetChanged = this.itemSet_ItemInSetChanged;
            this.itemSetChanged = this.itemSet_ItemSetChanged;

            this.itemSetInterfacePropertyChanged = itemSetOrFieldInterface_PropertyChanged;
            this.fieldPropertyChanged = itemSetOrFieldInterface_PropertyChanged;
            this.fieldInterfaceCollectionChanged = FieldInterfaceCollection_CollectionChanged;


            this.gridSave = this.HandleSave;
            this.gridEdit =  this.HandleEdit;
            this.gridCancel = this.HandleCancel;
            this.gridRemove = this.grid_Remove;

        }

        protected override void OnDimensionChanged()
        {
            base.OnDimensionChanged();

            if (this.Height != null && this.grid != null)
            {
                this.grid.Height = ((int)this.Height) - 35;
            }
        }


        public void DisposeItemInterfaceItems()
        {

        }

        private void ApplyAddButtonVisibility()
        {
            if (this.addButton == null)
            {
                return;
            }

            if (this.displayAddButton)
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

            if (this.exportButton != null)
            {
                this.exportButton.AddEventListener("mousedown", this.ExportButtonClick, true);
                this.exportButton.AddEventListener("touchstart", this.ExportButtonClick, true);
            }
        }

        private void grid_Remove(ModelEventArgs e)
        {
            Model model = e.Model;

            if (model != null)
            {
                String localId = (String)model.Get("LocalOnlyUniqueId");

                if (!String.IsNullOrEmpty(localId))
                {
                    IItem item = this.itemSet.GetItemByLocalOnlyUniqueId(localId);

                    if (item != null)
                    {
                        item.DeleteItem();

                        if (this.ItemDeleted != null)
                        {
                            DataStoreItemEventArgs dsiea = new DataStoreItemEventArgs(item);

                            this.ItemDeleted(this, dsiea);
                        }
                    }
                }
            }
        }

        private void HandleCancel(ModelEventArgs oe)
        {
            this.isEditingRow = false;

            this.UpdateIsEditing();
        }

        private void HandleEdit(ModelEventArgs oe)
        {
            this.isEditingRow = true;

            this.UpdateIsEditing();
      /*      Model model = oe.Model;

            if (model != null)
            {
                if (model.IsNew())
                {
                    String localId = (String)model.Get("LocalOnlyUniqueId");

                    if (String.IsNullOrEmpty(localId))
                    {
                        IItem item = this.itemSet.Type.CreateItem();

                        model.Set("LocalOnlyUniqueId", item.LocalOnlyUniqueId);

                        this.itemSet.Add(item);

                        if (this.ItemAdded != null)
                        {
                            DataStoreItemEventArgs dsiea = new DataStoreItemEventArgs(item);

                            this.ItemAdded(this, dsiea);
                        }
                    }
                }
            }*/
        }

        private void HandleSave(ModelEventArgs oe)
        {
            this.isEditingRow = false;

            this.UpdateIsEditing();

            Model model = oe.Model;

            if (model != null)
            {
                String id = null;

                Script.Literal("{0}={1}.id", id, model);

                IItem item = null;

                if (id != null)
                {
                    item = this.ItemSet.GetItemByLocalOnlyUniqueId(id.ToString());

                    if (item != null)
                    {
                        Item.SetDataObject(this.ItemSet, item, model);
                    }
                }
            }
        }

        private void itemSetOrFieldInterface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Update();
        }


        private void FieldInterfaceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Update();
        }

        private void UpdateIsEditing()
        {
            if (this.addButton == null)
            {
                return;
            }

            if (this.isEditingRow)
            {
                this.addButton.Disabled = true;
            }
            else
            {
                this.addButton.Disabled = false;
            }
        }

        private void ExportButtonClick(ElementEvent e)
        {
            this.grid.SaveAsExcel();
        }

        private void AddButtonClick(ElementEvent e)
        {
            Dictionary<String, object> newRow = new Dictionary<string, object>();
            IItem item = this.itemSet.Type.CreateItem();
            this.itemSet.Add(item);

            if (this.ItemAdded != null)
            {
                DataStoreItemEventArgs dsiea = new DataStoreItemEventArgs(item);

                this.ItemAdded(this, dsiea);
            }

            /*     this.grid.AddRow();

            ObservableObject oo = this.grid.DataItem;

            if (oo != null)
            {
                IItem item = this.itemSet.Type.CreateItem();
                oo.Set("LocalOnlyUniqueId", item.LocalOnlyUniqueId);

                this.itemSet.Add(item);
            }*/

        /*    
            this.grid.AddRow();

            int index = this.itemSet.Items.Count - 1;
 
            if (this.ItemAdded != null)
            {
                DataStoreItemEventArgs dsiea = new DataStoreItemEventArgs(item);

                this.ItemAdded(this, dsiea);
            }*/
        }

        private void ItemsRetrieved(IAsyncResult result)
        {
            this.Update();
        }

        public void Save()
        {
     
        }

        private void itemSet_ItemInSetChanged(object sender, DataStoreItemEventArgs e)
        {
            this.Update();
        }

        private void itemSet_ItemSetChanged(object sender, DataStoreItemSetEventArgs e)
        {
            if (e.AddedItems.Count >= 1 && this.activeDataSource != null)
            {
                foreach (IItem item in e.AddedItems)
                {
                    this.activeDataSource.Add(this.CreateDataObjectForItem(item));
                }
            }
            else
            {
                this.Update();
            }
        }


        private int CompareFields(Field fieldA, Field fieldB)
        {
            FieldInterfaceCollection fsc = this.ItemSetInterface.FieldInterfaces;

            FieldInterface fieldSettingsA = fsc.GetFieldByName(fieldA.Name);
            FieldInterface fieldSettingsB = fsc.GetFieldByName(fieldB.Name);

            if (fieldSettingsA == null && fieldSettingsB == null)
            {
                return fieldA.Name.CompareTo(fieldB.Name);
            }

            int orderA = -1;

            if (fieldSettingsA != null)
            {
                if (fieldSettingsA.Order == null)
                {
                    orderA = 0;
                }
                else
                {
                    orderA = (int)fieldSettingsA.Order;
                }
            }

            int orderB = -1;

            if (fieldSettingsB != null)
            {
                if (fieldSettingsB == null)
                {
                    orderB = 0;
                }
                else
                {
                    orderB = (int)fieldSettingsB.Order;
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

        public DisplayState GetAdjustedDisplayState(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetAdjustedDisplayState(fieldName);
        }
 
        protected override void OnUpdate()
        {
            if (this.gridContainer == null)
            {
                return;
            }

            ElementUtilities.ClearChildElements(this.gridContainer);

            if (this.grid != null)
            {
                this.grid.Save -= this.gridSave;
                this.grid.Edit -= this.gridEdit;
                this.grid.Cancel -= this.gridCancel;
                this.grid.Remove -= this.gridRemove; 
            }

            this.grid = new BL.UI.KendoControls.Grid();

            this.grid.Save += this.gridSave;
            this.grid.Edit += this.gridEdit;
            this.grid.Cancel += this.gridCancel;
            this.grid.Remove += this.gridRemove; 

            this.grid.EnsureElements();

            this.gridContainer.AppendChild(this.grid.Element);


            if (this.addItemCta != null)
            {
                this.addButton.Value = this.addItemCta;
            }

            if (this.persist != null)
            {
                if (this.displayPersistButton)
                {
                    this.persist.Element.Style.Display = "";
                }
                else
                {
                    this.persist.Element.Style.Display = "none";
                }

                this.persist.ItemSet = this.ItemSet;
                this.persist.ItemSetEditor = this;
            }


            if (this.itemSet != null)
            {
                GridOptions go = new GridOptions();
                go.Filterable = true;
                go.Sortable = new GridSortableOptions();
                go.Sortable.Mode = "single";
                go.Scrollable = true;
    //            go.Toolbar = new String[] { "create" };
                GridEditableOptions geo = new GridEditableOptions();
                geo.Mode = "inline";
                go.Editable = geo;

                List<GridColumn> columns = new List<GridColumn>();

                go.Columns = columns;

                List<Field> sortedFields = new List<Field>();

                foreach (Field field in this.ItemSet.Type.Fields)
                {
                    DisplayState afs = this.GetAdjustedDisplayState(field.Name);

                    if (afs == DisplayState.Show)
                    {
                        sortedFields.Add(field);
                    }
                }

                sortedFields.Sort(this.CompareFields);

                ModelOptions m = new ModelOptions();
                m.Fields = new Dictionary<string, ModelField>();
                m.Id = "id";

                ModelField mfId = new ModelField();
                mfId.Editable = false;
                m.Fields["id"] = mfId;

                int userFieldCount = 0;

                foreach (Field field in sortedFields)
                {
                    FieldInterface fs = this.ItemSetInterface.FieldInterfaces.GetFieldByName(field.Name);

                    if (fs != null)
                    {
                        fs.PropertyChanged -= this.fieldPropertyChanged;
                        fs.PropertyChanged += this.fieldPropertyChanged;
                    }

                    DisplayState afs = this.GetAdjustedDisplayState(field.Name);

                    if (afs == DisplayState.Show)
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

                        if ((field.InterfaceType == FieldInterfaceType.User && fs.InterfaceTypeOverride == null) || fs.InterfaceTypeOverride == FieldInterfaceType.User)
                        {

                            if (userFieldCount == 0)
                            {
                                this.userListFields = new List<Field>();
                            }


                            this.userListFields.Add(field);

                            // SUPER HACKY workaround: in a template display function, we don't know what field to pull to
                            // display the user for an item.
                            // so, use some pre-defined functions that will display using the appropriate field from the item
                            // base on the ordinal of the user field we're processing here.
                            switch (userFieldCount)
                            {
                                case 0:
                                    gc.Template = this.UserItemTemplateDisplay0;
                                    break;

                                case 1:
                                    gc.Template = this.UserItemTemplateDisplay1;
                                    break;


                                case 2:
                                    gc.Template = this.UserItemTemplateDisplay2;
                                    break;

                                case 3:
                                    gc.Template = this.UserItemTemplateDisplay3;
                                    break;

                                default:
                                    gc.Template = this.AmbiguousResponse;
                                    break;
                            }

                            userFieldCount++;
                        }

                        
                        if ((field.InterfaceType != FieldInterfaceType.TypeDefault && fs.InterfaceTypeOverride == null) || (fs.InterfaceTypeOverride != null && fs.InterfaceTypeOverride != FieldInterfaceType.TypeDefault))
                        {
                            gc.Editor = this.ColumnChoiceCreator;
                        }

                        if (fs.Mode == FieldMode.View)
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
                    objects.Add(CreateDataObjectForItem(item));
                }

                DataSourceOptions dso= new DataSourceOptions();
                dso.Schema = new Schema();
                dso.Schema.Model = m;

                dso.Data = objects;
                DataSource ds = new DataSource(dso);
               
                go.DataSource = ds;
                
                this.activeDataSource = ds;

                this.grid.Options = go;

                this.OnDimensionChanged();

                ds.Read();

                this.UpdateIsEditing();
            }
        }

        private String UserItemTemplateDisplay0(object item)
        {
            return this.GetUserValueByIndex(item, 0);
        }

        private String UserItemTemplateDisplay1(object item)
        {
            return this.GetUserValueByIndex(item, 1);
        }

        private String UserItemTemplateDisplay2(object item)
        {
            return this.GetUserValueByIndex(item, 2);
        }

        private String UserItemTemplateDisplay3(object item)
        {
            return this.GetUserValueByIndex(item, 3);
        }

        private String AmbiguousResponse(object item)
        {
            return String.Empty;
        }

        private String GetUserValueByIndex(object item, int index)
        {
            Field f = this.userListFields[index];

            String userFieldValue = null;

            Script.Literal("{0} = {1}[{2}]", userFieldValue, item, f.Name);

            if (userFieldValue == null)
            {
                return String.Empty;
            }

            userFieldValue = userFieldValue.Trim();

            if (userFieldValue.StartsWith("{") && userFieldValue.EndsWith("}"))
            {
                UserReference ur = new UserReference();

                ur.ApplyString(userFieldValue);

                return ur.NickName;
            }

            return userFieldValue;
        }

        private void ColumnChoiceCreator(jQueryObject container, GridColumnUserInterfaceFactoryOptions options)
        {
            String id = null;

            Script.Literal("{1} = {0}.model.LocalOnlyUniqueId", options, id);

            if (id != null)
            {
                IItem item = this.ItemSet.GetItemByLocalOnlyUniqueId(id);

                if (item != null)
                {
                    Item objectItem = new Item(item.Type);
                    objectItem.Data = options.Model;

                    Form f = this.EnsureForm(item);
                    f.ItemSetInterface = this.ItemSetInterface;
                    f.Item = objectItem;

                    FieldValue chfe = new FieldValue();

                  
                    chfe.Item = objectItem;
                    chfe.Form = f;
                    chfe.FieldName = options.Field;
                
                    chfe.EnsureElements();

                    container.GetElement(0).AppendChild(chfe.Element);
                }
            }
        }

        private Form EnsureForm(IItem item)
        {
            if (this.formsByItemId.ContainsKey(item.Id))
            {
                return this.formsByItemId[item.Id];
            }

            Form f = new Form();

            this.formsByItemId[item.Id] = f;

            return f;
        }

        private object CreateDataObjectForItem(IItem item)
        {
            object o = Item.GetDataObject(this.ItemSet, item);
            Script.Literal("{0}[\"id\"]={1}.get_localOnlyUniqueId()", o, item);

            return o;
        }
    }
}
