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
        
        /// <summary>
        /// Return an Organizer.
        /// </summary>
        /// <param name="sourceDirectoryPath"> Absolute path to source directory.</param>
        /// <param name="destinationDirectoryPath"> Absolute path to destination directory.</param>
        public Organizer(string sourceDirectoryPath, string destinationDirectoryPath)
        {
            this.sourceDirectoryPath = sourceDirectoryPath;
            this.destinationDirectoryPath = destinationDirectoryPath;
        }
        
        public event JPGFileFoundEventHandler JPGFileFoundEvent;
        public event UnsupportedFileFoundEventHandler UnsupportedFileFoundEvent;

        public delegate void UnsupportedFileFoundEventHandler(object sender, UnsupportedFileFoundEventArgs e);
        public delegate void JPGFileFoundEventHandler(object sender, JPGFileFoundEventArgs e);

        public void Organize()
        {
            string[] fileEntries = Directory.GetFiles(sourceDirectoryPath, "*", SearchOption.AllDirectories);
            foreach (string filePath in fileEntries)
            {
                ProcessFile(filePath);
            }
                
            return;
        }

        /// <summary>
        /// Process a file. Fire a JPGFileFound event is a JPG file is found; otherwise fire the UnsupportedFileFound event.
        /// </summary>
        /// <param name="filePath">
        /// Full file path to a file to be processed.
        /// </param>
        private void ProcessFile(string filePath)
        {
            if (filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    OnJPGFileFound(this, new JPGFileFoundEventArgs(filePath, destinationDirectoryPath));
                }
                catch (UnsupportedJPGFileException)
                {
                    OnUnsupportedFileFound(this, new UnsupportedFileFoundEventArgs(filePath, destinationDirectoryPath));
                }
            }
            else
            {
                OnUnsupportedFileFound(this, new UnsupportedFileFoundEventArgs(filePath, destinationDirectoryPath));
            }
        }

        private void OnJPGFileFound(Organizer sender, JPGFileFoundEventArgs e)
        {
            JPGFileFoundEvent?.Invoke(sender, e);
        }

        private void OnUnsupportedFileFound(Organizer sender, UnsupportedFileFoundEventArgs e)
        {
            UnsupportedFileFoundEvent?.Invoke(sender, e);
        }
    }

    public class JPGFileFoundEventArgs : EventArgs
    {
        public string FilePath { get; private set; }
        public string DestinationDirectoryPath { get; private set; }

        /// <summary>
        /// Return an instance of JPGFIleFoundEventArgs.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <param name="destinationDirectoryPath">The name of the destination directory.</param>
        public JPGFileFoundEventArgs(string filePath, string destinationDirectoryPath)
        {
            FilePath = filePath;
            DestinationDirectoryPath = destinationDirectoryPath;
        }
    }

    public class UnsupportedFileFoundEventArgs : EventArgs
    {      
        public string FilePath { get; private set; }
        public string DestinationDirectoryPath { get; private set; }

        /// <summary>
        /// Return an instance of UnsupportedFileFOundEventArgs.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <param name="destinationDirectoryPath">The name of the destination directory.</param>
        public UnsupportedFileFoundEventArgs(string filePath, string destinationDirectoryPath)
        {
            FilePath = filePath;
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
