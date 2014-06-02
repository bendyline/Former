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


        private List<InputElement> radioButtons;

        private InputElement selectedElement;

        public ScaleFieldValue()
        {
            this.radioButtons = new List<InputElement>();
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

                    b.AddEventListener("click", this.HandleButtonClick, true);

                    b.Value = (i+1).ToString();

                    this.radioButtons.Add(b);
                    this.scaleBin.AppendChild(b);
                }

         //       this.UpdateRadios();
            }
        }

        private void UpdateRadios()
        {
            String valStr = this.Item.GetStringValue(this.FieldName);

            for (int i=0; i<this.radioButtons.Count; i++)
            {
                if (valStr == this.radioButtons[i].Value)
                {
                    this.radioButtons[i].SetAttribute("checked", 1);
                }
                else
                {
                    this.radioButtons[i].RemoveAttribute("checked");
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

            String val = (String)element.GetAttribute("Value");

            this.Item.SetValue(this.FieldName, Int32.Parse(val));

            selectedElement = element;

            selectedElement.ClassName = this.GetElementClass("scaleButton selected");

            this.UpdateRadios();
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
