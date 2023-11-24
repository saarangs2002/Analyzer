﻿/******************************************************************************
 * Filename     = AnalyzerSerializerUnitTest.cs
 * 
 * Author       = Susan
 *
 * Product      = Analyzer
 * 
 * Project      = ContentUnitTesting
 *
 * Description  = Unit tests for IFileEncoder
*****************************************************************************/
using Analyzer;
using Content.Encoder;

namespace ContentUnitTesting.AnalyzerIntegrationTest
{
    
    /// <summary>
    /// Class to test the IFileEncoder interface
    /// </summary>
    [TestClass]
    public class AnalyzerSerializerUnitTest
    {
        /// <summary>
        /// Generates a dictionary representing file analysis results based on provided file paths and analyzer result details.
        /// </summary>
        /// <param name="filePaths">A list of file paths associated with the analysis results.</param>
        /// <param name="analyzerResultDetails">
        /// A nested list structure containing analyzer result details for each file, where each inner list represents an individual analysis entry
        /// with the elements [Analyser ID, Verdict, ErrorMessage].
        /// </param>
        /// <returns>A dictionary mapping file paths to lists of AnalyzerResult objects.</returns>
        /// <exception cref="Exception">Thrown when the number of file paths and analyzer result details do not match.</exception>

        private Dictionary<string, List<AnalyzerResult>> GenerateFileAnalysisDict(List<string> filePaths, List<List<List<object>>> analyzerResultDetails)
        {
            Dictionary<string , List<AnalyzerResult>> fileAnalysisDict = new();

            if (filePaths.Count != analyzerResultDetails.Count)
            {
                throw new Exception("Invalid");
            }

            for (int i = 0; i < filePaths.Count; i++)
            {
                string filePath = filePaths[i];
                List<List<object>> sampleList = analyzerResultDetails[i];
                List<AnalyzerResult> fileAnalysisList = new();

                foreach (List<object> entry in sampleList)
                {
                    string analyserId = (string)entry[0];
                    int verdict = (int)entry[1];
                    string errorMessage = (string)entry[2];

                    // Create AnalyzerObject
                    AnalyzerResult analyzerObject = new (analyserId, verdict, errorMessage);
                    fileAnalysisList.Add(analyzerObject);
                }
                fileAnalysisDict[filePath] = fileAnalysisList;
            }

            return fileAnalysisDict;
        }

        /// <summary>
        /// Test case to ensure proper serialization and deserialization of analyzer results.
        /// </summary>
        [TestMethod]
        public void TestAnalyzerSerialization()
        {
            List<string> filePaths = new() { "root/folder1/file2.dll", "root/folder2/file4.dll" };

            List<List<object>> sampleList1 = new()  
            {
                new List<object> { "abc123", 1, "No errors" },
                new List<object> { "xyz456", 0, "Invalid input" },
                new List<object> { "123def", 2, "Internal server error" },
                new List<object> { "qwe789", 1, "File not found : File 1" }
                // Add more entries as needed
            };

            List<List<object>> sampleList2 = new()
            {
                new List<object> { "ghi987", 2, "Permission denied" },
                new List<object> { "jkl012", 0, "Success" },
                new List<object> { "mno345", 1, "Configuration error" },
                new List<object> { "pqr678", 2, "Timeout" }
                // Add more entries as needed
            };

            List<List<List<object>>> analyzerResultDetails = new()
            {
                sampleList1 ,
                sampleList2
            };

            Assert.AreEqual(filePaths.Count, analyzerResultDetails.Count);

            Dictionary<string , List<AnalyzerResult>> fileAnalysisDict = GenerateFileAnalysisDict( filePaths , analyzerResultDetails );

            AnalyzerResultSerializer analyserSerializer = new();

            // Act
            string serializedData = analyserSerializer.Serialize(fileAnalysisDict);
                Dictionary<string, List<AnalyzerResult>> deserializedResult = analyserSerializer.Deserialize<Dictionary<string, List<AnalyzerResult>>>(serializedData);
            
            // Assert
            Assert.AreEqual(fileAnalysisDict.Count, deserializedResult.Count);

            foreach (string key in fileAnalysisDict.Keys)
            {
                Assert.IsTrue(deserializedResult.ContainsKey(key), $"Key '{key}' not found in deserialized result");
                
                CollectionAssert.AreEqual(fileAnalysisDict[key], deserializedResult[key], $"Lists for key '{key}' are not equal.");
            }
        }

        /// <summary>
        /// Test case to validate that an exception is thrown when the number of file paths and analyzer result details does not match.
        /// </summary>
        [TestMethod]
        public void TestException()
        {
            // Arrange
            List<string> filePaths = new() { "root/folder1/file2.dll", "root/folder2/file4.dll", "root/folder3/file6.dll" };

            List<List<object>> sampleList1 = new()
            {
                new List<object> { "abc123", 1, "No errors" },
                new List<object> { "xyz456", 0, "Invalid input" },
                new List<object> { "123def", 2, "Internal server error" },
                new List<object> { "qwe789", 1, "File not found" }

            };

            List<List<List<object>>> analyzerResultDetails = new()
            {
                sampleList1
            };

            // Act and Assert
            Assert.ThrowsException<Exception>(() => GenerateFileAnalysisDict(filePaths, analyzerResultDetails));

        }

        /// <summary>
        /// Test case to verify that an ArgumentNullException is thrown when attempting to serialize a null object.
        /// </summary>
        [TestMethod]
        public void Serialize_NullObject_ThrowsArgumentNullException()
        {
            // Arrange
            AnalyzerResultSerializer serializer = new();

            // Act and Assert
            Assert.ThrowsException<ArgumentNullException>(() => serializer.Serialize<AnalyzerResult>(null));
        }

        /// <summary>
        /// Test case to verify that ArgumentException is thrown when attempting to deserialize a null or whitespace serialized string.
        /// </summary>
        [TestMethod]
        public void Serialize_NullOrWhiteSpaceSerializedString_ThrowsArgumentException()
        {
            // Arrange
            AnalyzerResultSerializer serializer = new ();

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() => serializer.Deserialize<Dictionary<string, List<AnalyzerResult>>>(null));
            Assert.ThrowsException<ArgumentException>(() => serializer.Deserialize<Dictionary<string, List<AnalyzerResult>>>(""));
            Assert.ThrowsException<ArgumentException>(() => serializer.Deserialize<Dictionary<string, List<AnalyzerResult>>>("   "));
        }
    }
}
