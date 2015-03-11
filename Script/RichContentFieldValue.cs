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
using BL.UI.KendoControls;

namespace BL.Forms
{
    public class RichContentFieldValue : FieldControl
    {
        [ScriptName("c_editor")]
        private FullEditor editor;

        private bool commitPending = false;

        public RichContentFieldValue()
        {

        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.editor != null)
            {
                this.editor.Changed += editor_Changed;
            }
        }

        private void editor_Changed(object sender, EventArgs e)
        {
            this.Item.SetStringValue(this.FieldName, this.editor.Value);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.editor == null || !this.IsReady)
            {
                return;
            }

            if (this.Form != null) 
            {
                this.editor.EditorOptions.ImageBrowser = this.Form.DefaultImageBrowserOptions;
            }
            
            String val = this.Item.GetStringValue(this.FieldName);

            if (val == null)
            {
                val = String.Empty;
            }

            this.editor.Title = "Edit " + this.EffectiveFieldDisplayName;
            this.editor.Value = val;
        }

        public override void PersistToItem()
        {
            
        }       
    }
}
