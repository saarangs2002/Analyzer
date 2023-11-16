﻿using Analyzer.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer.Pipeline
{

    /// <summary>
    /// The main pipeline for running analyzers
    /// </summary>
    public class MainPipeline
    {

        private IDictionary<int, bool> _teacherOptions;
        private List<string> _studentDLLFiles;
        private readonly Dictionary<int, AnalyzerBase> _allAnalyzers;
        private List<ParsedDLLFile> _parsedDLLFiles;

        public MainPipeline()
        {
            _allAnalyzers = new();
            _teacherOptions = new Dictionary<int, bool> ();
            _studentDLLFiles = new List<string>();
            _parsedDLLFiles = new List<ParsedDLLFile> ();
        }

        /// <summary>
        /// Adds the given teacher options to the pipeline.
        /// </summary>
        /// <param name="TeacherOptions">The teacher options to add</param>
        public void AddTeacherOptions(IDictionary<int, bool> TeacherOptions)
        {
            _teacherOptions = TeacherOptions;
        }

        /// <summary>
        /// Adds the given student DLL files to the pipeline.
        /// </summary>
        /// <param name="PathOfDLLFilesOfStudent">The paths to the student DLL files to add</param>
        public void AddDLLFiles(List<string> PathOfDLLFilesOfStudent)
        {
            _studentDLLFiles = PathOfDLLFilesOfStudent;
            GenerateAnalysers();
        }

        /// <summary>
        /// Generates the analyzers that will be run by the pipeline.
        /// </summary>
        private void GenerateAnalysers()
        {
            foreach (string file in _studentDLLFiles)
            {
                _parsedDLLFiles.Add(new ParsedDLLFile(file));
            }


            _allAnalyzers[101] = new AbstractTypeNoPublicConstructor(_parsedDLLFiles);
            //_allAnalyzers[102] = new AvoidConstructorsInStaticTypes(parsedDLLFiles);
            _allAnalyzers[103] = new AvoidUnusedPrivateFieldsRule(_parsedDLLFiles);
            _allAnalyzers[104] = new NoEmptyInterface(_parsedDLLFiles);
            _allAnalyzers[105] = new DepthOfInheritance(_parsedDLLFiles);
            _allAnalyzers[106] = new ArrayFieldsShouldNotBeReadOnlyRule(_parsedDLLFiles);
            _allAnalyzers[107] = new AvoidSwitchStatementsAnalyzer(_parsedDLLFiles);
            _allAnalyzers[108] = new DisposableFieldsShouldBeDisposedRule(_parsedDLLFiles);
            _allAnalyzers[109] = new RemoveUnusedLocalVariablesRule(_parsedDLLFiles);
            _allAnalyzers[110] = new ReviewUselessControlFlowRule(_parsedDLLFiles);
        }


        /// <summary>
        /// Starts the pipeline and runs all of the analyzers that have been selected by teacher
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<AnalyzerResult>> Start()
        {
            Dictionary<string, List<AnalyzerResult>> results = new();

            foreach (ParsedDLLFile file in _parsedDLLFiles)
            {
                results[file.DLLFileName] = new List<AnalyzerResult>();
            }

            foreach(KeyValuePair<int,bool> option in _teacherOptions)
            {
                if(option.Value == true)
                {
                    Dictionary<string, AnalyzerResult> currentAnalyzerResult;

                    try
                    {
                        currentAnalyzerResult = _allAnalyzers[option.Key].AnalyzeAllDLLs();
                    }
                    catch (Exception _)
                    {
                        currentAnalyzerResult = new Dictionary<string, AnalyzerResult>();

                        foreach(ParsedDLLFile dllFile in _parsedDLLFiles)
                        {
                            currentAnalyzerResult[dllFile.DLLFileName] = new AnalyzerResult(option.Key.ToString(), 1, "Internal error, analyzer failed to execute");
                        }
                    }

                    foreach(KeyValuePair<string , AnalyzerResult> dllResult in currentAnalyzerResult)
                    {
                        results[dllResult.Key].Add(dllResult.Value);
                    }
                }
            }    

            return results;
        }

        public Byte[] GenerateClassDiagram(List<string> removableNamespaces)
        {
            // TODO: Call ClassDiagram.Run() after modifications
            Byte[] bytes = null;
            return bytes;
        }
    }
}