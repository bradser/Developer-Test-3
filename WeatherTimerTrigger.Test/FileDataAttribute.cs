using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace WeatherTimerTrigger.Test
{
    public class FileDataAttribute : DataAttribute
    {
        private readonly string _filePathIn;
        private readonly string _filePathOut;

        public FileDataAttribute(string filePathIn, string filePathOut)
        {
            this._filePathIn = filePathIn;

            this._filePathOut = filePathOut;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) { throw new ArgumentNullException(nameof(testMethod)); }

            string fileDataIn = this.getFileData(_filePathIn);
            string fileDataOut = this.getFileData(_filePathOut);

            string[] fileData = new string[] { fileDataIn, fileDataOut };

            return new List<string[]>() { fileData };
        }

        private string getFileData(string filePath)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "TestData", filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            return File.ReadAllText(path);
        }
    }
}