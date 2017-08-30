using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Models;
using ImageOrganizer.Validators;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Text;

namespace ImageOrganizerTests
{
    [TestClass]
    public class JPGFileHandlerTest
    {
        private static string sourceDirectoryPath = "JPGFileHandlerTest_SourceDirectory";
        private static string destinationDirectoryPath = "JPGFileHandlerTest_DestinationDirectory";

        static private Organizer organizer;
        static private PrivateObject exposedOrganizer;
        static private JPGFileHandler handler;
        static private PrivateObject exposedHandler;
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Directory.CreateDirectory(sourceDirectoryPath);
            Directory.CreateDirectory(destinationDirectoryPath);

            organizer = new Organizer(sourceDirectoryPath, destinationDirectoryPath, new SourceDirectoryValidator(), new DestinationDirectoryValidator());
            exposedOrganizer = new PrivateObject(organizer);

            handler = new JPGFileHandler(organizer);
            exposedHandler = new PrivateObject(handler);
        }

        [ClassCleanup]
        public static void ClassCleaup()
        {
            Directory.Delete(sourceDirectoryPath, true);
            Directory.Delete(destinationDirectoryPath, true);
        }

        [TestMethod]
        public void HandleJPGFileFoundEventShouldSucceedWhenImageHasEXIFDateTimeOriginalData()
        {
            string fileName = "TestFile.jpg";
            string date = "2015-10-17";
            string dateTimeOriginal = "2015:10:17 18:18:11";

            string fullSourcePath = Path.Combine(sourceDirectoryPath, fileName);
            string fullDestinationPath = Path.Combine(destinationDirectoryPath, date, fileName);

            Image image = createImage(dateTimeOriginal);
            image.Save(fullSourcePath, ImageFormat.Jpeg);

            JPGFileFoundEventArgs args = new JPGFileFoundEventArgs(fileName, sourceDirectoryPath, destinationDirectoryPath);
            exposedHandler.Invoke("handleJPGFileFoundEvent", organizer, args);

            Assert.IsTrue(File.Exists(fullDestinationPath));
        }

        [TestMethod]
        public void ParseDateTimeOriginalShouldSucceedWhenImageHasEXIFDateTimeOriginalData()
        {
            string expectedDateTime = "2015-01-06 10.27.56";
            Image image = createImage(expectedDateTime);

            string actualDateTime = (string)exposedHandler.Invoke("parseDateTimeOriginal", image);

            Assert.AreEqual(expectedDateTime, actualDateTime);
        }

        [TestMethod]
        public void ParseDateTimeOriginalShouldFailWhenImageHasNoEXIFDateTimeOriginalData()
        {
            Image image = new Bitmap(1, 1);

            try
            {
                string actualDateTime = (string)exposedHandler.Invoke("parseDateTimeOriginal", image);
            }
            catch (Exception e)
            {
                //MSTest doesn't have a built-in attribute for testing InnerExceptions...
                Assert.IsInstanceOfType(e.InnerException, typeof(UnsupportedJPGFileException));
            }
        }



        private Image createImage(string EXIFDateTime)
        {
            PropertyItem propertyItem = (PropertyItem)FormatterServices.GetSafeUninitializedObject(typeof(PropertyItem));
            propertyItem.Id = JPGFileHandler.EXIFDateTimeOriginalID;
            propertyItem.Type = 2;
            propertyItem.Value = Encoding.UTF8.GetBytes(EXIFDateTime);
            propertyItem.Len = propertyItem.Value.Length;

            Image image = new Bitmap(1, 1);
            image.SetPropertyItem(propertyItem);

            return image;
        }
    }
}
