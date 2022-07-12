

namespace GitWatcher.Source.Services
{
    internal class FileService
    {
        public string SelectGitFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}
