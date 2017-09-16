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
using System.Runtime.CompilerServices;

namespace ImageOrganizer.ViewModels
{
    public class OrganizerViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string sourceDirectoryPath;
        private string destinationDirectoryPath;
        private int progress;
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                if (value == progress)
                    return;

                progress = value;
                OnPropertyChanged();
            }
        }
        public bool RenameFilesbyDateAndTime { get; set; }

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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

 
        public void StartOrganization()
        {
            JPGFileHandler.Naming namingMode = RenameFilesbyDateAndTime ? JPGFileHandler.Naming.EXIFDateTime : JPGFileHandler.Naming.Original;

            string fullSourceDirectoryPath = Path.GetFullPath(sourceDirectoryPath);
            string fullDestinationDirectoryPath = Path.GetFullPath(destinationDirectoryPath);

            ValidateDirectoryPath(fullSourceDirectoryPath, sourceDirectoryValidator, "SourceDirectoryPath");
            ValidateDirectoryPath(destinationDirectoryPath, destinationDirectoryValidator, "DestinationDirectoryPath");

            if (!HasErrors)
            {
                Progress<int> progress = new Progress<int>();
                progress.ProgressChanged += HandleProgressChangedEvent;

                Organizer organizer = new Organizer(fullSourceDirectoryPath, fullDestinationDirectoryPath, progress);
                JPGFileHandler jpgFileHandler = new JPGFileHandler(organizer, namingMode);
                UnsupportedFileHandler unsupportedFileHandler = new UnsupportedFileHandler(organizer);

                Task.Run(() => organizer.Organize());
            }
        }

        private void HandleProgressChangedEvent(object sender, int e)
        {
            Progress = e;
        }

        private void ValidateDirectoryPath(string directoryPath, IDirectoryValidator directoryValidator, string propertyName)
        {
            bool isValid = directoryValidator.Validate(directoryPath, out ICollection<string> errors);

            if (!isValid)
            {
                validationErrors[propertyName] = errors;
                OnErrorsChanged(propertyName);
            }
            else if (validationErrors.ContainsKey(propertyName))
            {
                validationErrors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }

    public class StartOrganizationCommand : ICommand
    {
        private OrganizerViewModel viewModel;

        public StartOrganizationCommand(OrganizerViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.PropertyChanged += OnCanExecuteChanged;
        }
      
        public event EventHandler CanExecuteChanged;

        private void OnCanExecuteChanged(object sender, PropertyChangedEventArgs e)
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
