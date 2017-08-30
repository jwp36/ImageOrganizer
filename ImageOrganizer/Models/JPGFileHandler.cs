using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ImageOrganizer.Models
{
    public class JPGFileHandler
    {
        public static readonly int EXIFDateTimeOriginalID = 0x9003;
        private Organizer organizer;
        private ASCIIEncoding enc;

        public JPGFileHandler(Organizer organizer)
        {
            this.organizer = organizer;
            this.organizer.JPGFileFoundEvent += handleJPGFileFoundEvent;

            enc = new ASCIIEncoding();
        }

        private void handleJPGFileFoundEvent(Organizer organizer, JPGFileFoundEventArgs e)
        {
            string fullSourceFilePath = Path.Combine(e.SourceDirectoryPath, e.FileName);

            string imageDate = String.Empty;
            using (Image image = Image.FromFile(fullSourceFilePath))
            {
                imageDate = parseDateFromDateTimeOriginal(parseDateTimeOriginal(image));
            }
            
            string destinationSubdirectory = Path.Combine(e.DestinationDirectoryPath, imageDate);
            if (!Directory.Exists(destinationSubdirectory))
                Directory.CreateDirectory(destinationSubdirectory);

            string fullDestinationFilePath = Path.Combine(destinationSubdirectory, e.FileName);
            File.Copy(fullSourceFilePath, fullDestinationFilePath);
        }

        private string parseDateTimeOriginal(Image image)
        {
            try
            {
                PropertyItem propertyItem = image.GetPropertyItem(EXIFDateTimeOriginalID);
                return enc.GetString(propertyItem.Value);
            }
            catch (ArgumentException)
            {
                throw new UnsupportedJPGFileException();
            }
        }

        private string parseDateFromDateTimeOriginal(string dateTimeOriginal)
        {
            string date = dateTimeOriginal.Split(' ')[0];
            return date.Replace(':', '-');
        }

    }
}
