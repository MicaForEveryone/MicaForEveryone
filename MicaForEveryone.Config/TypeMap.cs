using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MicaForEveryone.Config.Primitives;
using MicaForEveryone.Config.Reflection;

namespace MicaForEveryone.Config
{
    public sealed class TypeMap
    {
        private readonly Context _context;

        private Dictionary<Type, XclType> _typeMap = new();

        public TypeMap(Context context)
        {
            _context = context;
            RegisterType(typeof(string), XclStringType.Instance);
            RegisterType(typeof(bool), XclBooleanType.Instance);
        }

        //public void RegisterByAttribute()
        //{
        //    var types = AppDomain.CurrentDomain.GetAssemblies()
        //        .SelectMany(assembly => assembly.GetTypes())
        //        .Where(type => type.IsDefined(typeof(XclTypeAttribute), true));
        //    foreach (var type in types)
        //    {
        //        if (type.IsEnum)
        //        {
        //            RegisterEnum(type);
        //        }
        //        else
        //        {
        //            RegisterClass(type);
        //        }
        //    }
        //}

        internal Dictionary<string, XclType> XclTypes { get; } = new();

        public void RegisterType(Type type, XclType xclType)
        {
            if (XclTypes.ContainsKey(xclType.Name))
                throw new RuntimeError(xclType, "The type is already registered.");
            if (xclType.Context == null)
                xclType.Context = _context;
            XclTypes.Add(xclType.Name, xclType);
            _typeMap.Add(type, xclType);
        }

        public void RegisterClass(Type type)
        {
            if (!type.IsClass)
                throw new RuntimeError(null, "Given type is not a class.");
            var className = (type.GetCustomAttributes(typeof(XclTypeAttribute), false).FirstOrDefault() as XclTypeAttribute)?.TypeName ?? type.Name;
            var xclClass = new XclClass(_context, className, type);
            RegisterType(type, xclClass);
        }

        public void RegisterEnum(Type type)
        {
            if (!type.IsEnum)
                throw new RuntimeError(null, "Given type is not an enum.");
            var name = (type.GetCustomAttributes(typeof(XclTypeAttribute), false).FirstOrDefault() as XclTypeAttribute)?.TypeName ?? type.Name;
            var xclType = new XclEnumType(name, type);
            RegisterType(type, xclType);
        }

        public XclType GetXclType(Type type)
        {
            if (!_typeMap.ContainsKey(type))
                return null;
            return _typeMap[type];
        }

        public XclType GetXclType(string name)
        {
            if (!XclTypes.ContainsKey(name))
                return null;
            return XclTypes[name];
        }
    }
}
