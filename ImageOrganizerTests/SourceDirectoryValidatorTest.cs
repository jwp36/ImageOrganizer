using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Validators;
using System.IO;
using System.Collections.Generic;

namespace ImageOrganizerTests
{
    [TestClass]
    public class SourceDirectoryValidatorTest
    {
        private static string emptyDirectory = typeof(SourceDirectoryValidatorTest).Name + "EmptyDirectory";
        private static string nonEmptyDirectory = typeof(SourceDirectoryValidatorTest).Name + "NonEmptyDirectory";
        private static string fileName = "TestFile";
        private static FileStream file;
        private static SourceDirectoryValidator sourceDirectoryValidator;
        
        [ClassInitialize()]
        public static void setUp(TestContext context)
        {
            Directory.CreateDirectory(emptyDirectory);
            Directory.CreateDirectory(nonEmptyDirectory);

            file = File.Create(Path.Combine(nonEmptyDirectory, fileName));
            sourceDirectoryValidator = new SourceDirectoryValidator();
        }

        [ClassCleanup()]
        public static void tearDown()
        {
            file.Close();
            Directory.Delete(emptyDirectory);
            Directory.Delete(nonEmptyDirectory, true);
        }
        
        [TestMethod]
        public void ValidateShouldFailWhenDirectoryDoesNotExist()
        {
            bool isValid = sourceDirectoryValidator.Validate("BrokenPath", out ICollection<string> errors);

            Assert.IsFalse(isValid);
            Assert.IsTrue(errors.Count == 1);
        }

        [TestMethod]
        public void ValidateShouldFailWhenDirectoryIsEmpty()
        {
            bool isValid = sourceDirectoryValidator.Validate(emptyDirectory, out ICollection<string> errors);

            Assert.IsFalse(isValid);
            Assert.IsTrue(errors.Count == 1);
        }

        [TestMethod]
        public void ValidateShouldSucceedWhenDirectoryIsNotEmpty()
        {
            bool isValid = sourceDirectoryValidator.Validate(nonEmptyDirectory, out ICollection<string> errors);

            Assert.IsTrue(isValid);
            Assert.IsTrue(errors.Count == 0);
        }
    }
}
