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
        private String groupName = null;

        [ScriptName("e_scaleBin")]
        private Element scaleBin;

        [ScriptName("e_scaleMinEnd")]
        private Element scaleMinEnd;

        [ScriptName("e_scaleMaxEnd")]
        private Element scaleMaxEnd;

        private List<InputElement> radioButtons;
        private List<Element> elementButtons;

        private Element selectedElement;
        private String lastOptionHash = null;

        private ElementEventListener clickListener;
        public ScaleFieldValue()
        {
            this.radioButtons = new List<InputElement>();
            this.elementButtons = new List<Element>();
            this.groupName = Utilities.CreateRandomId();
            this.clickListener = new ElementEventListener(this.HandleButtonClick);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private String GenerateOptionHash()
        {
            String results = this.EffectiveMode.ToString();

            return results;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.scaleBin == null || this.Field == null)
            {
                return;
            }


            String newHash = this.GenerateOptionHash();

            if (newHash != lastOptionHash)
            {
                lastOptionHash = newHash;

                while (this.scaleBin.ChildNodes.Length > 0)
                {
                    this.scaleBin.RemoveChild(this.scaleBin.ChildNodes[0]);
                }

                if (this.radioButtons != null)
                {
                    foreach (InputElement ie in this.radioButtons)
                    {
                        ie.RemoveEventListener("click", this.clickListener, true);
                    }
                    this.radioButtons.Clear();
                }

                Element divOuterElement = this.CreateElement("buttonItemOuter");
                Element divInnerElement = this.CreateElement("buttonItemInner");

                ScaleType scaleType = this.EffectiveUserInterfaceOptions.ScaleType;

                divOuterElement.AppendChild(divInnerElement);

                for (int i = 0; i < 5; i++)
                {
                    Element divButtonElement = this.CreateElement("buttonItemCell");

                    if (scaleType == ScaleType.FiveAgree || scaleType == ScaleType.FiveValues)
                    {
                        InputElement b = (InputElement)this.CreateElementWithType("scaleButton", "INPUT");
                        b.Type = "radio";
                        b.ClassName += " k-radiobutton";
                        b.Name = this.groupName;
                        b.Style.Margin = "4px";

                        if (this.EffectiveMode == FieldMode.Example)
                        {
                            b.Value = "Example " + (i + 1).ToString();
                        }
                        else
                        {
                            b.Value = (i + 1).ToString();
                            b.AddEventListener("click", this.clickListener, true);
                        }

                        this.radioButtons.Add(b);
                        divButtonElement.AppendChild(b);
                    }

                    if (scaleType == ScaleType.FiveStars)
                    {
                        Element e = this.CreateElement("star");
                        e.ClassName += " glyphicon glyphicon-star";
                        e.AddEventListener("click", this.clickListener, true);
                        e.SetAttribute("Value", (i + 1).ToString());

                        this.elementButtons.Add(e);
                        divButtonElement.AppendChild(e);
                    }
    
                    divInnerElement.AppendChild(divButtonElement);

                    if (scaleType == ScaleType.FiveAgree)
                    {
                        Element divLabelElement = this.CreateElement("buttonItemLabelCell");

                        switch (i)
                        {
                            case 0:
                                ElementUtilities.SetText(divLabelElement, "Strongly Disagree");
                                break;
                            case 1:
                                ElementUtilities.SetText(divLabelElement, "Disagree");
                                break;
                            case 2:
                                ElementUtilities.SetText(divLabelElement, "Neutral");
                                break;
                            case 3:
                                ElementUtilities.SetText(divLabelElement, "Agree");
                                break;
                            case 4:
                                ElementUtilities.SetText(divLabelElement, "Strongly Agree");
                                break;
                        }

                        divInnerElement.AppendChild(divLabelElement);
                    }
                }

                this.scaleBin.AppendChild(divOuterElement);
            }

            this.UpdateRadios();

            FieldInterfaceTypeOptions fuio = this.EffectiveUserInterfaceOptions;

            if (fuio != null)
            {
                if (fuio.RangeStartDescription != null && this.scaleMinEnd != null)
                {
                    ElementUtilities.SetText(this.scaleMinEnd, fuio.RangeStartDescription);
                }

                if (fuio.RangeEndDescription != null && this.scaleMaxEnd != null)
                {
                    ElementUtilities.SetText(this.scaleMaxEnd, fuio.RangeEndDescription);
                }
            }
        }

        private void UpdateRadios()
        {
            String valStr = this.Item.GetStringValue(this.FieldName);

            for (int i=0; i<this.radioButtons.Count; i++)
            {
                InputElement elt = this.radioButtons[i];

                if (valStr == this.radioButtons[i].Value)
                {
                    elt.ClassName = this.GetElementClass("scaleButton selected");
                    elt.SetAttribute("checked", 1);
                }
                else
                {
                    elt.RemoveAttribute("checked");
                    elt.ClassName = this.GetElementClass("scaleButton normal");
                }
            }
        }

        private void HandleButtonClick(ElementEvent e)
        {
            ScaleType scaleType = this.EffectiveUserInterfaceOptions.ScaleType;
            Element element = (Element)ElementUtilities.GetEventTarget(e);

            String val = (String)element.GetAttribute("Value");
            
            int oldSelected = (Int32)this.Item.GetValue(this.FieldName);

            int selected = Int32.Parse(val);
            this.Item.SetValue(this.FieldName, selected);

            if (scaleType == ScaleType.FiveStars)
            {
                for (int i = 0; i < oldSelected; i++)
                {
                    this.elementButtons[i].ClassName = this.GetElementClass("star normal") + " glyphicon glyphicon-star";
                }

                selectedElement = element;

                for (int i = 0; i < selected; i++)
                {
                    this.elementButtons[i].ClassName = this.GetElementClass("star selected") + " glyphicon glyphicon-star";
                }
            }
            else
            {
                if (selectedElement != null)
                {
                    selectedElement.ClassName = this.GetElementClass("scaleButton normal");
                }

                selectedElement = element;

                selectedElement.ClassName = this.GetElementClass("scaleButton selected");

                this.UpdateRadios();
            }
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
