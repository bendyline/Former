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
    public class OrderFieldValue : FieldControl
    {
        [ScriptName("e_reorderUpButton")]
        private InputElement reorderUpButton;

        [ScriptName("e_reorderDownButton")]
        private InputElement reorderDownButton;

        private bool commitPending = false;

        public OrderFieldValue()
        {
            // hacky, but this gives us a little delay after updates to assess this item's position amongst all other items.
            this.EnqueueUpdates = true;
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        [ScriptName("v_onReorderUpButtonClick")]
        public void HandleReorderUpClick(ElementEvent ev)
        {
            IItem previousItem = this.PreviousItem;

            if (previousItem == null)
            {
                return;
            }

            String orderField = this.Form.ItemSetInterface.SortField;

            if (orderField == null)
            {
                return;
            }

            Nullable<Int32> previousSortValue = previousItem.GetInt32Value(orderField);

            previousItem.SetInt32Value(orderField, this.Item.GetInt32Value(orderField));

            this.Item.SetInt32Value(orderField, previousSortValue);
        }

        [ScriptName("v_onReorderDownButtonClick")]
        public void HandleReorderDownClick(ElementEvent ev)
        {
            IItem nextItem = this.NextItem;

            if (nextItem == null)
            {
                return;
            }

            String orderField = this.Form.ItemSetInterface.SortField;

            if (orderField == null)
            {
                return;
            }

            Nullable<Int32> nextSortValue = nextItem.GetInt32Value(orderField);

            nextItem.SetInt32Value(orderField, this.Item.GetInt32Value(orderField));
            this.Item.SetInt32Value(orderField, nextSortValue);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (!this.IsReady)
            {
                return;
            }

            IItem previousItem = this.PreviousItem;
            IItem nextItem = this.NextItem;

            if (previousItem == null)
            {
                this.reorderUpButton.Style.Visibility = "hidden";
            }
            else
            {
                this.reorderUpButton.Style.Visibility = "";
            }

            if (nextItem == null)
            {
                this.reorderDownButton.Style.Visibility = "hidden";
            }
            else
            {
                this.reorderDownButton.Style.Visibility = "";
            }
        }

        public override void PersistToItem()
        {
            
        }       
    }
}
