using SPCWB.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SPCWB.Views.Windows
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow(
            MainWindowViewModel viewModel
        )
        {
            ViewModel = viewModel;
            DataContext = this;

            SystemThemeWatcher.Watch(this);

            InitializeComponent();
            ContentRendered += MainWindow_ContentRendered;
            Closing += MainWindow_Closing;
        }

        private async void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            //仅在发行版使用!!!
            Hide();
            await ViewModel.ReadMarkdownFileAsync();
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            //仅在发行版使用!!!
            e.Cancel = true;
            Hide();
        }

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }
    }
}
