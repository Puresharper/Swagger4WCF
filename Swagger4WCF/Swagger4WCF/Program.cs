using Mono.Cecil;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Swagger4WCF
{
    static class Program
    {
        static void Main(string[] args)
        {
            var _directory = Path.GetDirectoryName(args[4]);
            var _name = Path.GetFileNameWithoutExtension(args[1]);
            var _resolver = new DefaultAssemblyResolver();
            _resolver.AddSearchDirectory(_directory);
            var _domain = Directory.EnumerateFiles(_directory, "*.dll").Select(_File => { try { return new { Assembly = AssemblyDefinition.ReadAssembly(_File, new ReaderParameters() { AssemblyResolver = _resolver, ReadSymbols = true, ReadingMode = ReadingMode.Immediate }), Location = _File }; } catch { return null; } }).Where(_Entry => _Entry != null).ToArray();
            foreach (var _entry in _domain)
            {
                foreach (var _document in YAML.Generate(_entry.Assembly, Documentation.Load(_entry.Location, _entry.Assembly)))
                {
                    var _location = $@"{ _directory }\{ _entry.Assembly.Name.Name }.{_document.Type.Name }.yaml";
                    using (var _writer = new StreamWriter(_location, false, Encoding.UTF8))
                    {
                        _writer.Write(_document.ToString());
                        Console.WriteLine($"{ _name } -> { _location }");
                    }
                }
            }
        }
    }
}
