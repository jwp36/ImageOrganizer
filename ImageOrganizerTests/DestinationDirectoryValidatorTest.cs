using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Validators;
using System.IO;

namespace ImageOrganizerTests
{
    [TestClass]
    public class DestinationDirectoryValidatorTest
    {
        private static string emptyTestDirectory = "EmptyTestDirectory";
        private static string nonEmptyTestDirectory = "NonEmptyTestDirectory";
        private static string testFile = "TestFile";
        private static DestinationDirectoryValidator destinationDirectoryValidator;
        

        [ClassInitialize()]
        public static void setUp(TestContext context)
        {
            Directory.CreateDirectory(emptyTestDirectory);
            Directory.CreateDirectory(nonEmptyTestDirectory);
            File.Create(Path.Combine(nonEmptyTestDirectory, testFile));

            destinationDirectoryValidator = new DestinationDirectoryValidator();
        }

        [ClassCleanup()]
        public static void tearDown()
        {
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
