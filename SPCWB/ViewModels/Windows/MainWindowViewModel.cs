using QRCoder;
using SPCWB.Models;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace SPCWB.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
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
                    CanChangeBlocker = true;
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
            verificationCode = await GetVerificationCodeAsync();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://wasc.qzz.io/products/api/spcwb/qrcode.html?vc=" + verificationCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(100, Color.Black, Color.White, true);


            IntPtr hBitmap = qrCodeImage.GetHbitmap();
            // 将位图转换为 WPF 可以使用的 BitmapSource
            QrSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
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
                    while (randomCode.ToLower().Contains(_charArray[t].ToString().ToLower()))
                        t = rand.Next(strCode.Length - 1);
                if (temp == t)
                    return await GetVerificationCodeAsync();
                temp = t;

                randomCode += _charArray[t];
            }

            return randomCode;
        }
        #endregion
    }
}
