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
    public class ItemSetEditor : Control
    {
        private IDataStoreItemSet itemSet;

        [ScriptName("e_formBin")]
        private Element formBin;

        private List<IItem> itemsShown;
        private Dictionary<String, Form> formsByItem;
        private List<Form> forms;

        private FormSettings settings;
        private String itemFormTemplateId;

        [ScriptName("e_addButton")]
        private InputElement addButton;

        private bool showAddButton = true;

        public event DataStoreItemEventHandler ItemAdded;

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

        public FormSettings Settings
        {
            get
            {
                if (this.settings == null)
                {
                    this.settings = new FormSettings();
                }

                return this.settings;
            }

            set
            {
                this.settings = value;

                foreach (Form f in this.forms)
                {
                    f.Settings = this.settings;
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
                this.itemSet = value;

                this.itemSet.ItemSetChanged += itemSet_ItemSetChanged;

                this.itemSet.BeginRetrieve(this.ItemsRetrieved, null);
            }
        }

        public ItemSetEditor()
        {
            this.itemsShown = new List<IItem>();
            this.formsByItem = new Dictionary<String, Form>();
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
                this.addButton.AddEventListener("click", this.AddButtonClick, true);

                this.ApplyAddButtonVisibility();
            }
        }

        private void AddButtonClick(ElementEvent e)
        {
            IItem item = this.itemSet.Type.CreateItem();

            this.itemSet.Add(item);

            this.EnsureFormForItem(item);

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
                f.Save();
            }
        }

        private void itemSet_ItemSetChanged(object sender, DataStoreItemSetEventArgs e)
        {
            this.Update();
        }
        
        private void EnsureFormForItem(IItem item)
        {
            Form f = formsByItem[item.Id];

            if (f != null)
            {
                return;
            }

            f = new Form();

            f.Settings = this.Settings;
            f.Item = item;

            if (this.itemFormTemplateId != null)
            {
                f.TemplateId = this.itemFormTemplateId;
            }

            this.formsByItem[item.Id] = f;
            this.forms.Add(f);
            itemsShown.Add(item);

            f.EnsureElements();

            this.formBin.AppendChild(f.Element);            
        }

        protected override void OnUpdate()
        {
            if (this.itemSet == null || this.formBin == null)
            {
                return;
            }

            List<IItem> itemsNotSeen = new List<IItem>();

            foreach (IItem item in itemsShown)
            {
                itemsNotSeen.Add(item);
            }

            foreach (IItem item in itemSet.Items)
            {
                itemsNotSeen.Remove(item);

                this.EnsureFormForItem(item);
            }


            foreach (IItem item in itemsNotSeen)
            {
                Form f = formsByItem[item.Id];

                if (f != null)
                {
                    this.formBin.RemoveChild(f.Element);
                }
            }
        }
    }
}
