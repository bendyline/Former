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
    public class TextFieldValue : FieldControl
    {
        [ScriptName("e_textInput")]
        private InputElement textInput;

        public TextFieldValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.textInput != null)
            {
                this.textInput.AddEventListener("change", this.HandleTextInputChanged, true);
            }
        }

        private void HandleTextInputChanged(ElementEvent e)
        {
            this.Item.SetStringValue(this.FieldName, this.textInput.Value);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.IsReady && this.textInput != null)
            {
                this.textInput.Value = this.Item.GetStringValue(this.FieldName);
            }
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
