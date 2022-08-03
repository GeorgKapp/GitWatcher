namespace GitWatcher.Source.Base;
public abstract class PropertyChangedObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    protected void InvokePropertyChanged(string callerPropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerPropertyName));
    }
}
