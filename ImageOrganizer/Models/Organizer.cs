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


        /// <summary>
        /// Return an Organizer.
        /// </summary>
        /// <param name="sourceDirectoryPath"> Absolute path to source directory.</param>
        /// <param name="destinationDirectoryPath"> Absolute path to destination directory.</param>
        /// <param name="sourceDirectoryValidator"></param>
        /// <param name="destinationDirectoryValidator"></param>
        public Organizer(string sourceDirectoryPath, string destinationDirectoryPath, IDirectoryValidator sourceDirectoryValidator, IDirectoryValidator destinationDirectoryValidator)
        {
            this.sourceDirectoryPath = sourceDirectoryPath;
            this.destinationDirectoryPath = destinationDirectoryPath;
            this.sourceDirectoryValidator = sourceDirectoryValidator;
            this.destinationDirectoryValidator = destinationDirectoryValidator;
        }



        public event JPGFileFoundEventHandler JPGFileFoundEvent;
        public event UnsupportedFileFoundEventHandler UnsupportedFileFoundEvent;

        public delegate void UnsupportedFileFoundEventHandler(object sender, UnsupportedFileFoundEventArgs e);
        public delegate void JPGFileFoundEventHandler(object sender, JPGFileFoundEventArgs e);

        //TODO: Allow recursive processing of source directory
        public void Organize()
        {
            sourceDirectoryValidator.Validate(sourceDirectoryPath);
            destinationDirectoryValidator.Validate(destinationDirectoryPath);

            string[] fileEntries = Directory.GetFiles(sourceDirectoryPath);
            foreach (string filePath in fileEntries)
            {
                string fileName = Path.GetFileName(filePath);
                processFile(fileName);
            }
                
            return;
        }

        //TODO: Implement true JPG file detection through file signature analysis
        private void processFile(string fileName)
        {
            if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    onJPGFileFound(this, new JPGFileFoundEventArgs(fileName, sourceDirectoryPath, destinationDirectoryPath));
                }
                catch (UnsupportedJPGFileException)
                {
                    onUnsupportedFileFound(this, new UnsupportedFileFoundEventArgs(fileName, sourceDirectoryPath, destinationDirectoryPath));
                }
            }
            else
            {
                onUnsupportedFileFound(this, new UnsupportedFileFoundEventArgs(fileName, sourceDirectoryPath, destinationDirectoryPath));
            }
        }

        private void onJPGFileFound(Organizer sender, JPGFileFoundEventArgs e)
        {
            JPGFileFoundEvent?.Invoke(sender, e);
        }

        private void onUnsupportedFileFound(Organizer sender, UnsupportedFileFoundEventArgs e)
        {
            UnsupportedFileFoundEvent?.Invoke(sender, e);
        }
    }



    public class JPGFileFoundEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public string SourceDirectoryPath { get; private set; }
        public string DestinationDirectoryPath { get; private set; }

        public JPGFileFoundEventArgs(string fileName, string sourceDirectoryPath, string destinationDirectoryPath)
        {
            FileName = fileName;
            SourceDirectoryPath = sourceDirectoryPath;
            DestinationDirectoryPath = destinationDirectoryPath;
        }
    }

    public class UnsupportedFileFoundEventArgs : EventArgs
    {      
        public string FileName { get; private set; }
        public string SourceDirectoryPath { get; private set; }
        public string DestinationDirectoryPath { get; private set; }

        public UnsupportedFileFoundEventArgs(string fileName, string sourceDirectoryPath, string destinationDirectoryPath)
        {
            FileName = fileName;
            SourceDirectoryPath = sourceDirectoryPath;
            DestinationDirectoryPath = destinationDirectoryPath;
        }
    }

    [Serializable]
    public class UnsupportedJPGFileException : Exception
    {
        public UnsupportedJPGFileException()
        { 
        }
    }
}
