﻿using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Swagger4WCF
{
    static public partial class YAML
    {
        public partial class Document
        {
            private partial class Content
            {
                private StringBuilder m_Builder = new StringBuilder();
                private Tabulation m_Tabulation = new Tabulation("  ", 0);
                private List<TypeReference> definitionList = new List<TypeReference>();

                static public Document Generate(TypeDefinition type, Documentation documentation)
                {
                    return new Document(type, new Content(type, documentation));
                }

                static public implicit operator string(Content compiler)
                {
                    return compiler == null ? null : compiler.ToString();
                }

                public override string ToString()
                {
                    return this.m_Builder.ToString();
                }


                private Content(TypeDefinition type, Documentation documentation)
                {
                    this.Add("swagger: '2.0'");
                    this.Add("info:");
                    using (new Block(this))
                    {
                        this.Add("title: ", type.Name);
                        if (documentation[type] != null) { this.Add("description: ", documentation[type]); }
                        var _customAttribute = type.Module.Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                        var _argument = _customAttribute?.Argument<string>(0);
                        if (_argument != null)
                        {
                            this.Add($"version: \"{ _argument }\"");
                        }
                    }
                    this.Add("host: localhost");
                    this.Add("schemes:");
                    using (new Block(this))
                    {
                        this.Add("- http");
                        this.Add("- https");
                    }
                    this.Add("basePath: /", type.Name);
                    this.Add("paths:");
                    var _methods = type.Methods.Where(_Method => _Method.IsPublic && !_Method.IsStatic && _Method.GetCustomAttribute<OperationContractAttribute>() != null).OrderBy(_Method => _Method.MetadataToken.ToInt32()).ToArray();
                    using (new Block(this))
                    {
                        foreach (var _method in _methods)
                        {
                            this.Add(_method, documentation);
                        }
                    }
                    this.Add("definitions:");
                    using (new Block(this))
                    {
                        var responses = _methods.Select(_Method => _Method.ReturnType).Distinct()
                            .OrderBy(typeRef => typeRef.Name)
                            .Where(typeRef =>
                                !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(void)).Resolve())
                                && !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(bool)).Resolve())
                                && !(typeRef.Resolve() ==
                                     typeRef.Resolve().Module.ImportReference(typeof(string)).Resolve())
                                && !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(int)).Resolve())
                                && !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(long)).Resolve())
                                && !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(DateTime)).Resolve()))
                            .Select(_Type => _Type.IsArray ? _Type.GetElementType() : _Type).Distinct();

                        foreach (var response in responses)
                        {
                            if (!definitionList.Contains(response))
                            {
                                definitionList.Add(response);
                            }
                        }

                        var resparameters = _methods.SelectMany(_Method => _Method.Parameters).Select(x => x.ParameterType)
                            .OrderBy(typeRef => typeRef.Name)
                            .Where(typeRef =>
                                !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(void)).Resolve())
                                && !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(bool)).Resolve())
                                && !(typeRef.Resolve() ==
                                     typeRef.Resolve().Module.ImportReference(typeof(string)).Resolve())
                                && !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(int)).Resolve())
                                && !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(long)).Resolve())
                                && !(typeRef.Resolve() == typeRef.Resolve().Module.ImportReference(typeof(DateTime)).Resolve()))
                            .Select(_Type => _Type.IsArray ? _Type.GetElementType() : _Type).Distinct();

                        foreach (var resparameter in resparameters)
                        {
                            if (!definitionList.Contains(resparameter))
                            {
                                definitionList.Add(resparameter);
                            }
                        }
                        int beforeCnt = definitionList.Count;
                        for (int i = 0; i < beforeCnt; i++)
                        {
                            ParseComplexType(definitionList[i], documentation);
                        }

                        int afterCnt = definitionList.Count;

                        for (int i = beforeCnt; i < afterCnt; i++)
                        {
                            ParseComplexType(definitionList[i], documentation);
                        }

                        int afterafterCnt = definitionList.Count;

                        for (int i = afterCnt; i < afterafterCnt; i++)
                        {
                            ParseComplexType(definitionList[i], documentation);
                        }
                    }
                }

                private void Add(params string[] line)
                {
                    this.m_Builder.AppendLine(this.m_Tabulation.ToString() + string.Concat(line));
                }

                private void Add(MethodDefinition method, Documentation documentation)
                {
                    var _parameters = method.Parameters;
                    this.Add("/", method.Name, ":");
                    using (new Block(this))
                    {
                        var _attribute = method.GetCustomAttribute<WebInvokeAttribute>();
                        if (_attribute == null)
                        {
                            _attribute = method.GetCustomAttribute<WebGetAttribute>();
                            if (_attribute == null)
                            {
                                return;
                            }
                            this.Add("get:");
                        }
                        else if (string.IsNullOrEmpty(_attribute.Value<string>("Method")))
                        {
                            return;
                        }
                        else { this.Add(_attribute.Value<string>("Method").ToLower(), ":"); }
                        using (new Block(this))
                        {

                            this.Add("summary: ", method.Name);
                            if (documentation != null && documentation[method].Summary != null) { this.Add("description: ", documentation[method].Summary); }
                            this.Add("consumes:");
                            using (new Block(this))
                            {
                                if (_attribute.Value("RequestFormat") && _attribute.Value<WebMessageFormat>("RequestFormat") == WebMessageFormat.Json) { this.Add("- application/json"); }
                                else { this.Add("- application/xml"); }
                            }
                            this.Add("produces:");
                            using (new Block(this))
                            {
                                if (_attribute.Value("ResponseFormat") && _attribute.Value<WebMessageFormat>("ResponseFormat") == WebMessageFormat.Json) { this.Add("- application/json"); }
                                else { this.Add("- application/xml"); }
                            }
                            if (_parameters.Count > 0)
                            {
                                this.Add("parameters:");
                                using (new Block(this))
                                {
                                    foreach (var _parameter in _parameters)
                                    {
                                        this.Add(method, _parameter, documentation);
                                    }
                                }
                                this.Add("tags:");
                                using (new Block(this))
                                {
                                    this.Add("- ", method.DeclaringType.Name);
                                }
                            }
                            this.Add("responses:");
                            using (new Block(this))
                            {
                                this.Add("200:");
                                using (new Block(this))
                                {
                                    if (documentation != null && documentation[method].Response != null)
                                    {
                                        this.Add("description: ", documentation[method].Response);
                                    }
                                    else
                                    {
                                        this.Add("description: OK");
                                    }
                                    if (method.ReturnType.Resolve() != method.Module.ImportReference(typeof(void)).Resolve())
                                    {
                                        this.Add("schema:");
                                        using (new Block(this))
                                        {
                                            this.Add(method.ReturnType, documentation);
                                        }
                                    }
                                }
                                this.Add("default:");
                                using (new Block(this))
                                {
                                    this.Add("description: failed");
                                    this.Add("schema:");
                                    using (new Block(this)) { this.Add("type: \"string\""); }
                                }
                            }
                        }
                    }
                }

                private void Add(MethodDefinition method, ParameterDefinition parameter, Documentation documentation)
                {
                    var _type = parameter.ParameterType;
                    this.Add("- name: ", parameter.Name);
                    using (new Block(this))
                    {
                        if (_type.Resolve() == _type.Module.ImportReference(typeof(string)).Resolve()
                            || _type.Resolve() == _type.Module.ImportReference(typeof(int)).Resolve()
                            || _type.Resolve() == _type.Module.ImportReference(typeof(long)).Resolve()
                            || _type.Resolve() == _type.Module.ImportReference(typeof(DateTime)).Resolve()
                            || _type.IsArray)
                        {
                            this.Add("in: query");
                            if (documentation != null && documentation[method, parameter] != null) { this.Add("description: ", documentation[method, parameter]); }
                            this.Add("required: ", parameter.ParameterType.IsValueType.ToString().ToLower());
                            this.Add(parameter.ParameterType, documentation);
                        }
                        else
                        {
                            this.Add("in: body");
                            if (documentation != null && documentation[method, parameter] != null)
                            {
                                this.Add("description: ", documentation[method, parameter]);
                            }
                            this.Add("required: ", parameter.ParameterType.IsValueType.ToString().ToLower());
                            this.Add("schema:");
                            using (new Block(this))
                            {
                                this.Add(parameter.ParameterType, documentation);
                            }
                        }
                    }
                }

                private void Add(PropertyDefinition property, Documentation documentation)
                {
                    this.Add(property.Name, ":");
                    using (new Block(this))
                    {
                        this.Add(property.PropertyType, documentation);
                        if (documentation != null && documentation[property] != null) { this.Add("description: ", documentation[property]); }
                    }
                }

                private void Add(TypeReference type, Documentation documentation)
                {
                    if (type.Resolve() == type.Module.ImportReference(typeof(string)).Resolve())
                    {
                        this.Add("type: \"string\"");
                    }
                    else if (type.Resolve() == type.Module.ImportReference(typeof(bool)).Resolve())
                    {
                        this.Add("type: \"boolean\"");
                    }
                    else if (type.Resolve() == type.Module.ImportReference(typeof(int)).Resolve())
                    {
                        this.Add("type: \"number\"");
                        this.Add("format: int32");
                    }
                    else if (type.Resolve() == type.Module.ImportReference(typeof(long)).Resolve())
                    {
                        this.Add("type: \"number\"");
                        this.Add("format: int32");
                    }
                    else if (type.Resolve() == type.Module.ImportReference(typeof(decimal)).Resolve()
                             || type.Resolve() == type.Module.ImportReference(typeof(decimal?)).Resolve())
                    {
                        this.Add("type: \"number\"");
                        this.Add("format: decimal(9,2)");
                    }
                    else if (type.Resolve() == type.Module.ImportReference(typeof(DateTime)).Resolve())
                    {
                        this.Add("type: \"string\"");
                        this.Add("format: date-time");
                    }
                    else if (type.IsArray)
                    {
                        this.Add("type: array");
                        this.Add("items:");
                        using (new Block(this)) { this.Add(type.GetElementType(), documentation); }
                    }
                    else if (type is TypeDefinition typeDef && typeDef.IsEnum)
                    {
                        this.Add("type: \"string\"");
                        this.Add("enum:");

                        foreach (var field in typeDef.Fields)
                        {
                            if (field.Name == "value__")
                                continue;
                            this.Add($"- {field.Name}");
                        }
                    }
                    else
                    {
                        if (type.Resolve()?.GetCustomAttribute<DataContractAttribute>() == null || !definitionList.Contains(type))
                        {
                            definitionList.Add(type);
                            this.Add("$ref: \"#/definitions/", type.Name, "\"");
                        }
                        else
                        {
                            this.Add("$ref: \"#/definitions/", type.Name, "\"");
                        }

                    }
                }

                private void ParseComplexType(TypeReference referenceType, Documentation documentation)
                {
                    if (referenceType.Resolve() == referenceType.Module.ImportReference(typeof(void)).Resolve())
                    {
                        return;
                    }

                    if (referenceType.Resolve() == null)
                    {
                        return;
                    }
                    this.Add(referenceType.Name, ":");
                    using (new Block(this))
                    {
                        this.Add("type: object");
                        if (documentation != null && !String.IsNullOrEmpty(documentation[referenceType.Resolve()]))
                        {
                            this.Add(string.Concat("description: ", documentation[referenceType.Resolve()]));
                        }

                        if (referenceType.Resolve().Properties.Count > 0)
                        {
                            this.Add("properties:");
                            using (new Block(this))
                            {
                                var propertyDefinitions = referenceType.Resolve().Properties;

                                foreach (var propertyDefinition in propertyDefinitions)
                                {
                                    this.Add(propertyDefinition, documentation);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
