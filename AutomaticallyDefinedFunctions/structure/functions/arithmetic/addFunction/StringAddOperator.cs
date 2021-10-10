﻿using System;
using System.Collections.Generic;
using System.Text;
using AutomaticallyDefinedFunctions.structure.nodes;

namespace AutomaticallyDefinedFunctions.structure.functions.arithmetic.addFunction
{
    public class StringAddOperator : IArithmeticOperator<string>
    {
        public string GetResult(List<INode> children)
        {
            var builder = new StringBuilder();

            foreach (var child in children)
            {
                builder.Append(((INode<string>)child).GetValue());
            }

            return builder.ToString();
        }
    }
}