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
    public class ChoiceFieldValue : FieldControl
    {
        [ScriptName("e_choiceBin")]
        private Element choiceBin;

        public ChoiceFieldValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

        }


        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.choiceBin == null || this.Field == null)
            {
                return;
            }

            
            FieldChoiceCollection fcc = this.Field.Choices;

            FieldChoiceCollection alternateChoices = this.Form.GetFieldChoices(this.FieldName);

            if (alternateChoices != null)
            {
                fcc = alternateChoices;
            }

            while (this.choiceBin.ChildNodes.Length > 0)
            {
                this.choiceBin.RemoveChild(this.choiceBin.ChildNodes[0]);
            }

            foreach (FieldChoice fc in fcc)
            {
                InputElement b = (InputElement)this.CreateElementWithType("choiceButton", "INPUT"); ;
                b.Type = "button";
                
                b.SetAttribute("choiceId", fc.DisplayName);
                b.AddEventListener("click", this.HandleButtonClick, true);
                b.Value = fc.DisplayName;
                this.choiceBin.AppendChild(b);
            }
        }

        private void HandleButtonClick(ElementEvent e)
        {
            String val = (String)e.SrcElement.GetAttribute("choiceId");

            this.Item.SetStringValue(this.FieldName, val);
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
