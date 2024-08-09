using CommunityToolkit.Mvvm.Input;
using SWF = Microsoft.Win32;
using TextureCoordsCalculatorGUI.Services;
using SereniaBLPLib;
using System.IO;
using System.Windows;
using TextureCoordsCalculatorGUI.Misc;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;
using TextureCoordsCalculatorGUI.ViewModels.Base;
using TextureCoordsCalculatorGUI.Views;
using TextureCoordsCalculatorGUI.Shared;
using System.Globalization;
using System.Collections.Immutable;
using System.Diagnostics;

namespace TextureCoordsCalculatorGUI.ViewModels
{
    public partial class MainViewModel(WagoService wagoService) : BaseViewModel("Texture Coordinates Calculator")
    {
        private readonly IWagoService _wagoService = wagoService;

        private BlpFile? _blpFile;

        private Coordinates? _coordinates;

        private TextureBrowserDialog _browser = new(wagoService);

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

        [ObservableProperty]
        string? croppedAreaLabel;

        [ObservableProperty]
        string? newCoords;

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
                ApplyImage();

        }

        [RelayCommand]
        public async Task OpenBrowserAsync()
        {
            _browser.ShowDialog();

            var texture = _browser.SelectedTexture;

            if (!string.IsNullOrEmpty(texture))
            {
                var file = await _wagoService.GetCascFile(Listfile.Instance.GetFileDataId(texture));

                if (file is not null)
                {
                    _blpFile = new BlpFile(file);
                    ApplyImage();
                }
            }

        }

        [RelayCommand]
        public void ApplyChanges()
        {
            if (NewCoords is not null)
            {
                var rawCoords = NewCoords.Split(',')
                    .Where(x => float.TryParse(x, NumberStyles.Any, provider: CultureInfo.InvariantCulture, out var _))
                    .Select(x => float.Parse(x, NumberStyles.Any, provider: CultureInfo.InvariantCulture)).ToImmutableArray();


                float leftCoords = rawCoords.ElementAtOrDefault(0);
                float rightCoords = rawCoords.ElementAtOrDefault(1);
                float topCoords = rawCoords.ElementAtOrDefault(2);
                float bottomCoords = rawCoords.ElementAtOrDefault(3);

                Area.Move(leftCoords, rightCoords, topCoords, bottomCoords);
                NormalizedCoords = $"{NormalizeFloat(leftCoords)},{NormalizeFloat(rightCoords)},{NormalizeFloat(topCoords)},{NormalizeFloat(bottomCoords)}";
            }


            if (CroppedImageWidth > 0 && CroppedImageHeight > 0)
                Area.Resize(CroppedImageWidth, CroppedImageHeight);
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

        public void UpdateCroppedAreaLabel(int width, int height)
        {
            CroppedAreaLabel = $"Size: {width}x{height}";
        }

        private void CalculateCroppedImage(Point leftTopPixels, Point bottomRightPixels)
        {
            var width = (int)(bottomRightPixels.X - leftTopPixels.X);
            var height = (int)(bottomRightPixels.Y - leftTopPixels.Y);

            if (width > 0 && height > 0 && leftTopPixels.X >= 0 && leftTopPixels.Y >= 0 && bottomRightPixels.X <= ImageWidth && bottomRightPixels.Y <= ImageHeight)
            {
                var crop = new CroppedBitmap(BlpImage, new Int32Rect((int)leftTopPixels.X, (int)leftTopPixels.Y, width, height));
                CroppedImage = crop;
                CroppedImageHeight = height;
                CroppedImageWidth = width;
            }
        }


        private void ApplyImage()
        {
            if (_blpFile is null)
                return;

            BlpImage = Utilities.BitMapToImg(_blpFile.GetBitmap(0));

            ImageWidth = BlpImage.PixelWidth;
            ImageHeight = BlpImage.PixelHeight;
        }

        private static string NormalizeFloat(float value)
                => value.ToString().Replace(',', '.');


    }
}
