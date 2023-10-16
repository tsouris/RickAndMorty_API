using MaterialDesignThemes.Wpf;
using RickAndMorty_API_CORE.Data.API;
using RickAndMorty_API_CORE.Domain.Models;
using RickAndMorty_API_CORE.Domain.ProviderJson;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RickAndMorty_API
{
    public partial class MainWindow : Window
    {
        private RAM_API_ENGINE engine { set; get; }
        private List<Character> character { get; set; }
        private int page = 1;
        private int totalPages = 42;
        public MainWindow()
        {
            InitializeComponent();
            engine = new RAM_API_ENGINE();
        }

        private async void UpdatePage()
        {
            tbCount.Text = page.ToString();
            LV.ItemsSource = null;
            var pageResult = engine.GetPage(this.page);
            character = JsonProvider.FromJsonToCharacterList(pageResult);
            LV.ItemsSource = character;
            LV.Items.Refresh();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePage();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if(page != 1)
            {
                page--;
                UpdatePage();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            page++;
            UpdatePage();
        }

        private void Image_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Character selectedCharacter = (Character)((FrameworkElement)sender).DataContext;
            InfoCard.DataContext = selectedCharacter;

            labelInfo.Visibility = Visibility.Visible;
            InfoCard.Visibility = Visibility.Visible;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            InfoCard.Visibility=Visibility.Collapsed;
        }

        private void tbFilter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string filterText = tbFilter.Text.ToLower();
                List<Character> filteredCharacters = new List<Character>();

                for (int i = 1; i <= totalPages; i++)
                {
                    var pageResult = engine.GetPage(i);
                    List<Character> charactersOnPage = JsonProvider.FromJsonToCharacterList(pageResult);
                    filteredCharacters.AddRange(charactersOnPage.Where(c => c.Name.ToLower().Contains(filterText)));
                }

                LV.ItemsSource = filteredCharacters;
                LV.Items.Refresh();
            }
        }

        private void tbPageFilter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string filterText = tbPageFilter.Text;

                if (int.TryParse(filterText, out int pageNumber))
                {
                    if (pageNumber >= 1 && pageNumber <= totalPages)
                    {
                        var pageResult = engine.GetPage(pageNumber);
                        List<Character> charactersOnPage = JsonProvider.FromJsonToCharacterList(pageResult);
                        LV.ItemsSource = charactersOnPage;
                        LV.Items.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid page number.");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid number.");
                }
            }

            tbCount.Text = tbPageFilter.Text;
        }
    }
}
