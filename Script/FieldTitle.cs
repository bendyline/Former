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
    public class FieldTitle : FieldControl
    {
        [ScriptName("e_title")]
        private Element titleElement;

        public FieldTitle()
        {

        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.IsReady)
            {
                String fieldTitleOverride = this.Form.GetFieldTitleOverride(this.Field.Name);

                if (fieldTitleOverride != null)
                {
                    this.titleElement.InnerText = fieldTitleOverride;
                }
                else
                {
                    this.titleElement.InnerText = this.Field.DisplayName;
                }
            }
        }
    }
}
