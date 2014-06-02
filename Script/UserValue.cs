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
    public enum UserValueMode
    {
        MeOnly = 0,
        TextInput = 1,
        EmailAddress = 2
    }

    public class UserValue : FieldControl
    {
        [ScriptName("e_textInput")]
        private InputElement textInput;

        [ScriptName("e_toggleButton")]
        private InputElement toggleButton;

        public UserValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.textInput != null)
            {
                this.textInput.AddEventListener("change", this.HandleTextInputChanged, true);
            }

            if (this.toggleButton != null)
            {
                this.toggleButton.AddEventListener("click", this.HandleMeToggleButton, true);
            }
        }

        private void HandleTextInputChanged(ElementEvent e)
        {
            this.Item.SetStringValue(this.FieldName, this.textInput.Value);

        }

        private void HandleMeToggleButton(ElementEvent e)
        {
            Dialog d = new Dialog();

            Control c = Context.Current.ObjectProvider.CreateObject("userLoginControl") as Control;

            d.Content = c;
            d.MaxHeight = 400;
            d.MaxWidth = 500;

            d.Show();

            if (Context.Current.UserAccountName != null)
            {
                this.textInput.Value = Context.Current.UserAccountName;

                this.Item.SetStringValue(this.FieldName, this.textInput.Value);
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.textInput == null)
            {
                return;
            }

            if (this.EffectiveMode == FieldMode.Example)
            {
                this.textInput.Value = "(signed up name)";
                this.textInput.Disabled = true;
            }
            else
            {
                this.textInput.Disabled = false;
            }

            if (this.IsReady)
            {
                this.textInput.Value = this.Item.GetStringValue(this.FieldName);

            }
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
