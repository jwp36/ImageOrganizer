using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public ICommand StartOrganizationCommand { get; private set; }


 
        public ViewModel()
        {
            this.sourceDirectory = String.Empty;
            this.destinationDirectory = String.Empty;

            StartOrganizationCommand = new StartOrganizationCommand(this);
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));    
        }



        public void StartOrganization()
        {
            throw new NotImplementedException();
        }
    }

    public class StartOrganizationCommand : ICommand
    {
        private ViewModel viewModel;



        public StartOrganizationCommand(ViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.PropertyChanged += onCanExecuteChanged;
        }

        

        public event EventHandler CanExecuteChanged;

        private void onCanExecuteChanged(object sender, PropertyChangedEventArgs e)
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);   
        }



        public bool CanExecute(object parameter)
        {
            return !(String.IsNullOrWhiteSpace(viewModel.SourceDirectory) || String.IsNullOrWhiteSpace(viewModel.DestinationDirectory));
        }

        public void Execute(object parameter)
        {
            viewModel.StartOrganization();
        }
    }
}
