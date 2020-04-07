using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EFWService.Core.OpenAPI.DynamicController
{
    internal class CompileHelper
    {
        private static List<string> referenceAssList = new List<string>();

        public static void Init()
        {
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                referenceAssList.Add(item.Location);
            }
        }

        public static Assembly CompileAssembly(string[] code)
        {
            MetadataReference[] references = referenceAssList.Select(x =>
            {
                return MetadataReference.CreateFromFile(x);
            }).ToArray();
            var syntaxTrees = code.Select(x =>
            {
                return CSharpSyntaxTree.ParseText(x,
                    CSharpParseOptions.Default.WithPreprocessorSymbols("NETCORE"));
            }).ToArray();
            var option = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, allowUnsafe: true);
            CSharpCompilation compilation = CSharpCompilation.Create("OpenAPI.DynamicController.dll")
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTrees)
                .WithOptions(option);

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());
                    return assembly;
                }
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);
                foreach (Diagnostic diagnostic in failures)
                {
                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                }
                return null;
            }
        }
    }
}
