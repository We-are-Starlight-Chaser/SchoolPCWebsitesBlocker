using QRCoder;
using SPCWB.Models;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Wpf.Ui.Controls;

namespace SPCWB.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            _ = GetQrCodeAsync();
        }
        #region Window Helper
        [ObservableProperty]
        private string _applicationTitle = "SPCWB";
        [RelayCommand]
        private void RedirectToPage(Tuple<TabControl, int> t)
        {
            t.Item1.SelectedIndex = t.Item2;
        }
        [RelayCommand]
        private void ShowWindow()
        {
            Application.Current.MainWindow.Show();
        }
        #endregion
        #region Blocker Helper
        [ObservableProperty]
        private ObservableCollection<BlockerItem> _blockerItems = [
            new BlockerItem(){
                Name = "Bilibili",
                Description = "A well-known domestic video website.",
                Url = ["https://*.bilibili.com" ,"https://b23.tv"],
                IsBlocked = false
            }
            ];
        [ObservableProperty]
        private bool _canChangeBlocker = false;
        #endregion
        #region Unblocker Helper
        [ObservableProperty]
        private BitmapSource _qrSource;
        [ObservableProperty]
        private string _inputedVerificationCode = "";
        private readonly string strCode = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        private string verificationCode = "";
        private int createdTimes;
        [RelayCommand]
        private async Task VerifyTheVerificationCodeAsync()
        {
            if (!CanChangeBlocker) {
                if (string.IsNullOrWhiteSpace(verificationCode))
                {
                    await new Wpf.Ui.Controls.MessageBox()
                    {
                        Title = "Verification code",
                        Content = "Verification code has not been received yet!"
                    }.ShowDialogAsync();
                    return;
                }
                if (verificationCode.Equals(InputedVerificationCode))
                {
#pragma warning disable CS4014
                    ChangeBolcker();
#pragma warning restore CS4014
                    await new Wpf.Ui.Controls.MessageBox()
                    {
                        Title = "Verification code",
                        Content = "Verification successful, please enjoy these 40 minutes!"
                    }.ShowDialogAsync();
                    return;
                }
                await new Wpf.Ui.Controls.MessageBox()
                {
                    Title = "Verification code",
                    Content = "Verification failed!"
                }.ShowDialogAsync();
                return;
            }
            await new Wpf.Ui.Controls.MessageBox()
            {
                Title = "Verification code",
                Content = "You have not yet finished the unblocking period!"
            }.ShowDialogAsync();
        }
        [RelayCommand]
        private async Task GetQrCodeAsync()
        {
            QRCodeGenerator qrGenerator = new();
            verificationCode = await GetVerificationCodeAsync().ConfigureAwait(false);
            QRCodeData qrCodeData = qrGenerator.CreateQrCode($"https://wasc.qzz.io/api/products/spcwb/qrcode.html?vc={verificationCode}", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);

            using Bitmap qrCodeImage = qrCode.GetGraphic(100, Color.Black, Color.White, true);
            IntPtr hBitmap = qrCodeImage.GetHbitmap();

            try
            {
                QrSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    DeleteObject(hBitmap);
                }
                createdTimes++;
                if(createdTimes >= 10)
                {
                    GC.Collect();
                }
            }
        }
        [LibraryImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DeleteObject(IntPtr hObject);
        private async Task<string> GetVerificationCodeAsync()
        {

            var _charArray = strCode.ToCharArray();
            var randomCode = "";
            var temp = -1;
            var rand = new Random(Guid.NewGuid().GetHashCode());
            for (var i = 0; i < 8; i++)
            {
                if (temp != -1)
                    rand = new Random(i * temp * (int)DateTime.Now.Ticks);
                var t = rand.Next(strCode.Length - 1);
                if (!string.IsNullOrWhiteSpace(randomCode))
                    while (randomCode.Contains(_charArray[t].ToString(), StringComparison.CurrentCultureIgnoreCase))
                        t = rand.Next(strCode.Length - 1);
                if (temp == t)
                    return await GetVerificationCodeAsync().ConfigureAwait(false);
                temp = t;

                randomCode += _charArray[t];
            }

            return randomCode;
        }
        private async Task ChangeBolcker()
        {
            CanChangeBlocker = true;
            //生产环境使用
            //await Task.Delay(40 * 60 * 1000);
            await Task.Delay(6000);
            CanChangeBlocker = false;
        }
        #endregion
        #region Settings Helper
        [ObservableProperty]
        private string _markdownText = "";
        public async Task ReadMarkdownFileAsync()
        {
            try
            {
                Uri uri = new("pack://application:,,,/Assets/About.md", UriKind.Absolute);
                StreamResourceInfo info = Application.GetResourceStream(uri);
                using var reader = new StreamReader(info.Stream);
                MarkdownText = await reader.ReadToEndAsync();
            }
            catch{ 
                
            }
        }
        #endregion
    }
}
