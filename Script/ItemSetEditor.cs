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
    public class ItemSetEditor : Control
    {
        private IDataStoreItemSet itemSet;

        [ScriptName("e_formBin")]
        private Element formBin;

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

        private FormSettings formSettings;
        private String itemFormTemplateId;

        [ScriptName("e_addButton")]
        private InputElement addButton;

        private bool showAddButton = true;
        private String addItemCta;

        public event DataStoreItemEventHandler ItemAdded;

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

        public String ItemFormTemplateId
        {
            get
            {
                return this.itemFormTemplateId;
            }

            set
            {
                this.itemFormTemplateId = value;

                foreach (Form f in this.forms)
                {
                    f.TemplateId = this.itemFormTemplateId;
                }
            }
        }

        public FormSettings FormSettings
        {
            get
            {
                if (this.formSettings == null)
                {
                    this.formSettings = new FormSettings();
                }

                return this.formSettings;
            }

            set
            {
                this.formSettings = value;

                foreach (Form f in this.forms)
                {
                    f.Settings = this.formSettings;
                }
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

        public ItemSetEditor()
        {
            this.itemsShown = new List<IItem>();
            this.formsByLocalId = new Dictionary<String, Form>();
            this.forms = new List<Form>();
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

            this.EnsureFormForItem(item, this.itemSet.Items.Count - 1);

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
            this.Update();
        }
        
        private void EnsureFormForItem(IItem item, int index)
        {
            Form f = formsByLocalId[item.LocalOnlyUniqueId];

            if (f != null)
            {
                if (!this.itemsShown.Contains(item))
                {
                    f.Settings = this.FormSettings;
                    f.Item = item;
                    
                    this.itemsShown.Add(item);

                    this.formBin.AppendChild(f.Element);
                }

                return;
            }

            if (this.mode == ItemSetEditorMode.Rows)
            {
                f = new RowForm();
                f.IteratorFieldTemplateId = "bl-forms-horizontalunlabeledfield";
            }
            else
            {
                f = new Form();
                f.IteratorFieldTemplateId = "bl-forms-horizontalunlabeledfield";
            }

            if (this.itemFormTemplateId != null)
            {
                f.TemplateId = this.itemFormTemplateId;
            }

            f.Settings = this.FormSettings;
            f.Item = item;
            
            this.formsByLocalId[item.LocalOnlyUniqueId] = f;
            this.forms.Add(f);
            this.itemsShown.Add(item);

            f.EnsureElements();

            if (this.mode == ItemSetEditorMode.Rows)
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

            List<IItem> itemsNotSeen = new List<IItem>();

            foreach (IItem item in this.itemsShown)
            {
                itemsNotSeen.Add(item);
            }

            if (this.itemSet != null)
            {
                int index = 0;

                foreach (IItem item in itemSet.Items)
                {
                    itemsNotSeen.Remove(item);

                    this.EnsureFormForItem(item, index);
                    index++;
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
