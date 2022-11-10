using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodeReviewSample;

public class ViewModelBase : INotifyPropertyChanged
{
    // ...
    
    public virtual void OnAppearing() { }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
    // ...
}