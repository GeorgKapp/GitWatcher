namespace GitWatcher.Source.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadWindowSize();
        }
        private void ClearSorting_Click(object sender, RoutedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            if (view != null)
            {
                view.SortDescriptions.Clear();
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    column.SortDirection = null;
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ((MainViewModel)DataContext).Dispose();
            Settings.Default.WindowWidth = Width;
            Settings.Default.WindowHeight = Height;
            Settings.Default.Save();
        }

        private void LoadWindowSize()
        {
            Width = Settings.Default.WindowWidth;
            Height = Settings.Default.WindowHeight;
        }

    }
}
