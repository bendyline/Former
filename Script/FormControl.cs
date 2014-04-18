// Forms.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Diagnostics;
using jQueryApi;
using BL.UI;
using BL.Data;

namespace BL.Forms
{
    public class FormControl : ItemControl 
    {
        private Form form;

        public Form Form
        {
            get
            {
                return this.form;
            }

            set
            {
                this.form = value;
            }
        }

        public virtual void PersistToItem()
        {
            
        }
    }
}
