//from Windows Template Studio
//Copyright (c) .NET Foundation and Contributors.
//https://github.com/Microsoft/WindowsTemplateStudio/tree/master/templates/Uwp/_composition/MVVMBasic/Project/Helpers

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NNS_Z
{
    public class Observable : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if(Equals(storage, value)) return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
