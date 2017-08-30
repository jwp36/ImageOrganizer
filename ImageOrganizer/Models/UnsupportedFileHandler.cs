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
            string fullSourcePath = Path.Combine(e.SourceDirectoryPath, e.FileName);
            string fullDestinationPath = Path.Combine(e.DestinationDirectoryPath, e.FileName);

            File.Copy(fullSourcePath, fullDestinationPath);
        }
    }
}
