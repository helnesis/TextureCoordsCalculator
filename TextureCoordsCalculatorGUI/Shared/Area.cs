using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TextureCoordsCalculatorGUI.Shared
{
    /// <summary>
    /// Static Area
    /// </summary>
    public sealed class Area
    {
        private static Rectangle? _area;

        private static Canvas? _parent;

        public static Rectangle Rectangle { get { return _area!; } set { _area = value; } }

        public static void Initialize(Rectangle rectangle, Canvas parent)
        {
            _area = rectangle;
            _parent = parent;
        }

        public static void Resize(double newWidth, double newHeight)
        {
            if (_area == null || _parent is null)
                return;


            newWidth = Math.Max(0, Math.Min(newWidth, _parent.ActualWidth - Canvas.GetLeft(Rectangle)));
            newHeight = Math.Max(0, Math.Min(newHeight, _parent.ActualHeight - Canvas.GetTop(Rectangle)));

            Rectangle.Width = newWidth;
            Rectangle.Height = newHeight;

        }

        private static Point ConvertTextureCoordsToCanvasCoords(double texCoordX, double texCoordY)
        {
            if (_parent is null)
                return default;

            double canvasX = texCoordX * _parent.ActualWidth;
            double canvasY = texCoordY * _parent.ActualHeight;

            return new Point(canvasX, canvasY);
        }


        public static void Move(double left, double right, double top, double bottom)
        {
            if (Rectangle == null || _parent is null)
                return;

            Point canvasTopLeft = ConvertTextureCoordsToCanvasCoords(left, top);
            Point canvasBottomRight = ConvertTextureCoordsToCanvasCoords(right, bottom);

            double newWidth = canvasBottomRight.X - canvasTopLeft.X;
            double newHeight = canvasBottomRight.Y - canvasTopLeft.Y;

            double newLeft = Math.Max(0, Math.Min(canvasTopLeft.X, _parent.ActualWidth - newWidth));
            double newTop = Math.Max(0, Math.Min(canvasTopLeft.Y, _parent.ActualHeight - newHeight));

            Canvas.SetLeft(Rectangle, newLeft);
            Canvas.SetTop(Rectangle, newTop);

            Rectangle.Width = newWidth;
            Rectangle.Height = newHeight;
        }
    }
}
