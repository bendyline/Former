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
    public class IntegerFieldValue : FieldControl
    {
        [ScriptName("e_textInput")]
        private InputElement textInput;

        [ScriptName("e_textDisplay")]
        private Element textDisplay;

        private bool commitPending = false;

        public IntegerFieldValue()
        {
            this.EnsureScript("kendo.ui.Validator", "js/kendo/kendo.validator.min.js");
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.textInput != null)
            {
                this.textInput.AddEventListener("change", this.HandleTextInputChanged, true);
                this.textInput.AddEventListener("keypress", this.HandleTextInputKeyPressed, true);
                this.textInput.AddEventListener("keyup", this.HandleTextInputKeyUp, true);

                jQueryObject jqo = jQuery.FromObject(this.textInput);

                Script.Literal("{0}.kendoValidator().data(\"kendoValidator\")", jqo);
            }
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        private void HandleTextInputKeyPressed(ElementEvent e)
        {
            if (e.KeyCode == 13)
            {
                this.textInput.Blur();
                e.CancelBubble = true;
                e.StopPropagation();
                e.StopImmediatePropagation();
                e.PreventDefault();
            }
        }

        private void HandleTextInputKeyUp(ElementEvent e)
        {
            if (!this.commitPending)
            {
                this.commitPending = true;

                Window.SetTimeout(this.SaveValue, 3000);
            }
        }


        private void HandleTextInputChanged(ElementEvent e)
        {
            this.SaveValue();
        }

        private void SaveValue()
        {
            this.commitPending = false;

            if (this.textInput.Value == String.Empty)
            {
                if (AllowNull)
                {
                    this.Item.SetInt32Value(this.FieldName, null);
                }
                else
                {
                    this.Item.SetInt32Value(this.FieldName, 0);
                }
            }
            else
            {
                this.Item.SetInt32Value(this.FieldName, Int32.Parse(this.textInput.Value));
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.textInput == null || !this.IsReady)
            {
                return;
            }

            int? val = this.Item.GetInt32Value(this.FieldName);

            bool allowNull = this.AllowNull;

            if (val == null)
            {
                if (this.EffectiveUserInterfaceOptions != null && this.EffectiveUserInterfaceOptions.IntDefaultValue != null)
                {
                    val = (int)this.EffectiveUserInterfaceOptions.IntDefaultValue;

                    this.Item.SetInt32Value(this.FieldName, val);
                }
                else if (allowNull)
                {
                    this.Item.SetInt32Value(this.FieldName, null);
                }
                else
                {
                    val = 0; 
                    this.Item.SetInt32Value(this.FieldName, val);

                }
            }

            if (this.EffectiveMode == FieldMode.Example)
            {
                this.textInput.Value = "Example";
                this.textInput.Disabled = true;
                this.textDisplay.Style.Display = "none";
                this.textInput.Style.Display = "block";
                if (val == null)
                {
                    this.textInput.Value =String.Empty;
                }
                else
                {
                    this.textInput.Value = ((int)val).ToString();
                }
            }
            else if (this.EffectiveMode == FieldMode.Edit)
            {
                this.textInput.Disabled = false;
                this.textDisplay.Style.Display = "none";
                this.textInput.Style.Display = "block";

                if (val == null)
                {
                    this.textInput.Value = String.Empty;
                }
                else
                {
                    this.textInput.Value = ((int)val).ToString();
                }
            }
            else
            {
                this.textDisplay.Style.Display = "block";
                this.textInput.Style.Display = "none";

                if (val == null)
                {
                    ElementUtilities.SetText(this.textDisplay, String.Empty);
                }
                else
                {
                    ElementUtilities.SetText(this.textDisplay, ((int)val).ToString());
                }
            }


            if (this.EffectiveUserInterfaceOptions != null)
            {
                Nullable<int> suggestedWidth = this.EffectiveUserInterfaceOptions.SuggestedWidth;

                if (suggestedWidth != null)
                {
                    this.textDisplay.Style.MinWidth = (int)suggestedWidth + "px";
                    this.textInput.Style.MinWidth = this.textDisplay.Style.MinWidth;
                }
                else
                {
                    this.textDisplay.Style.MinWidth = String.Empty;
                    this.textInput.Style.MinWidth = String.Empty;
                }
            }

            if (this.FieldInterface != null && this.FieldInterface.InterfaceTypeOptionsOverride != null && this.FieldInterface.InterfaceTypeOptionsOverride.Placeholder != null)
            {
                this.textInput.SetAttribute("placeholder", this.FieldInterface.InterfaceTypeOptionsOverride.Placeholder);
            }
            else
            {
                this.textInput.SetAttribute("placeholder", "");
            }
        }

        public override void PersistToItem()
        {
            
        }       
    }
}
