/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Data;
using BL.UI;
using BL.UI.KendoControls;
using System;
using System.Html;
using System.Runtime.CompilerServices;
using System.Collections;

namespace BL.Forms
{
    public class DropDownFieldValue : ChoiceFieldControl
    {
        [ScriptName("c_dropDown")]
        private DropDownList dropDown;

        private String lastOptionsHash = null;

        public DropDownFieldValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            String resourceBasePath = Context.Current.ResourceBasePath + "qla/images/";

            this.dropDown.DropdownTemplate = String.Format(@"<div class=""k-state-default"" style=""display:table; min-height:48px; margin-right:3px;""><div style=""display:table-row""><div class=""k-state-default"" style=""padding:4px;display:table-cell"">#= text#</div></div></div>", resourceBasePath, Context.Current.VersionToken);
            this.dropDown.DropdownHeight = 300;
            this.dropDown.SetDropdownWidth(400);
            this.dropDown.DataTextField = "text";
            this.dropDown.DataValueField = "value";
            this.dropDown.Changed += dropDown_Changed;
        }

        private void dropDown_Changed(object sender, EventArgs e)
        {
            object val = this.dropDown.Value;

            if (this.Field.Type == FieldType.Integer)
            {
                if (val is String && (String)val == "null")
                {
                    this.Item.SetInt32Value(this.FieldName, null);
                }
                else
                {
                    if (val is String)
                    {
                        this.Item.SetStringValue(this.FieldName, (String)val);
                    }
                    else if (val is Int32)
                    {
                        this.Item.SetInt32Value(this.FieldName, (Int32)val);
                    }
                    else
                    {
                        this.Item.SetStringValue(this.FieldName, val.ToString());
                    }
                }
            }
            else
            {
                if ((String)val == "null")
                {
                    this.Item.SetStringValue(this.FieldName, null);
                }
                else
                {
                    this.Item.SetStringValue(this.FieldName, (String)val);
                }
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.dropDown == null || this.Field == null)
            {
                return;
            }

            String newHash = this.GetOptionsHash();

            if (this.lastOptionsHash != newHash)
            {
                this.lastOptionsHash = newHash;

                FieldChoiceCollection fcc = this.Field.Choices;

                FieldChoiceCollection alternateChoices = this.Form.GetFieldChoicesOverride(this.FieldName);

                if (alternateChoices != null)
                {
                    fcc = alternateChoices;
                }                

                ArrayList data = new ArrayList();

                foreach (FieldChoice fc in fcc)
                {
                    TextImageValue tv = new TextImageValue();
                    tv.Text = fc.DisplayName;
                    tv.Value = fc.Id;

                    data.Add(tv);
                }

                this.dropDown.Data = data;

            }

            this.dropDown.Value = this.Item.GetValue(this.FieldName);
        }

        public override void PersistToItem()
        {
            
        }       
    }
}
