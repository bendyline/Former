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

        [ScriptName("e_textDisplay")]
        private Element textDisplay;

        [ScriptName("e_userSummaryArea")]
        private Element userSummaryArea;

        [ScriptName("e_toggleButton")]
        private InputElement toggleButton;

        private UserControl userSummary;
        private UserReference activeReference;

        private bool commitPending = false;
        private bool useMeByDefault = false;
        private Control activeUserLoginDialogControl = null;
        private Control activeUserPropertyEditorDialogControl = null;

        private UserValueDisplayMode displayMode = UserValueDisplayMode.TextInput;

        public bool UseMeByDefault
        {
            get
            {
                return this.useMeByDefault;
            }

            set
            {
                this.useMeByDefault = value;
            }
        }

        public UserValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.textInput != null)
            {
                ElementUtilities.RegisterTextInputBehaviors(this.textInput);

                this.textInput.AddEventListener("change", this.HandleTextInputChanged, true);
                this.textInput.AddEventListener("keypress", this.HandleTextInputKeyPressed, true);
                this.textInput.AddEventListener("blur", this.HandleTextInputBlurred, true);
            }

            this.userSummaryArea.AddEventListener("mouseup", this.HandleSummaryTap, true);
            this.userSummaryArea.AddEventListener("click", this.HandleSummaryTap, true);

            if (this.toggleButton != null)
            {
                this.toggleButton.AddEventListener("click", this.HandleMeToggleButton, true);
            }
        }

        private void HandleSummaryTap(ElementEvent e)
        {
            if (this.EffectiveMode == FieldMode.View)
            {
                return;
            }

            if (this.displayMode != UserValueDisplayMode.TextInput)
            {
                this.displayMode = UserValueDisplayMode.TextInput;

                this.ApplyDisplayMode();

                this.textInput.Focus();

                e.CancelBubble = true;
                e.StopImmediatePropagation();
                e.StopPropagation();
            }
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
                if (this.activeUserLoginDialogControl == null)
                {
                    Dialog d = new Dialog();

                    Control c = Context.Current.ObjectProvider.CreateObject("assignuserlogin") as Control;

                    this.activeUserLoginDialogControl = c;

                    d.Content = c;
                    d.MaxHeight = 400;
                    d.MaxWidth = 500;

                    d.Show();

                    d.Closing += assignUserLogin_Closing;
                }
            }
            else if (Context.Current.User != null)
            {
                Context.Current.User.LoadUser(this.ConsiderMeContinue,null);
            }
        }

        private void ConsiderMeContinue(IAsyncResult result)
        {         
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
            if (this.activeUserPropertyEditorDialogControl == null)
            {
                Dialog d = new Dialog();

                Control c = Context.Current.ObjectProvider.CreateObject("editusermissingproperties") as Control;

                this.activeUserPropertyEditorDialogControl = c;

                if (c is UserControl)
                {
                    ((UserControl)c).User = Context.Current.User;
                }

                d.Content = c;
                d.MaxHeight = 450;
                d.MaxWidth = 500;
                d.Closing += editUserSummary_Closing;

                d.Show();
            }
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }

        private void editUserSummary_Closing(object sender, EventArgs e)
        {
            this.activeUserPropertyEditorDialogControl = null;

            this.AssignUserValue(false);
        }

        private void assignUserLogin_Closing(object sender, EventArgs e)
        {
            this.activeUserLoginDialogControl = null;

            this.AssignUserValue(true);
        }

        private void AssignUserValue(bool showUserSummaryIfNeeded)
        {
            if (Context.Current.User != null)
            {
                Context.Current.User.LoadUser(this.AssignUserValueContinue, showUserSummaryIfNeeded);
            }
        }

        private void AssignUserValueContinue(IAsyncResult result)
        {
            bool showUserSummaryIfNeeded = (bool)result.AsyncState;

            if (!String.IsNullOrEmpty(Context.Current.User.NickName))
            {
                this.textInput.Value = String.Empty;

                UserReference ur = new UserReference();

                ur.UniqueKey = Context.Current.User.UniqueKey;
                ur.NickName = Context.Current.User.NickName;

                this.Item.SetStringValue(this.FieldName, Json.Stringify(ur.GetObject()));
            }
            else if (showUserSummaryIfNeeded)
            {
                this.ShowUserSummaryDialog();
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.userSummaryArea == null)
            {
                return;
            }

            if (this.textInput != null)
            {
                if (this.EffectiveMode == FieldMode.Example)
                {
                    this.textInput.Value = "(signed up name)";
                    this.textInput.Disabled = true;
                }
                else
                {
                    this.textInput.Disabled = false;
                }


                if (this.EffectiveUserInterfaceOptions != null && this.EffectiveUserInterfaceOptions.Placeholder != null)
                {
                    this.textInput.SetAttribute("placeholder", this.EffectiveUserInterfaceOptions.Placeholder);
                }
                else
                {
                    this.textInput.SetAttribute("placeholder", "name");
                }
            }

            if (this.IsReady)
            {
                String value = this.Item.GetStringValue(this.FieldName);
                this.displayMode = UserValueDisplayMode.TextInput;

                if (value != null)
                {
                    value = value.Trim();

                    if (this.EffectiveDataFormat == FieldDataFormat.IdentifierOnly)
                    {
                        this.activeReference = new UserReference();
                        this.activeReference.UniqueKey = value;

                        this.displayMode = UserValueDisplayMode.UserSummary;
                    }
                    else if (value.StartsWith("{") && value.EndsWith("}"))
                    {
                        this.activeReference = new UserReference();

                        // temporary somewhat hacky upgrade code to undo that we used to call "uniqueKey" an "id" on the client.
                        value = value.Replace("\"id\"", "\"uniqueKey\"");

                        this.activeReference.ApplyString(value);

                        if (this.activeReference.UniqueKey != null && this.activeReference.NickName != null)
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
                        if (this.textInput != null)
                        {
                            this.textInput.Value = value;
                        }
                        
                        if (this.textDisplay != null)
                        {
                            ElementUtilities.SetText(this.textDisplay, value);
                        }
                    }
                }

                this.ApplyDisplayMode();

                if (this.useMeByDefault && Context.Current.User != null && !String.IsNullOrEmpty(Context.Current.User.NickName) && this.EffectiveMode != FieldMode.View && String.IsNullOrEmpty(value))
                {
                    this.AssignUserValue(false);
                }
            }
        }

        private void ApplyDisplayMode()
        {
            if (this.textInput != null)
            {
                if (this.displayMode == UserValueDisplayMode.TextInput)
                {
                    this.textInput.Style.Display = "";
                }
                else
                {
                    this.textInput.Style.Display = "none";
                }
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
