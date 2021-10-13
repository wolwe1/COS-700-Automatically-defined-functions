﻿using System;
using System.Linq;
using AutomaticallyDefinedFunctions.factories;
using AutomaticallyDefinedFunctions.factories.comparators;
using AutomaticallyDefinedFunctions.factories.functionFactories;
using AutomaticallyDefinedFunctions.factories.valueNodes;
using AutomaticallyDefinedFunctions.generators.adf;
using AutomaticallyDefinedFunctions.parsing;
using AutomaticallyDefinedFunctions.structure.functions;
using AutomaticallyDefinedFunctions.structure.functions.comparators;
using AutomaticallyDefinedFunctions.structure.nodes;

namespace AutomaticallyDefinedFunctions.generators
{
    public class FunctionCreator : FactoryManager
    {
        private readonly bool _useNullTerminals;
        private readonly AdfSettings _settings;

        public FunctionCreator(AdfSettings settings,bool useNullTerminals) : base(settings)
        {
            _useNullTerminals = useNullTerminals;
            _settings = settings;
        }
        
        public FunctionNode<T> CreateFunction<T>(int maxDepth) where T : IComparable
        {
            var choice = RandomNumberFactory.Next(3);

            return choice switch
            {
                0 => CreateFunction<T, string>(maxDepth),
                1 => CreateFunction<T, bool>(maxDepth),
                2 => CreateFunction<T, double>(maxDepth),
                _ => throw new InvalidOperationException("Cannot dispatch function factory on type " + typeof(T))
            };
        }
        
        private FunctionNode<T> CreateFunction<T, TU>(int maxDepth) where T : IComparable where TU : IComparable
        {
            var chosenFactory = FunctionPicker.PickFactoryWithAuxAs<T, TU,IFunctionFactory>(Factories);

            return chosenFactory.CreateFunction<T, TU>(maxDepth, this);
        }
        
        public NodeComparator<T> ChooseComparator<T>(int maxDepth) where T : IComparable
        {
            var chosenComparatorFactory = FunctionPicker.PickFactoryAs<T,ComparatorFactory>(Comparators);
            
            return (NodeComparator<T>)chosenComparatorFactory.CreateFunction<T,T>(maxDepth, this);
        }

        public INode<T> Choose<T>(int maxDepth) where T : IComparable
        {
            if (maxDepth <= 0) return GetTerminal<T>();
            
            var terminalOrFunction = RandomNumberFactory.AboveThreshold(_settings.TerminalChance);

            return terminalOrFunction switch
            {
                false => GetTerminal<T>(),
                true => CreateFunction<T>(maxDepth)
            };
        }

        public INode<T> GetTerminal<T>() where T : IComparable
        {
            if (_useNullTerminals) return ValueNodeFactory.GetNull<T>();

            var chosenFactory = FunctionPicker.PickFactoryAs<T,IValueNodeFactory>(ValueNodeFactories);
            
            return chosenFactory.Get<T>();
        }
        
        public INode<T> GenerateFunctionFromId<T>(string id) where T : IComparable
        {
            id = AdfParser.GetIdWithoutDelimiters(id);

            if (id.StartsWith(NodeCategory.FunctionDefinition))
                return FunctionFactory.GenerateDefinitionFromId<T>(id,this);

            var generator = Factories.FirstOrDefault(f => f.CanMap(id));
            
            if (generator != null) return generator.GenerateFunctionFromId<T>(id, this);
            
            
            //Use comparator
            generator = Comparators.FirstOrDefault(f => f.CanMap(id));
            
            if(generator == null)
                throw new Exception($"Unable to find generator for ID '{id}'");
            
            return generator.GenerateFunctionFromId<T>(id,this);
        }
        
        public INode<T> GenerateChildFromId<T>(ref string id) where T : IComparable
        {
            var child = GenerateFunctionFromId<T>(id);
            var childId = child.GetId();
            
            var remainingId = GetIdAfter(id,childId);
            
            id = remainingId;
            
            return child;
        }

        private string GetIdAfter(string id, string afterSubstring)
        {
            var afterSubstringWithoutDelimiters = AdfParser.GetIdWithoutDelimiters(afterSubstring);
            return id[(id.IndexOf(afterSubstringWithoutDelimiters, StringComparison.Ordinal) + afterSubstringWithoutDelimiters.Length)..];
        }
        
    }
}