using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Models;
using ImageOrganizer.Validators;
using System.IO;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ImageOrganizerTests
{
    [TestClass]
    public class UnsupportedFileHandlerTest
    {
        [TestMethod]
        public void HandleUnsupportedFileFoundEventShouldSucceedWhenEventIsFired()
        {
            string sourceDirectory = "SourceDirectoryWithUnsupportedFile";
            string destinationDirectory = "DestinationDirectoryWithUnsupportedFile";
            string unsupportedFileName = "UnsupportedFile.txt";
            string unsupportedFileContent = "Words to be written to the test file.";

            string sourceFilePath = Path.Combine(sourceDirectory, unsupportedFileName);
            string destinationFilePath = Path.Combine(destinationDirectory, unsupportedFileName);



            //DirectorySecurity directorySecurity = new DirectorySecurity();
            //directorySecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            //directorySecurity.AddAccessRule(new )
            
            Directory.CreateDirectory(sourceDirectory);
            Directory.CreateDirectory(destinationDirectory);

            StreamWriter streamWriter = File.CreateText(sourceDirectory);
            streamWriter.WriteLine(unsupportedFileContent);
            streamWriter.Close();



            Organizer organizer = new Organizer(sourceDirectory, destinationDirectory, new SourceDirectoryValidator(), new DestinationDirectoryValidator());
            UnsupportedFileHandler unsupportedFileHandler = new UnsupportedFileHandler(organizer);

            PrivateObject exposedOrganizer = new PrivateObject(organizer);
            exposedOrganizer.Invoke("processFile", sourceFilePath);

            byte[] sourceFileHash;
            byte[] destinationFileHash;

            using (var md5 = MD5.Create())
            using (var sourceFileStream = File.OpenRead(sourceFilePath))
            using (var destinationFileStream = File.OpenRead(destinationFilePath))
            {
                sourceFileHash = md5.ComputeHash(sourceFileStream);
                destinationFileHash = md5.ComputeHash(destinationFileStream);
            }

            Assert.AreEqual(sourceFileHash, destinationFileHash);
        }
    }
}
