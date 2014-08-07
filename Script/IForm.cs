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
    public interface IForm
    {
        FormSettings Settings { get; set;  }
        FormMode Mode { get; set; }
        IItem Item { get; set; }

        bool IsFieldValidForItem(IDataStoreField field, IItem item);
        String GetFieldTitleOverride(String fieldName);
        bool? GetFieldRequiredOverride(String fieldName);
        FieldChoiceCollection GetFieldChoicesOverride(String fieldName);
        AdjustedFieldState GetAdjustedFieldState(String fieldName);
        FieldUserInterfaceType GetFieldUserInterfaceTypeOverride(String fieldName);
        FieldUserInterfaceOptions GetFieldUserInterfaceOptionsOverride(String fieldName);
        FieldMode GetFieldModeOverride(String fieldName);
    }
}
