// SensorSystemCollection.cs
//

using System;
using System.Collections.Generic;
using System.Collections;
#if NET
using Bendyline.Base.ScriptCompatibility;
using System.ComponentModel;
using Bendyline.Base;
using Bendyline.Data;
namespace Bendyline.Forms
#elif SCRIPTSHARP
using BL;
using BL.Data;

namespace BL.Forms
#endif
{
    public class FieldInterfaceCollection : ISerializableCollection, IEnumerable, INotifyCollectionAndStateChanged
    {
        private ArrayList fields;
        private Dictionary<String, FieldInterface> fieldsByStorageFieldName;
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ArrayList Fields
        {
            get
            {
                return this.fields;
            }
        }

        public FieldInterface this[int index]
        {
            get
            {
                return (FieldInterface)this.fields[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.fields.GetEnumerator();
        }

        public FieldInterfaceCollection()
        {
            this.fields = new ArrayList();
            this.fieldsByStorageFieldName = new Dictionary<string, FieldInterface>();
        }

        public FieldInterface GetFieldByName(String fieldName)
        {
            return this.fieldsByStorageFieldName[fieldName];
        }

        public FieldInterface EnsureFieldByName(String fieldName)
        {
            if (this.fieldsByStorageFieldName.ContainsKey(fieldName))
            {
                return this.fieldsByStorageFieldName[fieldName]; 
            }

            FieldInterface fi = new FieldInterface();
            fi.Name = fieldName;
            this.Add(fi);

            return fi;
        }
        
        public void RemoveByName(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs != null)
            {
                this.fields.Remove(fs);
                this.fieldsByStorageFieldName[fieldName] = null;

                if (this.CollectionChanged != null)
                {
                    this.CollectionChanged(this, NotifyCollectionChangedEventArgs.ItemRemoved(fs));
                }
            }
        }

        public String GetFieldTitleOverride(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return null;
            }

            return fs.TitleOverride;
        }

        public bool? GetFieldRequiredOverride(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return null;
            }

            return fs.RequiredOverride;
        }


        public bool? GetFieldAllowNullOverride(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return null;
            }

            return fs.AllowNullOverride;
        }


        public FieldChoiceCollectionBase GetFieldChoicesOverride(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return null;
            }

            return fs.ChoicesOverride;
        }

        public DisplayState GetAdjustedDisplayState(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return DisplayState.DefaultState;
            }

            return fs.Display;
        }

        public Nullable<FieldInterfaceType> GetFieldInterfaceTypeOverride(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return FieldInterfaceType.TypeDefault;
            }

            return fs.InterfaceTypeOverride;
        }

        public FieldInterfaceTypeOptions GetFieldInterfaceTypeOptionsOverride(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return null;
            }

            return fs.InterfaceTypeOptionsOverride;
        }

        public FieldMode GetFieldModeOverride(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                return FieldMode.FormDefault;
            }

            return fs.Mode; ;
        }

        public FieldInterface Ensure(String fieldName)
        {
            FieldInterface fs = this.fieldsByStorageFieldName[fieldName];

            if (fs == null)
            {
                fs = (FieldInterface)this.Create();
                fs.Name = fieldName;

                this.Add(fs);
            }

            return fs;
        }

        public bool MoveFieldInterface(String oldStorageFieldName, String newFieldStorageFieldName)
        {
            FieldInterface fi = this.GetByStorageFieldName(oldStorageFieldName);

            if (fi == null)
            {
                return false;
            }

            this.fieldsByStorageFieldName[oldStorageFieldName] = null;
            this.fieldsByStorageFieldName[newFieldStorageFieldName] = fi;

            fi.Name = newFieldStorageFieldName;

            return true;
        }

        public FieldInterface GetByStorageFieldName(String storageFieldName)
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
            FieldInterface sens = new FieldInterface();

            return sens;
        }

        public void Add(SerializableObject fieldSettings)
        {
            this.fields.Add(fieldSettings);
            this.fieldsByStorageFieldName[((FieldInterface)fieldSettings).Name] = (FieldInterface)fieldSettings;

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
