﻿using ImageOrganizer.Models;
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
        private string sourceDirectoryPath;
        private string destinationDirectoryPath;

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
                if (value == DestinationDirectoryPath)
                    return;

                destinationDirectoryPath = value;
                onPropertyChanged("DestinationDirectoryPath");
            }
        }

        public ICommand StartOrganizationCommand { get; private set; }


 
        public ViewModel()
        {
            this.sourceDirectoryPath = String.Empty;
            this.destinationDirectoryPath = String.Empty;

            StartOrganizationCommand = new StartOrganizationCommand(this);
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));    
        }



        public void StartOrganization()
        {
            Organizer organizer = new Organizer(sourceDirectoryPath, destinationDirectoryPath);
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
            return !(String.IsNullOrWhiteSpace(viewModel.SourceDirectoryPath) || String.IsNullOrWhiteSpace(viewModel.DestinationDirectoryPath));
        }

        public void Execute(object parameter)
        {
            viewModel.StartOrganization();
        }
    }
}
