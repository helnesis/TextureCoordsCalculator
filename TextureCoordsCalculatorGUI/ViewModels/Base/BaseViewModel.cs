using CommunityToolkit.Mvvm.ComponentModel;

namespace TextureCoordsCalculatorGUI.ViewModels.Base
{
    public abstract class BaseViewModel(string windowTitle) : ObservableObject
    {
        private readonly string _windowTitle = windowTitle;
        public string WindowTitle => _windowTitle;
    }
}
