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
        private static string sourceDirectoryPath = Path.GetFullPath(typeof(UnsupportedFileHandlerTest).Name + "SourceDirectory");
        private static string destinationDirectoryPath = Path.GetFullPath(typeof(UnsupportedFileHandlerTest).Name + "DestinationDirectory");

        private Organizer organizer;
        private UnsupportedFileHandler handler;
        private PrivateObject exposedHandler;
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Directory.CreateDirectory(sourceDirectoryPath);
            Directory.CreateDirectory(destinationDirectoryPath);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Directory.Delete(sourceDirectoryPath, true);
            Directory.Delete(destinationDirectoryPath, true);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            organizer = new Organizer(sourceDirectoryPath, destinationDirectoryPath);
            handler = new UnsupportedFileHandler(organizer);
            exposedHandler = new PrivateObject(handler);
        }

        [TestMethod]
        public void HandleUnsupportedFileFoundEventShouldSucceedWhenEventIsFired()
        {
            string fileName = Path.GetRandomFileName();
            string sourceFilePath = Path.Combine(sourceDirectoryPath, fileName);

            using (StreamWriter streamWriter = File.CreateText(sourceFilePath))
            {
                streamWriter.WriteLine(Path.GetRandomFileName());
            }

            UnsupportedFileFoundEventArgs args = new UnsupportedFileFoundEventArgs(sourceFilePath, destinationDirectoryPath);
            exposedHandler.Invoke("HandleUnsupportedFileFoundEvent", organizer, args);

            string fileDestinationPath = Path.Combine(destinationDirectoryPath, fileName);
            Assert.IsTrue(File.Exists(fileDestinationPath));
        }
    }
}
