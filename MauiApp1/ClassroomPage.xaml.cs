using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiApp1;



public partial class ClassroomPage : ContentPage
{

    private Dictionary<int, bool> itemCheckStatus = new Dictionary<int, bool>();
    public Classroom Data { get; private set; }
    
    public ClassroomPage(Classroom data)
    {
        InitializeComponent();

        Title = "Информация о кабинете";

        Data = data ?? throw new ArgumentNullException(nameof(data));
        BindingContext = Data;
        DisplayClassroomInfo();
        DisplayItemInfo();
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Обновляем UI, если есть изменения
        RefreshItemsGrid();
    }

    void DisplayClassroomInfo()
    { // Создание и добавление информации о кабинете в StackLayout
        //classroomInfoLayout.Children.Add(new Label { Text = "ID:", FontSize = 16 });
        //classroomInfoLayout.Children.Add(new Label { Text = Data.Id.ToString(), FontAttributes = FontAttributes.Bold, FontSize = 24 });

        infoId.Children.Add(new Label { Text = Data.Id.ToString(), FontAttributes = FontAttributes.Bold});
        infoName.Children.Add(new Label { Text = Data.Name ?? "Недоступно", FontAttributes = FontAttributes.Bold});
        infoNameType.Children.Add(new Label { Text = Data.Type ?? "Неопределен", FontAttributes = FontAttributes.Bold });

        //// Для Имени
        //classroomInfoLayout.Children.Add(new Label { Text = "Имя:",FontSize = 16 });
        //classroomInfoLayout.Children.Add(new Label { Text = Data.Name ?? "Недоступно", FontAttributes = FontAttributes.Bold, FontSize = 24 });

        //// Для Типа
        //classroomInfoLayout.Children.Add(new Label { Text = "Тип:", FontSize = 16 });
        //classroomInfoLayout.Children.Add(new Label { Text = Data.Type ?? "Неопределен", FontAttributes = FontAttributes.Bold, FontSize = 24 });

        //classroomInfoLayout.Children.Add(new Label { Text = $"ID кафедры: {Data.IdKafedra}", FontAttributes = FontAttributes.Bold, FontSize=24 });
    }

    void DisplayItemInfo()
    {
        
        int row = 2; // Начинаем с первой строки, так как нулевая занята заголовками
        foreach (var item in Data.Items)
        {

            itemsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            itemsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            itemsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });


            bool isChecked = itemCheckStatus.ContainsKey(item.Id) && itemCheckStatus[item.Id];
            var checkLabel = new Image { Source = "check.png", IsVisible = isChecked,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 24, // Ширина изображения
                HeightRequest = 24 // Высота изображения
            };

            Grid.SetRow(checkLabel, row);
            Grid.SetColumn(checkLabel, 0);
            itemsGrid.Children.Add(checkLabel);

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

    
    private async void InventoryButton_Clicked(object sender, EventArgs e)
    {
        // Ваш код для обработки нажатия кнопки остается без изменений
        var scanPage = new ClassScan
        {
            CurrentClassroomId = Data.Id, // Установка ID текущего кабинета
            CurrentClassroomName=Data.Name,
            ClassroomPageInstance = this  // Передача текущего экземпляра ClassroomPage
        };
        await Navigation.PushAsync(scanPage);
    }

    public void UpdateItemCheckStatus(int itemId, bool isChecked)
    {
        // Обновление статуса проверки в словаре
        itemCheckStatus[itemId] = isChecked;

        // Обновление UI
        RefreshItemsGrid();
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
        DisplayItemInfo();

        // Обновляем видимость галочек для всех элементов
        foreach (var item in Data.Items)
        {
            var checkLabel = itemsGrid.Children.OfType<Image>().FirstOrDefault(img => Grid.GetRow(img) == Data.Items.IndexOf(item) * 0 + 3);
            if (checkLabel != null)
            {
                checkLabel.IsVisible = itemCheckStatus.ContainsKey(item.Id) && itemCheckStatus[item.Id];
            }
        }
    }

    public void AddNewItemAndUpdate(Item newItem)
    {
        // Проверяем, что новый предмет не равен null и что его нет в списке
        if (newItem != null && !Data.Items.Any(item => item.Id == newItem.Id))
        {
            // Добавляем новый предмет в список
            Data.Items.Add(newItem);
            // Устанавливаем галочку на новом предмете
            UpdateItemCheckStatus(newItem.Id, true);
            // Обновляем интерфейс
            RefreshItemsGrid();
        }
    }

    public void RemoveItemAndUpdate(Item itemToRemove)
{
    // Проверяем, что предмет для удаления не равен null и что он есть в списке
    if (itemToRemove != null && Data.Items.Any(item => item.Id == itemToRemove.Id))
    {
        // Удаляем предмет из списка
        var item = Data.Items.FirstOrDefault(i => i.Id == itemToRemove.Id);
        if (item != null)
        {
            Data.Items.Remove(item);
            // Обновляем статус проверки предмета
            if (itemCheckStatus.ContainsKey(item.Id))
            {
                itemCheckStatus.Remove(item.Id);
            }
            // Обновляем интерфейс
            RefreshItemsGrid();
        }
    }
}
   

}

