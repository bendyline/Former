// Forms.cs
//

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
