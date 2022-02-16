using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections;

namespace IDE.Core.ViewModels
{
    public class MultipleSelectionObject : //DynamicObject
                                         ICustomTypeDescriptor, INotifyPropertyChanged
    {
        public MultipleSelectionObject()
        {
        }

        IList selectedObjects;
        public IList SelectedObjects
        {
            get
            {
                return selectedObjects;
            }
            set
            {
                if (selectedObjects != null)
                    foreach (INotifyPropertyChanged selectedObject in selectedObjects)
                        selectedObject.PropertyChanged -= SelectedObject_PropertyChanged;

                selectedObjects = value;
                foreach (INotifyPropertyChanged selectedObject in selectedObjects)
                    selectedObject.PropertyChanged += SelectedObject_PropertyChanged;
            }
        }

        // public IDrawingViewModel Parent { get; set; }

        void SelectedObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var p = GetPropertyDescriptor(e.PropertyName);
            if (p != null)
            {
                UpdatePropertyValue(p);
                // HandlePropertyChanged(p, EventArgs.Empty);
                OnPropertyChanged(e.PropertyName);
            }
        }

        ObjectPropertyDescriptor GetPropertyDescriptor(string name)
        {
            //var p = propertiesCollectionCache.Cast<ObjectPropertyDescriptor>()
            //                                 .FirstOrDefault(pd => pd.Name == name);
            if (propertiesCollectionCache == null)
                return null;
            var p = propertiesCollectionCache[name] as ObjectPropertyDescriptor;
            return p;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

        #region Dynamic Object

        //public override bool TryGetMember(GetMemberBinder binder, out object result)
        //{
        //    result = null;
        //    var memberName = binder.Name;
        //    var p = GetPropertyDescriptor(memberName);
        //    if (p != null)
        //    {
        //        result= p.Value;
        //    }

        //    return true;
        //}

        //public override bool TrySetMember(SetMemberBinder binder, object value)
        //{
        //    var memberName = binder.Name;
        //    var p = GetPropertyDescriptor(memberName);
        //    if (p != null)
        //    {
        //        p.Value = value;
        //        OnPropertyChanged(string.Empty);
        //    }
        //    return true;
        //}

        #endregion


        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, noCustomTypeDesc: true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, noCustomTypeDesc: true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, noCustomTypeDesc: true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, noCustomTypeDesc: true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, noCustomTypeDesc: true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, noCustomTypeDesc: true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);
        }

        PropertyDescriptorCollection propertiesCollectionCache;
        public PropertyDescriptorCollection GetProperties()
        {
            CleanPropertiesCache();

            if (SelectedObjects != null && SelectedObjects.Count > 0)
            {
                propertiesCollectionCache = GetCommonProperties();
                return propertiesCollectionCache;
            }
            return null;
        }

        void CleanPropertiesCache()
        {
            //todo remove any handlers to avoid mem leak
            //if(propertiesCollectionCache!=null)
            //{
            //    foreach(PropertyDescriptor p in propertiesCollectionCache)
            //        p.
            //}
        }

        PropertyDescriptorCollection GetCommonProperties()
        {
            var commonProps = new PropertyDescriptorCollection(null);

            if (SelectedObjects != null)
            {
                IList<ObjectPropertyDescriptor> currentProperties = null;

                foreach (var selectedObject in SelectedObjects)
                {
                    var props = TypeDescriptor.GetProperties(selectedObject).Cast<PropertyDescriptor>().ToList();
                    if (currentProperties == null)
                    {
                        var owner = selectedObject;
                        currentProperties = props.Select(p => new ObjectPropertyDescriptor(p, owner)
                        {
                            // Owner = owner,//selectedObject,
                        }).ToList();
                    }
                    else
                    {
                        currentProperties = (from cp in currentProperties
                                             from p in props
                                             where cp.Name == p.Name && cp.PropertyType == p.PropertyType
                                             select cp).ToList();
                    }
                }

                if (currentProperties != null)
                {
                    foreach (var p in currentProperties)
                    {
                        UpdatePropertyValue(p);

                        commonProps.Add(p);
                        p.AddValueChanged(p, HandlePropertyChanged);
                    }
                }
            }
            return commonProps;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            //    if (SelectedObjects.Count > 0)
            //        return SelectedObjects[0];

            //    var opd = pd as ObjectPropertyDescriptor;
            //    if (opd != null)
            //        return opd.Owner;

            //    return null;

            return this;
        }

        void HandlePropertyChanged(object sender, EventArgs e)
        {

            //CustomTypeDescriptor t;t.
            var propDescriptor = sender as ObjectPropertyDescriptor;
            if (propDescriptor != null)
            {
                foreach (var selectedObject in SelectedObjects)
                {
                    var pi = selectedObject.GetType().GetProperty(propDescriptor.Name, propDescriptor.PropertyType);
                    if (pi != null)
                    {
                        pi.SetValue(selectedObject, propDescriptor.Value);
                    }
                }
            }
        }

        void UpdatePropertyValue(ObjectPropertyDescriptor p)
        {
            object currentValue = null;
            //get common property values
            foreach (var selectedObject in SelectedObjects)
            {
                var pi = selectedObject.GetType().GetProperty(p.Name, p.PropertyType);
                if (pi != null)
                {
                    var objValue = pi.GetValue(selectedObject);
                    if (currentValue != null)
                    {
                        if (objValue != null && !objValue.Equals(currentValue))
                        {
                            currentValue = null;
                            break;
                        }
                    }
                    else
                    {
                        currentValue = objValue;
                    }
                }
            }

            //how about struct?
            if (currentValue != null)
                p.Value = currentValue;
        }

        public override string ToString()
        {
            return $"{selectedObjects.Count} objects selected";
        }
    }

    /*
    #region Complex Object

    [ExpandableObject]
    public class ComplexObject
    {
        public int ID { get; set; }

        [ExpandableObject]
        public ExpandableObservableCollection<ComplexSubObject> Classes { get; set; }

        public ComplexObject()
        {
            ID = 1;
            Classes = new ExpandableObservableCollection<ComplexSubObject>();
            Classes.Add(new ComplexSubObject() { Name = "CustomFoo" });
            Classes.Add(new ComplexSubObject() { Name = "My Other Foo" });
        }
    }

    [ExpandableObject]
    public class ComplexSubObject
    {
        public string Name { get; set; }

        [ExpandableObject]
        public ExpandableObservableCollection<SimpleValues> Types { get; set; }

        public ComplexSubObject()
        {
            Types = new ExpandableObservableCollection<SimpleValues>();
            Types.Add(new SimpleValues() { name = "foo", value = "bar" });
            Types.Add(new SimpleValues() { name = "bar", value = "foo" });
        }
    }

    public class SimpleValues
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class ExpandableObservableCollection<T> : ObservableCollection<T>,
                                                     ICustomTypeDescriptor
    {
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < Count; i++)
            {
                pds.Add(new ItemPropertyDescriptor<T>(this, i));
            }

            return pds;
        }

        #region Use default TypeDescriptor stuff

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, noCustomTypeDesc: true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, noCustomTypeDesc: true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, noCustomTypeDesc: true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, noCustomTypeDesc: true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, noCustomTypeDesc: true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, noCustomTypeDesc: true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes, noCustomTypeDesc: true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    public class ItemPropertyDescriptor<T> : PropertyDescriptor
    {
        private readonly ObservableCollection<T> _owner;
        private readonly int _index;

        public ItemPropertyDescriptor(ObservableCollection<T> owner, int index)
          : base("#" + index, null)
        {
            _owner = owner;
            _index = index;
        }

        public override AttributeCollection Attributes
        {
            get
            {
                var attributes = TypeDescriptor.GetAttributes(GetValue(null), false);


                if (!attributes.OfType<ExpandableObjectAttribute>().Any())
                {
                    // copy all the attributes plus an extra one (the
                    // ExpandableObjectAttribute)
                    // this ensures that even if the type of the object itself doesn't have the
                    // ExpandableObjectAttribute, it will still be expandable. 
                    var newAttributes = new Attribute[attributes.Count + 1];
                    attributes.CopyTo(newAttributes, newAttributes.Length - 1);
                    newAttributes[newAttributes.Length - 1] = new ExpandableObjectAttribute();

                    // overwrite the original
                    attributes = new AttributeCollection(newAttributes);
                }

                return attributes;
            }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return Value;
        }

        private T Value
          => _owner[_index];

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            _owner[_index] = (T)value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
          => _owner.GetType();

        public override bool IsReadOnly
          => false;

        public override Type PropertyType
          => Value?.GetType();
    }

    #endregion Complex Object
    */
}
