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
    public class ChoiceFieldValue : FieldControl
    {
        [ScriptName("e_choiceBin")]
        private Element choiceBin;

        private InputElement selectedElement;
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

            while (this.choiceBin.ChildNodes.Length > 0)
            {
                this.choiceBin.RemoveChild(this.choiceBin.ChildNodes[0]);
            }

            if (this.EffectiveMode == FieldMode.Example)
            {
                InputElement b = (InputElement)this.CreateElementWithType("choiceButton example", "INPUT");
                b.Type = "button";

                b.Value = "Example 1";
                this.choiceBin.AppendChild(b);

                b = (InputElement)this.CreateElementWithType("choiceButton example", "INPUT");
                b.Type = "button";

                b.Value = "Example 2";
                this.choiceBin.AppendChild(b);
            }
            else
            {
                FieldChoiceCollection fcc = this.Field.Choices;

                FieldChoiceCollection alternateChoices = this.Form.GetFieldChoicesOverride(this.FieldName);

                if (alternateChoices != null)
                {
                    fcc = alternateChoices;
                }

                String id = this.Item.GetStringValue(this.FieldName);

                foreach (FieldChoice fc in fcc)
                {
                    String className;

                    if (id == fc.DisplayName)
                    {
                        className = "choiceButton selected";
                    }
                    else
                    {
                        className = "choiceButton normal";
                    }

                    InputElement b = (InputElement)this.CreateElementWithType(className, "INPUT"); ;
                    b.Type = "button";

                    b.SetAttribute("choiceId", fc.DisplayName);
                    b.AddEventListener("click", this.HandleButtonClick, true);
                    b.Value = fc.DisplayName;
                    this.choiceBin.AppendChild(b);
                }
            }
        }

        private void HandleButtonClick(ElementEvent e)
        {
            InputElement element = (InputElement)e.SrcElement;

            if (selectedElement != null)
            {
                selectedElement.ClassName = this.GetElementClass("choiceButton normal");
            }

            String val = (String)element.GetAttribute("choiceId");

            this.Item.SetStringValue(this.FieldName, val);

            selectedElement = element;

            selectedElement.ClassName = this.GetElementClass("choiceButton selected");
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
