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
    public class ImageFieldValue : FieldControl
    {
        [ScriptName("e_imageEditorContainer")]
        private Element imageEditorContainer;

        private Control imageEditor;

        [ScriptName("e_imageDisplay")]
        private Element imageDisplay;

        private bool calledImageBrowserCallback = false;

        public ImageFieldValue()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();

            this.Update();
        }


        private void HandleTextInputChanged(ElementEvent e)
        {
            this.SaveValue();
        }

        private void SaveValue()
        {
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.imageEditorContainer == null || !this.IsReady)
            {
                return;
            }

            String val = this.Item.GetStringValue(this.FieldName);

            if (val == null)
            {
                val = String.Empty;
            }

            if (this.EffectiveMode == FieldMode.Example)
            {
                if (this.imageEditor != null)
                {
                    this.imageEditor.Visible = false;
                }

                this.imageDisplay.Style.Display = "block";
            }
            else if (this.EffectiveMode == FieldMode.Edit)
            {
                if (this.imageEditor == null)
                {
                    this.imageEditor = Context.Current.ObjectProvider.CreateObject("image") as Control;

                    if (this.Form.DefaultImageBrowserOptions != null)
                    {
                        this.imageEditor.Visible = false;

                        this.Form.DefaultImageBrowserOptions.BeforeLaunch(this.PostLaunchContinue, this.imageEditor);
                    }

                    if (this.imageEditor is IImageEditor)
                    {
                        ((IImageEditor)this.imageEditor).PropertyChanged += ImageFieldValue_PropertyChanged;
                    }

                    this.imageEditor.EnsureElements();

                    this.imageEditorContainer.AppendChild(this.imageEditor.Element);
                }
                
                if (this.imageEditor is IImageEditor)
                {
                    ((IImageEditor)this.imageEditor).StringValue = val;
                }

                this.imageEditor.Visible = true;

                this.imageDisplay.Style.Display = "none";
            }
            else
            {
                if (!String.IsNullOrEmpty(val))
                {
                    this.imageDisplay.Style.BackgroundImage = "url(" + val + ")";
                }
                else
                {
                    this.imageDisplay.Style.BackgroundImage = String.Empty;
                }

                this.imageDisplay.Style.Display = "block";

                if (this.imageEditor != null)
                {
                    this.imageEditor.Visible = false;
                }
            }

            if (this.EffectiveUserInterfaceOptions != null)
            {
                Nullable<int> suggestedWidth = this.EffectiveUserInterfaceOptions.SuggestedWidth;

                if (suggestedWidth != null)
                {
                    this.imageDisplay.Style.MinWidth = (int)suggestedWidth + "px";

                    if (this.imageEditor != null)
                    {
                        this.imageEditor.Width = suggestedWidth;
                    }
                }
                else
                {
                    this.imageDisplay.Style.MinWidth = "150px";

                    if (this.imageEditor != null)
                    {
                        this.imageEditor.Width = 150;
                    }
                }

                Nullable<int> suggestedHeight= this.EffectiveUserInterfaceOptions.SuggestedHeight;

                if (suggestedHeight != null)
                {
                    this.imageDisplay.Style.MinHeight = (int)suggestedHeight + "px";

                    if (this.imageEditor != null)
                    {
                        this.imageEditor.Height = suggestedHeight;
                    }
                }
                else
                {
                    this.imageDisplay.Style.MinHeight= "150px";

                    if (this.imageEditor != null)
                    {
                        this.imageEditor.Height = 150;
                    }
                }

            }    
        }

        private void ImageFieldValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.imageEditor != null && this.imageEditor is IImageEditor && this.Item != null)
            {
                this.Item.SetStringValue(this.FieldName, ((IImageEditor)this.imageEditor).StringValue);
            }
        }

        private void PostLaunchContinue(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                this.calledImageBrowserCallback = true;

                this.imageEditor.Visible = true;
            }
        }


        public override void PersistToItem()
        {
            
        }       
    }
}
