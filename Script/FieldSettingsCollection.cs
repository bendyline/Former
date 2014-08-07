// SensorSystemCollection.cs
//

using System;
using System.Collections.Generic;
using System.Collections;
using BL;
using BL.Data;

namespace BL.Forms
{
    public class FieldSettingsCollection : ISerializableCollection, IEnumerable, INotifyCollectionAndStateChanged
    {
        private ArrayList fields;
        private Dictionary<String, FieldSettings> fieldsByStorageFieldName;
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ArrayList Fields
        {
            get
            {
                return this.fields;
            }
        }

        public FieldSettings this[int index]
        {
            get
            {
                return (FieldSettings)this.fields[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.fields.GetEnumerator();
        }

        public FieldSettingsCollection()
        {
            this.fields = new ArrayList();
            this.fieldsByStorageFieldName = new Dictionary<string, FieldSettings>();
        }

        public FieldSettings GetFieldByName(String fieldName)
        {
            return this.fieldsByStorageFieldName[fieldName];
        }
        
        public void RemoveByName(String fieldName)
        {
            FieldSettings fs = this.fieldsByStorageFieldName[fieldName];

            if (fs != null)
            {
                this.fields.Remove(fs);
                this.fieldsByStorageFieldName[fieldName] = null;
            }
        }

        public String GetFieldTitleOverride(String fieldName)
        {
            FieldSettings fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return null;
            }

            return fs.TitleOverride;
        }

        public bool? GetFieldRequiredOverride(String fieldName)
        {
            FieldSettings fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return null;
            }

            return fs.RequiredOverride;
        }

        public FieldChoiceCollection GetFieldChoicesOverride(String fieldName)
        {
            FieldSettings fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return null;
            }

            return fs.ChoicesOverride;
        }

        public AdjustedFieldState GetAdjustedFieldState(String fieldName)
        {
            FieldSettings fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return AdjustedFieldState.DefaultState;
            }

            return fs.FieldState;
        }

        public FieldUserInterfaceType GetFieldUserInterfaceTypeOverride(String fieldName)
        {
            FieldSettings fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return FieldUserInterfaceType.TypeDefault;
            }

            return fs.UserInterfaceTypeOverride;
        }

        public FieldUserInterfaceOptions GetFieldUserInterfaceOptionsOverride(String fieldName)
        {
            FieldSettings fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return null;
            }

            return fs.UserInterfaceOptionsOverride;
        }

        public FieldMode GetFieldModeOverride(String fieldName)
        {
            FieldSettings fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return FieldMode.FormDefault;
            }

            return fs.FieldModeOverride; ;
        }

        public FieldSettings Ensure(String fieldName)
        {
            FieldSettings fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                fs = (FieldSettings)this.Create();
                fs.Name = fieldName;

                this.Add(fs);
            }

            return fs;
        }

        public FieldSettings GetByStorageFieldName(String storageFieldName)
        {
            return this.fieldsByStorageFieldName[storageFieldName];
        }

        public void Clear()
        {
            this.fields.Clear();
            this.fieldsByStorageFieldName.Clear();
        }

        public SerializableObject Create()
        {
            FieldSettings sens = new FieldSettings();

            return sens;
        }

        public void Add(SerializableObject fieldSettings)
        {
            this.fields.Add(fieldSettings);
            this.fieldsByStorageFieldName[((FieldSettings)fieldSettings).Name] = (FieldSettings)fieldSettings;

            fieldSettings.PropertyChanged += fieldSettings_PropertyChanged;
            
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, NotifyCollectionChangedEventArgs.ItemAdded(fieldSettings));  
            }
        }

        private void fieldSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, NotifyCollectionChangedEventArgs.ItemStateChange(sender, e.PropertyName));
            }
        }
    }
}
