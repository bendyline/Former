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
using BL.UI.KendoControls;

namespace BL.Forms
{
    public enum ItemSetEditorMode
    {
        Rows = 0,
        TemplatePlacedItems=1,
        Linear=2
    }


    public class ItemSetEditor : Control, IItemSetEditor
    {
        private IDataStoreItemSet itemSet;

        [ScriptName("e_formBin")]
        private Element formBin;

        [ScriptName("c_persist")]
        private PersistButton persist;

        private FormMode formMode = FormMode.EditForm;

        private ImageBrowserOptions defaultImageBrowserOptions;

        private String itemPlacementFieldName;

        private ItemSetEditorMode mode = ItemSetEditorMode.Rows;

        private List<Element> itemElements;

        private List<IItem> itemsShown;
        private Dictionary<String, Form> formsByLocalId;
        private List<Form> forms;

        private ItemSetInterface itemSetInterface;
        private String itemFormTemplateId;
        private String itemFormTemplateIdSmall;

        [ScriptName("e_addButton")]
        private InputElement addButton;

        private Dictionary<String, DropZoneTarget> dropZoneTargetsByLocalId;

        private bool reorderItemsOnNextUpdate = false;
        private bool useRowFormsIfPossible = true;
        private bool displayAddButton = true;
        private bool displayPersistButton = true;
        private String addItemCta;

        private Element headerRowElement;
        private event DataStoreItemSetEventHandler itemSetEventHandler;
        private event DataStoreItemChangedEventHandler itemChangedEventHandler;
        public event DataStoreItemEventHandler ItemAdded;
        public event DataStoreItemEventHandler ItemDeleted;

        private Form draggingForm;
        private Element draggingElement;

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

                foreach (Form f in this.forms)
                {
                    f.Mode = this.formMode;
                }
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

        [ScriptName("s_addItemCta")]
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

        public bool UseRowFormsIfPossible
        {
            get
            {
                return this.useRowFormsIfPossible;
            }

            set
            {
                this.useRowFormsIfPossible = value;
            }
        }

        public bool IsReorderable
        {
            get
            {
                return this.ItemSetInterface != null && this.ItemSetInterface.IsReorderable;
            }
        }

        [ScriptName("s_itemPlacementFieldName")]
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

        [ScriptName("i_mode")]
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

        [ScriptName("b_displayAddAndDeleteButtons")]
        public bool DisplayAddAndDeleteButtons
        {
            get
            {
                return this.displayAddButton;
            }

            set
            {
                this.displayAddButton = value;

                this.ApplyAddButtonVisibility();
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

                if (!Context.Current.IsSmallFormFactor)
                {
                    foreach (Form f in this.forms)
                    {
                        f.TemplateId = this.itemFormTemplateId;
                    }
                }
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
                this.itemFormTemplateIdSmall = value;

                if (Context.Current.IsSmallFormFactor)
                {
                    foreach (Form f in this.forms)
                    {
                        f.TemplateId = this.itemFormTemplateId;
                    }
                }
            }
        }

