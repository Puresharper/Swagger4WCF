using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Swagger4WCF
{
    static public partial class YAML
    {
        public partial class Document
        {
            static public Document Generate(TypeDefinition type, Documentation documentation)
            {
                return Content.Generate(type, documentation ?? Documentation.Empty());
            }

            static public implicit operator string(YAML.Document document)
            {
                return document?.ToString();
            }

            private TypeDefinition m_Type;
            private string m_Value;

            private Document(TypeDefinition type, string value)
            {
                this.m_Type = type;
                this.m_Value = value;
            }

            public TypeDefinition Type
            {
                get { return this.m_Type; }
            }

            override public string ToString()
            {
                return this.m_Value;
            }
        }
    }
}
