/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BL.Forms
{
    public class FormSettings :  SerializableObject
    {
        private FormMode formMode = FormMode.EditForm;

        private FieldSettingsCollection fieldSettings;

        [ScriptName("i_mode")]
        public FormMode Mode
        {
            get
            {
                return this.formMode;
            }

            set
            {
                this.formMode = value;
            }
        }

        [ScriptName("p_fieldSettingsCollection")]
        public FieldSettingsCollection FieldSettingsCollection
        {
            get
            {
                return this.fieldSettings;
            }
        }

        public FieldSettings this[String fieldName]
        {
            get
            {
                return this.fieldSettings.Ensure(fieldName);
            }
        }

        public FormSettings()
        {
            this.fieldSettings = new FieldSettingsCollection();
        }
    }
}
