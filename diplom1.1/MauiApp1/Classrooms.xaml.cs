using System.Text.Json;

namespace MauiApp1;

public partial class Classrooms : ContentPage
{
    public Classrooms()
    {
        InitializeComponent();
        DisplayClasses();

    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Обновляем UI, если есть изменения
        RefreshClassGrid();
    }

    private async void OnConnectButtonClicked(object sender, EventArgs e)
    {
        DisplayClasses();
    }

    private async void DisplayClasses()
    {
#if ANDROID
            var classrooms = await GetClassroomsFromApiAsync("http://10.0.2.2:5283/api/Classroom");
#endif

#if WINDOWS
        var classrooms = await GetClassroomsFromApiAsync("https://localhost:7194/api/Classroom");
#endif

        //var classrooms = await GetClassroomsFromApiAsync("https://localhost:7194/api/Classroom");

        if (classrooms != null)
        {

            int row = 2;
            foreach (var classroom in classrooms)
            {
                ClassGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                ClassGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                ClassGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });


                var buttonInfo = new Button { Text = "Подробнее" };
                Grid.SetRow(buttonInfo, row);
                Grid.SetColumn(buttonInfo, 0); // Кнопка в первом столбце
                ClassGrid.Children.Add(buttonInfo);
                buttonInfo.Clicked += async (s, e) =>
                {
                    var classroomPage = new ClassroomPage(classroom);
                    await Navigation.PushAsync(classroomPage);
                };



                var labelId = new Label { Text = classroom.Id.ToString(), VerticalTextAlignment = TextAlignment.Center };
                Grid.SetRow(labelId, row);
                Grid.SetColumn(labelId, 1);
                ClassGrid.Children.Add(labelId);

                var labelName = new Label { Text = classroom.Name, VerticalTextAlignment = TextAlignment.Center };
                Grid.SetRow(labelName, row);
                Grid.SetColumn(labelName, 2);
                ClassGrid.Children.Add(labelName);

                var labelType = new Label { Text = classroom.Type, VerticalTextAlignment = TextAlignment.Center };
                Grid.SetRow(labelType, row);
                Grid.SetColumn(labelType, 3);
                ClassGrid.Children.Add(labelType);


                row++; // Переход к следующей строке для дивайдера

                var divider = new BoxView
                {
                    HeightRequest = 1,
                    BackgroundColor = Color.FromArgb("#D3D3D3"),
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                Grid.SetRow(divider, row);
                Grid.SetColumnSpan(divider, 8);
                ClassGrid.Children.Add(divider);

                row++; // Переход к следующей строке для следующего элемента данных
            }
        }
    }

    public async Task<IEnumerable<Classroom>> GetClassroomsFromApiAsync(string apiUrl)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<Classroom>>(json);
            }
            else
            {
                // Обработка ошибок
                return null;
            }
        }
    }

    private void RefreshClassGrid()
    {
        // Удаляем все строки с данными предметов, кроме заголовков
        for (int i = ClassGrid.Children.Count - 1; i >= 1; i--)
        {
            var child = ClassGrid.Children[i] as View;
            if (child != null && Grid.GetRow(child) > 1)
            {
                ClassGrid.Children.RemoveAt(i);
            }
        }

        // Очищаем определения строк, кроме первой для заголовков
        ClassGrid.RowDefinitions.Clear();
        ClassGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Пересоздаем список предметов
        DisplayClasses();

    }
}