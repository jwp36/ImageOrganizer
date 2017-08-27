using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Validators;
using System.IO;

namespace ImageOrganizerTests
{
    [TestClass]
    public class SourceDirectoryValidatorTest
    {
        private static string emptyTestDirectory = "EmptyTestSourceDirectory";
        private static string nonEmptyTestDirectory = "NonEmptyTestSourceDirectory";
        private static string testFilename = "TestFile";
        private static FileStream testFile;
        private static SourceDirectoryValidator sourceDirectoryValidator;



        [ClassInitialize()]
        public static void setUp(TestContext context)
        {
            Directory.CreateDirectory(emptyTestDirectory);
            Directory.CreateDirectory(nonEmptyTestDirectory);

            testFile = File.Create(Path.Combine(nonEmptyTestDirectory, testFilename));
            sourceDirectoryValidator = new SourceDirectoryValidator();
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
            sourceDirectoryValidator.Validate("BrokenPath");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateShouldFailWhenDirectoryIsEmpty()
        {
            sourceDirectoryValidator.Validate(emptyTestDirectory);
        }

        [TestMethod]
        public void ValidateShouldSucceedWhenDirectoryIsNotEmpty()
        {
            sourceDirectoryValidator.Validate(nonEmptyTestDirectory);
        }
    }
}
