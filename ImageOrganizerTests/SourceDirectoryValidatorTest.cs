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
        private static string emptyDirectory = Path.GetFullPath(typeof(SourceDirectoryValidatorTest).Name + Path.GetRandomFileName());
        private static string nonEmptyDirectory = Path.GetFullPath(typeof(SourceDirectoryValidatorTest).Name + Path.GetRandomFileName());
        
        private static SourceDirectoryValidator sourceDirectoryValidator;
        
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            Directory.CreateDirectory(emptyDirectory);
            Directory.CreateDirectory(nonEmptyDirectory);

            FileStream file;
            using (file = File.Create(Path.Combine(nonEmptyDirectory, Path.GetRandomFileName())))
            {
            }

            sourceDirectoryValidator = new SourceDirectoryValidator();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Directory.Delete(emptyDirectory, true);
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
