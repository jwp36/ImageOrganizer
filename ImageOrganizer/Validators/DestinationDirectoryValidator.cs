using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Validators
{
    public class DestinationDirectoryValidator : IDirectoryValidator
    {
        //TODO: Implement handling for exceptions
        public void Validate(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new ArgumentException();
            
            if (Directory.EnumerateFileSystemEntries(directoryPath).Any())
                throw new ArgumentException();
        }
    }
}
