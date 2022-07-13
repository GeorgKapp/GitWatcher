

namespace GitWatcher.Source.ViewModels
{
    internal partial class MainViewModel : ObservableObject, IDisposable
    {
        public MainViewModel()
        {
            _messageService = new DialogService();
            _gitService = new GitService();
            _fileService = new FileService();

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            var defaultDir = Settings.Default.DefaultDirectory;
            if (!string.IsNullOrWhiteSpace(defaultDir))
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

            if (remoteBranchesView != null)
                remoteBranchesView.Refresh();
        }

        [ICommand]
        private void DeleteItem(System.Collections.IList items)
        {
            var branchModels = items.Cast<BranchModel>();
            foreach(var model in branchModels)
            {
                if (model.IsDefault)
                {
                    _messageService.ShowError($"Removing of the default branch: {model.Name} is not possible !");
                    return;
                }

                if (model.IsCurrentHead)
                {
                    _messageService.ShowError($"Removing of the current head branch: {model.Name} is not possible !");
                    return;
                }
            }

            var result = _messageService.Show("Are you sure you want to delete the selected branches ?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                return;

            foreach(var item in items.Cast<BranchModel>())
                _gitService.RemoveBranch(_gitRepos, item.Branch);

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
                var branches = _gitService.GetGitBranchesForRepository(_gitRepos);
                var defaultBranchTargetIdentifier = _gitService.GetRemoteBranchTargetIdentifier(_gitRepos);

                var branchModels = branches.Select(p => new BranchModel
                {
                    Branch = p,
                    IsTracking = p.IsTracking,
                    Name = p.FriendlyName,
                    RemoteName = p.CanonicalName,
                    IsCurrentHead = p.IsCurrentRepositoryHead,
                    IsDefault = p.Reference.TargetIdentifier == defaultBranchTargetIdentifier
                }).ToList();

                Branches = branchModels.Where(p => !p.Branch.IsRemote).ToList();
                RemoteBranches = branchModels.Where(p => p.Branch.IsRemote).ToList();

                BranchesView = CollectionViewSource.GetDefaultView(Branches);
                BranchesView.Filter = branchModel => ApplyFilter((BranchModel)branchModel);

                RemoteBranchesView = CollectionViewSource.GetDefaultView(RemoteBranches);
                RemoteBranchesView.Filter = branchModel => ApplyFilter((BranchModel)branchModel);
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

        private readonly DialogService _messageService;
        private readonly GitService _gitService;
        private readonly FileService _fileService;

    }
}
