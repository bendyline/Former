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

        private String lastOptionsHash = null;
        public ChoiceFieldValue()
        {

        }

        private String GetOptionsHash()
        {
            String results = this.EffectiveMode.ToString();

            FieldChoiceCollection fcc = this.Field.Choices;

            FieldChoiceCollection alternateChoices = this.Form.GetFieldChoicesOverride(this.FieldName);

            if (alternateChoices != null)
            {
                fcc = alternateChoices;
            }

            String id = this.Item.GetStringValue(this.FieldName);

            foreach (FieldChoice fc in fcc)
            {
                results += "|" + fc.Id + "|" + fc.DisplayName + "|" + fc.ImageUrl;
            }

            results += id;

            return results;
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        protected override void OnFieldChanged()
        {
            base.OnFieldChanged();

            this.Update();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.choiceBin == null || this.Field == null)
            {
                return;
            }

            String newHash = this.GetOptionsHash();

            if (this.lastOptionsHash != newHash)
            {
                this.lastOptionsHash = newHash;

                while (this.choiceBin.ChildNodes.Length > 0)
                {
                    this.choiceBin.RemoveChild(this.choiceBin.ChildNodes[0]);
                }

                if (this.EffectiveMode == FieldMode.Example)
                {
                    InputElement b = (InputElement)this.CreateElementWithType("choiceButton example", "BUTTON");

                    ElementUtilities.SetText(b, "Example 1");
                    this.choiceBin.AppendChild(b);

                    b = (InputElement)this.CreateElementWithType("choiceButton example", "BUTTON");
                    ElementUtilities.SetText(b, "Example 2");

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

                    object selectedVal= this.Item.GetValue(this.FieldName);

                    if (selectedVal == null)
                    {
                        selectedVal = "null";
                    }

                    foreach (FieldChoice fc in fcc)
                    {
                        String className;

                        bool isSelected = false;

                        object abstractedId = fc.Id;

                        if (abstractedId == null)
                        {
                            abstractedId = fc.DisplayName;
                        }

                        if (selectedVal == abstractedId)
                        {
                            isSelected = true;
                        }

                        if (isSelected)
                        {
                            className = "choiceButton selected";
                        }
                        else
                        {
                            className = "choiceButton normal";
                        }


                        InputElement b = (InputElement)this.CreateElementWithType(className, "BUTTON");

                        if (this.Form.ItemSetInterface != null)
                        {
                            String color = this.Form.ItemSetInterface.AccentColor1;

                            if (color != null)
                            {
                                ColorDefinition originalColor = ColorDefinition.CreateFromString(color);

                                // selected = natural (darker) accent color
                                if (isSelected)
                                {
                                    if (originalColor.IsPrimarilyLight)
                                    {
                                        b.Style.BackgroundColor = originalColor.GetPercentageAdjustedColor(-0.35).ToString();
                                     }
                                    else
                                    {
                                        b.Style.BackgroundColor = color;
                                    }
                                }
                                else
                                {
                                    if (originalColor.IsPrimarilyLight)
                                    {
                                        color = ColorDefinition.CreateFromString(color).GetPercentageAdjustedColor(-0.1).ToString();
                                    }
                                    else
                                    {
                                        color = ColorDefinition.CreateFromString(color).GetPercentageAdjustedColor(.25).ToString();
                                    }


                                    b.Style.BackgroundColor = color;
                                }

                                ColorDefinition backgroundColor = ColorDefinition.CreateFromString(b.Style.BackgroundColor);

                                if (backgroundColor.IsPrimarilyLight)
                                {
                                    b.Style.Color = "#303030";
                                }
                                else
                                {
                                    b.Style.Color = "white";
                                }
                            }
                        }

                        b.SetAttribute("data-choiceId", abstractedId);
                        b.AddEventListener("click", this.HandleButtonClick, true);

                        Element choiceOuterElement = this.CreateElement("choiceOuter");

                        Element choiceInnerElement = this.CreateElement("choiceInner");

                        b.AppendChild(choiceOuterElement);
                        choiceOuterElement.AppendChild(choiceInnerElement);

                        if (fc.ImageUrl != null)
                        {
                            Element imgElement = this.CreateElement("choiceImage");
                            imgElement.Style.BackgroundImage = "url('" + fc.ImageUrl + "')";
                            choiceInnerElement.AppendChild(imgElement);
                        }

                        Element spanElement = this.CreateElement("choiceText");

                        String val = fc.DisplayName;

                        if (val == null)
                        {
                            val = String.Empty;
                        }

                        ElementUtilities.SetText(spanElement, val);
                        choiceInnerElement.AppendChild(spanElement);

                        this.choiceBin.AppendChild(b);
                    }
                }
            }
        }

        private void HandleButtonClick(ElementEvent e)
        {
            if (this.EffectiveMode == FieldMode.View)
            {
                e.CancelBubble = true;
                e.PreventDefault();
                return;
            }

            InputElement element = (InputElement)ElementUtilities.GetEventTarget(e);

            if (selectedElement != null)
            {
                selectedElement.ClassName = this.GetElementClass("choiceButton normal");
            }

            object val = element.GetAttribute("data-choiceId");

            FieldChoiceCollection fcc = this.Field.Choices;

            FieldChoiceCollection alternateChoices = this.Form.GetFieldChoicesOverride(this.FieldName);

            if (alternateChoices != null)
            {
                fcc = alternateChoices;
            }

            // convert val, which is a string, back into the native data type.
            foreach (FieldChoice fc in fcc)
            {
                if (!(fc.Id is String) && fc.Id.ToString() == val)
                {
                    val = fc.Id;
                }
            }

            if (this.Field.Type == FieldType.Integer)
            {
                if (val == "null")
                {
                    this.Item.SetInt32Value(this.FieldName, null);
                }
                else
                {
                    if (val is String)
                    {
                        this.Item.SetStringValue(this.FieldName, (String)val);
                    }
                    else if (val is Int32)
                    {
                        this.Item.SetInt32Value(this.FieldName, (Int32)val);
                    }
                    else
                    {
                        this.Item.SetStringValue(this.FieldName, val.ToString());
                    }
                }
            }
            else
            {
                if (val == "null")
                {
                    this.Item.SetStringValue(this.FieldName, null);
                }
                else
                {
                    this.Item.SetStringValue(this.FieldName, (String)val);
                }
            }

            selectedElement = element;

            selectedElement.ClassName = this.GetElementClass("choiceButton selected");

            e.CancelBubble = true;
            e.PreventDefault();
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
