using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Validators;
using System.IO;

namespace ImageOrganizerTests
{
    [TestClass]
    public class DestinationDirectoryValidatorTest
    {
        private static string emptyTestDirectory = "EmptyTestDestinationDirectory";
        private static string nonEmptyTestDirectory = "NonEmptyTestDestinationDirectory";
        private static string testFileName = "TestFile";
        private static FileStream testFile;
        private static DestinationDirectoryValidator destinationDirectoryValidator;
        
        

        [ClassInitialize()]
        public static void setUp(TestContext context)
        {
            Directory.CreateDirectory(emptyTestDirectory);
            Directory.CreateDirectory(nonEmptyTestDirectory);

            testFile = File.Create(Path.Combine(nonEmptyTestDirectory, testFileName));
            destinationDirectoryValidator = new DestinationDirectoryValidator();
        }

        [ClassCleanup()]
        public static void tearDown()
        {
            testFile.Close();
            Directory.Delete(emptyTestDirectory);
            Directory.Delete(nonEmptyTestDirectory, true);
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateShouldFailWhenDirectoryDoesNotExist()
        {
            destinationDirectoryValidator.Validate("BrokenPath");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateShouldFailWhenDirectoryIsNotEmpty()
        {
            destinationDirectoryValidator.Validate(nonEmptyTestDirectory);
        }

        [TestMethod]
        public void ValidateShouldSucceedWhenDirectoryExistsAndIsEmpty()
        {
            destinationDirectoryValidator.Validate(emptyTestDirectory);
        }
    }
}
