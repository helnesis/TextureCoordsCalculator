using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace TextureCoordsCalculatorGUI.Shared
{
    public static class Utilities
    {
        public static BitmapImage BitMapToImg(Bitmap bitmap)
        {
            using MemoryStream memory = new();

            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}
