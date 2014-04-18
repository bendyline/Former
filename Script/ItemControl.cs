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
    public class ItemControl : Control 
    {
        private IItem item;

        public IItem Item
        {
            get
            {
                return this.item;
            }

            set
            {
                this.item = value;

                this.Update();
            }
        }
    }
}
