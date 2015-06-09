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
using kendo.ui;

namespace BL.Forms
{

    public class GridItemSetEditor : Control, IItemSetEditor
    {
        [ScriptName("e_gridContainer")]
        private Element gridContainer;

        [ScriptName("e_toolbar")]
        private Element toolbar;

        [ScriptName("c_persist")]
        private PersistButton persist;

        private BL.UI.KendoControls.Grid grid;

        private DataStoreItemEventHandler itemInSetChanged;
        private DataStoreItemSetEventHandler itemSetChanged;

        private ImageBrowserOptions defaultImageBrowserOptions;

        private List<IItem> itemsShown;

        private List<Field> userListFields;
        private List<Field> contentListFields;
        private List<Field> userListListFields; 
        
        private IDataStoreItemSet itemSet;
        private DataSource activeDataSource;
        private String saveAsFileNameBase;

        private FormMode formMode = FormMode.EditForm;

        private ItemSetInterface itemSetInterface;
        private String itemFormTemplateId;
        private String itemFormTemplateIdSmall;
        private String itemPlacementFieldName;

        [ScriptName("e_addButton")]
        private InputElement addButton;

        [ScriptName("e_deleteButton")]
        private InputElement deleteButton;

        [ScriptName("e_exportButton")]
        private InputElement exportButton;

        private bool displayAddAndDeleteButtons = true;
        private bool displayPersistButton = true;

        private bool isEditingRow = false;
        private bool isReadOnly = false;

        private String addItemCta;

        private ItemSetEditorMode mode;
        private PropertyChangedEventHandler itemSetInterfacePropertyChanged;
        private PropertyChangedEventHandler fieldPropertyChanged;
        private NotifyCollectionChangedEventHandler fieldInterfaceCollectionChanged;
        public event DataStoreItemEventHandler ItemAdded;
        public event DataStoreItemEventHandler ItemDeleted;

        private event ModelEventHandler gridSave;
        private event ModelEventHandler gridEdit;
        private event SelectionEventHandler gridChange;
        private event ModelEventHandler gridRemove;
        private event ModelEventHandler gridCancel;

        private jQueryObject activeSelectedObject;

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

        public String SaveAsFileNameBase
        {
            get
            {
                return this.saveAsFileNameBase;
            }

            set
            {
                this.saveAsFileNameBase = value;
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

                this.ApplyToolbarVisibility();
            }
        }

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

                this.ApplyAddAndDeleteButtonVisibility();
                this.ApplyToolbarVisibility();
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
            KendoUtilities.EnsureKendoBaseUx(this);
            KendoUtilities.EnsureKendoData(this);

            this.itemsShown = new List<IItem>();
            this.formsByItemId = new Dictionary<string, Form>();

            this.itemInSetChanged = this.itemSet_ItemInSetChanged;
            this.itemSetChanged = this.itemSet_ItemSetChanged;

            this.itemSetInterfacePropertyChanged = itemSetOrFieldInterface_PropertyChanged;
            this.fieldPropertyChanged = itemSetOrFieldInterface_PropertyChanged;
            this.fieldInterfaceCollectionChanged = FieldInterfaceCollection_CollectionChanged;


            this.gridSave = this.HandleSave;
            this.gridEdit =  this.HandleEdit;
            this.gridChange = this.HandleChange;
            this.gridCancel = this.HandleCancel;
            this.gridRemove = this.grid_Remove;

            this.EnqueueUpdates = true;
        }

