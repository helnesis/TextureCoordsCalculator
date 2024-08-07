using SereniaBLPLib;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TextureCoordsCalculatorGUI.Misc;
using TextureCoordsCalculatorGUI.Services;
using TextureCoordsCalculatorGUI.Shared;

namespace TextureCoordsCalculatorGUI.Views
{
    /// <summary>
    /// Logique d'interaction pour TextureBrowserDialog.xaml
    /// </summary>
    public partial class TextureBrowserDialog : Window
    {
        private readonly FrozenDictionary<string, uint> _textures = Listfile.Instance.Textures;
        private readonly WagoService _wagoService;
        private string _selectedTexture = string.Empty;
        public string SelectedTexture
            => _selectedTexture;
        public TextureBrowserDialog(WagoService wagoService)
        {
            InitializeComponent();
            FillListView();
            _wagoService = wagoService;
        }


        private void FillListView()
        {
            if (_textures is not null)
            {
                AvailableFiles.ItemsSource = _textures.Keys;
            }
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            var tbx = sender as TextBox;

            if (tbx is not null && !string.IsNullOrEmpty(tbx.Text))
            {
                var filtered = _textures.Where(x => x.Key.Contains(tbx.Text)).Select(x => x.Key);
                AvailableFiles.ItemsSource = filtered;
            }
            else
            {
                AvailableFiles.ItemsSource = _textures.Keys;
            }
        }


        private async void AvailableFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;

            if (lv is not null)
            {
                var selectedItem = lv.SelectedItem as string;

                if (!string.IsNullOrEmpty(selectedItem))
                {
                    _selectedTexture = selectedItem;

                    var file = await _wagoService.GetCascFile(Listfile.Instance.GetFileDataId(_selectedTexture));

                    if (file is not null)
                    {
                        var blpFile = new BlpFile(file);
                        Preview.Source = Utilities.BitMapToImg(blpFile.GetBitmap(0));
                    }


                }
            }
        }
    }
}
