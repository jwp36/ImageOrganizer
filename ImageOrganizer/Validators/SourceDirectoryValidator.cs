using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Validators
{
    public class SourceDirectoryValidator : IDirectoryValidator
    {
        public bool Validate(string directoryPath, out ICollection<string> validationErrors)
        {
            validationErrors = new List<string>();

            if (!Directory.Exists(directoryPath))
            {
                validationErrors.Add("The supplied directory does not exist. Please specify a directory that exists.");
            }
            else if (!Directory.EnumerateFileSystemEntries(directoryPath).Any())
            {
               validationErrors.Add("The supplied directory is empty. Please specify a non-empty directory.");

            }
                
            return validationErrors.Count == 0;
        }
    }
}
