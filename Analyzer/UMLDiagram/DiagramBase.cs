﻿using Analyzer.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer.UMLDiagram
{
    abstract public class DiagramBase
    {
        /// <summary>
        /// The parsed DLL files to be used for analysis.
        /// </summary>
        public List<ParsedDLLFile> parsedDLLFiles;

        /// <summary>
        /// Initializes a new instance of the BaseAnalyzer with parsed DLL files.
        /// </summary>
        /// <param name="dllFiles">The parsed DLL files for analysis.</param>
        public DiagramBase(List<ParsedDLLFile> dllFiles)
        {
            // Set the parsedDLLFiles field with the provided DLL files
            parsedDLLFiles = dllFiles;
        }
    }
}
