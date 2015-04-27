/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Diagnostics;
using jQueryApi;
using BL.UI;
using BL.Data;
using System.Runtime.CompilerServices;

namespace BL.Forms
{
    public class DateTimeFieldControl : FieldControl
    {
        public Date CurrentValue
        {
            get
            {
                object val = this.Item.GetValue(this.FieldName);

                if (this.Field.Type == FieldType.DateTime)
                {
                    if (val == null)
                    {
                        return null;
                    }

                    return (Date)val;
                }
                else if (this.Field.Type == FieldType.ShortText || this.Field.Type == FieldType.UnboundedText)
                {
                    if (val == null)
                    {
                        return null;
                    }

                    return Date.Parse((String)val);
                }

                return null;
            }

            set
            {
                if (this.Field.Type == FieldType.DateTime)
                {
                    this.Item.SetDateValue(this.FieldName, value);
                }
                else if (this.Field.Type == FieldType.ShortText || this.Field.Type == FieldType.UnboundedText)
                {
                    if (value != null)
                    {
                        this.Item.SetStringValue(this.FieldName, JsonUtilities.EncodeDate(value));
                    }
                    else
                    {
                        this.Item.SetStringValue(this.FieldName, null);
                    }
                }
            }
        }
    }
}
