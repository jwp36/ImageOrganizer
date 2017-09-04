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

        private void handleJPGFileFoundEvent(object sender, JPGFileFoundEventArgs e)
        {
            string imageDate = String.Empty;
            using (Image image = Image.FromFile(e.FilePath))
            {
                imageDate = parseDateFromDateTimeOriginal(parseDateTimeOriginal(image));
            }
            
            string destinationSubdirectory = Path.Combine(e.DestinationDirectoryPath, imageDate);
            if (!Directory.Exists(destinationSubdirectory))
                Directory.CreateDirectory(destinationSubdirectory);

            string destinationFilePath = Path.Combine(destinationSubdirectory, Path.GetFileName(e.FilePath));
            File.Copy(e.FilePath, destinationFilePath);
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
