using CommunityToolkit.Mvvm.Input;
using SWF = Microsoft.Win32;
using TextureCoordsCalculatorGUI.Services;
using SereniaBLPLib;
using System.IO;
using System.Diagnostics;
using System.Windows;
using TextureCoordsCalculatorGUI.Misc;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;
using TextureCoordsCalculatorGUI.ViewModels.Base;
using TextureCoordsCalculatorGUI.Views;

namespace TextureCoordsCalculatorGUI.ViewModels
{
    public partial class MainViewModel(WagoService wagoService) : BaseViewModel("Texture Coordinates Calculator")
    {
        private readonly IWagoService _wagoService = wagoService;
 
        private BlpFile? _blpFile;

        private Coordinates? _coordinates;

        [ObservableProperty]
        string? normalizedCoords;

        [ObservableProperty]
        BitmapImage? blpImage;

        [ObservableProperty]
        CroppedBitmap? croppedImage;

        [ObservableProperty]
        int imageWidth;

        [ObservableProperty]
        int imageHeight;

        [ObservableProperty]
        int croppedImageWidth;

        [ObservableProperty]
        int croppedImageHeight;

        /// <summary>
        /// Open a .blp file, locally or directly through Wago API.
        /// </summary>
        /// <param name="onlineMode">File will be opened through Wago API.</param>

        [RelayCommand]
        public async Task OpenImageFile(bool onlineMode)
        {
            if (!onlineMode)
            {
                var fileDialog = new SWF.OpenFileDialog
                {
                    DefaultExt = ".blp",
                    Filter = "BLP Files (*.blp)|*.blp"
                };

                var result = fileDialog.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    _blpFile = new BlpFile(File.OpenRead(fileDialog.FileName));

                }
            }
            else
            {
                InputDialog inputDialog = new();
                var result = inputDialog.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    var stream = await _wagoService.GetCascFile((uint)inputDialog.FileDataId);
                    
                    if (stream is not null)
                    {
                        try
                        {
                            _blpFile = new BlpFile(stream);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Invalid file format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
     
                }
        
            }

            if (_blpFile is not null)
            {
                BlpImage = BitMapToImg(_blpFile.GetBitmap(0));

                ImageWidth = BlpImage.PixelWidth;
                ImageHeight = BlpImage.PixelHeight;
            }



        }


        [RelayCommand]
        public void CopyAs(string type)
        {
            if (_coordinates is null)
                return;

            string content = string.Empty;

            switch (type)
            {
                case "lua":
                    content = $"({NormalizeFloat(_coordinates.Left)}, {NormalizeFloat(_coordinates.Right)}, {NormalizeFloat(_coordinates.Top)}, {NormalizeFloat(_coordinates.Bottom)})";
                    break;
                case "xml":
                    content = $"<TexCoords left=\"{NormalizeFloat(_coordinates.Left)}\" right=\"{NormalizeFloat(_coordinates.Right)}\" top=\"{NormalizeFloat(_coordinates.Top)}\" bottom=\"{NormalizeFloat(_coordinates.Bottom)}\"/>";
                    break;
            }


            Clipboard.SetText(content);
        }


        public void CalculateCoordinates(int width, int height, Point leftTopPixels, Point bottomRightPixels)
        {
            var calculator = new TexCoordinatesCalculator(width, height, leftTopPixels, bottomRightPixels);
            _coordinates = calculator.TextureCoordinates;

            if (_coordinates is not null)
            {
                NormalizedCoords = $"{NormalizeFloat(_coordinates.Left)},{NormalizeFloat(_coordinates.Right)},{NormalizeFloat(_coordinates.Top)},{NormalizeFloat(_coordinates.Bottom)}";
                CalculateCroppedImage(leftTopPixels, bottomRightPixels);    
            }
        }

        private void CalculateCroppedImage(Point leftTopPixels, Point bottomRightPixels)
        {
            var width = (int)(bottomRightPixels.X - leftTopPixels.X);
            var height = (int)(bottomRightPixels.Y - leftTopPixels.Y);

            if (width > 0 && height > 0)
            {
                var crop = new CroppedBitmap(BlpImage, new((int)leftTopPixels.X, (int)leftTopPixels.Y, width, height));
                CroppedImage = crop;
                CroppedImageHeight = crop.PixelHeight;
                CroppedImageWidth = crop.PixelWidth;


            }
        }

        private static string NormalizeFloat(float value)
                => value.ToString().Replace(',', '.');

        private static BitmapImage BitMapToImg(Bitmap bitmap)
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
