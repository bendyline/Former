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
    public class ScaleFieldValue : FieldControl
    {
        [ScriptName("e_scaleBin")]
        private Element scaleBin;

        private InputElement selectedElement;
        public ScaleFieldValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

        }


        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.scaleBin == null || this.Field == null)
            {
                return;
            }

            while (this.scaleBin.ChildNodes.Length > 0)
            {
                this.scaleBin.RemoveChild(this.scaleBin.ChildNodes[0]);
            }

            if (this.EffectiveMode == FieldMode.Example)
            {
                for (int i = 0; i < 5; i++)
                {
                    InputElement b = (InputElement)this.CreateElementWithType("scaleButton example", "INPUT");
                    b.Type = "radio";

                    b.Value = "Example 1";
                    this.scaleBin.AppendChild(b);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    InputElement b = (InputElement)this.CreateElementWithType("scaleButton", "INPUT"); ;
                    b.Type = "radio";

          //          b.SetAttribute("choiceId", fc.DisplayName);
                    b.AddEventListener("click", this.HandleButtonClick, true);
                    b.Value = i.ToString();
                    this.scaleBin.AppendChild(b);
                }
            }
        }

        private void HandleButtonClick(ElementEvent e)
        {
            InputElement element = (InputElement)e.SrcElement;

            if (selectedElement != null)
            {
                selectedElement.ClassName = this.GetElementClass("scaleButton normal");
            }

            String val = (String)element.GetAttribute("choiceId");

            this.Item.SetStringValue(this.FieldName, val);

            selectedElement = element;

            selectedElement.ClassName = this.GetElementClass("scaleButton selected");
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
