using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AkkaDemoConsole.ViewModels
{
    /// <summary>
    /// A base class for implementing the INotifyPropertViewModelsyChanged event. This class can also be used to validate
    /// properties and get a collection errors related to validation.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value that indicates that the object has property value changes.
        /// </summary>
        public bool HasChanges { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (string propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }
        }
        protected bool Set<T>(ref T current, T value, [CallerMemberName] string propertyName = "")
        {
            bool sameValue = current?.Equals(value) ?? false;

            if (!sameValue)
            {
                current = value;
                HasChanges = true;
                OnPropertyChanged(propertyName);
            }

            // Return true if the value changed
            return !sameValue;
        }
    }
}
