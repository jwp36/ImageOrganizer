﻿using ImageOrganizer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ImageOrganizerTests
{
    [TestClass]
    public class JPGFileHandlerTest
    {
        private static string sourceDirectoryPath = Path.GetFullPath(typeof(JPGFileHandlerTest).Name + "SourceDirectory");
        private static string destinationDirectoryPath = Path.GetFullPath(typeof(JPGFileHandlerTest).Name + "DestinationDirectory");

        private Organizer organizer;
        private JPGFileHandler handler;
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
            handler = new JPGFileHandler(organizer);
            exposedHandler = new PrivateObject(handler);
        }

        [TestMethod]
        public void HandleJPGFileFoundEventShouldSucceedWhenImageHasEXIFDateTimeOriginalData()
        {
            string fileName = Path.GetRandomFileName();
            string date = "2015-10-17";
            string dateTimeOriginal = "2015:10:17 18:18:11";

            string sourceFilePath = Path.Combine(sourceDirectoryPath, fileName);
            string destinationFilePath = Path.Combine(destinationDirectoryPath, date, fileName);

            Image image = CreateImage(dateTimeOriginal);
            image.Save(sourceFilePath, ImageFormat.Jpeg);

            JPGFileFoundEventArgs args = new JPGFileFoundEventArgs(sourceFilePath, destinationDirectoryPath);
            exposedHandler.Invoke("HandleJPGFileFoundEvent", organizer, args);

            Assert.IsTrue(File.Exists(destinationFilePath));
        }

        [TestMethod]
        public void HandleJPGFileFoundEventShouldSucceedWhenRenamingImageWithEXIFDataTimeOriginalData()
        {
            handler = new JPGFileHandler(organizer, JPGFileHandler.Naming.EXIFDateTime);
            exposedHandler = new PrivateObject(handler);

            string fileName = Path.GetRandomFileName() + ".jpg";
            string dateTimeOriginal = "2015:10:18 18:18:11";

            string date = "2015-10-18";
            string newFileName = "2015-10-18 18.18.11.jpg";

            string sourceFilePath = Path.Combine(sourceDirectoryPath, fileName);
            string destinationFilePath = Path.Combine(destinationDirectoryPath, date, newFileName);

            Image image = CreateImage(dateTimeOriginal);
            image.Save(sourceFilePath, ImageFormat.Jpeg);

            JPGFileFoundEventArgs args = new JPGFileFoundEventArgs(sourceFilePath, destinationDirectoryPath);
            exposedHandler.Invoke("HandleJPGFileFoundEvent", organizer, args);

            Assert.IsTrue(File.Exists(destinationFilePath));
        }

        [TestMethod]
        public void HandleJPGFileFoundEventShouldSucceedWhenRenamingImageAndAFileWithThatFileNameAlreadyExists()
        {
            handler = new JPGFileHandler(organizer, JPGFileHandler.Naming.EXIFDateTime);
            exposedHandler = new PrivateObject(handler);

            List<string> subdirectories = new List<string>
            {
                Path.GetRandomFileName(),
                Path.GetRandomFileName(),
                Path.GetRandomFileName()
            };

            string fileName = Path.GetRandomFileName() + ".jpg";
            string dateTimeOriginal = "2015:10:17 18:18:11";
            foreach (string subdirectory in subdirectories)
            {
                Directory.CreateDirectory(Path.Combine(sourceDirectoryPath, subdirectory));

                string sourceFilePath = Path.Combine(sourceDirectoryPath, subdirectory, fileName);

                Image image = CreateImage(dateTimeOriginal);
                image.Save(sourceFilePath, ImageFormat.Jpeg);
                image.Dispose();

                JPGFileFoundEventArgs args = new JPGFileFoundEventArgs(sourceFilePath, destinationDirectoryPath);
                exposedHandler.Invoke("HandleJPGFileFoundEvent", organizer, args);
            }

            List<string> newFileNames = new List<string>
            {
                "2015-10-17 18.18.11.jpg",
                "2015-10-17 18.18.11 1.jpg",
                "2015-10-17 18.18.11 2.jpg"
            };

            string date = "2015-10-17";
            foreach (string newFileName in newFileNames)
            {
                string destinationFilePath = Path.Combine(destinationDirectoryPath, date, newFileName);

                Assert.IsTrue(File.Exists(destinationFilePath));

            }
        }

        [TestMethod]
        public void ParseDateTimeOriginalShouldSucceedWhenImageHasEXIFDateTimeOriginalData()
        {
            string expectedDateTime = "2015-01-06 10.27.56";
            Image image = CreateImage(expectedDateTime);

            string actualDateTime = (string)exposedHandler.Invoke("ParseDateTimeOriginal", image);

            Assert.AreEqual(expectedDateTime + '\0', actualDateTime);
        }

        [TestMethod]
        public void ParseDateTimeOriginalShouldFailWhenImageHasNoEXIFDateTimeOriginalData()
        {
            Image image = new Bitmap(1, 1);

            try
            {
                string actualDateTime = (string)exposedHandler.Invoke("ParseDateTimeOriginal", image);
            }
            catch (Exception e)
            {
                //MSTest doesn't have a built-in attribute for testing InnerExceptions...
                Assert.IsInstanceOfType(e.InnerException, typeof(UnsupportedJPGFileException));
            }
        }

        /// <summary>
        /// Create an Image with the specified EXIFDateTimeOriginal data.
        /// </summary>
        /// <param name="EXIFDateTime">EXIFDateTimeOrignial data to use when creating the Image.</param>
        /// <returns></returns>
        private Image CreateImage(string EXIFDateTime)
        {
            PropertyItem propertyItem = (PropertyItem)FormatterServices.GetSafeUninitializedObject(typeof(PropertyItem));
            propertyItem.Id = JPGFileHandler.EXIFDateTimeOriginalID;
            propertyItem.Type = 2;
            propertyItem.Value = Encoding.UTF8.GetBytes(EXIFDateTime + '\0');
            propertyItem.Len = propertyItem.Value.Length;

            Image image = new Bitmap(1, 1);
            image.SetPropertyItem(propertyItem);

            return image;
        }
    }
}
