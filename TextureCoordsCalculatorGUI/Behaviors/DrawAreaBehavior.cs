using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TextureCoordsCalculatorGUI.ViewModels;

namespace TextureCoordsCalculatorGUI.Behaviors
{
    public class DrawAreaBehavior : Behavior<Canvas>
    {
        private Point _topLeftPoint;
        
        private Point _bottomRightPoint;
        
        private Rectangle? _area;

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
            if (_area != null)
                AssociatedObject.Children.Remove(_area);

            _topLeftPoint = e.GetPosition(AssociatedObject);

            _area = new Rectangle
            {
                Stroke = Brushes.GhostWhite,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(80, 102, 255, 102))
            };

            Canvas.SetLeft(_area, _topLeftPoint.X);
            Canvas.SetTop(_area, _topLeftPoint.Y);

            AssociatedObject.Children.Add(_area);
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || _area == null)
                return;

            var pos = e.GetPosition(AssociatedObject);

            var x = Math.Min(pos.X, _topLeftPoint.X);
            var y = Math.Min(pos.Y, _topLeftPoint.Y);

            var w = Math.Max(pos.X, _topLeftPoint.X) - x;
            var h = Math.Max(pos.Y, _topLeftPoint.Y) - y;

            _area.Width = w;
            _area.Height = h;

            Canvas.SetLeft(_area, x);
            Canvas.SetTop(_area, y);
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_area != null)
            {
                _bottomRightPoint = e.GetPosition(AssociatedObject);

                var imgTopLeft = _topLeftPoint;
                var imgBottomRight = _bottomRightPoint;

                ViewModel?.CalculateCoordinates((int)AssociatedObject.ActualWidth, (int)AssociatedObject.ActualHeight, imgTopLeft, imgBottomRight);
            }
        }



    }
}
