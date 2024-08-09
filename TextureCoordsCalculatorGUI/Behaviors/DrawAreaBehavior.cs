using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TextureCoordsCalculatorGUI.Shared;
using TextureCoordsCalculatorGUI.ViewModels;

namespace TextureCoordsCalculatorGUI.Behaviors
{
    enum ResizeDirection 
    {
        None, 

        TopLeft, 

        TopRight, 

        BottomLeft,
        
        BottomRight 
    }

    public class DrawAreaBehavior : Behavior<Canvas>
    {
        private Point _topLeftPoint;
        
        private Point _bottomRightPoint;

        private bool _isDragging;

        private Point _dragStartPoint;

        private ResizeDirection _resizeDirection;

        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(DrawAreaBehavior), new PropertyMetadata(null));

        public MainViewModel ViewModel
        {
            get => (MainViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        protected override void OnAttached()
        {
         

            base.OnAttached();
            AssociatedObject.MouseDown += MouseDown;
            AssociatedObject.MouseMove += MouseMove;
            AssociatedObject.MouseUp += MouseUp;

            Area.Initialize(new(), AssociatedObject);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseDown -= MouseDown;
            AssociatedObject.MouseMove -= MouseMove;
            AssociatedObject.MouseUp -= MouseUp;

        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(AssociatedObject);

            if (Area.Rectangle != null)
            {
                if (IsOnResizeHandle(pos, out _resizeDirection))
                {
                    _isDragging = true;
                    _dragStartPoint = pos;
                    return;
                }
                else if (Area.Rectangle.IsMouseOver)
                {
                    _isDragging = true;
                    _resizeDirection = ResizeDirection.None;
                    _dragStartPoint = pos;
                    return;
                }
            }

            if (Area.Rectangle != null)
            {
                AssociatedObject.Children.Remove(Area.Rectangle);
            }

            Area.Rectangle = new Rectangle
            {
                Stroke = Brushes.GhostWhite,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(80, 102, 255, 102))
            };

            _topLeftPoint = pos;

            Canvas.SetLeft(Area.Rectangle, _topLeftPoint.X);
            Canvas.SetTop(Area.Rectangle, _topLeftPoint.Y);

            AssociatedObject.Children.Add(Area.Rectangle);
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (Area.Rectangle == null || e.LeftButton == MouseButtonState.Released)
                return;

            var pos = e.GetPosition(AssociatedObject);

            if (_isDragging)
            {
                if (_resizeDirection != ResizeDirection.None)
                {
                    ResizeRectangle(pos);
                    UpdateCoordinates();
                    return;
                }
                else
                {
                    var offsetX = pos.X - _dragStartPoint.X;
                    var offsetY = pos.Y - _dragStartPoint.Y;

                    Canvas.SetLeft(Area.Rectangle, Canvas.GetLeft(Area.Rectangle) + offsetX);
                    Canvas.SetTop(Area.Rectangle, Canvas.GetTop(Area.Rectangle) + offsetY);

                    _dragStartPoint = pos;
                    UpdateCoordinates();
                    return;
                }
            }

            var x = Math.Min(pos.X, _topLeftPoint.X);
            var y = Math.Min(pos.Y, _topLeftPoint.Y);

            var w = Math.Max(pos.X, _topLeftPoint.X) - x;
            var h = Math.Max(pos.Y, _topLeftPoint.Y) - y;

            Area.Rectangle.Width = w;
            Area.Rectangle.Height = h;

            Canvas.SetLeft(Area.Rectangle, x);
            Canvas.SetTop(Area.Rectangle, y);
            UpdateCoordinates();
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _resizeDirection = ResizeDirection.None;
                return;
            }

            if (Area.Rectangle != null)
            {
                _bottomRightPoint = e.GetPosition(AssociatedObject);

                var imgTopLeft = _topLeftPoint;
                var imgBottomRight = _bottomRightPoint;

                ViewModel?.CalculateCoordinates((int)AssociatedObject.ActualWidth, (int)AssociatedObject.ActualHeight, imgTopLeft, imgBottomRight);
            }
        }

        private void UpdateCoordinates()
        {
            if (Area.Rectangle == null)
                return;

            var left = Canvas.GetLeft(Area.Rectangle);
            var top = Canvas.GetTop(Area.Rectangle);
            var right = left + Area.Rectangle.Width;
            var bottom = top + Area.Rectangle.Height;

            var imgTopLeft = new Point(left, top);
            var imgBottomRight = new Point(right, bottom);

      
            ViewModel?.CalculateCoordinates((int)AssociatedObject.ActualWidth, (int)AssociatedObject.ActualHeight, imgTopLeft, imgBottomRight);
        }

        private void ResizeRectangle(Point pos)
        {
            if (Area.Rectangle == null)
                return;

            var left = Canvas.GetLeft(Area.Rectangle);
            var top = Canvas.GetTop(Area.Rectangle);
            var right = left + Area.Rectangle.Width;
            var bottom = top + Area.Rectangle.Height;

            switch (_resizeDirection)
            {
                case ResizeDirection.TopLeft:
                    left = pos.X;
                    top = pos.Y;
                    break;
                case ResizeDirection.TopRight:
                    right = pos.X;
                    top = pos.Y;
                    break;
                case ResizeDirection.BottomLeft:
                    left = pos.X;
                    bottom = pos.Y;
                    break;
                case ResizeDirection.BottomRight:
                    right = pos.X;
                    bottom = pos.Y;
                    break;
            }

            Area.Rectangle.Width = Math.Max(0, right - left);
            Area.Rectangle.Height = Math.Max(0, bottom - top);

            Canvas.SetLeft(Area.Rectangle, left);
            Canvas.SetTop(Area.Rectangle, top);
        }

        private static bool IsOnResizeHandle(Point pos, out ResizeDirection direction)
        {
            if (Area.Rectangle is null)
            {
                direction = ResizeDirection.None;
                return false;
            }

            const double handleSize = 10.0;
            direction = ResizeDirection.None;

            var left = Canvas.GetLeft(Area.Rectangle);
            var top = Canvas.GetTop(Area.Rectangle);
            var right = left + Area.Rectangle.Width;
            var bottom = top + Area.Rectangle.Height;

            if (IsNear(pos, new Point(left, top), handleSize))
            {
                direction = ResizeDirection.TopLeft;
                return true;
            }
            if (IsNear(pos, new Point(right, top), handleSize))
            {
                direction = ResizeDirection.TopRight;
                return true;
            }
            if (IsNear(pos, new Point(left, bottom), handleSize))
            {
                direction = ResizeDirection.BottomLeft;
                return true;
            }
            if (IsNear(pos, new Point(right, bottom), handleSize))
            {
                direction = ResizeDirection.BottomRight;
                return true;
            }

            return false;
        }

        private static bool IsNear(Point p1, Point p2, double distance)
            => Math.Abs(p1.X - p2.X) <= distance && Math.Abs(p1.Y - p2.Y) <= distance;

    }
}
