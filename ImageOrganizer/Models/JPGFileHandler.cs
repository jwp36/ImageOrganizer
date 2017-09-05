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

        public enum Naming { Original, EXIFDateTime };

        private Organizer organizer;
        private ASCIIEncoding enc;
        private Naming naming;
        
        public JPGFileHandler(Organizer organizer, Naming naming = Naming.Original)
        {
            this.organizer = organizer;
            this.organizer.JPGFileFoundEvent += HandleJPGFileFoundEvent;

            this.naming = naming;

            enc = new ASCIIEncoding();
        }

        private void HandleJPGFileFoundEvent(object sender, JPGFileFoundEventArgs e)
        {
            string fileName = String.Empty;
            string destinationSubdirectory = String.Empty;
            using (Image image = Image.FromFile(e.FilePath))
            {
                string dateTimeOriginal = (ParseDateTimeOriginal(image));

                //Subdirectory determination
                string imageDate = ParseDateFromDateTimeOriginal(dateTimeOriginal);
                destinationSubdirectory = Path.Combine(e.DestinationDirectoryPath, imageDate);
                if (!Directory.Exists(destinationSubdirectory))
                    Directory.CreateDirectory(destinationSubdirectory);

                //Filename determination
                switch (naming)
                {
                    case Naming.Original:
                        fileName = Path.GetFileName(e.FilePath);
                        break;

                    case Naming.EXIFDateTime:
                        fileName = GenerateFileNameFromDateTimeOriginal(dateTimeOriginal) + Path.GetExtension(e.FilePath);
                        break;
                }
            }

            string destinationFilePath = Path.Combine(destinationSubdirectory, fileName);
            File.Copy(e.FilePath, destinationFilePath);
        }

        private string ParseDateTimeOriginal(Image image)
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

        private string ParseDateFromDateTimeOriginal(string dateTimeOriginal)
        {
            string date = dateTimeOriginal.Split(' ')[0];
            return date.Replace(':', '-');
        }

        private string GenerateFileNameFromDateTimeOriginal(string dateTimeOriginal)
        {
            string[] partitioned = dateTimeOriginal.Split(' ');
            string date = partitioned[0];
            string time = partitioned[1].Substring(0, partitioned[1].Length - 1); //Removes null char

            return String.Format("{0} {1}", date.Replace(':', '-'), time.Replace(':', '.'));
        }
    }
}
