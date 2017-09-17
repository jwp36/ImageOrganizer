using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Models;
using ImageOrganizer.Validators;
using System.IO;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Linq;
using System.Collections.Generic;

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

        [TestMethod]
        public void HandleUnsupportedFileFoundEventShouldWhenFileWithThatFileNameAlreadyExists()
        {
            //Set up subdirectories
            IEnumerable<string> sourceSubdirectories = new List<string>
            {
                Path.GetRandomFileName(),
                Path.GetRandomFileName(),
                Path.GetRandomFileName()
            };
            sourceSubdirectories = sourceSubdirectories.Select(subdir => { return Path.Combine(sourceDirectoryPath, subdir); });

            foreach (string sourceSubdirectory in sourceSubdirectories)
            {
                Directory.CreateDirectory(sourceSubdirectory);
            }

            //Set up filenames
            string fileName = Path.GetRandomFileName();
            IEnumerable<string> sourceFilePaths = new List<string>(sourceSubdirectories.Select(subdir => Path.Combine(subdir, fileName)));

            //Set up files and event args
            List<UnsupportedFileFoundEventArgs> args = new List<UnsupportedFileFoundEventArgs>();
            foreach (string sourceFilePath in sourceFilePaths)
            {
                using (StreamWriter streamWriter = new StreamWriter(sourceFilePath))
                {
                    streamWriter.WriteLine(Path.GetRandomFileName());
                }

                args.Add(new UnsupportedFileFoundEventArgs(sourceFilePath, destinationDirectoryPath));
            }

            //Trigger the event handler
            foreach (UnsupportedFileFoundEventArgs arg in args)
            {
                exposedHandler.Invoke("HandleUnsupportedFileFoundEvent", organizer, arg);
            }

            string fileDestinationPath = Path.Combine(destinationDirectoryPath, fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            string renamedFile1DestinationPath = Path.Combine(destinationDirectoryPath, String.Format("{0} 1{1}", fileNameWithoutExtension, extension));
            string renamedFile2DestinationPath = Path.Combine(destinationDirectoryPath, String.Format("{0} 2{1}", fileNameWithoutExtension, extension));
            Assert.IsTrue(File.Exists(fileDestinationPath));
            Assert.IsTrue(File.Exists(renamedFile1DestinationPath));
            Assert.IsTrue(File.Exists(renamedFile2DestinationPath));
        }
    }
}
