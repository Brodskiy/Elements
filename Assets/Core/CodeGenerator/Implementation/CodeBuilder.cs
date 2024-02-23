using System.Collections.Generic;
using Core.CodeGenerator.Data;
using UnityEngine;

namespace Core.CodeGenerator.Implementation
{
    public class CodeBuilder
    {
        private static readonly Dictionary<BuilderType, BuilderBase> Builders = new Dictionary<BuilderType, BuilderBase>
        {
            {BuilderType.Interface, new InterfaceBuilder()},
            {BuilderType.Class, new ClassBuilder()},
        };
        
        private static BuilderBase _currentBuilder;
        
        private BuilderType _builderType;

        public static CodeBuilder Make(BuilderType builderType, string namespaceValue, string name)
        {
            return new CodeBuilder(builderType, namespaceValue, name);
        }

        public CodeBuilder AddUsing(params string[] allUsing)
        {
            _currentBuilder.AddUsing(allUsing);

            return this;
        }

        public CodeBuilder AddAttribute(string name, params string[] body)
        {
            _currentBuilder.AddAttribute(name, body);
            
            return this;
        }

        public CodeBuilder AddModifiers(params string[] modifiers)
        {
            _currentBuilder.AddModifiers(modifiers);
            
            return this;
        }

        public CodeBuilder AddBaseType(string baseName)
        {
            _currentBuilder.AddBaseType(baseName);

            return this;
        }
        
        public CodeBuilder AddProperty(string type, string name, string setterModifier, params string[] modifiers)
        {
            _currentBuilder.AddProperty(modifiers, type, name, setterModifier);
            
            return this;
        }
        
        public CodeBuilder AddProperty(string type, string name, params string[] modifiers)
        {
            if (_currentBuilder.Type == BuilderType.Interface)
            {
                LogError("An interface cannot contain modifiers.");
                AddProperty(name, type);
                
                return this;
            }
            
            _currentBuilder.AddProperty(modifiers, type, name);
            
            return this;
        }
        
        public CodeBuilder AddAutoGetProperty(string type, string name, string body, params string[] modifiers)
        {
            if (_currentBuilder.Type == BuilderType.Interface)
            {
                LogError("An interface cannot contain auto property.");
                
                return this;
            }
            
            _currentBuilder.AddAutoGetProperty(modifiers, type, name, body);
            
            return this;
        }

        public CodeBuilder AddGetProperty(string type, string name, params string[] modifiers)
        {
            if (_currentBuilder.Type == BuilderType.Interface)
            {
                LogError("An interface cannot contain modifiers.");
                AddGetProperty(type, name);
                
                return this;
            }
            
            _currentBuilder.AddGetProperty(modifiers, type, name);

            return this;
        }

        public CodeBuilder AddField(string type, string name, params string[] modifiers)
        {
            return AddField(type, name, string.Empty, modifiers);
        }
        
        public CodeBuilder AddMethod(string type, string name, string bodyLine, params string[] modifiers)
        {
            return AddMethod(type,  name, new[] {bodyLine}, modifiers);
        }
        
        public override string ToString()
        {
            return _currentBuilder.ToString();
        }
        
        private CodeBuilder AddField(string type, string name, string initialization, params string[] modifiers)
        {
            if (_currentBuilder.Type == BuilderType.Interface)
            {
                LogError("An interface cannot contain fields.");
                
                return this;
            }
            
            _currentBuilder.AddField(modifiers, type, name, initialization);
            
            return this;
        }

        private CodeBuilder AddMethod(string type, string name, string[] body, params string[] modifiers)
        {
            if (_currentBuilder.Type == BuilderType.Interface)
            {
                LogError("An interface cannot contain method with body.");
                
                return this;
            }
            
            _currentBuilder.AddMethod(modifiers, type, name, body);

            return this;
        }

        private CodeBuilder(BuilderType builderType, string @namespace, string name)
        {
            _currentBuilder = GetBuilderByType(builderType);
            _currentBuilder.Initialize(@namespace, name);
        }
        
        private BuilderBase GetBuilderByType(BuilderType builderType)
        {
            if (Builders.TryGetValue(builderType, out var builder))
            {
                return builder;
            }
            
            LogError($"Builder by type - {builderType} not found");
            
            return Builders[0];
        }

        private void LogError(string message)
        {
            Debug.LogError($"[CodeBuilder] {message}");
        }
    }
}