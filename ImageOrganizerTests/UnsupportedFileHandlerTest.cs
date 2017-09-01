using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Models;
using ImageOrganizer.Validators;
using System.IO;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Linq;

namespace ImageOrganizerTests
{
    [TestClass]
    public class UnsupportedFileHandlerTest
    {
        static string sourceDirectory = "SourceDirectoryWithUnsupportedFile";
        static string destinationDirectory = "DestinationDirectoryWithUnsupportedFile";
        static string unsupportedFileName = "UnsupportedFile.txt";
        static string unsupportedFileContent = "Words to be written to the test file.";
        
        static string sourceFilePath = Path.Combine(sourceDirectory, unsupportedFileName);
        static string destinationFilePath = Path.Combine(destinationDirectory, unsupportedFileName);

        [TestInitialize]
        public void TestInitialize()
        {
            Directory.CreateDirectory(sourceDirectory);
            Directory.CreateDirectory(destinationDirectory);

            using (StreamWriter streamWriter = File.CreateText(sourceFilePath))
            {
                streamWriter.WriteLine(unsupportedFileContent);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Directory.Delete(sourceDirectory, true);
            Directory.Delete(destinationDirectory, true);
        }
    
        [TestMethod]
        public void HandleUnsupportedFileFoundEventShouldSucceedWhenEventIsFired()
        {
            Organizer organizer = new Organizer(sourceDirectory, destinationDirectory);
            UnsupportedFileHandler unsupportedFileHandler = new UnsupportedFileHandler(organizer);

            PrivateObject exposedOrganizer = new PrivateObject(organizer);
            exposedOrganizer.Invoke("processFile", unsupportedFileName);

            byte[] sourceFileHash;
            byte[] destinationFileHash;

            using (var md5 = MD5.Create())
            using (var sourceFileStream = File.OpenRead(sourceFilePath))
            using (var destinationFileStream = File.OpenRead(destinationFilePath))
            {
                sourceFileHash = md5.ComputeHash(sourceFileStream);
                destinationFileHash = md5.ComputeHash(destinationFileStream);
            }

            Assert.IsTrue(sourceFileHash.SequenceEqual(destinationFileHash));
        }
    }
}
