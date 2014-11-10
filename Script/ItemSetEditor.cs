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
    public enum ItemSetEditorMode
    {
        Rows = 0,
        TemplatePlacedItems=1
    }

    public class ItemSetEditor : Control, IItemSetEditor
    {
        private IDataStoreItemSet itemSet;

        [ScriptName("e_formBin")]
        private Element formBin;

        [ScriptName("c_persist")]
        private PersistButton persist;

        [ScriptName("e_item0")]
        private Element item0;

        [ScriptName("e_item1")]
        private Element item1;

        [ScriptName("e_item2")]
        private Element item2;

        [ScriptName("e_item3")]
        private Element item3;

        [ScriptName("e_item4")]
        private Element item4;

        [ScriptName("e_item5")]
        private Element item5;

        [ScriptName("e_item6")]
        private Element item6;

        [ScriptName("e_item7")]
        private Element item7;

        [ScriptName("e_item8")]
        private Element item8;

        [ScriptName("e_item9")]
        private Element item9;

        [ScriptName("e_item10")]
        private Element item10;

        [ScriptName("e_item11")]
        private Element item11;

        [ScriptName("e_item12")]
        private Element item12;

        [ScriptName("e_item13")]
        private Element item13;

        [ScriptName("e_item14")]
        private Element item14;

        [ScriptName("e_item15")]
        private Element item15;

        [ScriptName("e_item16")]
        private Element item16;

        [ScriptName("e_item17")]
        private Element item17;

        [ScriptName("e_item18")]
        private Element item18;

        [ScriptName("e_item19")]
        private Element item19;

        [ScriptName("e_item20")]
        private Element item20;

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

        private bool useRowFormsIfPossible = true;
        private bool displayAddButton = true;
        private bool displayPersistButton = true;
        private String addItemCta;

        private Element headerRowElement;
        private event DataStoreItemSetEventHandler itemSetEventHandler;
        public event DataStoreItemEventHandler ItemAdded;
        public event DataStoreItemEventHandler ItemDeleted;

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

        [ScriptName("b_displayAddButton")]
        public bool DisplayAddButton
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
                }

                this.itemSet = value;

                if (this.itemSet != null)
                {
                    this.itemSet.ItemSetChanged += this.itemSetEventHandler;

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
            this.itemsShown = new List<IItem>();
            this.formsByLocalId = new Dictionary<String, Form>();
            this.forms = new List<Form>();

            this.itemSetEventHandler = this.itemSet_ItemSetChanged;
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
        }

        private void AddButtonClick(ElementEvent e)
        {
            IItem item = this.itemSet.Type.CreateItem();

            this.itemSet.Add(item);

            int index = this.itemSet.Items.Count - 1;
 
            if (this.ItemPlacementFieldName != null)
            {
                index = (int)item.GetValue(this.ItemPlacementFieldName);
            }

            this.EnsureFormForItem(item, index);

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
                        if (ElementUtilities.ElementIsChildOf(f.Element, this.formBin))
                        {
                            this.formBin.RemoveChild(f.Element);
                        }

                        this.formsByLocalId[item.LocalOnlyUniqueId] = null;
                        this.forms.Remove(f);
                        this.itemsShown.Remove(item);

                        f.Dispose();
                    }
                }
            }

            this.Update();
        }
        
        private void EnsureHeaderRow()
        {
            if (this.formBin == null || this.ItemSet == null)
            {
                return;
            }

            if (this.ItemSetInterface != null && (!this.ItemSetInterface.DisplayColumns || Context.Current.IsSmallFormFactor))
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

            foreach (Field field in sortedFields)
            {
                DisplayState afs = this.GetAdjustedDisplayState(field.Name);

                if (afs == DisplayState.Show)
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
                    f.Item = item;
                    
                    this.itemsShown.Add(item);

                    this.formBin.AppendChild(f.Element);
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

            if (this.itemFormTemplateId != null && (!Context.Current.IsSmallFormFactor || this.itemFormTemplateIdSmall == null))
            {
                f.TemplateId = this.itemFormTemplateId;
            }
            else if (this.itemFormTemplateIdSmall  != null && Context.Current.IsSmallFormFactor)
            {
                f.TemplateId = this.itemFormTemplateIdSmall;
            }

            f.ItemSetInterface = this.ItemSetInterface;
            f.Item = item;
            
            this.formsByLocalId[item.LocalOnlyUniqueId] = f;
            this.forms.Add(f);
            this.itemsShown.Add(item);

            f.EnsureElements();

            if (this.mode == ItemSetEditorMode.Rows || Context.Current.IsSmallFormFactor)
            {
                this.formBin.AppendChild(f.Element);
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

        public override void Dispose()
        {
            this.DisposeItemInterfaceItems();

            base.Dispose();
        }

        protected override void OnUpdate()
        {
            if (this.formBin == null)
            {
                return;
            }

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

            List<IItem> itemsNotSeen = new List<IItem>();

            foreach (IItem item in this.itemsShown)
            {
                itemsNotSeen.Add(item);
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
                    if (this.formBin.Contains(f.Element))
                    {
                        this.formBin.RemoveChild(f.Element);
                    }
                }

                this.itemsShown.Remove(item);
            }
        }
    }
}
