using MySqlConnector;
using System.Data.Common;
using System.Text.Json;



namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public MainPage()
        {
            InitializeComponent();
            DisplayItems();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            RefreshItemsGrid();
            //await LoadGreetingAsync();
        }

        //private async Task LoadGreetingAsync()
        //{
        //    var response = await _httpClient.GetAsync("http://10.0.2.2:7194/api/greeting");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var greeting = await response.Content.ReadAsStringAsync();
        //        GreetingLabel.Text = greeting;
        //    }
           
        //}


        private async void OnConnectButtonClicked(object sender, EventArgs e)
        {
            RefreshItemsGrid();
        }

        private async void OnScanButtonClicked(object sender, EventArgs e)
        {
            var CameraPage = new CameraPage();
            await Navigation.PushAsync(CameraPage);
        }

        private async void DisplayItems() 
        {
#if ANDROID
            var items = await GetItemsFromApiAsync("http://10.0.2.2:5283/api/items");
#endif

#if WINDOWS
            var items = await GetItemsFromApiAsync("https://localhost:7194/api/items");
#endif
            if (items != null)
            {

                int row = 2;
                foreach (var item in items)
                {

                    itemsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    itemsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    itemsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });


                    var buttonInfo = new Button { Text = "Подробнее" };
                    Grid.SetRow(buttonInfo, row);
                    Grid.SetColumn(buttonInfo, 0);
                    itemsGrid.Children.Add(buttonInfo);

                    buttonInfo.Clicked += async (s, e) =>
                    {
                        var itemPage = new ItemPage(item);
                        await Navigation.PushAsync(itemPage);
                    };


                    var idLabel = new Label { Text = item.Id.ToString(), VerticalTextAlignment = TextAlignment.Center };
                    Grid.SetRow(idLabel, row);
                    Grid.SetColumn(idLabel, 1);
                    itemsGrid.Children.Add(idLabel);

                    var nameLabel = new Label { Text = item.Name ?? "Недоступно", VerticalTextAlignment = TextAlignment.Center };
                    Grid.SetRow(nameLabel, row);
                    Grid.SetColumn(nameLabel, 2);
                    itemsGrid.Children.Add(nameLabel);

                    var nameTypeLabel = new Label { Text = item.NameType ?? "Недоступно", VerticalTextAlignment = TextAlignment.Center };
                    Grid.SetRow(nameTypeLabel, row);
                    Grid.SetColumn(nameTypeLabel, 3);
                    itemsGrid.Children.Add(nameTypeLabel);

                    var quantityLabel = new Label { Text = item.Quantity.ToString(), VerticalTextAlignment = TextAlignment.Center };
                    Grid.SetRow(quantityLabel, row);
                    Grid.SetColumn(quantityLabel, 4);
                    itemsGrid.Children.Add(quantityLabel);

                    var priceLabel = new Label { Text = item.Price.ToString("F2"), VerticalTextAlignment = TextAlignment.Center };
                    Grid.SetRow(priceLabel, row);
                    Grid.SetColumn(priceLabel, 5);
                    itemsGrid.Children.Add(priceLabel);

                    var stateLabel = new Label { Text = item.State ?? "Неизвестно", VerticalTextAlignment = TextAlignment.Center };
                    Grid.SetRow(stateLabel, row);
                    Grid.SetColumn(stateLabel, 6);
                    itemsGrid.Children.Add(stateLabel);

                    var ClassNameLabel = new Label { Text = item.ClassName ?? "Неизвестно", VerticalTextAlignment = TextAlignment.Center };
                    Grid.SetRow(ClassNameLabel, row);
                    Grid.SetColumn(ClassNameLabel, 7);
                    itemsGrid.Children.Add(ClassNameLabel);

                    var inventoryCodeLabel = new Label { Text = item.InventoryCode.ToString(), VerticalTextAlignment = TextAlignment.Center };
                    Grid.SetRow(inventoryCodeLabel, row);
                    Grid.SetColumn(inventoryCodeLabel, 8);
                    itemsGrid.Children.Add(inventoryCodeLabel);

                    var factoryCodeLabel = new Label { Text = item.FactoryCode.ToString(), VerticalTextAlignment = TextAlignment.Center };
                    Grid.SetRow(factoryCodeLabel, row);
                    Grid.SetColumn(factoryCodeLabel, 9);
                    itemsGrid.Children.Add(factoryCodeLabel);

                    row++; // Переход к следующей строке для дивайдера

                    var divider = new BoxView
                    {
                        HeightRequest = 1,
                        BackgroundColor = Color.FromArgb("#D3D3D3"),
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    Grid.SetRow(divider, row);
                    Grid.SetColumnSpan(divider, 10);
                    itemsGrid.Children.Add(divider);

                    row++; // Переход к следующей строке для следующего элемента данных

                }


            }

        }


        public async Task<IEnumerable<Item>> GetItemsFromApiAsync(string apiUrl)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<IEnumerable<Item>>(json);
                }
                else
                {
                    // Обработка ошибок
                    return null;
                }

            }
        }

        private void RefreshItemsGrid()
        {
            // Удаляем все строки с данными предметов, кроме заголовков
            for (int i = itemsGrid.Children.Count - 1; i >= 1; i--)
            {
                var child = itemsGrid.Children[i] as View;
                if (child != null && Grid.GetRow(child) > 1)
                {
                    itemsGrid.Children.RemoveAt(i);
                }
            }

            // Очищаем определения строк, кроме первой для заголовков
            itemsGrid.RowDefinitions.Clear();
            itemsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Пересоздаем список предметов
            DisplayItems();
           
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {

            var newPage = new AddPage();
            await Navigation.PushAsync(newPage);

        }


    }

}
