using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace AISSystem
{
    public class CodeDomHelper
    {
        public static CompilerResults Compile(string compilerVersion, string[] compilyFrom, string[] importNamespaces)
        {
            CompilerResults result = null;
            CSharpCodeProvider csp = GetCsp(compilerVersion);
            CompilerParameters opt = GetOpt(importNamespaces);
            result = csp.CompileAssemblyFromFile(opt, compilyFrom);
            return result;
        }

        public static CompilerResults Compile(string compilerVersion, string code, string[] importNamespaces)
        {
            CompilerResults result = null;
            CSharpCodeProvider csp = GetCsp(compilerVersion);
            CompilerParameters opt = GetOpt(importNamespaces);
            result = csp.CompileAssemblyFromSource(opt, code);
            return result;

        }

        static CSharpCodeProvider GetCsp(string compilerVersion)
        {
            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            string version = string.IsNullOrEmpty(compilerVersion ) ? "v3.5" : compilerVersion;
            providerOptions.Add("CompilerVersion", version);
            return new CSharpCodeProvider(providerOptions);
        }

        static CompilerParameters GetOpt(string[] importNamespaces)
        {
            CompilerParameters opt = new CompilerParameters();
            opt.GenerateExecutable = false;
            opt.TreatWarningsAsErrors = false;
            opt.IncludeDebugInformation = true;
            opt.GenerateInMemory = true;

            if (importNamespaces != null && importNamespaces.Length > 0)
                foreach (string ns in importNamespaces)
                    opt.ReferencedAssemblies.Add(ns);
            return opt;
        }

        

    }
}
