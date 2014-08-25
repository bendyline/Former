/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BL.Forms
{
    public class FieldInterface : SerializableObject
    {
        private String titleOverride;
        private bool? requiredOverride;
        private DisplayState display;
        private FieldChoiceCollection choicesOverride;
        private String name;
        private int order;
        private FieldMode fieldMode;
        private Nullable<FieldInterfaceType> interfaceTypeOverride;
        private FieldInterfaceTypeOptions interfaceTypeOptionsOverride;

        [ScriptName("s_name")]
        public String Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name == value)
                {
                    return;
                }

                this.name = value;

                this.NotifyPropertyChanged("Name");
            }
        }

        [ScriptName("i_mode")]
        public FieldMode Mode
        {
            get
            {
                return this.fieldMode;
            }

            set
            {
                if (this.fieldMode == value)
                {
                    return;
                }

                this.fieldMode = value;

                this.NotifyPropertyChanged("Mode");
            }
        }

        [ScriptName("s_titleOverride")]
        public String TitleOverride
        {
            get
            {
                return this.titleOverride;
            }

            set
            {
                if (this.titleOverride == value)
                {
                    return;
                }

                this.titleOverride = value;

                this.NotifyPropertyChanged("TitleOverride");
            }
        }

        [ScriptName("b_requiredOverride")]
        public bool? RequiredOverride
        {
            get
            {
                return this.requiredOverride;
            }

            set
            {
                if (this.requiredOverride == value)
                {
                    return;
                }

                this.requiredOverride = value;

                this.NotifyPropertyChanged("RequiredOverride");
            }
        }

        [ScriptName("i_order")]
        public int Order
        {
            get
            {
                return this.order;
            }

            set
            {
                if (this.order == value)
                {
                    return;
                }

                this.order = value;

                this.NotifyPropertyChanged("Order");
            }
        }

        [ScriptName("i_interfaceTypeOverride")]
        public Nullable<FieldInterfaceType> InterfaceTypeOverride
        {
            get
            {
                return this.interfaceTypeOverride;
            }

            set
            {
                if (this.interfaceTypeOverride == value)
                {
                    return;
                }

                this.interfaceTypeOverride = value;

                this.NotifyPropertyChanged("InterfaceTypeOverride");
            }
        }

        [ScriptName("o_interfaceTypeOptionsOverride")]
        public FieldInterfaceTypeOptions InterfaceTypeOptionsOverride
        {
            get
            {
                return this.interfaceTypeOptionsOverride;
            }

            set
            {
                if (this.interfaceTypeOptionsOverride == value)
                {
                    return;
                }

                this.interfaceTypeOptionsOverride = value;

                this.NotifyPropertyChanged("InterfaceTypeOptionsOverride");
            }
        }

        [ScriptName("p_choicesOverride")]
        public FieldChoiceCollection ChoicesOverride
        {
            get
            {
                if (this.choicesOverride == null)
                {
                    this.choicesOverride = new FieldChoiceCollection();
                }

                return this.choicesOverride;
            }

            set
            {
                this.choicesOverride = value;

                this.NotifyPropertyChanged("ChoicesOverride");
            }
        }

        [ScriptName("i_display")]
        public DisplayState Display
        {
            get
            {
                return this.display;
            }

            set
            {
                if (this.display == value)
                {
                    return;
                }

                this.display = value;

                this.NotifyPropertyChanged("Display");
            }
        }

        public FieldInterface()
        {
        }

        public bool IsEqualTo(FieldInterface settings)
        {
            if (this.Display != settings.Display)
            {
                return false;
            }

            if (this.Display != settings.Display)
            {
                return false;
            }

            return true;
        }

    }
}
