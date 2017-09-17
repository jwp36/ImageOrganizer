using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Models
{
    public class UnsupportedFileHandler
    {
        private Organizer organizer;

        public UnsupportedFileHandler(Organizer organizer)
        {
            this.organizer = organizer;
            this.organizer.UnsupportedFileFoundEvent += HandleUnsupportedFileFoundEvent;
        }

        private void HandleUnsupportedFileFoundEvent(object sender, UnsupportedFileFoundEventArgs e)
        {
            string destinationFilePath = Path.Combine(e.DestinationDirectoryPath, Path.GetFileName(e.FilePath));

            try
            {
                File.Copy(e.FilePath, destinationFilePath);
            }
            catch (IOException) //File already exists...
            {
                string fileName = Path.GetFileName(e.FilePath);
                string extension = Path.GetExtension(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName);

                int suffix = 1;
                string newFileName = String.Empty;
                while (File.Exists(destinationFilePath))
                {
                    newFileName = String.Format("{0} {1}{2}", fileName, suffix, extension);
                    destinationFilePath = Path.Combine(e.DestinationDirectoryPath, newFileName);
                    suffix++;
                }

                File.Copy(e.FilePath, destinationFilePath);
            }
        }
    }
}
