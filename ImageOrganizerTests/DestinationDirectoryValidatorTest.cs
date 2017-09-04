using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageOrganizer.Validators;
using System.IO;
using System.Collections.Generic;

namespace ImageOrganizerTests
{
    [TestClass]
    public class DestinationDirectoryValidatorTest
    {
        private static string emptyDirectory = Path.GetFullPath(typeof(DestinationDirectoryValidatorTest).Name + Path.GetRandomFileName());
        private static string nonEmptyDirectory = Path.GetFullPath(typeof(DestinationDirectoryValidatorTest).Name + Path.GetRandomFileName());

        private static DestinationDirectoryValidator destinationDirectoryValidator;

        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            Directory.CreateDirectory(emptyDirectory);
            Directory.CreateDirectory(nonEmptyDirectory);

            FileStream file;
            using (file = File.Create(Path.Combine(nonEmptyDirectory, Path.GetRandomFileName())))
            {

            }

            destinationDirectoryValidator = new DestinationDirectoryValidator();
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
            bool isValid = destinationDirectoryValidator.Validate("BrokenPath", out ICollection<string> errors);

            Assert.IsFalse(isValid);
            Assert.IsTrue(errors.Count == 1);
        }

        [TestMethod]
        public void ValidateShouldFailWhenDirectoryIsNotEmpty()
        {
            bool isValid = destinationDirectoryValidator.Validate(nonEmptyDirectory, out ICollection<string> errors);

            Assert.IsFalse(isValid);
            Assert.IsTrue(errors.Count == 1);
        }

        [TestMethod]
        public void ValidateShouldSucceedWhenDirectoryExistsAndIsEmpty()
        {
            bool isValid = destinationDirectoryValidator.Validate(emptyDirectory, out ICollection<string> errors);

            Assert.IsTrue(isValid);
            Assert.IsTrue(errors.Count == 0);
        }
    }
}