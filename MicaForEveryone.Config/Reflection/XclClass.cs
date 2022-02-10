using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MicaForEveryone.Config.Parser;

namespace MicaForEveryone.Config.Reflection
{
    public class XclClass : XclType
    {
        public static Type GetFieldOrPropertyType(MemberInfo member)
        {
            if (member is FieldInfo field)
            {
                return field.FieldType;
            }
            else if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }
            return null;
        }

        private readonly Type _type;
        private readonly Dictionary<string, XclField> _fields;
        private readonly Dictionary<XclField, MemberInfo> _fieldsMap;
        private readonly ConstructorInfo _constructor;
        private readonly MemberInfo _parameterField;

        internal XclClass(string name, XclType parameterType, XclField[] fields) : base(name)
        {
            ParameterType = parameterType;
            _fields = fields.ToDictionary(field => field.Name, field => field);
        }

        internal XclClass(string name, Type type) : base(name)
        {
            _type = type;
            _fields = new();
            _fieldsMap = new();

            _parameterField = type.GetMembers().FirstOrDefault(member => member.IsDefined(typeof(XclParameterAttribute)));
            ParameterType = _parameterField == null ? null : TypeMap.Instance.GetXclType(GetFieldOrPropertyType(_parameterField));

            _constructor = type.GetConstructors().FirstOrDefault(ctor => ctor.IsDefined(typeof(XclConstructorAttribute), false));
            var parameters = _constructor?.GetParameters();
            if (parameters?.Length > 1)
                throw new RuntimeError(null, $"Error in registring type {name}, only one parameter supported in constructor.");
            if (parameters?.Length == 1 && parameters[0].ParameterType != GetFieldOrPropertyType(_parameterField))
                throw new RuntimeError(null, $"Error in registring type {name}, constructor should have parameter of type {ParameterType.Name}.");

            foreach (var member in type.GetMembers().Where(
                member => member.IsDefined(typeof(XclFieldAttribute), false)))
            {
                var fieldName = member.GetCustomAttribute<XclFieldAttribute>().Name ?? member.Name;
                var fieldType = TypeMap.Instance.GetXclType(GetFieldOrPropertyType(member));
                var field = new XclField(fieldName, fieldType);
                _fields.Add(fieldName, field);
                _fieldsMap.Add(field, member);
            }
        }

        public XclType ParameterType { get; }

        public XclField GetField(string name)
        {
            return _fields[name];
        }

        public XclInstance CreateXclInstance(XclValue parameter)
        {
            return new XclInstance(null, this, parameter);
        }

        public override XclValue ToXclValue(object value)
        {
            return ToXclInstance(value);
        }

        public XclInstance ToXclInstance(object value)
        {
            if (!_type.IsAssignableFrom(value.GetType()))
                throw new RuntimeError(null, $"Can't convert value of type {value.GetType()} to {Name}.");
            if (_parameterField is FieldInfo field)
            {
                return ToXclInstance(value, ParameterType.ToXclValue(field.GetValue(value)));
            }
            else if (_parameterField is PropertyInfo property)
            {
                return ToXclInstance(value, ParameterType.ToXclValue(property.GetValue(value)));
            }
            return ToXclInstance(value, null);
        }

        public XclInstance ToXclInstance(object value, XclValue parameter)
        {
            if (!_type.IsAssignableFrom(value.GetType()))
                throw new RuntimeError(null, $"Can't convert value of type {value.GetType()} to {Name}.");
            return new XclInstance(null, this, parameter, value);
        }

        public XclField[] GetFields()
        {
            return _fields.Values.ToArray();
        }

        internal object CreateInstance(XclValue parameter)
        {
            if (_type == null)
                return null;
            if (ParameterType == null)
                return Activator.CreateInstance(_type);
            if (_constructor == null)
                return Activator.CreateInstance(_type, parameter);
            return _constructor.Invoke(new[] { parameter.Value });
        }

        internal void SetFieldValue(XclValue instance, XclField field, XclValue value)
        {
            var member = _fieldsMap[field];
            if (member is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(instance.Value, value.Value);
            }
            else if (member is PropertyInfo property)
            {
                property.SetValue(instance.Value, value.Value);
            }
        }

        internal object GetFieldValue(XclInstance instance, XclField field)
        {
            var member = _fieldsMap[field];
            if (member is FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(instance.Value);
            }
            else if (member is PropertyInfo property)
            {
                return property.GetValue(instance.Value);
            }
            return null;
        }

        internal override XclValue SymbolToValue(Symbol symbol)
        {
            throw new RuntimeError(symbol, "Can't create value of type XclClass.");
        }

        internal override Symbol ValueToSymbol(XclValue value)
        {
            throw new RuntimeError(value, "Can't create only one symbol from XclInstance.");
        }
    }
}
