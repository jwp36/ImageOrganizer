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
            this.organizer.UnsupportedFileFoundEvent += handleUnsupportedFileFoundEvent;
        }

        private void handleUnsupportedFileFoundEvent(object sender, UnsupportedFileFoundEventArgs e)
        {
            string destinationFilePath = Path.Combine(e.DestinationDirectoryPath, Path.GetFileName(e.FilePath));
            File.Copy(e.FilePath, destinationFilePath);
        }
    }
}
