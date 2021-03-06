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
    public class ChoiceFieldControl : FieldControl
    {

        
        public FieldChoiceCollectionBase EffectiveFieldChoices
        {
            get
            {
                FieldChoiceCollectionBase fcc = this.Field.Choices;

                FieldChoiceCollectionBase alternateChoices = this.Form.GetFieldChoicesOverride(this.FieldName);

                if (alternateChoices != null)
                {
                    fcc = alternateChoices;
                }

                return fcc;
            }
        }
        public ChoiceFieldControl()
        {

        }

        public bool IsFieldChoiceSelected(FieldChoice fieldChoice)
        {
            object selectedVal= this.Item.GetValue(this.FieldName);
            
            object effectiveId = fieldChoice.EffectiveId;

            if (selectedVal == null)
            {
                if (selectedVal == effectiveId)
                {
                    return true;
                }

                selectedVal = "null";
            }

            if (selectedVal == effectiveId)
            {
                return true;
            }

            return false;
        }
        protected String GetOptionsHash()
        {
            String results = this.EffectiveMode.ToString();

            FieldChoiceCollectionBase fcc = this.Field.Choices;

            FieldChoiceCollectionBase alternateChoices = this.Form.GetFieldChoicesOverride(this.FieldName);

            if (alternateChoices != null)
            {
                fcc = alternateChoices;
            }

            foreach (FieldChoice fc in fcc)
            {
                results += "|" + fc.Id + "|" + fc.DisplayName + "|" + fc.ImageUrl;
            }
            
            return results;
        }

        protected String GetOptionsHashWithValue()
        {
            String results = this.GetOptionsHash();

            String id = this.Item.GetStringValue(this.FieldName);
            
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
    }
}
