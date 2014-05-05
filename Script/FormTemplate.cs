/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Diagnostics;
using jQueryApi;
using System.Runtime.CompilerServices;

namespace BL.Forms
{
    public class FormTemplate : SerializableObject
    {
        private String template;
        private String id;

        [ScriptName("s_id")]
        public String Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        [ScriptName("s_template")]
        public String Template
        {
            get
            {
                return this.template;
            }

            set
            {
                this.template = value;
            }
        }
    }
}
