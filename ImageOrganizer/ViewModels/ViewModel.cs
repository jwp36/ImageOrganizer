using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string sourceDirectory;
        private string destinationDirectory;

        public string SourceDirectory
        {
            get
            {
                return sourceDirectory;
            }
            set
            {
                if (value == sourceDirectory)
                    return;
                
                sourceDirectory = value;
                onPropertyChanged("SourceDirectory");
            }
        }
        public string DestinationDirectory
        {
            get
            {
                return destinationDirectory;
            }
            set
            {
                if (value == DestinationDirectory)
                    return;

                destinationDirectory = value;
                onPropertyChanged("DestinationDirectory");
            }
        }



        public ViewModel()
        {
            this.sourceDirectory = String.Empty;
            this.destinationDirectory = String.Empty;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        


        private void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));    
        }
    }
}
