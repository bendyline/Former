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
    public class FieldLabel : FieldControl
    {
        [ScriptName("e_title")]
        private Element titleElement;

        public FieldLabel()
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

            if (this.titleElement != null)
            {
                ElementUtilities.SetText(this.titleElement, this.EffectiveFieldDisplayName);
            }
        }
    }
}
