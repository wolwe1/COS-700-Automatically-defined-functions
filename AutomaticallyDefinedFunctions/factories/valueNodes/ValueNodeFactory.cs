﻿using System;
using AutomaticallyDefinedFunctions.factories.functionFactories;
using AutomaticallyDefinedFunctions.generators;
using AutomaticallyDefinedFunctions.parsing;
using AutomaticallyDefinedFunctions.structure.functions;
using AutomaticallyDefinedFunctions.structure.nodes;
using AutomaticallyDefinedFunctions.structure.nodes.valueNodes;

namespace AutomaticallyDefinedFunctions.factories.valueNodes
{
    public class ValueNodeFactory : FunctionFactory, IValueNodeFactory
    {
        public ValueNodeFactory(): base(NodeCategory.ValueNode){}

        private static INode<T> Get<T>(string id) where T : IComparable
        {
            if (id.StartsWith("Null"))
                return new NullNode<T>();
            
            if (typeof(T) == typeof(string))
            {
                return (ValueNode<T>) (object) StringValueNodeFactory.Get(id);
            }

            if (typeof(T) == typeof(double))
            {
                return (ValueNode<T>) (object) DoubleValueNodeFactory.Get(id);
            }
            
            if (typeof(T) == typeof(bool))
            {
                return (ValueNode<T>) (object) BooleanValueNodeFactory.Get(id);
            }
            
            throw new InvalidOperationException($"Unable to generate value node of type {typeof(T)}");
        }

        public static INode<T> GetNull<T>() where T : IComparable
        {
            return new NullNode<T>();
        }

        public override FunctionNode<T> CreateFunction<T, TU>(int maxDepth, FunctionCreator parent)
        {
            throw new NotImplementedException();
        }
        
        protected override INode<T> GenerateFunctionFromId<T, TU>(string id, FunctionCreator functionCreator)
        {
            return Get<T>(AdfParser.GetValueFromQuotes(id));
        }


        public INode<T> Get<T>() where T : IComparable
        {
            if (typeof(T) == typeof(string))
            {
                return (ValueNode<T>) (object) StringValueNodeFactory.Get();
            }

            if (typeof(T) == typeof(double))
            {
                return (ValueNode<T>) (object) DoubleValueNodeFactory.Get();
            }
            
            if (typeof(T) == typeof(bool))
            {
                return (ValueNode<T>) (object) BooleanValueNodeFactory.Get();
            }

            throw new Exception($"Value node factory could not dispatch for type {typeof(T)}");
        }

        public override bool CanDispatch<T>()
        {
            return typeof(T) == typeof(string) || typeof(T) == typeof(bool) || typeof(T) == typeof(double);
        }

        public override bool CanDispatchAux<T>()
        {
            return true;
        }
    }
}