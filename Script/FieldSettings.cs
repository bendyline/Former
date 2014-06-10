/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using BL.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BL.Forms
{
    public class FieldSettings : SerializableObject
    {
        private String titleOverride;
        private AdjustedFieldState fieldState;
        private FieldChoiceCollection choicesOverride;
        private String name;
        private FieldMode fieldMode;
        private FieldUserInterfaceType userInterfaceTypeOverride;
        private FieldUserInterfaceOptions userInterfaceOptionsOverride;

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

        [ScriptName("i_fieldMode")]
        public FieldMode FieldModeOverride
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

                this.NotifyPropertyChanged("FieldMode");
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

                this.NotifyPropertyChanged("Title");
            }
        }

        [ScriptName("i_userInterfaceTypeOverride")]
        public FieldUserInterfaceType UserInterfaceTypeOverride
        {
            get
            {
                return this.userInterfaceTypeOverride;
            }

            set
            {
                if (this.userInterfaceTypeOverride == value)
                {
                    return;
                }

                this.userInterfaceTypeOverride = value;

                this.NotifyPropertyChanged("UserInterfaceTypeOverride");
            }
        }

        [ScriptName("o_userInterfaceOptionsOverride")]
        public FieldUserInterfaceOptions UserInterfaceOptionsOverride
        {
            get
            {
                return this.userInterfaceOptionsOverride;
            }

            set
            {
                if (this.userInterfaceOptionsOverride == value)
                {
                    return;
                }

                this.userInterfaceOptionsOverride = value;

                this.NotifyPropertyChanged("UserInterfaceOptionsOverride");
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

        [ScriptName("i_fieldState")]
        public AdjustedFieldState FieldState
        {
            get
            {
                return this.fieldState;
            }

            set
            {
                if (this.fieldState == value)
                {
                    return;
                }

                this.fieldState = value;

                this.NotifyPropertyChanged("FieldState");
            }
        }

        public FieldSettings()
        {
        }
    }
}
