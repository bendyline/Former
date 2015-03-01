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
                results += "|" + fc.Id + "|" + fc.DisplayName + "|";
            }

            results += id;

            return results;
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

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

                        InputElement b = (InputElement)this.CreateElementWithType(className, "INPUT");
                        b.Type = "button";

                        if (this.Form.ItemSetInterface != null)
                        {
                            String color = this.Form.ItemSetInterface.AccentColor1;

                            if (color != null)
                            {
                                // selected = natural (darker) accent color
                                if (id == fc.DisplayName)
                                {
                                    b.Style.BackgroundColor = color;
                                }
                                else
                                {

                                    color = ColorDefinition.CreateFromString(color).GetPrecentageAdjustedColor(.25).ToString();

                                    b.Style.BackgroundColor = color;
                                }
                            }
                        }

                        b.SetAttribute("choiceId", fc.DisplayName);
                        b.AddEventListener("click", this.HandleButtonClick, true);
                        b.Value = fc.DisplayName;

                        this.choiceBin.AppendChild(b);
                    }
                }
            }
        }

        private void HandleButtonClick(ElementEvent e)
        {
            InputElement element = (InputElement)ElementUtilities.GetEventTarget(e);

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
