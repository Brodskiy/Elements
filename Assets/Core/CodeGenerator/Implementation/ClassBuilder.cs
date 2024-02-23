using System.Collections.Generic;
using System.Linq;
using Core.CodeGenerator.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace Core.CodeGenerator.Implementation
{
    internal class ClassBuilder : BuilderBase
    {
        public override BuilderType Type => BuilderType.Class;

        public override void AddField(IEnumerable<string> modifiers, string type, string name, string initialization)
        {
            if (string.IsNullOrEmpty(initialization))
            {
                AddNewField(modifiers, type, initialization, name);
            }
            else
            {
                AddNewField(modifiers, type, name);
            }
        }

        public override void AddMethod(IEnumerable<string> modifiers, string type, string name, IEnumerable<string> body)
        {
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(type), name).
                NormalizeWhitespace(indentation: "", eol: " ").
                WithBody(SyntaxFactory.Block(body.Select(bodyLine => SyntaxFactory.ParseStatement(bodyLine)))).
                WithAdditionalAnnotations(Formatter.Annotation).
                NormalizeWhitespace();

            method = method.AddModifiers(modifiers.Select(item => SyntaxFactory.ParseToken(item)).ToArray());
            TypeDeclaration = TypeDeclaration.AddMembers(method);
        }

        public override void AddProperty(IEnumerable<string> modifiers, string type, string name, string setterModifier)
        {
            var getter = GetAccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            var setter = GetAccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .AddModifiers(SyntaxFactory.ParseToken(setterModifier));
            var property = GetPropertyDeclaration(name, type, getter, setter);
            
            AddPropertyInDeclaration(modifiers, property);
        }

        public override void AddProperty(IEnumerable<string> modifiers, string type, string name) 
        {
            var getter = GetAccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            var setter = GetAccessorDeclaration(SyntaxKind.SetAccessorDeclaration);
            var property = GetPropertyDeclaration(name, type, getter, setter);

            AddPropertyInDeclaration(modifiers, property);
        }

        protected override void SetType(string name)
        {
            TypeDeclaration = SyntaxFactory.ClassDeclaration(name);
        }

        private void AddNewField(IEnumerable<string> modifiers, string type, string name, string initialization)
        {
            var declarator = SyntaxFactory.VariableDeclarator(SyntaxFactory.ParseToken(name), 
                null,
                SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(initialization)));

            var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(type), 
                    SyntaxFactory.SeparatedList(new[] {declarator})));
            
            var modifiersList = modifiers.Select(item => SyntaxFactory.ParseToken(item)).ToArray();
            field = field.AddModifiers(modifiersList);
            TypeDeclaration = TypeDeclaration.AddMembers(field);
        }

        private void AddNewField(IEnumerable<string> modifiers, string type, string name)
        {
            var fieldDeclaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(type))
                .AddVariables(SyntaxFactory.VariableDeclarator(name));

            var modifiersList = modifiers.Select(item => SyntaxFactory.ParseToken(item)).ToArray();
            var field = SyntaxFactory.FieldDeclaration(fieldDeclaration).AddModifiers(modifiersList);
            TypeDeclaration = TypeDeclaration.AddMembers(field);
        }
    }
}