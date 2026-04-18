using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace SPCWB.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "SPCWB";
    }
}
