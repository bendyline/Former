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

        [ScriptName("s_name")]
        public String Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
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
                this.fieldMode = value;
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
                this.titleOverride = value;
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
                this.userInterfaceTypeOverride = value;
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
                this.fieldState = value;
            }
        }

        public FieldSettings()
        {
        }
    }
}
