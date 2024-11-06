using System.IO;
using System.Windows.Controls;
using System.Drawing; // для Bitmap
using System.Windows.Media.Imaging;
using QRCoder;

namespace Study_Kamalov_wpf_320P.Pages
{
    /// <summary>
    /// Логика взаимодействия для QR_Kod.xaml
    /// </summary>
    public partial class QR_Kod : Page
    {
        public QR_Kod()
        {
            InitializeComponent();
            GenerateQRCode();
        }

        private BitmapImage LoadImage(byte[] imageData)
        {
            using (var ms = new MemoryStream(imageData))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
        }

        private void GenerateQRCode()
        {
            string url = "https://mck-ktits.ru/";

            if (!string.IsNullOrEmpty(url))
            {
                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new PngByteQRCode(qrCodeData))
                    {
                        var qrCodeImage = qrCode.GetGraphic(20);
                        QRCodeImage.Source = LoadImage(qrCodeImage);
                    }
                }
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}
