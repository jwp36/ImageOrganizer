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
using System.Collections;

namespace ImageOrganizer.ViewModels
{
    public class OrganizerViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string sourceDirectoryPath;
        private string destinationDirectoryPath;
        private IDirectoryValidator sourceDirectoryValidator;
        private IDirectoryValidator destinationDirectoryValidator;
        
        private readonly Dictionary<string, ICollection<string>> validationErrors;

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
        public bool HasErrors
        {
            get
            {
                return validationErrors.Count > 0;
            }
        }
        public IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName) || !validationErrors.ContainsKey(propertyName))
                return null;

            return validationErrors[propertyName];
        }

        public OrganizerViewModel()
        {
            validationErrors = new Dictionary<string, ICollection<string>>();

            sourceDirectoryValidator = new SourceDirectoryValidator();
            destinationDirectoryValidator = new DestinationDirectoryValidator();

            SourceDirectoryPath = String.Empty;
            DestinationDirectoryPath = String.Empty;
            StartOrganizationCommand = new StartOrganizationCommand(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));    
        }
        private void onErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public void StartOrganization()
        {
            string fullSourceDirectoryPath = Path.GetFullPath(sourceDirectoryPath);
            string fullDestinationDirectoryPath = Path.GetFullPath(destinationDirectoryPath);

            validateDirectoryPath(fullSourceDirectoryPath, sourceDirectoryValidator, "SourceDirectoryPath");
            validateDirectoryPath(destinationDirectoryPath, destinationDirectoryValidator, "DestinationDirectoryPath");

            if (!HasErrors)
            {
                Organizer organizer = new Organizer(fullSourceDirectoryPath, fullDestinationDirectoryPath);
                JPGFileHandler jpgFileHandler = new JPGFileHandler(organizer);
                UnsupportedFileHandler unsupportedFileHandler = new UnsupportedFileHandler(organizer);

                organizer.Organize();
            }
        }

        private void validateDirectoryPath(string directoryPath, IDirectoryValidator directoryValidator, string propertyName)
        {
            bool isValid = directoryValidator.Validate(directoryPath, out ICollection<string> errors);

            if (!isValid)
            {
                validationErrors[propertyName] = errors;
                onErrorsChanged(propertyName);
            }
            else if (validationErrors.ContainsKey(propertyName))
            {
                validationErrors.Remove(propertyName);
                onErrorsChanged(propertyName);
            }
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
