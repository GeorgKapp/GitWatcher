namespace GitWatcher.Source.Models
{
    internal record BranchModel
    {
        public Branch Branch { get; set; }
        public string Name { get; set; }  
        public string RemoteName { get; set; }
        public bool IsTracking { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCurrentHead { get; set; }
    }
}
