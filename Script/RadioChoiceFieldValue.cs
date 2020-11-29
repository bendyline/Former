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
    public class RadioChoiceFieldValue : ChoiceFieldControl
    {
        [ScriptName("e_choiceBin")]
        private Element choiceBin;

        private Element selectedElement;

        private String lastOptionsHash = null;

        public RadioChoiceFieldValue()
        {

        }
        
        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.choiceBin == null || this.Field == null)
            {
                return;
            }

            String newHash = this.GetOptionsHashWithValue();

            if (this.lastOptionsHash != newHash)
            {
                this.lastOptionsHash = newHash;

                while (this.choiceBin.ChildNodes.Length > 0)
                {
                    this.choiceBin.RemoveChild(this.choiceBin.ChildNodes[0]);
                }

                if (this.EffectiveMode == FieldMode.View)
                {
                    FieldChoiceCollectionBase fcc = this.EffectiveFieldChoices;

                    foreach (FieldChoice fc in fcc)
                    {
                        if (IsFieldChoiceSelected(fc))
                        {
                            Element displayTextElement = this.CreateElement("textDisplay");

                            ElementUtilities.SetText(displayTextElement, fc.DisplayName);

                            this.choiceBin.AppendChild(displayTextElement);
                        }
                    }
                }
                else if (this.EffectiveMode == FieldMode.Example)
                {
                    Element row = this.CreateChoiceRow("Example 1",null, null, false);
                    this.choiceBin.AppendChild(row);

                    row = this.CreateChoiceRow("Example 2", null, null, false);
                    this.choiceBin.AppendChild(row);
                }
                else
                {
                    FieldChoiceCollectionBase fcc = this.EffectiveFieldChoices;

                    foreach (FieldChoice fc in fcc)
                    {
                        bool isSelected = IsFieldChoiceSelected(fc);

                        String val = fc.DisplayName;

                        if (val == null)
                        {
                            val = String.Empty;
                        }

                        Element row = this.CreateChoiceRow(val, fc.ImageUrl, fc.EffectiveId, isSelected);
     
                        this.choiceBin.AppendChild(row);
                    }
                }
            }
        }

        private Element CreateChoiceRow(String text, String imageUrl, object id, bool isSelected)
        {
            Element row = this.CreateElement("choiceRow");

            Element buttonCell = this.CreateElement("buttonCell");

            String cssClass = "choiceButton";

            if (isSelected)
            {
                cssClass += " selected";
            }
            else
            {
                cssClass += " normal";
            }

            Element bouter = this.CreateElementWithType("choiceButtonOuter", "BUTTON");
            bouter.SetAttribute("data-choiceId", id);
            bouter.AddEventListener("click", this.HandleButtonClick, true);

            buttonCell.AppendChild(bouter);

            Element b = this.CreateElementWithType(cssClass, "BUTTON");
            b.InnerHTML = "&#160;";
            b.SetAttribute("data-choiceId", id);
            b.AddEventListener("click", this.HandleButtonClick, true);

            bouter.AppendChild(b);
            row.AppendChild(buttonCell);

            Element imageCell = this.CreateElement("imageCell");
            imageCell.AddEventListener("click", this.HandleButtonClick, true);

            imageCell.SetAttribute("data-choiceId", id);
            imageCell.AddEventListener("click", this.HandleButtonClick, true);

            if (imageUrl != null)
            {
                ImageElement ie = (ImageElement)this.CreateElementWithType("choiceImage", "IMG");
                ie.Src = imageUrl;
                imageCell.AppendChild(ie);
            }
            else
            {
                ElementUtilities.SetHtml(imageCell, "&#160;");
            }

            row.AppendChild(imageCell);


            Element labelCell = this.CreateElement("labelCell");
            labelCell.AddEventListener("click", this.HandleButtonClick, true);

            ElementUtilities.SetText(labelCell, text);

            row.AppendChild(labelCell);

            return row;
        }

        private void HandleButtonClick(ElementEvent e)
        {
            if (this.EffectiveMode == FieldMode.View)
            {
                e.CancelBubble = true;
                e.PreventDefault();
                return;
            }

            Element element = ElementUtilities.GetEventTarget(e);

            if (element.ClassName.IndexOf("choiceButtonOuter") > 0)
            {
                element = element.ChildNodes[0];
            }
            else if (element.ClassName.IndexOf("choiceImage") > 0)
            {
                element = element.ParentNode;
            }
            
            if (element.ClassName.IndexOf("labelCell") > 0 || element.ClassName.IndexOf("imageCell") > 0)
            {
                //            up to row/down to button cell/down to choice outer/down to choice
                element = element.ParentNode.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            }

            if (selectedElement != null)
            {
                selectedElement.ClassName = this.GetElementClass("choiceButton normal");
            }

            object val = element.GetAttribute("data-choiceId");

            FieldChoiceCollectionBase fcc = this.Field.Choices;

            FieldChoiceCollectionBase alternateChoices = this.Form.GetFieldChoicesOverride(this.FieldName);

            if (alternateChoices != null)
            {
                fcc = alternateChoices;
            }

            // convert val, which is a string, back into the native data type.
            foreach (FieldChoice fc in fcc)
            {
                if ( fc.Id != null && !(fc.Id is String) && val is String && fc.Id.ToString() == (String)val)
                {
                    val = fc.Id;
                }
            }

            if (this.Field.Type == FieldType.Integer)
            {
                if (val is String && (String)val == "null")
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
