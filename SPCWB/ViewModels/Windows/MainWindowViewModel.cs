using QRCoder;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SPCWB.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "SPCWB";
        [ObservableProperty]
        private BitmapSource _qrSource;
        private readonly string strCode = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";

        [RelayCommand]
        private void RedirectToPage(Tuple<TabControl, int> t)
        {
            t.Item1.SelectedIndex = t.Item2;
        }
        [RelayCommand]
        private async Task GetQrCodeAsync()
        {
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://wasc.qzz.io/products/api/spcwb/qrcode.html?vc=" + await GetVerificationCodeAsync(), QRCodeGenerator.ECCLevel.Q);
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
    }
}
