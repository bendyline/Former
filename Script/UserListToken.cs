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
    public class UserListToken : Control
    {
        [ScriptName("e_userSummaryArea")]
        private Element userSummaryArea;

        private UserControl userSummary;
        private UserReference userReference;
        private UserList userList;

        public UserList UserList
        {
            get
            {
                return this.userList;
            }

            set
            {
                this.userList = value;
            }
        }

        public UserReference UserReference
        {
            get
            {
                return this.userReference;
            }

            set
            {
                this.userReference = value;
            }
        }
  
        public UserListToken()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        [ScriptName("v_onDeleteCellClick")]
        protected void HandleDeleteClick(ElementEvent ee)
        {
            this.UserList.RemoveReference(this.userReference);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            ElementUtilities.ClearChildElements(this.userSummaryArea);

            if (this.userReference == null)
            {
                return;
            }

             if (this.userReference.Type == UserReferenceType.StructuredUser)
            {
                if (this.userSummary == null)
                {
                    this.userSummary = Context.Current.ObjectProvider.CreateObject("userSummaryNoBorder") as UserControl;

                    this.userSummary.EnsureElements();
                }

                this.userSummary.UserReference = this.userReference;
                this.userSummaryArea.AppendChild(this.userSummary.Element);

                this.userSummaryArea.Style.Display = "";
            }
             else if (this.userReference.Type == UserReferenceType.Adhoc)
             {
                 Element e = this.CreateElement("userSummaryText");

                 ElementUtilities.SetText(e, this.userReference.NickName);

                 this.userSummaryArea.AppendChild(e);
             }
             else
            {
                this.userSummaryArea.Style.Display = "none";
            }
        }       
    }
}
