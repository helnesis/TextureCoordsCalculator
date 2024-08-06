using SereniaBLPLib;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SWF = Microsoft.Win32;

namespace TextureCoordsCalculatorGUI
{
    public sealed record Coords(float Left, float Top, float Right, float Bottom);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlpFile? _blp;
        private Rectangle? _rectangle;

        private Point _topLeftPoint;
        private Point _bottomRightPoint;

        private float _magicNumberX;
        private float _magicNumberY;

        private (int Width, int Height) _imageSize;

        private Coords? _coords;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void OpenBlpFile(object sender, RoutedEventArgs e)
        {
            var dialog = new SWF.OpenFileDialog
            {
                DefaultExt = ".blp",
                Filter = "BLP Files (*.blp)|*.blp"
            };

            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value && dialog.CheckFileExists)
            {
                _blp = new(File.OpenRead(dialog.FileName));

                var image = BitMapToImg(_blp.GetBitmap(0));

                _imageSize = new(image.PixelWidth, image.PixelHeight);

                BlpImage.Width = _imageSize.Width;
                BlpImage.Height = _imageSize.Height;

                Canvas.Width = _imageSize.Width;
                Canvas.Height = _imageSize.Height;

                BlpImage.Source = image;


                _magicNumberX = (1.0f / image.PixelWidth) / 2.0f;
                _magicNumberY = (1.0f / image.PixelHeight) / 2.0f;
            }
            else
            {
                MessageBox.Show("A error occured while trying to open the file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_rectangle is not null)
                Canvas.Children.Remove(_rectangle);

            _topLeftPoint = e.GetPosition(Canvas);

            _rectangle = new Rectangle
            {

                Stroke = Brushes.GhostWhite,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(80, 102, 255, 102))
            };



            Canvas.SetLeft(_rectangle, _topLeftPoint.X);
            Canvas.SetTop(_rectangle, _topLeftPoint.Y);

            Canvas.Children.Add(_rectangle);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || _rectangle == null)
                return;

            var pos = e.GetPosition(Canvas);

            var x = Math.Min(pos.X, _topLeftPoint.X);
            var y = Math.Min(pos.Y, _topLeftPoint.Y);

            var w = Math.Max(pos.X, _topLeftPoint.X) - x;
            var h = Math.Max(pos.Y, _topLeftPoint.Y) - y;

            _rectangle.Width = w;
            _rectangle.Height = h;


            Canvas.SetLeft(_rectangle, x);
            Canvas.SetTop(_rectangle, y);

        }


        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_rectangle != null)
            {
                _bottomRightPoint = e.GetPosition(Canvas);

                var imgTopLeft = _topLeftPoint;
                var imgBottomRight = _bottomRightPoint;

                float left = (float)((imgTopLeft.X / _imageSize.Width) + _magicNumberX);
                float top = (float)((imgTopLeft.Y / _imageSize.Height) + _magicNumberY);
                float right = (float)((imgBottomRight.X / _imageSize.Width) - _magicNumberX);
                float bottom = (float)((imgBottomRight.Y / _imageSize.Height) - _magicNumberY);


                _coords = new(left, top, right, bottom);

                TextureCoordinates.Text = $"{NormalizeFloat(left)}, {NormalizeFloat(right)}, {NormalizeFloat(top)}, {NormalizeFloat(bottom)}";



                var width = (int)(imgBottomRight.X - imgTopLeft.X);
                var height = (int)(imgBottomRight.Y - imgTopLeft.Y);

                if (width > 0 && height > 0)
                {
                    var crop = new CroppedBitmap(BlpImage.Source as BitmapSource, new((int)imgTopLeft.X, (int)imgTopLeft.Y, width, height));
                    Texture.Width = width;
                    Texture.Height = height;
                    Texture.Source = crop;
                }

            }
        }


        private static string NormalizeFloat(float value)
            => value.ToString().Replace(',', '.');


        private void CopyAs(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || _coords is null)
                return;

            string content = string.Empty;

            switch (btn.Name)
            {
                case "lua":
                    content = $"({NormalizeFloat(_coords.Left)}, {NormalizeFloat(_coords.Right)}, {NormalizeFloat(_coords.Top)}, {NormalizeFloat(_coords.Bottom)})";
                    break;
                case "xml":
                    content = $"<TexCoords left=\"{NormalizeFloat(_coords.Left)}\" right=\"{NormalizeFloat(_coords.Right)}\" top=\"{NormalizeFloat(_coords.Top)}\" bottom=\"{NormalizeFloat(_coords.Bottom)}\"/>";
                    break;
            }

            Clipboard.SetText(content);
        }

        private static BitmapImage BitMapToImg(System.Drawing.Bitmap bitmap)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}