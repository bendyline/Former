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
using Kendo.UI;

namespace BL.Forms
{
    public interface IImageEditor
    {
        String ContainerName
        {
            get;
            set;
        }

        String StringValue
        {
            get;
            set;
        }

        event PropertyChangedEventHandler PropertyChanged;
    }
}
