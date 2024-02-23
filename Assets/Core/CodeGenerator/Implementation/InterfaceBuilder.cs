using System.Collections.Generic;
using Core.CodeGenerator.Data;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.CodeGenerator.Implementation
{
    internal class InterfaceBuilder : BuilderBase
    {
        public override BuilderType Type => BuilderType.Interface;

        public override void AddField(IEnumerable<string> modifiers, string type, string name, string initialization) 
        {
            LogError("An interface cannot contain fields.");
        }

        public override void AddMethod(IEnumerable<string> modifiers, string type, string name, IEnumerable<string> body)
        {
            LogError("An interface cannot contain method with body.");
        }
        
        public override void AddProperty(IEnumerable<string> modifiers, string type, string name)
        {
            LogError("An interface cannot contain modifier.");
            AddProperty(type, name);
        }

        public override void AddProperty(IEnumerable<string> modifiers, string type, string name, string setterModifier)
        {
            LogError("An interface cannot contain setter modifier.");
            AddProperty(type, name);
        }

        protected override void SetType(string name)
        {
            TypeDeclaration = SyntaxFactory.InterfaceDeclaration(name);
        }
    }
}