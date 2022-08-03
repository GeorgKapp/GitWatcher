namespace GitWatcher.Source.ViewModels
{
    internal partial class MainViewModel : IDisposable
    {
        private ICommand _openGitRepositoryCommand;
        public ICommand OpenGitRepositoryCommand => _openGitRepositoryCommand ??= new SimpleCommand(() => OpenGitRepository());

        private ICommand _initializeAndLoadGitBranches;
        public ICommand InitializeAndLoadGitBranchesCommand => _initializeAndLoadGitBranches ??= new SimpleCommand(() => InitializeAndLoadGitBranches());

        private ICommand _refreshBranchesViewBranches;
        public ICommand RefreshBranchesViewCommand => _refreshBranchesViewBranches ??= new SimpleCommand(() => RefreshBranchesView());

        private ICommand _deleteItemCommand;
        public ICommand DeleteItemCommand => _deleteItemCommand ??= new SimpleParamaterCommand((object items) => DeleteItem(items));


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

        private void InitializeAndLoadGitBranches()
        {
            InitializeRepository();
            LoadBranches();
        }

        private void RefreshBranchesView()
        {
            if (_branchesView != null)
                _branchesView.Refresh();

            if (_remoteBranchesView != null)
                _remoteBranchesView.Refresh();
        }

        private void DeleteItem(object items)
        {
            var branchModels = ((System.Collections.IList)items).Cast<BranchModel>();
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

            foreach(var item in ((System.Collections.IList)items).Cast<BranchModel>())
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
