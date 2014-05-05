// Forms.cs
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
    public enum FieldMode
    {
        FormDefault = 0,
        Edit = 1,
        View = 2,
        Example = 3
    }

    public class FieldControl : FormControl 
    {
        private IDataStoreField field;
        private String fieldName;
        private FieldMode mode = FieldMode.Edit;

        public FieldMode EffectiveMode
        {
            get
            {
                if (this.mode == FieldMode.FormDefault)
                {
                    if (this.Form.Mode == FormMode.Example)
                    {
                        return FieldMode.Example;
                    }
                    else if (this.Form.Mode == FormMode.NewForm || this.Form.Mode == FormMode.EditForm)
                    {
                        return FieldMode.Edit;
                    }
                    else
                    {
                        return FieldMode.View;
                    }
                }

                return this.mode;
            }
        }

        [ScriptName("i_mode")]
        public FieldMode Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                this.mode = value;
            }
        }

        public IDataStoreField Field
        {
            get
            {
                if (this.field == null && this.fieldName != null)
                {
                    this.field = this.Item.Type.GetField(this.fieldName);
                }

                return this.field;
            }
        }

        public bool IsReady
        {
            get
            {
                return this.fieldName != null && this.Item != null;
            }
        }

        [ScriptName("s_name")]
        public String FieldName
        {
            get
            {
                return this.fieldName;
            }

            set
            {
                this.fieldName = value;

                this.Update();
            }
        }


        protected void ApplyToControl(FieldControl fc)
        {
            fc.Form = this.Form;
            fc.Item = this.Item;
            fc.Mode = this.Mode;
            fc.FieldName = this.FieldName;
        }
    }
}