        protected override void OnDimensionChanged()
        {
            base.OnDimensionChanged();

            if (this.Height != null && this.grid != null)
            {
                this.grid.Height = ((int)this.Height) - 35;
            }
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

        public void DisposeItemInterfaceItems()
        {

        }

        private void ApplyAddAndDeleteButtonVisibility()
        {
            if (this.addButton == null)
            {
                return;
            }

            if (this.displayAddAndDeleteButtons)
            {
                this.addButton.Style.Display = "";
                this.deleteButton.Style.Display = "";
            }
            else
            {
                this.addButton.Style.Display = "none";
                this.deleteButton.Style.Display = "none";
            }
        }

        private void ApplyToolbarVisibility()
        {
            if (this.toolbar == null)
            {
                return;

            }

            if (!this.displayAddAndDeleteButtons && !this.displayPersistButton)
            {
                this.toolbar.Style.Display = "none";
            }
            else
            {
                this.toolbar.Style.Display = String.Empty;
            }
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.addButton != null)
            {
                this.addButton.AddEventListener("mousedown", this.AddButtonClick, true);
                this.addButton.AddEventListener("touchstart", this.AddButtonClick, true);

                this.ApplyAddAndDeleteButtonVisibility();
            }

            if (this.exportButton != null)
            {
                this.exportButton.AddEventListener("mousedown", this.ExportButtonClick, true);
                this.exportButton.AddEventListener("touchstart", this.ExportButtonClick, true);
            }

            if (this.deleteButton != null)
            {
                this.deleteButton.Disabled = true;
            }

            this.ApplyToolbarVisibility();
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

        private void HandleChange(SelectionEventArgs oe)
        {
            if (oe.Elements.Count > 0)
            {
                if (this.activeSelectedObject != null)
                {
                    this.grid.SaveRow();
                }

                ICollection<Element> selectedElts = (ICollection<Element>)this.grid.Select(null);

                if (selectedElts.Count >= 1 && !this.isReadOnly)
                {
                    jQueryObject jqo = jQuery.FromElement(selectedElts[0]);

                    this.activeSelectedObject = jqo;

                    this.grid.EditRow(this.activeSelectedObject);
                    if (this.deleteButton != null)
                    {
                        this.deleteButton.Disabled = false;
                    }
                }
                else
                {
                    this.activeSelectedObject = null;
                    if (this.deleteButton != null)
                    {
                        this.deleteButton.Disabled = true;
                    }
                }
            }
        }

        private void EditActiveRow()
        {
            this.grid.EditRow(this.activeSelectedObject);
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

        public void SaveAsExcel()
        {
            this.grid.SaveAsExcel();
        }

        public void SaveAsPdf()
        {
            this.grid.SaveAsPdf();
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

                    if (item != null && ModelIsValid(item.Type, model))
                    {
                        Item.SetDataObject(this.ItemSet, item, model);
                    }
                }
            }
        }

        private bool ModelIsValid(IDataStoreType itemType, Model model)
        {
   
            foreach (IDataStoreField f in itemType.Fields)
            {
                if (!this.IsModelFieldValidForItem(f, model))
                {
                    return false;
                }
            }

            return true;
        
        }


