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

    public class UserList : FieldControl
    { 
        [ScriptName("e_userSummaryBin")]
        private Element userSummaryBin;

        [ScriptName("e_inputOuter")]
        private Element inputOuter;

        [ScriptName("e_updateMessage")]
        private Element updateMessage;

        [ScriptName("e_addTextValue")]
        private InputElement addTextValue;

        [ScriptName("e_addTextButton")]
        private InputElement addTextButton;

        private bool commitPending = false;

        private Date addUserFocusLoss;
        private String lastAddUserValue;

        private UserReferenceSet userReferenceSet;

        private Dictionary<String, UserListToken> tokensByReferenceUniqueKey;
        
        public UserReferenceSet UserReferenceSet
        {
            get
            {
                return this.userReferenceSet;
            }

            set
            {
                this.userReferenceSet = value;

                this.userReferenceSet.UserReferences.CollectionChanged += referenceList_CollectionChanged;
            }
        }


        public UserList()
        {
            this.tokensByReferenceUniqueKey = new Dictionary<string, UserListToken>();
            this.UserReferenceSet = new UserReferenceSet();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.addTextValue.AddEventListener("keypress", this.KeyPressCheck, true);
            this.addTextValue.AddEventListener("focus", this.AddTextFocus, true);
            this.addTextValue.AddEventListener("blur", this.AddTextBlur, true);
            this.addTextButton.AddEventListener("focus", this.AddTextButtonFocus, true);
        }

        private void AddTextFocus(ElementEvent e)
        {
            this.addTextButton.Style.Opacity = "1";
        }

        private void AddTextBlur(ElementEvent e)
        {
            this.addUserFocusLoss = Date.Now;

            this.lastAddUserValue = this.addTextValue.Value;
            this.addTextValue.Value = String.Empty;

            this.addTextButton.Style.Opacity = "0";
        }

        // If the user jumped from the text box to the add text button, e.g., via tab, add that value in.
        private void AddTextButtonFocus(ElementEvent e)
        {
            if (this.addUserFocusLoss == null || String.IsNullOrEmpty(this.lastAddUserValue))
            {
                return;
            }

            if (Date.Now.GetTime() - this.addUserFocusLoss.GetTime() < 100)
            {
                this.CommitAddUser(this.lastAddUserValue);
                this.lastAddUserValue = null;
                this.addTextValue.Focus();
            }
        }


        private void referenceList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           this.Update();
        }

        private void KeyPressCheck(ElementEvent ee)
        {
            if (ee.KeyCode == 13)
            {
                this.CommitAdd();
            }
        }

        [ScriptName("v_onAddTextButtonClick")]
        private void HandleAddTextButton(ElementEvent e)
        {
            this.CommitAdd();
        }

        private void CommitAdd()
        {
            String valu = this.addTextValue.Value;

            this.CommitAddUser(valu);
        }

        private void CommitAddUser(String valu)
        {
            if (!String.IsNullOrEmpty(valu))
            {
                UserReference currentUserReference = new UserReference();

                currentUserReference.UniqueKey = Utilities.CreateRandomId();
                currentUserReference.Type = UserReferenceType.Adhoc;
                currentUserReference.NickName = valu;

                this.UserReferenceSet.UserReferences.Add(currentUserReference);

                this.SaveValue();

                this.addTextValue.Value = String.Empty;
            }
        }

        [ScriptName("v_onAddMeButtonClick")]
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
                    this.EnsureThisUserIsInList(false);
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
            d.MaxHeight = 450;
            d.MaxWidth = 500;
            d.Closing += editUserSummary_Closing;

            d.Show();
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.ConsiderUpdate();
        }

        protected override void OnFieldChanged()
        {
            base.OnFieldChanged();

            this.ConsiderUpdate();
        }

        private void UpdateAddVisibility()
        {
            String fieldName = this.EffectiveUserInterfaceOptions.RelatedField0;

            if (fieldName != null)
            {
                Nullable<Int32> limit = this.Item.GetInt32Value(fieldName);

                if (limit != null)
                {
                    if (this.userReferenceSet.UserReferences.UserReferences.Count >= (int)limit)
                    {
                        this.inputOuter.Style.Display = "none";
                        this.updateMessage.Style.Display = "block";

                        ElementUtilities.SetText(this.updateMessage, "(maximum of " + (int)limit + " people reached.)");
                        return;
                    }
                }

            }

            if (this.EffectiveMode == FieldMode.View || this.EffectiveMode == FieldMode.Example)
            {
                this.inputOuter.Style.Display = "none";
                this.updateMessage.Style.Display = "none";
                return;
            }


            this.inputOuter.Style.Display = "";
            this.updateMessage.Style.Display = "none";
        }

        private void ConsiderUpdate()
        {

            if (this.Item == null || this.FieldName == null)
            {
                return;
            }

            String val = this.Item.GetStringValue(this.FieldName);

            if (val != null)
            {
                // temporary somewhat hacky upgrade code to undo that we used to call "uniqueKey" an "id" on the client.
                val = val.Replace("\"id\"", "\"uniqueKey\"");

                this.UserReferenceSet.LoadFromJson(val);
            }

            this.Update();
        }

        public void RemoveReference(UserReference reference)
        {
            this.userReferenceSet.UserReferences.Remove(reference);
            this.SaveValue();
        }

        private void editUserSummary_Closing(object sender, EventArgs e)
        {
            this.EnsureThisUserIsInList(false);
        }

        private void assignUserLogin_Closing(object sender, EventArgs e)
        {
            this.EnsureThisUserIsInList(true);
        }

        private void EnsureThisUserIsInList(bool showUserSummaryIfNeeded)
        {
            if (Context.Current.User != null)
            {
                foreach (UserReference ur in this.UserReferenceSet.UserReferences)
                {
                    if (ur.Type == UserReferenceType.StructuredUser && (ur.Id == Context.Current.User.Id || ur.UniqueKey == Context.Current.User.UniqueKey))
                    {
                        return;
                    }
                }

                UserReference currentUserReference = new UserReference();

                currentUserReference.UniqueKey = Context.Current.User.UniqueKey;
                currentUserReference.Id = Context.Current.User.Id;
                currentUserReference.Type = UserReferenceType.StructuredUser;
                currentUserReference.NickName = Context.Current.User.NickName;

                this.UserReferenceSet.UserReferences.Add(currentUserReference);

                this.SaveValue();
            }
        }

        private void SaveValue()
        {
            this.Item.SetStringValue(this.FieldName, this.UserReferenceSet.ToJson());
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.userReferenceSet  == null || this.userSummaryBin == null)
            {
                return;
            }

            this.UpdateAddVisibility();
            
            ElementUtilities.ClearChildElements(this.userSummaryBin);

            List<UserListToken> usedTokens = new List<UserListToken>();
            foreach (KeyValuePair<String, UserListToken> token in this.tokensByReferenceUniqueKey)
            {
                usedTokens.Add(token.Value);
            }

            foreach (UserReference ur in this.UserReferenceSet.UserReferences)
            {
                UserListToken ult = null;

                if (this.tokensByReferenceUniqueKey.ContainsKey(ur.UniqueKey) && this.tokensByReferenceUniqueKey[ur.UniqueKey] != null)
                {
                    ult = this.tokensByReferenceUniqueKey[ur.UniqueKey];
                    usedTokens.Remove(ult);
                }
                else
                {
                    ult = new UserListToken();
                    ult.UserList = this;
                    ult.UserReference = ur;

                    ult.EnsureElements();

                    this.tokensByReferenceUniqueKey[ur.UniqueKey] = ult;
                }

                this.userSummaryBin.AppendChild(ult.Element);
            }

            foreach (UserListToken ult in usedTokens)
            {
                if (ult != null)
                {
                    this.tokensByReferenceUniqueKey[ult.UserReference.UniqueKey] = null;
                    ult.Dispose();
                }
            }
        }

        public override void PersistToItem()
        {
            
        }
       
    }
}
