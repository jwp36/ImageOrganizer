using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Models
{
    public class Organizer
    {
        private string sourceDirectory;
        private string destinationDirectory;



        public Organizer(string sourceDirectory, string destinationDirectory)
        {
            this.sourceDirectory = sourceDirectory;
            this.destinationDirectory = destinationDirectory;
        }



        public void Organize()
        {
            return;
        }
    }
}
