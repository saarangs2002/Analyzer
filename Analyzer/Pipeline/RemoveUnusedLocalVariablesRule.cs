﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analyzer.Parsing;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Analyzer.Pipeline
{
    /// <summary>
    /// This class represents an analyzer for removing unused local variables in methods using Mono.Cecil.
    /// </summary>
    public class RemoveUnusedLocalVariablesRule : AnalyzerBase
    {
        /// <summary>
        /// Initializes a new instance of the RemoveUnusedLocalVariablesRule with parsed DLL files.
        /// </summary>
        /// <param name="dllFiles">The parsed DLL files to analyze.</param>
        public RemoveUnusedLocalVariablesRule(List<ParsedDLLFile> dllFiles) : base(dllFiles)
        {
            // The constructor sets the parsedDLLFiles field with the provided DLL files.
        }

        /// <summary>
        /// Gets the result of the analysis, which includes the number of unused local variables removed.
        /// </summary>
        /// <returns>An AnalyzerResult containing the analysis results.</returns>
        protected override AnalyzerResult AnalyzeSingleDLL(ParsedDLLFile parsedDLLFile)
        {
            int totalUnusedLocals = 0;
            List<string> unusedVariableNames = new List<string>();

            foreach (ParsedClassMonoCecil classObj in parsedDLLFile.classObjListMC)
            {
                foreach (MethodDefinition method in classObj.TypeObj.Methods)
                {
                    int unusedLocalsCount = RemoveUnusedLocalVariables(method, unusedVariableNames);
                    totalUnusedLocals += unusedLocalsCount;
                }
            }

            string errorString = totalUnusedLocals > 0
                ? $"Removed {totalUnusedLocals} unused local variables: {string.Join(", ", unusedVariableNames)}"
                : "No unused local variables found.";

            return new AnalyzerResult("109", totalUnusedLocals, errorString);
        }

        /// <summary>
        /// Removes unused local variables from a method and returns the count of removed local variables.
        /// </summary>
        /// <param name="method">The method to analyze and remove unused local variables from.</param>
        /// <param name="unusedVariableNames">A list to store the names of unused variables.</param>
        /// <returns>The count of unused local variables removed from the method.</returns>
        private static int RemoveUnusedLocalVariables(MethodDefinition method, List<string> unusedVariableNames)
        {
            int unusedLocalsCount = 0;
            _ = method.Body.GetILProcessor();
            List<VariableDefinition> unusedLocals = new();

            foreach (VariableDefinition localVar in method.Body.Variables)
            {
                // Check if the local variable is used within the method
                if (!IsLocalVariableUsed(localVar, method.Body.Instructions))
                {
                    unusedLocals.Add(localVar);
                    unusedVariableNames.Add(localVar.ToString()); 
                }
            }

            foreach (VariableDefinition localVar in unusedLocals)
            {
                // Remove the instructions that load or store the unused local variable
                RemoveUnusedLocalVariableInstructions(localVar, method.Body.Instructions);
                // Remove the local variable definition from the method
                method.Body.Variables.Remove(localVar);
                unusedLocalsCount++;
            }

            return unusedLocalsCount;
        }

        /// <summary>
        /// Checks if a local variable is used within a collection of instructions.
        /// </summary>
        /// <param name="localVar">The local variable to check for usage.</param>
        /// <param name="instructions">The collection of instructions to analyze.</param>
        /// <returns>True if the local variable is used; otherwise, false.</returns>
        private static bool IsLocalVariableUsed(VariableDefinition localVar, Collection<Instruction> instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                // Check if the current instruction is a load (Ldloc) or store (Stloc) operation for a local variable
                if (instruction.OpCode == OpCodes.Ldloc || instruction.OpCode == OpCodes.Stloc)
                {
                    VariableDefinition localVariableReference = (VariableDefinition)instruction.Operand;
                    if (localVariableReference == localVar)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes instructions that reference a specific unused local variable from a collection of instructions.
        /// </summary>
        /// <param name="localVar">The unused local variable to remove instructions for.</param>
        /// <param name="instructions">The collection of instructions to modify.</param>
        private static void RemoveUnusedLocalVariableInstructions(VariableDefinition localVar, Collection<Instruction> instructions)
        {
            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                Instruction instruction = instructions[i];
                if (instruction.OpCode == OpCodes.Ldloc || instruction.OpCode == OpCodes.Stloc)
                {
                    VariableReference localVariableReference = (VariableReference)instruction.Operand;
                    if (localVariableReference == localVar)
                    {
                        instructions.RemoveAt(i);
                    }
                }
            }
        }
    }
}