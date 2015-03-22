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
    public class PersistButton : ItemSetControl
    {
        [ScriptName("e_text")]
        private Element text;

        [ScriptName("e_icon")]
        private Element icon;

        [ScriptName("e_animatingIcon")]
        private Element animatingIcon;

        private bool savedBefore = false;

        private IItemSetEditor editor;

        public IItemSetEditor ItemSetEditor
        {
            get
            {
                return this.editor;
            }

            set
            {
                this.editor = value;
            }
        }

        public PersistButton()
        {
            this.TrackInteractionEvents = true;
        }

        protected override void OnClick(ElementEvent e)
        {
            base.OnClick(e);

            if (this.editor != null)
            {
                this.editor.Save();
            }
        }

        protected override void OnItemSetChanged()
        {
            if (this.ItemSet != null)
            {
                this.ItemSet.SaveStateChanged += ItemSet_SaveStateChanged;
            }

            base.OnItemSetChanged();
        }

        private void ItemSet_SaveStateChanged(object sender, DataStoreItemSetEventArgs e)
        {
            this.UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (this.text == null || this.ItemSet == null)
            {
                return;
            }

            if (this.ItemSet.IsSaving)
            {
                this.icon.Style.Display = "none";
                this.animatingIcon.Style.Display = "";
                ElementUtilities.SetText(this.text, "Saving");
                this.savedBefore = true;

                this.Element.Style.BorderWidth = "1px";
                this.Element.Style.BorderColor = "#F0F0F0";
                this.text.Style.BorderColor = "#A0A0A0";
            }
            else if (this.ItemSet.NeedsSaving)
            {
                this.icon.Style.Display = "";
                this.animatingIcon.Style.Display = "none";
                ElementUtilities.SetText(this.text, "Save");

                this.Element.Style.BorderWidth = "1px";
                this.text.Style.BorderColor = "#303030";
                this.Element.Style.BorderColor = "#B0B0B0";
            }
            else
            {
                this.icon.Style.Display = "none";
                this.animatingIcon.Style.Display = "none";

                if (this.savedBefore)
                {
                    ElementUtilities.SetText(this.text, "Saved");
                }
                else
                {
                    ElementUtilities.SetText(this.text, "");
                }

                this.Element.Style.BorderWidth = "0px";
                this.Element.Style.BorderColor = "transparent";
                this.text.Style.BorderColor = "#A0A0A0";
            }
        }

        protected override void OnUpdate()
        {
            this.UpdateStatus();
        }
    }
}
