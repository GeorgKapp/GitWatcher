namespace GitWatcher.Source.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? repositoryPath;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(branchesView))]
        [AlsoNotifyChangeFor(nameof(remoteBranchesView))]
        private string? searchText;

        [ObservableProperty]
        private List<BranchModel> branches = new List<BranchModel>();

        [ObservableProperty]
        private List<BranchModel> remoteBranches = new List<BranchModel>();

        [ObservableProperty]
        private ICollectionView? branchesView;

        [ObservableProperty]
        private ICollectionView? remoteBranchesView;

        private Repository _gitRepos;
    }
}
