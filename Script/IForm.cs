/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;
using System.Collections.Generic;
using System.Html;
using System.Diagnostics;
using jQueryApi;
using BL.UI;
using BL.Data;
using Kendo.UI;

namespace BL.Forms
{
    public interface IForm
    {

        ImageBrowserOptions DefaultImageBrowserOptions { get; set; }

        ItemSetInterface ItemSetInterface { get; set;  }
        FormMode Mode { get; set; }
        IItem Item { get; set; }

        bool IsFieldValidForItem(IDataStoreField field, IItem item);
        String GetFieldDisplayNameOverride(String fieldName);
        bool? GetFieldRequiredOverride(String fieldName);
        bool? GetFieldAllowNullOverride(String fieldName);
        FieldChoiceCollection GetFieldChoicesOverride(String fieldName);
        DisplayState GetAdjustedDisplayState(String fieldName);
        Nullable<FieldInterfaceType> GetFieldInterfaceTypeOverride(String fieldName);
        FieldInterfaceTypeOptions GetFieldInterfaceTypeOptionsOverride(String fieldName);
        FieldMode GetFieldModeOverride(String fieldName);
    }
}
