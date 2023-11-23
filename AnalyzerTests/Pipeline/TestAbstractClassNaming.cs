﻿using Analyzer.Parsing;
using Analyzer.Pipeline;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnalyzerTests.Pipeline
{
    /// <summary>
    /// Test class for testing the analyzer - PrefixChecker.
    /// </summary>
    [TestClass()]
    public class TestAbstractClassNaming
    {
        /// <summary>
        /// Test method for a case in which all classes follow the rule 
        /// </summary>
        [TestMethod()]
        public void TestGoodExample()
        {
            List<ParsedDLLFile> DllFileObjs = new();

            string path = "..\\..\\..\\TestDLLs\\Abstract1.dll";
            var parsedDllObj = new ParsedDLLFile( path );

            DllFileObjs.Add( parsedDllObj );

            AbstractClassNamingChecker abstractClassNamingChecker = new( DllFileObjs );

            Dictionary<string , Analyzer.AnalyzerResult> resultObj = abstractClassNamingChecker.AnalyzeAllDLLs();

            Analyzer.AnalyzerResult result = resultObj["Abstract1.dll"];
            Assert.AreEqual( 1 , result.Verdict );
        }

        /// <summary>
        /// Test method for a case in which classes don't follow the above mentioned rule.
        /// </summary>
        [TestMethod()]
        public void TestBadExample()
        {
            List<ParsedDLLFile> DllFileObjs = new();

            string path = "..\\..\\..\\TestDLLs\\Abstract.dll";
            var parsedDllObj = new ParsedDLLFile( path );

            DllFileObjs.Add( parsedDllObj );

            AbstractClassNamingChecker abstractClassNamingChecker = new( DllFileObjs );

            Dictionary<string , Analyzer.AnalyzerResult> resultObj = abstractClassNamingChecker.AnalyzeAllDLLs();

            Analyzer.AnalyzerResult result = resultObj["Abstract.dll"];
            Assert.AreEqual( 0 , result.Verdict );
        }

    }
}
