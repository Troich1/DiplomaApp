using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class AddPage : ContentPage
    {
        public AddPage()
        {
            InitializeComponent();
            LoadClassroomsAsync();

            // Создаем Picker для состояния и добавляем в него элементы
            var statePicker = new Picker { };
            statePicker.Items.Add("В запасе");
            statePicker.Items.Add("В эксплуатации");
            statePicker.Items.Add("Требуется ремонт");
            statePicker.Items.Add("Сломан");
            statePicker.Items.Add("Требует списания");
            statePicker.SelectedIndex = 0;
            infoState.Children.Add(statePicker);

            // Создаем Picker для типа имущества и добавляем в него элементы
            var typePicker = new Picker { };
            typePicker.Items.Add("Ценный");
            typePicker.Items.Add("Малоценный");
            typePicker.Items.Add("Расходник");
            typePicker.SelectedIndex = 0;
            nameTypePicker.Children.Add(typePicker);
        }

        private async void OnAddToDatabaseClicked(object sender, EventArgs e)
        {
            // Получение выбранных значений из Picker'ов
            var statePicker = infoState.Children.OfType<Picker>().FirstOrDefault();
            var typePicker = nameTypePicker.Children.OfType<Picker>().FirstOrDefault();
            var classroomPicker = classroomPickerLayout.Children.OfType<Picker>().FirstOrDefault();
            string state = statePicker.Items[statePicker.SelectedIndex];
            string nameType = typePicker.Items[typePicker.SelectedIndex];
            string className = classroomPicker.Items[classroomPicker.SelectedIndex]; 


            // Попытка преобразования введенных данных в соответствующие типы
            //bool success = int.TryParse(idEntry.Text, out int id);
           bool success = int.TryParse(quantityEntry.Text, out int quantity);
            success &= float.TryParse(priceEntry.Text, out float price);
            string inventoryCode = inventoryCodeEntry.Text;
            string factoryCode = factoryCodeEntry.Text;

            // Если преобразование не удалось, выводим сообщение об ошибке
            if (!success)
            {
                await DisplayAlert("Ошибка", "Пожалуйста, проверьте введенные данные.", "OK");
                return;
            }

            // Создание объекта МА
            var item = new Item
            {
                Name = nameEntry.Text,
                NameType = nameType,
                Quantity = quantity,
                Price = price,
                State = state,
                ClassName = className,
                InventoryCode = inventoryCode,
                FactoryCode = factoryCode,
            };

            // Отправка данных на сервер
            using (var client = new HttpClient())
            {
                var json = JsonSerializer.Serialize(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
#if ANDROID
                var response = await client.PostAsync("http://10.0.2.2:5283/api/Items", content);
#endif

#if WINDOWS
                var response = await client.PostAsync("https://localhost:7194/api/Items", content);
#endif
                //var response = await client.PostAsync("https://localhost:7194/api/Items", content);
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Успех", "Элемент успешно добавлен в базу данных.", "OK");
                }
                else
                {
                    await DisplayAlert("Ошибка", "Произошла ошибка при добавлении элемента.", "OK");
                }
            }

            // Возвращение на предыдущую страницу
            //await Navigation.PopAsync();
        }

        private async Task LoadClassroomsAsync()
        {
            var classrooms = await GetClassroomsFromApi();
            classroomPicker.Items.Clear(); // Очистка предыдущих элементов
            foreach (var classroom in classrooms)
            {
                classroomPicker.Items.Add(classroom.Name);
            }
            classroomPicker.SelectedIndex = 0; // Установка начального выбранного индекса
        }

        private async Task<List<Classroom>> GetClassroomsFromApi()
        {

#if ANDROID
                var url = "http://10.0.2.2:5283/api/Classroom";
#endif

#if WINDOWS
            var url = "https://localhost:7194/api/Classroom";
#endif

            //var url = "https://localhost:7194/api/Classroom";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Classroom>>(json);


            }

        }
    }
}




