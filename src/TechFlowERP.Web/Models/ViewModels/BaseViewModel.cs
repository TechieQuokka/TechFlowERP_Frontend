using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TechFlowERP.Web.Models.ViewModels
{
    /// <summary>
    /// 모든 ViewModel의 기본 클래스
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public bool IsLoading { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        protected void ClearError()
        {
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasError));
        }

        protected void SetError(string message)
        {
            ErrorMessage = message;
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasError));
        }
    }
}