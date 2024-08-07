using System.Windows;


namespace TextureCoordsCalculatorGUI.Views
{
    /// <summary>
    /// Logique d'interaction pour InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public int FileDataId { get; private set; } = 0;
        public InputDialog()
        {
            InitializeComponent();
        }

        private void OnButtonConfirmClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(FidTextBox.Text, out int value))
            {
                FileDataId = value;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
