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
    public class ItemLastModifiedDate : FormControl
    {
        [ScriptName("e_date")]
        private Element dateElement;

        public ItemLastModifiedDate()
        {

        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.IsReady && this.dateElement != null)
            {
                String val = String.Empty;

                if (this.Item.ModifiedDateTime != null && this.Item.ModifiedDateTime.GetFullYear() > 2000)
                {
                    val = Utilities.GetFriendlyDateDescription(this.Item.ModifiedDateTime);
                }

                ElementUtilities.SetText(this.dateElement, val);
            }
        }
    }
}
