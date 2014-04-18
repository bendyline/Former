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
    public class FullField : FieldControl
    {
        [ScriptName("c_fieldTitle")]
        private FieldTitle fieldTitle;

        [ScriptName("c_fieldValue")]
        private FieldValue fieldValue;

        public FullField()
        {

        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.IsReady)
            {
                if (this.fieldTitle != null)
                {
                    this.ApplyToControl(this.fieldTitle);

                  
                }

                if (this.fieldValue!= null)
                {
                    this.ApplyToControl(this.fieldValue);
                }
            }
        }

        public override void PersistToItem()
        {
            if (this.fieldTitle != null)
            {
                this.fieldTitle.Update();
            }

            if (this.fieldValue != null)
            {
                this.fieldValue.Update();
            }
        }
    }
}
