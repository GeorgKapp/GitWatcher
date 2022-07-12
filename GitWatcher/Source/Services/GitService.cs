namespace GitWatcher.Source.Services
{
    internal class GitService
    {
        public Repository GetRepositoryForPath(string repositoryPath)
        {
            if (!Directory.Exists(repositoryPath))
            {
                throw new FileNotFoundException($"Path does not exist for {repositoryPath}");
            }
            return new Repository(repositoryPath);
        }

        public IEnumerable<Branch> GetGitBranchesForRepository(Repository repository)
        {
            return repository.Branches;
        }

        public IEnumerable<Commit> GetCommitsForBranch(Branch branch)
        {
            return branch.Commits;
        }

        public void RemoveBranch(Repository repository, Branch branch)
        {
            repository.Branches.Remove(branch);
        }
    }
}
