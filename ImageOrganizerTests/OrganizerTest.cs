using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Models;
using ImageOrganizer.Validators;
using System.Collections.Generic;

namespace ImageOrganizerTests
{
    [TestClass]
    public class OrganizerTest
    {
        private Organizer organizer;
        private PrivateObject exposedOrganizer;

        private string sourceDirectoryPath = "SourceDirectory";
        private string destinationDirectoryPath = "DestinationDirectory";

        private delegate void EventListener(Object sender, EventArgs e);

        [TestInitialize]
        public void TestInitialize()
        {
            organizer = new Organizer(sourceDirectoryPath, destinationDirectoryPath, new SourceDirectoryValidator(), new DestinationDirectoryValidator());
            exposedOrganizer = new PrivateObject(organizer);
        }

        [TestMethod]
        public void ProcessFileShouldFireJPGFileFoundEventWhenFileIsJPG()
        {
            string fileNameThatEndsWithLowerCaseJPG = "Test.jpg";
            string fileNameThatEndsWithCapitalJPG = "Test.JPG";
            string fileNameThatEndsWithLowerCaseJPEG = "Test.jpeg";
            string fileNameThatEndsWithCapitalJPEG = "Test.JPEG";

            List<string> testFileNames = new List<string>();
            testFileNames.Add(fileNameThatEndsWithCapitalJPEG);
            testFileNames.Add(fileNameThatEndsWithLowerCaseJPEG);
            testFileNames.Add(fileNameThatEndsWithCapitalJPG);
            testFileNames.Add(fileNameThatEndsWithLowerCaseJPG);

            JPGFileFoundEventArgs receivedEventArgs;
            organizer.JPGFileFoundEvent += (Organizer sender, JPGFileFoundEventArgs e) => { receivedEventArgs = e; };
            
            foreach (string testFileName in testFileNames)
            {
                receivedEventArgs = null;
                exposedOrganizer.Invoke("processFile", testFileName);

                Assert.AreEqual(sourceDirectoryPath, receivedEventArgs.SourceDirectoryPath);
                Assert.AreEqual(destinationDirectoryPath, receivedEventArgs.DestinationDirectoryPath);
                Assert.AreEqual(testFileName, receivedEventArgs.FileName);
            }
        }

        [TestMethod]
        public void ProcessFileShouldFireUnsupportedFileFoundEventWhenFileIsNotJPG()
        {
            string testFileName = "Test.txt";

            UnsupportedFileFoundEventArgs receivedEventArgs = null;
            organizer.UnsupportedFileFoundEvent += (Organizer sender, UnsupportedFileFoundEventArgs e) => { receivedEventArgs = e; };

            exposedOrganizer.Invoke("processFile", testFileName);

            Assert.AreEqual(sourceDirectoryPath, receivedEventArgs.SourceDirectoryPath);
            Assert.AreEqual(destinationDirectoryPath, receivedEventArgs.DestinationDirectoryPath);
            Assert.AreEqual(testFileName, receivedEventArgs.FileName);
        }
    }
}
