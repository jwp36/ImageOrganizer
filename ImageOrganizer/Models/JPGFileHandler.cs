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

        private void HandleJPGFileFoundEvent(object sender, JPGFileFoundEventArgs args)
        {
            string dateTimeOriginal = String.Empty;
            using (Image image = Image.FromFile(args.FilePath))
            {
                dateTimeOriginal = ParseDateTimeOriginal(image);
            }
           
            string destinationSubdirectory = GenerateSubdirectoryPath(args, dateTimeOriginal);
            if (!Directory.Exists(destinationSubdirectory))
                Directory.CreateDirectory(destinationSubdirectory);
            
            string fileName = GenerateFileName(args, dateTimeOriginal);

            string destinationFilePath = Path.Combine(destinationSubdirectory, fileName);

            try
            {
                File.Copy(args.FilePath, destinationFilePath);
            }
            catch (IOException) //File already exists...
            {
                string extension = Path.GetExtension(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName);

                int suffix = 1;
                string newFileName = String.Empty;
                while (File.Exists(destinationFilePath))
                {
                    newFileName = String.Format("{0} {1}{2}", fileName, suffix, extension);
                    destinationFilePath = Path.Combine(destinationSubdirectory, newFileName);
                    suffix++;
                }

                File.Copy(args.FilePath, destinationFilePath);
            }
        }

        private string GenerateSubdirectoryPath(JPGFileFoundEventArgs args, string dateTimeOriginal)
        {
            string imageDate = ParseDateFromDateTimeOriginal(dateTimeOriginal);
            return Path.Combine(args.DestinationDirectoryPath, imageDate);
        }

        private string GenerateFileName(JPGFileFoundEventArgs args, string dateTimeOriginal)
        {
            string fileName = String.Empty;
            switch (naming)
            {
                case Naming.Original:
                    fileName = Path.GetFileName(args.FilePath);
                    break;

                case Naming.EXIFDateTime:
                    fileName = GenerateFileNameFromDateTimeOriginal(dateTimeOriginal) + Path.GetExtension(args.FilePath);
                    break;
            }

            return fileName;
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
