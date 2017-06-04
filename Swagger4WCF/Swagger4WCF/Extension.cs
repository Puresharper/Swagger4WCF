using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Swagger4WCF
{
    static internal class Extension
    {
        static public T GetCustomAttribute<T>(this MemberInfo member)
            where T : Attribute
        {
            return member.GetCustomAttributes(typeof(T), true).SingleOrDefault() as T;
        }

        static public CustomAttribute GetCustomAttribute<T>(this AssemblyDefinition assembly)
            where T : Attribute
        {
            var _type = assembly.MainModule.Import(typeof(T)).Resolve();
            return assembly.CustomAttributes.SingleOrDefault(_Attribute => assembly.MainModule.Import(_Attribute.AttributeType).Resolve() == _type);
        }

        static public CustomAttribute GetCustomAttribute<T>(this TypeDefinition type)
            where T : Attribute
        {
            var _type = type.Module.Import(typeof(T)).Resolve();
            return type.CustomAttributes.SingleOrDefault(_Attribute => _Attribute.AttributeType.Resolve() == _type);
        }

        static public CustomAttribute GetCustomAttribute<T>(this MethodDefinition method)
            where T : Attribute
        {
            var _type = method.Module.Import(typeof(T)).Resolve();
            return method.CustomAttributes.SingleOrDefault(_Attribute => _Attribute.AttributeType.Resolve() == _type);
        }

        static public CustomAttribute GetCustomAttribute<T>(this PropertyDefinition property)
            where T : Attribute
        {
            var _type = property.Module.Import(typeof(T)).Resolve();
            return property.CustomAttributes.SingleOrDefault(_Attribute => _Attribute.AttributeType.Resolve() == _type);
        }

        static public T Argument<T>(this CustomAttribute attribute, int index)
        {
            return (T)attribute.ConstructorArguments[index].Value;
        }

        static public bool Value(this CustomAttribute attribute, string name)
        {
            return attribute.Properties.Any(_Property => _Property.Name == name);
        }

        static public T Value<T>(this CustomAttribute attribute, string name)
        {
            return (T)attribute.Properties.Single(_Property => _Property.Name == name).Argument.Value;
        }
    }
}
