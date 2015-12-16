// FieldMode.cs
/* Copyright (c) Bendyline LLC. All rights reserved. Licensed under the Apache License, Version 2.0.
    You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. */

using System;

#if NET
namespace Bendyline.Forms
#elif SCRIPTSHARP

namespace BL.Forms
#endif
{
    public enum FormMode
    {
        EditForm = 0,
        NewForm = 1,
        ViewForm = 2,
        Example = 3
    }
}
