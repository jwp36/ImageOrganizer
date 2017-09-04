using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Models;
using ImageOrganizer.Validators;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace ImageOrganizerTests
{
    [TestClass]
    public class OrganizerTest
    {
        private static string sourceDirectoryPath = Path.GetFullPath(typeof(OrganizerTest).Name + "SourceDirectory");
        private static string destinationDirectoryPath = Path.GetFullPath(typeof(OrganizerTest).Name + "DestinationDirectory");

        private Organizer organizer;
        private PrivateObject exposedOrganizer;
                
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Directory.CreateDirectory(sourceDirectoryPath);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Directory.Delete(sourceDirectoryPath, true);
        }

        [TestInitialize]
        public void TestInitialize() 
        {
            organizer = new Organizer(sourceDirectoryPath, destinationDirectoryPath);
            exposedOrganizer = new PrivateObject(organizer);
        }

        [TestMethod]
        public void OrganizeShouldFireEventsWhenSearchingForFilesRecursively()
        {
            string subdirectoryName = Path.GetRandomFileName();
            string subdirectoryPath = Path.Combine(sourceDirectoryPath, subdirectoryName);
            Directory.CreateDirectory(subdirectoryPath);

            string jpgFilePath = Path.Combine(subdirectoryPath, Path.GetRandomFileName() + ".jpg");
            string txtFilePath = Path.Combine(subdirectoryPath, Path.GetRandomFileName() + ".txt");
            using (Image image = new Bitmap(1, 1))
            using (var streamWriter = new StreamWriter(txtFilePath))
            {
                image.Save(jpgFilePath);
                streamWriter.WriteLine(Path.GetRandomFileName());
            }
            
            JPGFileFoundEventArgs receivedJPGFileFoundEventArgs = null;
            UnsupportedFileFoundEventArgs receivedUnsupportedFileFoundEventArgs = null;

            organizer.JPGFileFoundEvent += (object sender, JPGFileFoundEventArgs e) => receivedJPGFileFoundEventArgs = e;
            organizer.UnsupportedFileFoundEvent += (object sender, UnsupportedFileFoundEventArgs e) => receivedUnsupportedFileFoundEventArgs = e;
           
            organizer.Organize();

            Assert.IsTrue(receivedJPGFileFoundEventArgs.FilePath == jpgFilePath);
            Assert.IsTrue(receivedJPGFileFoundEventArgs.DestinationDirectoryPath == destinationDirectoryPath);

            Assert.IsTrue(receivedUnsupportedFileFoundEventArgs.FilePath == txtFilePath);
            Assert.IsTrue(receivedUnsupportedFileFoundEventArgs.DestinationDirectoryPath == destinationDirectoryPath);
        }

        [TestMethod]
        public void ProcessFileShouldFireJPGFileFoundEventWhenFileIsJPG()
        {
            string fileNameThatEndsWithLowerCaseJPG = Path.GetRandomFileName() + ".jpg";
            string fileNameThatEndsWithLowerCaseJPEG = Path.GetRandomFileName() + ".jpeg";
            string fileNameThatEndsWithCapitalJPG = Path.GetRandomFileName() + ".JPG";
            string fileNameThatEndsWithCapitalJPEG = Path.GetRandomFileName() + ".JPEG";

            List<string> filePaths = new List<string>();
            filePaths.Add(fileNameThatEndsWithCapitalJPEG);
            filePaths.Add(fileNameThatEndsWithLowerCaseJPEG);
            filePaths.Add(fileNameThatEndsWithCapitalJPG);
            filePaths.Add(fileNameThatEndsWithLowerCaseJPG);
            for (int i = 0; i < filePaths.Count; i++)
            {
                filePaths[0] = Path.GetFullPath(filePaths[0]);
            }

            JPGFileFoundEventArgs receivedEventArgs;
            organizer.JPGFileFoundEvent += (object sender, JPGFileFoundEventArgs e) => { receivedEventArgs = e; };
            
            foreach (string filePath in filePaths)
            {
                receivedEventArgs = null;
                exposedOrganizer.Invoke("processFile", filePath);

                Assert.AreEqual(filePath, receivedEventArgs.FilePath);
                Assert.AreEqual(destinationDirectoryPath, receivedEventArgs.DestinationDirectoryPath);
            }
        }

        [TestMethod]
        public void ProcessFileShouldFireUnsupportedFileFoundEventWhenFileIsNotJPG()
        {
            string filePath = Path.Combine(sourceDirectoryPath, Path.GetRandomFileName());

            UnsupportedFileFoundEventArgs receivedEventArgs = null;
            organizer.UnsupportedFileFoundEvent += (object sender, UnsupportedFileFoundEventArgs e) => { receivedEventArgs = e; };

            exposedOrganizer.Invoke("processFile", filePath);

            Assert.AreEqual(filePath, receivedEventArgs.FilePath);
            Assert.AreEqual(destinationDirectoryPath, receivedEventArgs.DestinationDirectoryPath);
        }

        [TestMethod]
        public void ProcessFileShouldFireUnsupportedFileFoundEventWhenFileCausesUnsupportedJPGFileException()
        {
            string filePath = Path.Combine(sourceDirectoryPath, Path.GetRandomFileName() + ".jpg");

            UnsupportedFileFoundEventArgs receivedEventArgs = null;

            organizer.JPGFileFoundEvent += (object organizer, JPGFileFoundEventArgs e) => { throw new UnsupportedJPGFileException(); };
            organizer.UnsupportedFileFoundEvent += (object organizer, UnsupportedFileFoundEventArgs e) => { receivedEventArgs = e; };
            
            exposedOrganizer.Invoke("processFile", filePath);

            Assert.AreEqual(filePath, receivedEventArgs.FilePath);
            Assert.AreEqual(destinationDirectoryPath, receivedEventArgs.DestinationDirectoryPath);
        }
    }
}
