using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace MauiApp1;

public partial class ItemStatusUpdate : ContentPage
{
    public Item Data { get; private set; }

    public ItemStatusUpdate(Item data)
    {
        InitializeComponent();

        Data = data ?? throw new ArgumentNullException(nameof(data));

        // Создаем Picker и добавляем в него элементы
        var statePicker = new Picker { Title = "Выберите новое состояние" };
        statePicker.Items.Add("В запасе");
        statePicker.Items.Add("В эксплуатации");
        statePicker.Items.Add("Требуется ремонт");
        statePicker.Items.Add("Сломан");
        statePicker.Items.Add("Требует списания");

        // Устанавливаем начальное значение Picker на основе текущего состояния объекта Data
        int currentStateIndex = statePicker.Items.IndexOf(Data.State);
        statePicker.SelectedIndex = currentStateIndex != -1 ? currentStateIndex : 0;

        // Добавляем Picker в VerticalStackLayout
        newInfoState.Children.Add(statePicker);

        var labelState = new Label { Text = Data.State ?? "Неизвестно" };
        infoState.Children.Add(labelState);
    }

    private async void OnAddToDatabaseClicked(object sender, EventArgs e)
    {
        var statePicker = (Picker)newInfoState.Children.FirstOrDefault(c => c is Picker);
        string state = statePicker.Items[statePicker.SelectedIndex];

        // Создаем JSON с ключом newState
        var json = JsonSerializer.Serialize(new { newState = state });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Отправка данных на сервер
        using (var client = new HttpClient())
        {
            
#if ANDROID
        var response = await client.PutAsync("http://10.0.2.2:5283/api/Items/update/state/" + Data.Id, content);
#endif

#if WINDOWS
            var response = await client.PutAsync("https://localhost:7194/api/Items/update/state/" + Data.Id, content);
#endif
            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Успех", "Состояние элемента успешно обновлено.", "OK");
            }
            else
            {
                await DisplayAlert("Ошибка", "Произошла ошибка при обновлении состояния элемента.", "OK");
            }
        }
    }


}