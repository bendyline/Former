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
    public class ItemSetControl : Control 
    {
        private IDataStoreItemSet itemSet;
        private bool monitorItemSetEvents = true;

        protected bool MonitorItemEvents
        {
            get
            {
                return this.monitorItemSetEvents;
            }

            set
            {
                this.monitorItemSetEvents = value;
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

                if (this.itemSet != null && this.monitorItemSetEvents)
                {
                    this.itemSet.ItemSetChanged -= itemSet_ItemSetChanged;
                }

                this.DelayApplyTemplate = false;

                this.itemSet = value;

                this.OnItemSetChanged();

                if (this.itemSet != null && this.monitorItemSetEvents)
                {
                    this.itemSet.ItemSetChanged += itemSet_ItemSetChanged;
                }

                this.Update();
            }
        }

        protected ItemSetControl() : base()
        {
            this.DelayApplyTemplate = true;
        }

        private void itemSet_ItemSetChanged(object sender, DataStoreItemSetEventArgs e)
        {
            this.Update();
        }

        protected virtual void OnItemSetChanged()
        {

        }
    }
}
