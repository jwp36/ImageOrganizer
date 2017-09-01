using ImageOrganizer.Models;
using ImageOrganizer.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageOrganizer.ViewModels
{
    public class OrganizerViewModel : INotifyPropertyChanged
    {
        private string sourceDirectoryPath;
        private string destinationDirectoryPath;
        private IDirectoryValidator sourceDirectoryValidator;
        private IDirectoryValidator destinationDirectoryValidator;

        public string SourceDirectoryPath
        {
            get
            {
                return sourceDirectoryPath;
            }
            set
            {
                if (value == sourceDirectoryPath)
                    return;
                
                sourceDirectoryPath = value;
                onPropertyChanged("SourceDirectoryPath");
            }
        }
        public string DestinationDirectoryPath
        {
            get
            {
                return destinationDirectoryPath;
            }
            set
            {
                if (value == destinationDirectoryPath)
                    return;

                destinationDirectoryPath = value;
                onPropertyChanged("DestinationDirectoryPath");
            }
        }

        public ICommand StartOrganizationCommand { get; private set; }


 
        public OrganizerViewModel()
        {
            sourceDirectoryValidator = new SourceDirectoryValidator();
            destinationDirectoryValidator = new DestinationDirectoryValidator();

            SourceDirectoryPath = String.Empty;
            DestinationDirectoryPath = String.Empty;
            StartOrganizationCommand = new StartOrganizationCommand(this);
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));    
        }



        public void StartOrganization()
        {
            string fullSourceDirectoryPath = Path.GetFullPath(sourceDirectoryPath);
            string fullDestinationDirectoryPath = Path.GetFullPath(destinationDirectoryPath);
            
            sourceDirectoryValidator.Validate(fullSourceDirectoryPath);
            destinationDirectoryValidator.Validate(fullDestinationDirectoryPath);

            Organizer organizer = new Organizer(fullSourceDirectoryPath, fullDestinationDirectoryPath);
            JPGFileHandler jpgFileHandler = new JPGFileHandler(organizer);
            UnsupportedFileHandler unsupportedFileHandler = new UnsupportedFileHandler(organizer);

            organizer.Organize();
        }
    }

    public class StartOrganizationCommand : ICommand
    {
        private OrganizerViewModel viewModel;



        public StartOrganizationCommand(OrganizerViewModel viewModel)
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
            return !(String.IsNullOrWhiteSpace(viewModel.SourceDirectoryPath) || String.IsNullOrWhiteSpace(viewModel.DestinationDirectoryPath));
        }

        public void Execute(object parameter)
        {
            viewModel.StartOrganization();
        }
    }
}
