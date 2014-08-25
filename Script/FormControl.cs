/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

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
        private IForm form;

        public IForm Form
        {
            get
            {
                return this.form;
            }

            set
            {
                if (this.form == value)
                {
                    return;
                }

                this.form = value;

                this.Update();
            }
        }

        public virtual void PersistToItem()
        {
            
        }

        internal protected virtual void OnInterfaceChange()
        {

        }
    }
}
