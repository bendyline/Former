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
    public class BooleanFieldControl : FieldControl
    {

        public Boolean CurrentValue
        {
            get
            {
                object val = this.Item.GetValue(this.FieldName);

                if (this.Field.Type == FieldType.BoolChoice)
                {
                    if (val == null)
                    {
                        return false;
                    }

                    return (Boolean)val;
                }
                else if (this.Field.Type == FieldType.ShortText || this.Field.Type == FieldType.UnboundedText)
                {
                    if (this.EffectiveUserInterfaceOptions != null && (this.EffectiveUserInterfaceOptions.StringFalseValue != null || this.EffectiveUserInterfaceOptions.StringTrueValue != null))
                    {
                        if ((String)val == this.EffectiveUserInterfaceOptions.StringTrueValue)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (val == null)
                    {
                        return false;
                    }

                    return Boolean.Parse((String)val);
                }
                else if (this.Field.Type == FieldType.Integer)
                {
                    if (this.EffectiveUserInterfaceOptions != null && (this.EffectiveUserInterfaceOptions.IntFalseValue != null || this.EffectiveUserInterfaceOptions.IntTrueValue != null))
                    {
                        if ((Nullable<Int32>)val == this.EffectiveUserInterfaceOptions.IntTrueValue)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if ((Nullable<Int32>)val == 0)
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }

            set
            {
                if (this.Field.Type == FieldType.BoolChoice)
                {
                    this.Item.SetBooleanValue(this.FieldName, value);
                }
                else if (this.Field.Type == FieldType.Integer)
                {
                    if (this.EffectiveUserInterfaceOptions != null && (this.EffectiveUserInterfaceOptions.IntFalseValue != null || this.EffectiveUserInterfaceOptions.IntTrueValue != null))
                    {
                        if (!value )
                        {
                            this.Item.SetInt32Value(this.FieldName, this.EffectiveUserInterfaceOptions.IntFalseValue);
                        }
                        else
                        {
                            this.Item.SetInt32Value(this.FieldName, this.EffectiveUserInterfaceOptions.IntTrueValue);
                        }

                        return;
                    }

                    if (value)
                    {
                        this.Item.SetInt32Value(this.FieldName, 1);
                    }
                    else
                    {
                        this.Item.SetInt32Value(this.FieldName, 0);
                    }
                }
                else if (this.Field.Type == FieldType.ShortText || this.Field.Type == FieldType.UnboundedText)
                {
                    if (this.EffectiveUserInterfaceOptions != null && (this.EffectiveUserInterfaceOptions.StringFalseValue != null || this.EffectiveUserInterfaceOptions.StringTrueValue != null))
                    {
                        if (!value)
                        {
                            this.Item.SetStringValue(this.FieldName, this.EffectiveUserInterfaceOptions.StringFalseValue);
                        }
                        else
                        {
                            this.Item.SetStringValue(this.FieldName, this.EffectiveUserInterfaceOptions.StringTrueValue);
                        }
                    } 
                    
                    if (value)
                    {
                        this.Item.SetStringValue(this.FieldName, "True");
                    }
                    else
                    {
                        this.Item.SetStringValue(this.FieldName, "False");
                    }
                }
            }
        }
    }
}
