using System.Windows;

namespace TextureCoordsCalculatorGUI.Misc
{
    internal sealed record Coordinates(float Left, float Top, float Right, float Bottom);

    internal sealed class TexCoordinatesCalculator(int imageWidth, int imageHeight, Point topLeftPixels, Point bottomRightPixels)
    {
        private readonly float _magicNumberX = (1.0f / imageWidth) / 2.0f;
        
        private readonly float _magicNumberY = (1.0f / imageHeight) / 2.0f;
        
        private readonly Point _topLeftPixels = topLeftPixels;
        
        private readonly Point _bottomRightPixels = bottomRightPixels;
        

        private Coordinates CalculateCoordinates()
        {
            float left = (float)((_topLeftPixels.X / ImagePixelSize.Width) + _magicNumberX);

            float top = (float)(_topLeftPixels.Y / ImagePixelSize.Height) + _magicNumberY;

            float right = (float)(_bottomRightPixels.X / ImagePixelSize.Width) - _magicNumberX;

            float bottom = (float)(_bottomRightPixels.Y / ImagePixelSize.Height) - _magicNumberY;

            return new(left, top, right, bottom);
        }


        public Coordinates? TextureCoordinates => CalculateCoordinates();
        public (int Width, int Height) ImagePixelSize => (imageWidth, imageHeight);
    }
}
