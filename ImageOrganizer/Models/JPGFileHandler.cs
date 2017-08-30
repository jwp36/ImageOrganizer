using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ImageOrganizer.Models
{
    public class JPGFileHandler
    {
        private Organizer organizer;
        private ASCIIEncoding enc;

        public JPGFileHandler(Organizer organizer)
        {
            this.organizer = organizer;
            this.organizer.JPGFileFoundEvent += HandleJPGFileFoundEvent;

            enc = new ASCIIEncoding();
        }

        private void HandleJPGFileFoundEvent(Organizer organizer, JPGFileFoundEventArgs e)
        {
            throw new NotImplementedException();
        }

        private string parseDateTimeOriginal(Image image)
        {
            try
            {
                PropertyItem propertyItem = image.GetPropertyItem(0x9003); //what if doesn't exist?
                return enc.GetString(propertyItem.Value); //Split by space, take first elem, replace : with - 
            }
            catch (ArgumentException)
            {
                throw new UnsupportedJPGFileException();
            }
        }
    }
}
