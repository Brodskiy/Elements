using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using UnityEngine;
using System.Linq;
using Core.CodeGenerator.Data;

namespace Core.CodeGenerator.Implementation
{
    internal abstract class BuilderBase
    {
        public abstract BuilderType Type { get; }
        
        protected TypeDeclarationSyntax TypeDeclaration { get; set; } 
        
        private NamespaceDeclarationSyntax _namespace;
        private CompilationUnitSyntax _root;
        
        public abstract void AddField(IEnumerable<string> modifiers, string type, string name, string initialization);
        public abstract void AddMethod(IEnumerable<string> modifiers, string type, string name, IEnumerable<string> body);
        public abstract void AddProperty(IEnumerable<string> modifiers, string type, string name, string setterModifier);
        public abstract void AddProperty(IEnumerable<string> modifiers, string type, string name);

        public void Initialize(string @namespace, string name)
        {
            _root = SyntaxFactory.CompilationUnit();
            var nameSyntax = SyntaxFactory.ParseName(@namespace);
            _namespace = SyntaxFactory.NamespaceDeclaration(nameSyntax);

            SetType(name);
        }

        public void AddUsing(IEnumerable<string> @using)
        {
            var usingDirectives = @using.Select(GetUsingDirective).ToArray();
            _root = _root.AddUsings(usingDirectives);
        }

        public void AddAttribute(string name, IEnumerable<string> body)
        {
            var attributeArgumentList = SyntaxFactory.AttributeArgumentList();

            foreach (var item in body)
            {
                var attributeArgument = SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression(item));
                attributeArgumentList = attributeArgumentList.AddArguments(attributeArgument);
            }

            var attributeList = new SeparatedSyntaxList<AttributeSyntax>();
            var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(name), attributeArgumentList);
            attributeList = attributeList.Add(attribute);
            TypeDeclaration = TypeDeclaration.AddAttributeLists(SyntaxFactory.AttributeList(attributeList));
        }

        public void AddModifiers(IEnumerable<string> modifiers)
        {
            var allModifiers = modifiers.Select(modifier => SyntaxFactory.ParseToken(modifier)).ToArray();
            TypeDeclaration = TypeDeclaration.AddModifiers(allModifiers);
        }

        public void AddBaseType(string baseType)
        {
            var simpleBaseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseType));
            TypeDeclaration = TypeDeclaration.AddBaseListTypes(simpleBaseType) as TypeDeclarationSyntax;
        }
        
        public void AddGetProperty(IEnumerable<string> modifiers, string type, string name)
        {
            var nodes = new[]
            {
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            };
            var accessorList = SyntaxFactory.AccessorList(SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                SyntaxFactory.List(nodes),
                SyntaxFactory.Token(SyntaxKind.CloseBraceToken)).NormalizeWhitespace(indentation: "", eol: " ");
            
            var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(type), name)
                .WithAccessorList(accessorList);

            AddPropertyInDeclaration(modifiers, property);
        }
        
        public void AddAutoGetProperty(IEnumerable<string> modifiers, string type, string name, string body)
        {
            var arrowExpressionClauseSyntax = SyntaxFactory.ArrowExpressionClause(type.Equals("string") 
                    ? SyntaxFactory.ParseExpression($"\"{body}\"" ) 
                    : SyntaxFactory.ParseExpression(body));

            var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(type), name)
                .WithAccessorList(null)
                .WithExpressionBody(arrowExpressionClauseSyntax)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithAdditionalAnnotations(Formatter.Annotation);

            AddPropertyInDeclaration(modifiers, property);
        }

        public override string ToString()
        {
            return _root.AddMembers(_namespace.AddMembers(TypeDeclaration)).NormalizeWhitespace().ToFullString();
        }

        protected abstract void SetType(string name);
        
        protected static AccessorDeclarationSyntax GetAccessorDeclaration(SyntaxKind accessorType)
        {
            return SyntaxFactory.AccessorDeclaration(accessorType)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
        
        protected static PropertyDeclarationSyntax GetPropertyDeclaration(string name, string type,
            AccessorDeclarationSyntax getter, AccessorDeclarationSyntax setter)
        {
            var accessorList = SyntaxFactory.AccessorList(SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                SyntaxFactory.List(new[] {getter, setter}),
                SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

            var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(type), name)
                .WithAccessorList(accessorList);
            
            return property;
        }
        
        protected void AddProperty(string type, string name)
        {
            var getter = GetAccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            var setter = GetAccessorDeclaration(SyntaxKind.SetAccessorDeclaration);

            var property = GetPropertyDeclaration(name, type, getter, setter);

            AddPropertyInDeclaration(null, property);
        }
        
        protected void AddPropertyInDeclaration(IEnumerable<string> modifiers, PropertyDeclarationSyntax property)
        {
            if (modifiers != null)
            {
                property = property.AddModifiers(modifiers.Select(item => SyntaxFactory.ParseToken(item)).ToArray());
            }
            
            TypeDeclaration = TypeDeclaration.AddMembers(property);
        }
        
        protected void LogError(string message)
        {
            Debug.LogError($"[CodeBuilderBase] {message}");
        }
        
        private static UsingDirectiveSyntax GetUsingDirective(string @using)
        {
            var identifierName = SyntaxFactory.IdentifierName(@using);
            var usingDirective = SyntaxFactory.UsingDirective(identifierName);
            
            return usingDirective;
        }
    }
}