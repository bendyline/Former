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
using System.Serialization;

namespace BL.Forms
{
    public enum UserValueMode
    {
        MeOnly = 0,
        TextInput = 1,
        EmailAddress = 2
    }

    public enum UserValueDisplayMode
    {
        TextInput=0,
        UserSummary = 1
    }

    public class UserValue : FieldControl
    {
        [ScriptName("e_textInput")]
        private InputElement textInput;
        
        [ScriptName("e_userSummaryArea")]
        private Element userSummaryArea;

        [ScriptName("e_toggleButton")]
        private InputElement toggleButton;

        private UserControl userSummary;
        private UserReference activeReference;

        private bool commitPending = false;

        private UserValueDisplayMode displayMode = UserValueDisplayMode.TextInput;

        public UserValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.textInput != null)
            {
                this.textInput.AddEventListener("change", this.HandleTextInputChanged, true);
                this.textInput.AddEventListener("keypress", this.HandleTextInputKeyPressed, true);
                this.textInput.AddEventListener("blur", this.HandleTextInputBlurred, true);
            }

            this.userSummaryArea.AddEventListener("mouseup", this.HandleSummaryTap, true);

            if (this.toggleButton != null)
            {
                this.toggleButton.AddEventListener("click", this.HandleMeToggleButton, true);
            }
        }

        private void HandleSummaryTap(ElementEvent e)
        {
            this.displayMode = UserValueDisplayMode.TextInput;
            
            this.ApplyDisplayMode();

            this.textInput.Focus();
        }

        private void HandleTextInputKeyPressed(ElementEvent e)
        {
            if (!this.commitPending)
            {
                this.commitPending = true;

                Window.SetTimeout(this.SaveValue, 2000);
            }
        }

        private void HandleTextInputChanged(ElementEvent e)
        {
            this.SaveValue();
        }

        private void SaveValue()
        {
            this.commitPending = false;

            if (this.textInput.Value.Length > 0)
            {
                this.Item.SetStringValue(this.FieldName, this.textInput.Value);

                if (this.activeReference != null)
                {
                    this.activeReference = null;
                }
            }
        }

        private void HandleTextInputBlurred(ElementEvent e)
        {
            if (this.textInput.Value.Length == 0 && this.activeReference != null)
            {
                this.displayMode = UserValueDisplayMode.UserSummary;

                this.ApplyDisplayMode();
            }
        }

        private void HandleMeToggleButton(ElementEvent e)
        {
            if (Context.Current.User == null)
            {
                Dialog d = new Dialog();

                Control c = Context.Current.ObjectProvider.CreateObject("assignuserlogin") as Control;

                d.Content = c;
                d.MaxHeight = 400;
                d.MaxWidth = 500;

                d.Show();

                d.Closing += assignUserLogin_Closing;
            }

            if (Context.Current.User != null)
            {
                if (String.IsNullOrEmpty(Context.Current.User.NickName))
                {
                    this.ShowUserSummaryDialog();
                }
                else
                {
                    this.AssignUserValue(false);
                }
            }
        }

        private void ShowUserSummaryDialog()
        {
            Dialog d = new Dialog();

            Control c = Context.Current.ObjectProvider.CreateObject("editusermissingproperties") as Control;

            if (c is UserControl)
            {
                ((UserControl)c).User = Context.Current.User;
            }

            d.Content = c;
            d.MaxHeight = 400;
            d.MaxWidth = 500;
            d.Closing += editUserSummary_Closing;

            d.Show();
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        private void editUserSummary_Closing(object sender, EventArgs e)
        {
            this.AssignUserValue(false);
        }

        private void assignUserLogin_Closing(object sender, EventArgs e)
        {
            this.AssignUserValue(true);
        }

        private void AssignUserValue(bool showUserSummaryIfNeeded)
        {
            if (Context.Current.User != null)
            {
                if (!String.IsNullOrEmpty(Context.Current.User.NickName))
                {
                    this.textInput.Value = String.Empty;

                    UserReference ur = new UserReference();

                    ur.Id = Context.Current.User.Id;
                    ur.NickName = Context.Current.User.NickName;

                    this.Item.SetStringValue(this.FieldName, Json.Stringify(ur.GetObject()));
                }
                else if (showUserSummaryIfNeeded)
                {
                    this.ShowUserSummaryDialog();
                }
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
                String value = this.Item.GetStringValue(this.FieldName);
                this.displayMode = UserValueDisplayMode.TextInput;

                if (value != null)
                {
                    value = value.Trim();

                    if (value.StartsWith("{") && value.EndsWith("}"))
                    {
                        this.activeReference = new UserReference();

                        this.activeReference.ApplyString(value);

                        if (this.activeReference.Id != null && this.activeReference.NickName != null)
                        {
                            this.displayMode = UserValueDisplayMode.UserSummary;
                        }
                        else
                        {
                            this.activeReference = null;
                        }
                    }
                    else
                    {
                        this.textInput.Value = value;
                    }
                }

                this.ApplyDisplayMode();
            }
        }

        private void ApplyDisplayMode()
        {
            if (this.displayMode == UserValueDisplayMode.TextInput)
            {
                this.textInput.Style.Display = "";
            }
            else
            {
                this.textInput.Style.Display = "none";
            }


            if (this.displayMode == UserValueDisplayMode.UserSummary)
            {
                if (this.userSummary == null)
                {
                    this.userSummary = Context.Current.ObjectProvider.CreateObject("userSummary") as UserControl;

                    this.userSummary.EnsureElements();
                }

                this.userSummary.UserReference = this.activeReference;
                this.userSummaryArea.AppendChild(this.userSummary.Element);

                this.userSummaryArea.Style.Display = "";
            }
            else
            {
                this.userSummaryArea.Style.Display = "none";
            }
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
