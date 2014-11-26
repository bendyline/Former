/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BL.Forms
{
    public class ItemSetInterface :  SerializableObject
    {
        private FormMode formMode = FormMode.EditForm;
        private bool displayColumns = true;
        private bool displayDeleteItemButton = true;
        private FieldInterfaceCollection fieldInterfaces;

        private String accentColor1;
        private String accentColor2;

        public Nullable<int> MaxFieldOrder
        {
            get
            {
                int max = 0;

                foreach (FieldInterface fi in this.FieldInterfaces)
                {
                    if (fi.Order != null && ((int)fi.Order) > max)
                    {
                        max = (int)fi.Order;
                    }
                }

                return max;
            }
        }

        [ScriptName("s_accentColor1")]
        public String AccentColor1
        {
            get
            {
                return this.accentColor1;
            }

            set
            {
                this.accentColor1 = value;
            }
        }

        [ScriptName("s_accentColor2")]
        public String AccentColor2
        {
            get
            {
                return this.accentColor2;
            }

            set
            {
                this.accentColor2 = value;
            }
        }

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

        [ScriptName("b_displayDeleteItemButton")]
        public bool DisplayDeleteItemButton
        {
            get
            {
                return this.displayDeleteItemButton;
            }

            set
            {
                this.displayDeleteItemButton = value;
            }
        }

        [ScriptName("b_displayColumns")]
        public bool DisplayColumns
        {
            get
            {
                return this.displayColumns;
            }

            set
            {
                this.displayColumns = value;
            }
        }

        [ScriptName("p_fieldInterfaces")]
        public FieldInterfaceCollection FieldInterfaces
        {
            get
            {
                return this.fieldInterfaces;
            }
        }

        public FieldInterface this[String fieldName]
        {
            get
            {
                return this.fieldInterfaces.Ensure(fieldName);
            }
        }

        public ItemSetInterface()
        {
            this.fieldInterfaces = new FieldInterfaceCollection();
        }


        public int CompareFields(Field fieldA, Field fieldB)
        {
            FieldInterfaceCollection fsc = this.FieldInterfaces;

            FieldInterface fieldSettingsA = fsc.GetFieldByName(fieldA.Name);
            FieldInterface fieldSettingsB = fsc.GetFieldByName(fieldB.Name);

            if (fieldSettingsA == null && fieldSettingsB == null)
            {
                return fieldA.Name.CompareTo(fieldB.Name);
            }

            int orderA = -1;

            if (fieldSettingsA != null)
            {
                orderA = (int)fieldSettingsA.Order;
            }

            int orderB = -1;

            if (fieldSettingsB != null)
            {
                orderB = (int)fieldSettingsB.Order;
            }

            if (orderA < 0)
            {
                orderA = 100000;
            }

            if (orderB < 0)
            {
                orderB = 100000;
            }


            if (orderA == orderB)
            {
                return fieldA.Name.CompareTo(fieldB.Name);
            }

            return orderA - orderB;
        }

    }
}
