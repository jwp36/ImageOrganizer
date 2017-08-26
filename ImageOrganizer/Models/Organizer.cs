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



        public Organizer(string sourceDirectoryPath, string destinationDirectoryPath)
        {
            this.sourceDirectoryPath = sourceDirectoryPath;
            this.destinationDirectoryPath = destinationDirectoryPath;
        }



        public void Organize()//ISourceDirectoryValidator x, IDestinationDirectoryValidator y)
        {
            //x.Validate(sourceDirectory);
            //y.Validate(destinationDirectory);
            //Validate sourceDir is a Dir
            //sourceDir must contain contain elements. 

            //If dstDir is not a dir, just make it.
            //If dstDir is a dir, it must not contain elements.

            //Directory.Exists(sourceDirectoryPath);

            return;
        }
    }
}
