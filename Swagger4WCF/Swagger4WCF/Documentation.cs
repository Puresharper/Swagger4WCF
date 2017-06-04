using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Swagger4WCF
{
    public class Documentation
    {
        private Dictionary<string, string> m_Dictionary = new Dictionary<string, string>();

        static private string Arrange(string summary)
        {
            var _summary = summary.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
            while (_summary.Contains("  ")) { _summary = _summary.Replace("  ", " "); }
            return _summary.Trim();
        }

        static public Documentation Load(string location, AssemblyDefinition assembly)
        {
            var _location = string.Concat(new Uri(Path.GetDirectoryName(location)).LocalPath, @"\", assembly.Name.Name, ".xml");
            if (File.Exists(_location)) { return new Documentation(_location); }
            return new Documentation();
        }

        static public Documentation Empty()
        {
            return new Documentation();
        }

        private Documentation()
        {
        }

        private Documentation(string location)
        {
            foreach (var _member in XDocument.Load(location).Descendants("member"))
            {
                var _name = _member.Attribute("name").Value;
                this.m_Dictionary.Add(_name, Documentation.Arrange(_member.Element("summary").Value));
                if (_name.StartsWith("M:"))
                {
                    this.m_Dictionary.Add(string.Concat("R", _name.Substring(1)), Documentation.Arrange(_member.Element("returns").Value));
                    foreach (var _parameter in _member.Elements("param"))
                    {
                        this.m_Dictionary.Add(string.Concat("A", _name.Substring(1), ".", _parameter.Attribute("name").Value), Documentation.Arrange(_parameter.Value));
                    }
                }
            }
        }

        public struct Method
        {
            public string Summary;
            public string Response;
        }

        public string this[TypeDefinition type]
        {
            get { return this[string.Concat("T:", type.FullName)]; }
        }

        public Method this[MethodDefinition method]
        {
            get
            {
                return new Method()
                {
                    Summary = this[string.Concat("M:", method.DeclaringType.FullName, ".", method.Name, "(", string.Join(", ", method.Parameters.Select(_Parameter => _Parameter.ParameterType.FullName)), ")")],
                    Response = this[string.Concat("R:", method.DeclaringType.FullName, ".", method.Name, "(", string.Join(", ", method.Parameters.Select(_Parameter => _Parameter.ParameterType.FullName)), ")")]
                };
            }
        }

        public string this[PropertyDefinition property]
        {
            get { return this[string.Concat("P:", property.DeclaringType.FullName, ".", property.Name)]; }
        }

        public string this[MethodDefinition method, ParameterDefinition parameter]
        {
            get { return this[string.Concat("A:", method.DeclaringType.FullName, ".", method.Name, "(", string.Join(", ", method.Parameters.Select(_Parameter => _Parameter.ParameterType.FullName)), ").", parameter.Name)]; }
        }

        public string this[string identity]
        {
            get
            {
                if (this.m_Dictionary.ContainsKey(identity)) { return this.m_Dictionary[identity]; }
                return null;
            }
        }
    }
}
