

namespace GitWatcher.Source.ViewModels
{
    internal partial class MainViewModel : ObservableObject, IDisposable
    {
        public MainViewModel()
        {
            var defaultDir = Settings.Default.DefaultDirectory;
            if(!string.IsNullOrWhiteSpace(defaultDir))
            {
                RepositoryPath = defaultDir;
                InitializeAndLoadGitBranches();
            }
        }

        [ICommand]
        private void OpenGitRepository()
        {
            try
            {
                var path = _fileService.SelectGitFolder();
                RepositoryPath = path;

                if(!string.IsNullOrEmpty(path))
                {
                    Settings.Default.DefaultDirectory = path;
                    Settings.Default.Save();
                    InitializeAndLoadGitBranches();
                }
            }
            catch(Exception exception)
            {
                _messageService.ShowError(exception);
            }
        }

        [ICommand]
        private void InitializeAndLoadGitBranches()
        {
            InitializeRepository();
            LoadBranches();
        }

        [ICommand]
        private void RefreshBranchesView()
        {
            if (branchesView != null)
                branchesView.Refresh();
        }

        [ICommand]
        private void DeleteItem(System.Collections.IList items)
        {
            var result = _messageService.Show("Are you sure you want to delete the selected branches ?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                return;

            foreach(var item in items.Cast<BranchModel>())
            {
                _gitService.RemoveBranch(_gitRepos, item.Branch);
            }
            LoadBranches();
        }

        private bool ApplyFilter(BranchModel model)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            foreach(var searchTerm in SearchText.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (model.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private void InitializeRepository()
        {
            try
            {
                _gitRepos = _gitService.GetRepositoryForPath(RepositoryPath);
            }
            catch (Exception exception)
            {
                _messageService.ShowError(exception);
            }
        }

        private void LoadBranches()
        {
            try
            {
                Branches = _gitService.GetGitBranchesForRepository(_gitRepos).Select(p => new BranchModel
                {
                    Branch = p,
                    IsTracking = p.IsTracking,
                    Name = p.FriendlyName,
                    RemoteName = p.CanonicalName
                }).ToList();

                BranchesView = CollectionViewSource.GetDefaultView(Branches);
                BranchesView.Filter = branchModel => ApplyFilter((BranchModel)branchModel);
            }
            catch (Exception exception)
            {
                _messageService.ShowError(exception);
            }
        }

        public void Dispose()
        {
            _gitRepos.Dispose();
        }

        private readonly GitService _gitService = new GitService();
        private readonly FileService _fileService = new FileService();
        private readonly DialogService _messageService = new DialogService();
    }
}