        public ItemSetInterface ItemSetInterface
        {
            get
            {
                if (this.itemSetInterface == null)
                {
                    this.itemSetInterface = new ItemSetInterface();
                }

                return this.itemSetInterface;
            }

            set
            {
                this.itemSetInterface = value;

                foreach (Form f in this.forms)
                {
                    f.ItemSetInterface = this.itemSetInterface;
                }

                this.Update();
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
                    this.itemSet.ItemSetChanged -= this.itemSetEventHandler;
                    this.itemSet.ItemInSetChanged -= this.itemChangedEventHandler;
                }

                this.itemSet = value;

                if (this.itemSet != null)
                {
                    this.itemSet.ItemSetChanged += this.itemSetEventHandler;
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
            KendoUtilities.EnsureKendoBaseUx(this);

            this.itemsShown = new List<IItem>();
            this.formsByLocalId = new Dictionary<String, Form>();
            this.forms = new List<Form>();
            this.dropZoneTargetsByLocalId = new Dictionary<string, DropZoneTarget>();

            this.itemSetEventHandler = this.itemSet_ItemSetChanged;
            this.itemChangedEventHandler = this.item_ItemChanged;

            this.EnqueueUpdates = true;
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

            this.itemElements = new List<Element>();

            for (int i=0; i<20; i++)
            {
                Element e = this.GetTemplateElementById("item" + i);

                if (e != null)
                {
                    while (this.itemElements.Count < i)
                    {
                        this.itemElements.Add(null);
                    }

                    this.itemElements[i] = e;
                }
            }
        }

        private void AddButtonClick(ElementEvent e)
        {
            IItem item = this.itemSet.Type.CreateItem();

            this.itemSet.Add(item);

            int placementIndex = this.itemSet.Items.Count - 1;
 
            if (this.ItemPlacementFieldName != null)
            {
                //IDataStoreField f = this.ItemSet.Type.GetField(this.ItemPlacementFieldName);

                placementIndex = (int)item.GetInt32Value(this.ItemPlacementFieldName);                
            }


            // set default data values, where applicable
            foreach (IDataStoreField f in this.itemSet.Type.Fields)
            {
                FieldInterface fi = this.itemSetInterface.FieldInterfaces.GetFieldByName(f.Name);

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

            this.EnsureFormForItem(item, placementIndex);

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
            foreach (Form f in this.forms)
            {
                f.Save(null, null);
            }
        }

        private void itemSet_ItemSetChanged(object sender, DataStoreItemSetEventArgs e)
        {
            if (e.RemovedItems != null)
            {
                foreach (IItem item in e.RemovedItems)
                {
                    Debug.WriteLine("(ItemSetEditor::itemSet_ItemSetChanged) - Item " + item.LocalOnlyUniqueId + " was removed.");

                    Form f = this.formsByLocalId[item.LocalOnlyUniqueId];

                    if (f != null)
                    {
                        this.RemoveFormFromDisplay(f);

                        this.formsByLocalId[item.LocalOnlyUniqueId] = null;
                        this.forms.Remove(f);

                        f.Dispose();
                    }
                }
            }

            this.Update();
        }

        private void RemoveFormFromDisplay(Form f)
        {
            if (ElementUtilities.ElementIsChildOf(f.Element, this.formBin))
            {
                this.formBin.RemoveChild(f.Element);
            }

            DropZoneTarget dtz = this.dropZoneTargetsByLocalId[f.Item.LocalOnlyUniqueId];

            if (dtz != null)
            {
                if (this.formBin != null && this.formBin.Contains(dtz.Element))
                {
                    this.formBin.RemoveChild(dtz.Element);
                }
            }


            this.itemsShown.Remove(f.Item);
        }
        
        private void EnsureHeaderRow()
        {
            if (this.formBin == null || this.ItemSet == null)
            {
                return;
            }

            if (this.ItemSetInterface != null && (!this.ItemSetInterface.DisplayColumns || Context.Current.IsSmallFormFactor || this.Mode == ItemSetEditorMode.Linear))
            {
                if (this.headerRowElement != null && ElementUtilities.ElementIsChildOf(this.headerRowElement, this.formBin))
                {
                    this.formBin.RemoveChild(this.headerRowElement);
                }

                return;
            }

            if (this.headerRowElement == null)
            {
                this.headerRowElement = this.CreateElement("headerRow");

                if (this.formBin.ChildNodes.Length > 0)
                {
                    this.formBin.InsertBefore(this.headerRowElement, this.formBin.ChildNodes[0]);
                }
                else
                {
                    this.formBin.AppendChild(this.headerRowElement);
                }
            }

            ElementUtilities.ClearChildElements(this.headerRowElement);

            List<Field> sortedFields = new List<Field>();

            foreach (Field field in this.ItemSet.Type.Fields)
            {
                sortedFields.Add(field);
            }

            sortedFields.Sort(this.ItemSetInterface.CompareFields);

            // add a placeholder cell for the drag-and-drop grippie column
            if (this.IsReorderable)
            {
                Element cellElement = this.CreateElement("headerCell");

                ElementUtilities.SetHtml(cellElement, "&#160;");

                this.headerRowElement.AppendChild(cellElement);
            }

            foreach (Field field in sortedFields)
            {
                DisplayState afs = this.GetAdjustedDisplayState(field.Name);

                if (afs == DisplayState.Show || afs == DisplayState.ShowInListHideInDetail)
                {
                    Element cellElement = this.CreateElement("headerCell");

                    String text = this.GetFieldTitleOverride(field.Name);

                    if (text == null)
                    {
                        text = String.Empty;
                    }

                    ElementUtilities.SetText(cellElement, text);

                    this.headerRowElement.AppendChild(cellElement);
                }
            }

            Element spacer = this.CreateElement("headerCell");
            spacer.Style.Width = "100%";
            ElementUtilities.SetHtml(spacer, "&#160;");
            this.headerRowElement.AppendChild(spacer);
        }

        private DisplayState GetAdjustedDisplayState(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetAdjustedDisplayState(fieldName);
        }

        public String GetFieldTitleOverride(String fieldName)
        {
            return this.ItemSetInterface.FieldInterfaces.GetFieldTitleOverride(fieldName);
        }

        private void EnsureFormForItem(IItem item, int index)
        {
            Form f = formsByLocalId[item.LocalOnlyUniqueId];

            if (f != null)
            {
                if (!this.itemsShown.Contains(item))
                {
                    f.ItemSetInterface = this.ItemSetInterface;
                    f.ItemSet = this.itemSet;
                    f.Item = item;
                    
                    this.itemsShown.Add(item);

                    this.InsertFormInOrder(f);
                }

                return;
            }

            if (this.mode == ItemSetEditorMode.Rows && !Context.Current.IsSmallFormFactor && this.useRowFormsIfPossible)
            {
                f = new RowForm();

                Debug.WriteLine("(ItemSetEditor::EnsureFormForItem) - Creating new rowform for item " + item.LocalOnlyUniqueId);
                f.IteratorFieldTemplateId = "bl-forms-horizontalunlabeledfield";
            }
            else
            {
                f = new Form();
                Debug.WriteLine("(ItemSetEditor::EnsureFormForItem) - Creating new form for item " + item.LocalOnlyUniqueId);
            }

            f.ItemDeleted += f_ItemDeleted;
            f.GrippieElementChanged += f_GrippieChanged;

            if (this.itemFormTemplateId != null && (!Context.Current.IsSmallFormFactor || this.itemFormTemplateIdSmall == null))
            {
                f.TemplateId = this.itemFormTemplateId;
            }
            else if (this.itemFormTemplateIdSmall  != null && Context.Current.IsSmallFormFactor)
            {
                f.TemplateId = this.itemFormTemplateIdSmall;
            }

            f.DefaultImageBrowserOptions = this.DefaultImageBrowserOptions;
            f.ItemSetInterface = this.ItemSetInterface;
            f.ItemSet = this.itemSet;
            f.Item = item;
            f.Mode = this.formMode;
            

            this.formsByLocalId[item.LocalOnlyUniqueId] = f;
            this.forms.Add(f);
            this.itemsShown.Add(item);

            f.EnsureElements();

            /*
            // Banding effect for Row Forms
            if (f is RowForm)
            {
                if (index % 2 == 1)
                {
                    f.Element.Style.BackgroundColor = "#F8F8F8";
                }
                else
                {
                    f.Element.Style.BackgroundColor = "";
                }
            }*/

            if (this.mode == ItemSetEditorMode.Rows || this.mode == ItemSetEditorMode.Linear || Context.Current.IsSmallFormFactor)
            {
                if (this.mode == ItemSetEditorMode.Rows)
                {
                    this.formBin.Style.Display = "table";
                }

                this.InsertFormInOrder(f);
            }
            else
            {
                if (index < this.itemElements.Count)
                {
                    Element itemBin = this.itemElements[index];

                    while (itemBin.ChildNodes.Length > 0)
                    {
                        itemBin.RemoveChild(itemBin.ChildNodes[0]);
                    }

                    itemBin.AppendChild(f.Element);
                }
            }
        }

        private void f_GrippieChanged(object sender, EventArgs e)
        {
            if (this.IsReorderable && ((Form)sender).GrippieElement != null)
            {
                DraggableOptions options = new DraggableOptions();

                options.Hint = this.CreateDragElement;
                options.Axis = "y";
                options.DragStart = this.HandleDragStart;
                options.DragEnd = this.HandleDragEnd;
                options.DragCancel = this.HandleDragEnd;

                KendoUtilities.CreateDraggable(((Form)sender).GrippieElement, options);
            }
        }

        
        private void HandleDragStart(jQueryEvent eventData)
        {
            Element targetElt = eventData.CurrentTarget;

            // the jqueryevent definition says that this is an element, but it looks like it's coming in as a collection of elements.
            Script.Literal("{0}={0}[0]", targetElt);

            this.draggingForm = null;

            while (targetElt != null && this.draggingForm == null)
            {
                foreach (Form f in this.forms)
                {
                    if (f.Element == targetElt)
                    {
                        this.draggingForm = f;
                        break;
                    }
                }

                targetElt = targetElt.ParentNode;
            }

            this.draggingForm.Element.Style.Opacity = ".1";

            ElementUtilities.SetText(this.draggingElement, this.draggingForm.Item.Title);

            ClientRect rect = ElementUtilities.GetBoundingRect(this.draggingForm.Element);

            this.draggingElement.Style.MinWidth = (rect.Right - rect.Left) + "px";

            this.draggingElement.Style.MaxWidth = this.draggingElement.Style.MinWidth;

            foreach (String targetItemId in this.dropZoneTargetsByLocalId.Keys)
            {
                DropZoneTarget zoneTarget = this.dropZoneTargetsByLocalId[targetItemId];

                zoneTarget.ExpandedHeight = (int)(rect.Bottom - rect.Top);
                if (zoneTarget.CurrentControl != this.draggingForm && zoneTarget.PreviousControl != this.draggingForm)
                {
                    zoneTarget.IsActive = true;
                }
                else
                {
                    zoneTarget.IsActive = false;
                }
            }
        }

        private void HandleDragEnd(jQueryEvent eventData)
        {
            this.draggingForm.Element.Style.Opacity = "1";

            foreach (String targetItemId in this.dropZoneTargetsByLocalId.Keys)
            {
                DropZoneTarget zoneTarget = this.dropZoneTargetsByLocalId[targetItemId];

                zoneTarget.IsActive = false;
            }
        }

        private Element CreateDragElement(Element[] elements)
        {
            this.draggingElement = this.CreateElement("draggableBar");


            return this.draggingElement;
        }

        private void item_ItemChanged(object sender, DataStoreItemChangedEventArgs e)
        {
            if (this.ItemSetInterface != null)
            {
                if ((this.ItemSetInterface.Sort == ItemSetSort.FieldAscending || this.ItemSetInterface.Sort == ItemSetSort.FieldDescending) && this.ItemSetInterface.SortField != null)
                {
                    foreach (String changedFieldName in e.ChangedProperties)
                    {
                        if (changedFieldName == this.ItemSetInterface.SortField)
                        {
                            this.reorderItemsOnNextUpdate = true;
                            this.Update();
                            return;
                        }
                    }
                }
            }
        }

        public void SetItemSetInterfaceAndItems(ItemSetInterface isi, IDataStoreItemSet newItemSet)
        {
            if (this.itemSet == newItemSet && this.itemSetInterface == isi)
            {
                return;
            }

            this.itemSetInterface = isi;

            foreach (Form f in this.forms)
            {
                f.ItemSetInterface = this.itemSetInterface;
            }

            if (this.itemSet != null)
            {
                this.itemSet.ItemSetChanged -= this.itemSetEventHandler;
                this.itemSet.ItemInSetChanged -= this.itemChangedEventHandler;
            }

            this.itemSet = newItemSet;

            if (this.itemSet != null)
            {
                this.itemSet.ItemSetChanged += this.itemSetEventHandler;
                this.itemSet.ItemInSetChanged += this.itemChangedEventHandler;

                this.itemSet.BeginRetrieve(this.ItemsRetrieved, null);
            }
            else
            {
                this.Update();
            }
        }

        private DropZoneTarget EnsureDropZoneTargetForForm(Form form)
        {
            DropZoneTarget dropZoneTarget = this.dropZoneTargetsByLocalId[form.Item.LocalOnlyUniqueId];

            if (dropZoneTarget == null)
            {
                dropZoneTarget = new DropZoneTarget();

                dropZoneTarget.TemplateId = "bl-forms-rowdropzonetarget";

                dropZoneTarget.EnsureElements();
                dropZoneTarget.DroppedOn += dropZoneTarget_DroppedOn;

                this.dropZoneTargetsByLocalId[form.Item.LocalOnlyUniqueId] = dropZoneTarget;
            }

            return dropZoneTarget;
        }

        private void InsertFormInOrder(Form formToInsert)
        {
            if (this.formBin == null)
            {
                return;
            }

            DropZoneTarget dropZoneTarget = this.EnsureDropZoneTargetForForm(formToInsert);

            Form previousForm = null;

            if (this.ItemSetInterface.Sort != ItemSetSort.DefaultState)
            {
                for (int i=0; i<this.formBin.Children.Length; i++)
                {
                    Element e = this.formBin.Children[i];

                    Form existingFormInList = this.GetFormForElement(e);

                    if (existingFormInList != null)
                    {
                        if (existingFormInList.Item.CompareTo(formToInsert.Item, this.ItemSetInterface.Sort, this.ItemSetInterface.SortField) >= 0)
                        {
                            dropZoneTarget.PreviousControl = previousForm;
                            dropZoneTarget.NextControl = existingFormInList;
                            dropZoneTarget.CurrentControl = formToInsert;

                            Element targetToInsertBefore = null;

                            if (previousForm != null)
                            {
                                DropZoneTarget previousFormZoneTarget = this.EnsureDropZoneTargetForForm(previousForm);
                                previousFormZoneTarget.NextControl = formToInsert;
                            }

                            DropZoneTarget nextFormZoneTarget = this.EnsureDropZoneTargetForForm(existingFormInList);
                            nextFormZoneTarget.PreviousControl = formToInsert;

                            targetToInsertBefore = nextFormZoneTarget.Element;

                            this.formBin.InsertBefore(formToInsert.Element, targetToInsertBefore);
                            this.formBin.InsertBefore(dropZoneTarget.Element, formToInsert.Element);
                            return;
                        }

                        previousForm = existingFormInList;
                    }
                }
            }

            dropZoneTarget.NextControl = null;
            dropZoneTarget.CurrentControl = formToInsert;
            dropZoneTarget.PreviousControl = previousForm;

            this.formBin.AppendChild(dropZoneTarget.Element);
            this.formBin.AppendChild(formToInsert.Element);
        }


        private void dropZoneTarget_DroppedOn(object sender, EventArgs e)
        {
            DropZoneTarget droppedOnTarget = (DropZoneTarget)sender;

            Form targetNextForm= droppedOnTarget.CurrentControl as Form;
            Form targetPreviousForm = droppedOnTarget.PreviousControl as Form;

            List<IItem> items= this.ItemSet.GetSortedItems(this.ItemSetInterface.Sort, this.ItemSetInterface.SortField);

            int targetNextIndex = 0;

            if (targetNextForm != null)
            {
                targetNextIndex = items.IndexOf(targetNextForm.Item);
            }

            int targetPreviousIndex = 0;

            if (targetPreviousForm!= null)
            {
                targetPreviousIndex = items.IndexOf(targetPreviousForm.Item);
            }

            int sourceIndex = items.IndexOf(draggingForm.Item);

            if (sourceIndex < Math.Max(targetNextIndex, targetPreviousIndex)) // we're dragging an element lower in the list (down)
            {
                 int? fieldTargetOrder = 0;

                IItem targetItem = targetPreviousForm.Item;
                fieldTargetOrder = targetItem.GetInt32Value(this.ItemSetInterface.SortField);

                for (int i = targetPreviousIndex; i > sourceIndex; i--)
                {
                    IItem currentItem= items[i];
                    IItem previousItem = items[i - 1];

                    currentItem.SetInt32Value(this.ItemSetInterface.SortField, previousItem.GetInt32Value(this.ItemSetInterface.SortField));
                }

                this.draggingForm.Item.SetInt32Value(this.ItemSetInterface.SortField, fieldTargetOrder);
            }
            else // we're dragging an element higher in the list (up)
            {
                int? fieldTargetOrder = 0;

                IItem targetItem = targetNextForm.Item;
                fieldTargetOrder = targetItem.GetInt32Value(this.ItemSetInterface.SortField);

                for (int i = targetNextIndex; i < sourceIndex; i++)
                {
                    IItem currentItem = items[i];

                    int nextOrder = 0;

                    if (i + 1 < items.Count)
                    {
                        IItem nextItem = items[i + 1];

                        if (nextItem.GetInt32Value(this.ItemSetInterface.SortField) != null)
                        {
                            nextOrder = (int)nextItem.GetInt32Value(this.ItemSetInterface.SortField);
                        }
                    }

                    currentItem.SetInt32Value(this.ItemSetInterface.SortField, nextOrder);
                }

                this.draggingForm.Item.SetInt32Value(this.ItemSetInterface.SortField, fieldTargetOrder);
            }

            this.reorderItemsOnNextUpdate = true;
            this.Update();
        }

        private Form GetFormForElement(Element e)
        {
            foreach (Form f in this.forms)
            {
                if (f.Element == e)
                {
                    return f;
                }
            }

            return null;
        }

        private void f_ItemDeleted(object sender, DataStoreItemEventArgs e)
        {
            if (this.ItemDeleted != null)
            {
                DataStoreItemEventArgs dsiea = new DataStoreItemEventArgs(e.Item);

                this.ItemDeleted(this, dsiea);
            }
        }

        public void DisposeItemInterfaceItems()
        {
            foreach (String key in this.formsByLocalId.Keys)
            {
                if (key != null)
                {
                    Form f = this.formsByLocalId[key];

                    if (f != null)
                    {
                        f.Dispose();
                    }

                }
            }

            this.formsByLocalId = new Dictionary<string, Form>();
        }

        protected override void OnTemplateDisposed()
        {
            this.DisposeItemInterfaceItems();

            base.OnTemplateDisposed();
        }

        protected override void OnUpdate()
        {
            if (!this.TemplateWasApplied)
            {
                return;
            }

            if (this.addButton != null && this.addItemCta != null)
            {
                this.addButton.Value = this.addItemCta;
            }
            else if (this.addButton != null && this.ItemSetInterface != null && this.ItemSetInterface.AddItemCta != null)
            {
                this.addButton.Value = this.ItemSetInterface.AddItemCta;
            }
            else if (this.addButton != null)
            {
                this.addButton.Value = "add item";
            }

            if (this.persist != null)
            {
                if (this.displayPersistButton)
                {
                    this.persist.Visible = true;
                }
                else
                {
                    this.persist.Visible = false;
                }

                this.persist.ItemSet = this.ItemSet;
                this.persist.ItemSetEditor = this;
            }

            List<IItem> itemsNotSeen = new List<IItem>();

            foreach (IItem item in this.itemsShown)
            {
                itemsNotSeen.Add(item);
            }

            if (this.reorderItemsOnNextUpdate)
            {
                List<IItem> tempShownItems = new List<IItem>();

                foreach (IItem item in this.itemsShown)
                {
                    tempShownItems.Add(item);
                }

                foreach (IItem item in tempShownItems)
                {
                    Form f = formsByLocalId[item.LocalOnlyUniqueId];

                    this.RemoveFormFromDisplay(f);
                }

                this.reorderItemsOnNextUpdate = false;
            }

            this.EnsureHeaderRow();

            if (this.itemSet != null)
            {
                int index = 0;

                foreach (IItem item in itemSet.Items)
                {
                    itemsNotSeen.Remove(item);

                    Nullable<int>indexToUse = index;

                    if (this.ItemPlacementFieldName != null)
                    {
                        indexToUse = item.GetInt32Value(this.ItemPlacementFieldName);
                    }

                    if (indexToUse != null)
                    {
                        this.EnsureFormForItem(item, (int)indexToUse);

                        index++;
                    }
                }
            }

            foreach (IItem item in itemsNotSeen)
            {
                Form f = formsByLocalId[item.LocalOnlyUniqueId];

                if (f != null)
                {
                    if (this.formBin != null && this.formBin.Contains(f.Element))
                    {
                        this.formBin.RemoveChild(f.Element);
                    }
                }

                DropZoneTarget dtz = this.dropZoneTargetsByLocalId[item.LocalOnlyUniqueId];

                if (dtz != null)
                {
                    if (this.formBin != null && this.formBin.Contains(dtz.Element))
                    {
                        this.formBin.RemoveChild(dtz.Element);
                    }
                }

                this.itemsShown.Remove(item);
            }
        }
    }
}