        public bool IsModelFieldValidForItem(IDataStoreField field, Model model)
        {
            bool? requiredOverride = this.GetFieldRequiredOverride(field.Name);

            object value = null;

            Script.Literal("{0}={1}[{2}]", value, model, field.Name);

            if (requiredOverride == true || (requiredOverride == null && field.Required))
            {

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
                    String email = (String)value;

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

        [ScriptName("v_onDeleteButtonClick")]
        private void DeleteButtonClick(ElementEvent e)
        {
            if (this.activeSelectedObject != null)
            {
                this.grid.RemoveRow(this.activeSelectedObject);
                this.deleteButton.Disabled = true;
            }
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
                this.grid.Change -= this.gridChange;
                this.grid.Cancel -= this.gridCancel;
                this.grid.Remove -= this.gridRemove; 
            }

            this.grid = new BL.UI.KendoControls.Grid();

            this.grid.Save += this.gridSave;
            this.grid.Edit += this.gridEdit;
            this.grid.Change += this.gridChange;
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

                if (this.saveAsFileNameBase != null)
                {
                    String saveAsFileNameBaseSafe = KendoUtilities.SafeifyFileName(this.SaveAsFileNameBase);

                    go.Excel = new ExportFileOptions();
                    go.Excel.FileName = saveAsFileNameBaseSafe + ".xlsx";

                    go.Pdf = new ExportFileOptions();
                    go.Pdf.FileName = saveAsFileNameBaseSafe + ".pdf";
                }

                GridEditableOptions geo = new GridEditableOptions();
                geo.Mode = "inline";
                geo.Update = true;
                go.Editable = geo;
                go.Selectable = true;
                go.Resizable = true;

                List<GridColumn> columns = new List<GridColumn>();

                go.Columns = columns;

                List<Field> sortedFields = new List<Field>();

                foreach (Field field in this.ItemSet.Type.Fields)
                {
                    DisplayState afs = this.GetAdjustedDisplayState(field.Name);

                    if (afs == DisplayState.Show || afs == DisplayState.ShowInListHideInDetail)
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

                    if (afs == DisplayState.Show || afs == DisplayState.ShowInListHideInDetail)
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

                        if (this.GetFieldRequiredOverride(field.Name) == true)
                        {
                            mf.Validation = new ModelFieldValidation();
                            mf.Validation.Required = true;
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
                        else if (field.Type == FieldType.RichContent)
                        {
                            if (this.contentListFields == null)
                            {
                                this.contentListFields = new List<Field>();
                            }

                            this.contentListFields.Add(field);

                            // SUPER HACKY workaround: in a template display function, we don't know what field to pull to
                            // display the content for an item.
                            // so, use some pre-defined functions that will display using the appropriate field from the item
                            // base on the ordinal of the content field we're processing here.
                            switch (this.contentListFields.Count - 1)
                            {
                                case 0:
                                    gc.Template = this.ContentItemTemplateDisplay0;
                                    break;

                                case 1:
                                    gc.Template = this.ContentItemTemplateDisplay1;
                                    break;


                                case 2:
                                    gc.Template = this.ContentItemTemplateDisplay2;
                                    break;

                                case 3:
                                    gc.Template = this.ContentItemTemplateDisplay3;
                                    break;

                                default:
                                    gc.Template = this.AmbiguousResponse;
                                    break;
                            }
                        }
                        else if ((field.InterfaceType == FieldInterfaceType.UserList && fs.InterfaceTypeOverride == null) || fs.InterfaceTypeOverride == FieldInterfaceType.UserList)
                        {
                            if (this.userListListFields == null)
                            {
                                this.userListListFields = new List<Field>();
                            }

                            this.userListListFields.Add(field);

                            switch (this.userListListFields.Count - 1)
                            {
                                case 0:
                                    gc.Template = this.UserListItemTemplateDisplay0;
                                    break;

                                case 1:
                                    gc.Template = this.UserListItemTemplateDisplay1;
                                    break;


                                case 2:
                                    gc.Template = this.UserListItemTemplateDisplay2;
                                    break;

                                case 3:
                                    gc.Template = this.UserListItemTemplateDisplay3;
                                    break;

                                default:
                                    gc.Template = this.AmbiguousResponse;
                                    break;
                            }
                        }
                        else
                        {
                            // force the value of the column to have a nbsp at the end to force it to have a space.
                            String template = "#if (" + field.Name + " == null) {# #=''# #} else {# #: " + field.Name + "# #}#&\\#160;";
                            Script.Literal("{0}={1}", gc.Template, template);
                        }
                       
                        if (field.Type == FieldType.RichContent || 
                                    (field.InterfaceType != FieldInterfaceType.TypeDefault && fs.InterfaceTypeOverride == null) || 
                                    (fs.InterfaceTypeOverride != null && fs.InterfaceTypeOverride != FieldInterfaceType.TypeDefault)
                            )
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

                if (!this.isReadOnly)
                {
             /*       GridColumn gcCommand = new GridColumn();

                    gcCommand.Command = new String[] { "edit", "destroy" };
                    go.Columns.Add(gcCommand);*/
                }

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

        private String ContentItemTemplateDisplay0(object item)
        {
            return this.GetContentValueByIndex(item, 0);
        }

        private String ContentItemTemplateDisplay1(object item)
        {
            return this.GetContentValueByIndex(item, 1);
        }

        private String ContentItemTemplateDisplay2(object item)
        {
            return this.GetContentValueByIndex(item, 2);
        }

        private String ContentItemTemplateDisplay3(object item)
        {
            return this.GetContentValueByIndex(item, 3);
        }

        private String UserListItemTemplateDisplay0(object item)
        {
            return this.GetUserListValueByIndex(item, 0);
        }

        private String UserListItemTemplateDisplay1(object item)
        {
            return this.GetUserListValueByIndex(item, 1);
        }

        private String UserListItemTemplateDisplay2(object item)
        {
            return this.GetUserListValueByIndex(item, 2);
        }

        private String UserListItemTemplateDisplay3(object item)
        {
            return this.GetUserListValueByIndex(item, 3);
        }

        private String AmbiguousResponse(object item)
        {
            return String.Empty;
        }

        private String GetUserListValueByIndex(object item, int index)
        {
            Field f = this.userListListFields[index];

            String userListFieldValue = null;

            Script.Literal("{0} = {1}[{2}]", userListFieldValue, item, f.Name);

            if (String.IsNullOrEmpty(userListFieldValue))
            {
                return String.Empty;
            }

            String userList = String.Empty;

            UserReferenceSet urs = new UserReferenceSet();

            try
            {
                urs.LoadFromJson(userListFieldValue);
            }
            catch (Exception)
            {
                return String.Empty;
            }

            foreach (UserReference ur in urs.UserReferences)
            {
                if (userList != String.Empty)
                {
                    userList += ", ";
                }

                userList += ur.NickName;
            }

            return userList;
        }


        private String GetContentValueByIndex(object item, int index)
        {
            Field f = this.contentListFields[index];

            String contentFieldValue = null;

            Script.Literal("{0} = {1}[{2}]", contentFieldValue, item, f.Name);

            if (contentFieldValue == null)
            {
                return String.Empty;
            }
 
            return contentFieldValue;
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

                try
                {
                    ur.ApplyString(userFieldValue);
                }
                catch (Exception)
                {
                    return String.Empty;
                }

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
                    f.ItemSet = this.itemSet;
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
