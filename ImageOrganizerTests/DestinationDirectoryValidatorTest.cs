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
        private static string emptyDirectory = typeof(DestinationDirectoryValidatorTest).Name + "EmptyDirectory";
        private static string nonEmptyDirectory = typeof(DestinationDirectoryValidatorTest).Name + "NonEmptyDirectory";
        private static string fileName = "TestFile";
        private static FileStream file;
        private static DestinationDirectoryValidator destinationDirectoryValidator;

        [ClassInitialize()]
        public static void setUp(TestContext context)
        {
            Directory.CreateDirectory(emptyDirectory);
            Directory.CreateDirectory(nonEmptyDirectory);

            file = File.Create(Path.Combine(nonEmptyDirectory, fileName));
            destinationDirectoryValidator = new DestinationDirectoryValidator();
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
