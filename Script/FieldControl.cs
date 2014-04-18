// Forms.cs
//

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
    public class FieldControl : FormControl 
    {
        private IDataStoreField field;
        private String fieldName;

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
            fc.FieldName = this.FieldName;
        }
    }
}
