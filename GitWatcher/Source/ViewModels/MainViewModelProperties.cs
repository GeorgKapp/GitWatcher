namespace GitWatcher.Source.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? repositoryPath;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(branchesView))]
        private string? searchText;

        [ObservableProperty]
        private List<BranchModel> branches = new List<BranchModel>();

        [ObservableProperty]
        private ICollectionView? branchesView;

        private Repository _gitRepos;
    }
}
