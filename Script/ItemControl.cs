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
    public class ItemControl : Control 
    {
        private IItem item;
        private bool monitorItemEvents = true;

        protected bool MonitorItemEvents
        {
            get
            {
                return this.monitorItemEvents;
            }

            set
            {
                this.monitorItemEvents = value;
            }
        }

        public IItem Item
        {
            get
            {
                return this.item;
            }

            set
            {
                if (this.item == value)
                {
                    return;
                }

                if (this.item != null && this.monitorItemEvents)
                {
                    this.item.ItemChanged -= item_ItemChanged;
                }

                this.DelayApplyTemplate = false;

                this.item = value;

                if (this.item != null && this.monitorItemEvents)
                {
                    this.item.ItemChanged += item_ItemChanged;
                }

                this.Update();
            }
        }

        protected ItemControl() : base()
        {
            this.DelayApplyTemplate = true;
        }

        private void item_ItemChanged(object sender, DataStoreItemEventArgs e)
        {
            this.Update();
        }
    }
}
