﻿using Analyzer.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer.Pipeline
{
    /// <summary>
    /// A base class providing a common structure for various analyzers.
    /// </summary>
    abstract public class AnalyzerBase
    {
        /// <summary>
        /// The parsed DLL files to be used for analysis.
        /// </summary>
        public List<ParsedDLLFile> parsedDLLFilesList;
        private Dictionary<string, AnalyzerResult> result;

        /// <summary>
        /// Initializes a new instance of the BaseAnalyzer with parsed DLL files.
        /// </summary>
        /// <param name="dllFiles">The parsed DLL files for analysis.</param>
        public AnalyzerBase(List<ParsedDLLFile> dllFiles)
        {
            // Set the parsedDLLFiles field with the provided DLL files
            parsedDLLFilesList = dllFiles;
            result = new Dictionary<string, AnalyzerResult>();
        }

        public Dictionary<string, AnalyzerResult> AnalyzeAllDLLs()
        {
            foreach(ParsedDLLFile parsedDLL in  parsedDLLFilesList)
            {
                result[parsedDLL.DLLFileName] = AnalyzeSingleDLL(parsedDLL);
            }

            return result;
        }

        protected abstract AnalyzerResult AnalyzeSingleDLL(ParsedDLLFile _parsedDLLFile);
    }
}