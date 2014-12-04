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
    public interface IItemSetEditor
    {
        event DataStoreItemEventHandler ItemAdded;
        event DataStoreItemEventHandler ItemDeleted;

        bool DisplayAddButton { get; set; }

        [ScriptName("s_itemPlacementFieldName")]
        String ItemPlacementFieldName { get; set; }

        [ScriptName("i_mode")]
        ItemSetEditorMode Mode { get; set; }

        String ItemFormTemplateId { get; set;  }
        String ItemFormTemplateIdSmall { get; set; }

        IDataStoreItemSet ItemSet { get; set; }

        ItemSetInterface ItemSetInterface { get; set;  }

        [ScriptName("b_visible")]
        bool Visible { get; set; }

        [ScriptName("s_templateId")]
        String TemplateId { get; set; }

        Element Element { get; set; }

        void EnsureElements();
        
        void Save();

        void Dispose();
        void DisposeItemInterfaceItems();
    }
}
