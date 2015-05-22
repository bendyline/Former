/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Diagnostics;
using jQueryApi;
using BL.UI;
using BL.Data;

namespace BL.Forms
{
    public class FormControl : ItemControl 
    {
        private IForm form;

        public List<IItem> EffectiveItemList
        {
            get
            {
                if (this.ItemSet == null)
                {
                    return null;
                }

                ItemSetSort sort = ItemSetSort.None;
                String sortField = null;

                if (this.form.ItemSetInterface != null)
                {
                    sort = this.form.ItemSetInterface.Sort;
                    sortField = this.form.ItemSetInterface.SortField;
                }

                List<IItem> items = null;

                if (sort == ItemSetSort.None)
                {
                    items = this.ItemSet.Items;
                }
                else
                {
                    items = this.ItemSet.GetSortedItems(sort, sortField);
                }

                return items;
            }
        }

        public IItem PreviousItem
        {
            get
            {
                if (this.ItemSet == null)
                {
                    return null;
                }

                List<IItem> items = this.EffectiveItemList;
                
                IItem previousItem = null;

                foreach (IItem item in items)
                {
                    if (item == this.Item)
                    {
                        return previousItem;
                    }

                    previousItem = item;
                }

                Debug.Fail("Couldn't find a form item in the collection it is in.");
                return null;
            }
        }

        public IItem NextItem
        {
            get
            {
                if (this.ItemSet == null)
                {
                    return null;
                }

                List<IItem> items = this.EffectiveItemList;

                bool foundThisItem = false;

                foreach (IItem item in items)
                {
                    if (item == this.Item)
                    {
                        foundThisItem = true;
                    }
                    else if (foundThisItem)
                    {
                        return item;
                    }
                }

                Debug.Assert(foundThisItem, "Couldn't find a form item in the collection it is in.");
                return null;
            }
        }

        public IForm Form
        {
            get
            {
                return this.form;
            }

            set
            {
                if (this.form == value)
                {
                    return;
                }

                this.form = value;
                this.OnFormChange();
                this.Update();
            }
        }

        public override bool IsReady
        {
            get
            {
                return base.IsReady && this.Form != null;
            }
        }


        public virtual void PersistToItem()
        {
            
        }

        internal protected virtual void OnFormChange()
        {
        }

        internal protected virtual void OnInterfaceChange()
        {

        }
    }
}
