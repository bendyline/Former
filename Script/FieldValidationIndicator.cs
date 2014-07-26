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
    public class FieldValidationIndicator : FieldControl
    {
        [ScriptName("e_asterisk")]
        private Element asteriskElement;

        public FieldValidationIndicator()
        {

        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.IsReady)
            {
                if (this.Form.IsFieldValidForItem(this.Field, this.Item))
                {
                    this.asteriskElement.Style.Display = "none";
                }
                else
                {
                    this.asteriskElement.Style.Display = "block";
                }
            }
        }
    }
}
