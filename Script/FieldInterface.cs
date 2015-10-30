/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BL.Forms
{
    public enum FieldDataFormat
    {
        DefaultFormat = 0,
        Literal = 1,
        JSONStructure = 2,
        IdentifierOnly = 3
    }

    public enum FieldInitializer
    {
        None = 0,
        Choices5DailyDatesFromTomorrow = 1
    }

    public class FieldInterface : SerializableObject
    {
        private FieldDataFormat dataFormatOverride = FieldDataFormat.DefaultFormat;
        private String titleOverride;
        private bool? requiredOverride;
        private bool? allowNullOverride;
        private DisplayState display;
        private FieldChoiceCollection choicesOverride;
        private FieldInitializer initializer;
        private String name;
        private Nullable<int> order;
        private FieldMode fieldMode;
        private Nullable<FieldInterfaceType> interfaceTypeOverride;
        private FieldInterfaceTypeOptions interfaceTypeOptionsOverride;
        private PropertyChangedEventHandler interfaceTypeOptionsOverridePropertyChanged;

        [ScriptName("i_dataFormatOverride")]
        public FieldDataFormat DataFormatOverride
        {
            get
            {
                return this.dataFormatOverride;
            }

            set
            {
                if (this.dataFormatOverride == value)
                {
                    return;
                }

                this.dataFormatOverride = value;

                this.NotifyPropertyChanged("DataFormatOverride");
            }
        }

        [ScriptName("i_initializer")]
        public FieldInitializer Initializer
        {
            get
            {
                return this.initializer;
            }

            set
            {
                if (this.initializer == value)
                {
                    return;
                }

                this.initializer = value;

                this.NotifyPropertyChanged("Initializer");
            }
        }

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

        [ScriptName("b_allowNullOverride")]
        public bool? AllowNullOverride
        {
            get
            {
                return this.allowNullOverride;
            }

            set
            {
                if (this.allowNullOverride == value)
                {
                    return;
                }

                this.allowNullOverride = value;

                this.NotifyPropertyChanged("AllowNullOverride");
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
        public Nullable<int> Order
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

                if (this.interfaceTypeOptionsOverride != null)
                {
                    this.interfaceTypeOptionsOverride.PropertyChanged -= interfaceTypeOptionsOverridePropertyChanged;
                }

                this.interfaceTypeOptionsOverride = value;

                this.interfaceTypeOptionsOverride.PropertyChanged += interfaceTypeOptionsOverridePropertyChanged;

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
            this.interfaceTypeOptionsOverridePropertyChanged = this.interfaceTypeOptionsOverride_PropertyChanged;

            this.interfaceTypeOptionsOverride = new FieldInterfaceTypeOptions();

            this.interfaceTypeOptionsOverride.PropertyChanged += interfaceTypeOptionsOverridePropertyChanged;
        }

        public int EnsureOrderDefaultToLast(ItemSetInterface itemSetInterface)
        {
            if (this.order != null)
            {
                return (int)this.order;
            }
            
            int maxOrder = 0;

            foreach (FieldInterface fi in itemSetInterface.FieldInterfaces)
            {
                if (fi.Order != null && (int)fi.Order > maxOrder)
                {
                    maxOrder = (int)fi.Order;
                }
            }

            this.order = maxOrder + 10;

            return maxOrder + 10;
        }

        private void interfaceTypeOptionsOverride_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged("InterfaceTypeOptionsOverride");
        }
    }
}
