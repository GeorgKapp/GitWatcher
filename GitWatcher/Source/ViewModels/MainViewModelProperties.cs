namespace GitWatcher.Source.ViewModels
{
    internal partial class MainViewModel : PropertyChangedObject
    {
        private string? _repositoryPath;
        public string? RepositoryPath
        {
            get => _repositoryPath;
            set
            {
                SetField(ref _repositoryPath, value);
            }
        }

        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                SetField(ref _searchText, value);
                InvokePropertyChanged(nameof(BranchesView));
                InvokePropertyChanged(nameof(RemoteBranchesView));
            }
        }


        private List<BranchModel> _branches = new List<BranchModel>();
        public List<BranchModel> Branches
        {
            get => _branches;
            set
            {
                SetField(ref _branches, value);
            }
        }


        private List<BranchModel> _remoteBranches = new List<BranchModel>();
        public List<BranchModel> RemoteBranches
        {
            get => _remoteBranches;
            set
            {
                SetField(ref _remoteBranches, value);
            }
        }


        private ICollectionView? _branchesView;
        public ICollectionView? BranchesView
        {
            get => _branchesView;
            set
            {
                SetField(ref _branchesView, value);
            }
        }


        private ICollectionView? _remoteBranchesView;
        public ICollectionView? RemoteBranchesView
        {
            get => _remoteBranchesView;
            set
            {
                SetField(ref _remoteBranchesView, value);
            }
        }

        private Repository _gitRepos;
    }
}
