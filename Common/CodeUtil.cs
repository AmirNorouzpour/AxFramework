using System;
using System.Collections.Generic;
using System.IO;
using DynamicExpresso;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Common
{
    public class CodeUtil
    {
        public static T Execute<T>(string csharpCode)
        {
            var interpreter = new Interpreter();
            var result = interpreter.Eval(csharpCode);
            return (T)result;
        }
    }


    namespace CSCodeExecuter
    {
        public class ScriptManager
        {
            public static object ExecuteScript(string code, List<Skender.Stock.Indicators.Quote> p)
            {
                string inputSript = GetScript(code);
                var result = Execute(inputSript, p);
                return result;
            }

            private static string GetScript(string code)
            {

                string csSript = @"
            using Skender.Stock.Indicators;
            using System.Collections.Generic;

            namespace DynamicCode 
            {
                public class ScriptedClass
                {
                    public object Run(List<Quote> quotes)
                    {
                        " + code + @"
                    }
                }
            }";

                return csSript;
            }




            public static object Execute(string code, List<Skender.Stock.Indicators.Quote> p)
            {
                var value = GenerateCode(code, p);
                return value;
            }

            private static object GenerateCode(string sourceCode, List<Skender.Stock.Indicators.Quote> p)
            {
                var codeString = SourceText.From(sourceCode);
                var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
                var idxs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skender.Stock.Indicators.dll");
                var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);
                var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

                if (assemblyPath != null)
                {
                    var references = new MetadataReference[]
                    {
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                        MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(assemblyPath,"System.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(assemblyPath,"System.Core.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(assemblyPath,"System.Runtime.dll")),
                        MetadataReference.CreateFromFile(idxs)

                    };

                    var dllName = Guid.NewGuid() + ".dll";

                    var dll = CSharpCompilation.Create(dllName,
                        new[] { parsedSyntaxTree },
                        references: references,
                        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                            optimizationLevel: OptimizationLevel.Release,
                            assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));


                    var outputPath = Path.Combine(Path.GetTempPath(), dllName);

                    var result = dll.Emit(outputPath);
                    if (!result.Success)
                        return null;

                    dynamic testType = Activator.CreateInstanceFrom(outputPath, "DynamicCode.ScriptedClass")?.Unwrap();
                    var value = testType?.Run(p);
                    return value;
                }

                return null;
            }
        }
    }
}
