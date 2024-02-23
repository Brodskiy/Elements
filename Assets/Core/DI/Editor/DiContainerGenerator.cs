using System.IO;
using Core.CodeGenerator.Data;
using Core.CodeGenerator.Implementation;
using UnityEngine;

namespace Core.DI.Editor
{
    internal static class DiContainerGenerator
    {
        public static void Generation(string @namespace, string @class)
        {
            var resultDirectoryPath = GetResultDirectoryPath(@class);
            var sourceCode = CreateSourceCode(@namespace, @class);
            
            File.WriteAllText(Path.Combine(resultDirectoryPath, $"{@class}.cs"), sourceCode);
        }

        private static string GetResultDirectoryPath(string @class)
        {
            var directoryInfo = new DirectoryInfo(Application.dataPath);
            var resultDirectoryPath = Path.Combine(directoryInfo.FullName, @class);

            if (!Directory.Exists(resultDirectoryPath))
            {
                Directory.CreateDirectory(resultDirectoryPath);
            }

            return resultDirectoryPath;
        }

        private static string CreateSourceCode(string @namespace, string @class)
        {
            return CodeBuilder.Make(BuilderType.Class, @namespace, @class)
                .AddUsing("Core.DI.Implementation", "UnityEngine")
                .AddAttribute("CreateAssetMenu", $"fileName = \"{@class}\"", $"menuName = \"DIContainer/{@class}\"")
                .AddModifiers("internal")
                .AddBaseType("ContainerBase")
                .AddMethod("void", "Registration", "", new[] {"protected", "override"})
                .ToString();
        }
    }
}