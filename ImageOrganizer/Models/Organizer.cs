using ImageOrganizer.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Models
{
    public class Organizer
    {
        private string sourceDirectoryPath;
        private string destinationDirectoryPath;
        private IDirectoryValidator sourceDirectoryValidator;
        private IDirectoryValidator destinationDirectoryValidator;



        public Organizer(string sourceDirectoryPath, string destinationDirectoryPath, IDirectoryValidator sourceDirectoryValidator, IDirectoryValidator destinationDirectoryValidator)
        {
            this.sourceDirectoryPath = sourceDirectoryPath;
            this.destinationDirectoryPath = destinationDirectoryPath;
            this.sourceDirectoryValidator = sourceDirectoryValidator;
            this.destinationDirectoryValidator = destinationDirectoryValidator;
        }



        public void Organize()
        {
            sourceDirectoryValidator.Validate(sourceDirectoryPath);
            destinationDirectoryValidator.Validate(destinationDirectoryPath);

            //Validate sourceDir is a Dir
            //sourceDir must contain contain elements. 

            //If dstDir is not a dir, just make it.
            //If dstDir is a dir, it must not contain elements.

            //Directory.Exists(sourceDirectoryPath);

            /*
            string[] fileEntries = Directory.GetFiles(sourceDirectoryPath);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);
                */

            //foreach item in source directory
            //check if supported. 
            //If supported, raise an event saying a supported image was found.

            //if not supported, raise an event saying an unsupported image was found.

            return;
        }
    }
}
