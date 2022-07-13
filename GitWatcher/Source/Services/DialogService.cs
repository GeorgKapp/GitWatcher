namespace GitWatcher.Source.Services
{
    internal class DialogService
    {
        public void ShowError(string error) => Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        public void ShowError(Exception exception) => Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        public MessageBoxResult Show(string messageBoxText) => MessageBox.Show(messageBoxText);
        public MessageBoxResult Show(string messageBoxText, string caption) => MessageBox.Show(messageBoxText, caption);
        public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button) => MessageBox.Show(messageBoxText, caption, button);
        public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon) => MessageBox.Show(messageBoxText, caption, button, icon);
        public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult) => MessageBox.Show(messageBoxText, caption, button, icon, defaultResult);
        public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options) => MessageBox.Show(messageBoxText, caption, button, icon, defaultResult, options);
    }
}
