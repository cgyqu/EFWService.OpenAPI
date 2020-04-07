using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;


namespace EFWService.OpenAPI.DynamicController
{
    internal class CompileHelper
    {
        private static List<string> referenceAssList = new List<string>();

        public static void Init()
        {
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                //如果是动态程序集，则跳过，说明是在程序初始化之前编译好的程序集
                if (item.IsDynamic)
                {
                    continue;
                }
                referenceAssList.Add(item.Location);
            }

        }

        public static Assembly CompileAssembly(string[] codes)
        {
            #region 编译参数
            CompilerParameters objCompilerParams = new CompilerParameters();
            //编译器选项：编译成（存储在内存中）的DLL
            objCompilerParams.CompilerOptions = "/target:library /optimize";
            //编译时在内存输出 
            objCompilerParams.GenerateInMemory = true;
            //不生成调试信息 
            objCompilerParams.IncludeDebugInformation = false;
            //添加引用
            objCompilerParams.ReferencedAssemblies.AddRange(referenceAssList.Distinct().ToArray());
            #endregion

            #region 编译
            //创建编译类
            CSharpCodeProvider objCompiler = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });

            //进行编译
            CompilerResults objCompileResults = objCompiler.CompileAssemblyFromSource(objCompilerParams, codes);
            #endregion

            if (codes.Length == 0)
            {
                throw new Exception("当前程序域中没有API方法");
            }
            Assembly objAssembly = objCompileResults.CompiledAssembly;

            return objAssembly;
        }
    }
}
