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

        private FormSettings settings;

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

                this.OnUpdate();
            }
        }

        public ItemSetEditor()
        {
            this.itemsShown = new List<IItem>();
            this.formsByItem = new Dictionary<String, Form>();
        }

        private void itemSet_ItemSetChanged(object sender, DataStoreItemSetEventArgs e)
        {
            this.Update();
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

                Form f = formsByItem[item.Id];

                if (f == null)
                {
                    f = new Form();
                    f.Settings = this.Settings;
                    f.Item = item;

                    formsByItem[item.Id] = f;
                    itemsShown.Add(item);

                    f.EnsureElements();

                    this.formBin.AppendChild(f.Element);
                }
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
