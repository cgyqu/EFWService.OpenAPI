using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EFWService.Core.OpenAPI.DynamicController
{
    internal static class AssemblyLocator
    {
        private static readonly ReadOnlyCollection<Assembly> AllAssemblies;
        private static readonly ReadOnlyCollection<Assembly> BinAssemblies;

        static AssemblyLocator()
        {
            AllAssemblies = new ReadOnlyCollection<Assembly>(
                AppDomain.CurrentDomain.GetAssemblies().Cast<Assembly>().ToList());

            IList<Assembly> binAssemblies = new List<Assembly>();

            string binFolder = AppDomain.CurrentDomain.BaseDirectory;
            IList<string> dllFiles = Directory.GetFiles(binFolder, "*.dll",
                SearchOption.TopDirectoryOnly).ToList();

            foreach (string dllFile in dllFiles)
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(dllFile);
                //判断两个程序集是否在引用中，防止有些废弃的程序集仍然加载
                Assembly locatedAssembly = AllAssemblies.FirstOrDefault(a =>
                    AssemblyName.ReferenceMatchesDefinition(
                        a.GetName(), assemblyName));

                if (locatedAssembly != null)
                {
                    binAssemblies.Add(locatedAssembly);
                }
            }

            BinAssemblies = new ReadOnlyCollection<Assembly>(binAssemblies);
        }

        public static ReadOnlyCollection<Assembly> GetAssemblies()
        {
            return AllAssemblies;
        }

        public static ReadOnlyCollection<Assembly> GetBinFolderAssemblies()
        {
            return BinAssemblies;
        }
    }
}
