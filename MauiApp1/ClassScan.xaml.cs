using System.Diagnostics;
using ZXing.Net.Maui.Controls;
using ZXing.QrCode.Internal;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Text;

namespace MauiApp1;

public partial class ClassScan : ContentPage
{
    public int CurrentClassroomId { get; set; } // ID текущего кабинета
    public string CurrentClassroomName { get; set; } // Имя текущего кабинета
    public ClassroomPage ClassroomPageInstance { get; set; }

    public ClassScan()
    {
        InitializeComponent();

        barcodeReader.IsDetecting = true;

        barcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = true
        };
    }

    private void barcodeReader_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var first = e.Results.FirstOrDefault();
        if (first == null) return;

        // Остановка сканирования для предотвращения повторных срабатываний
        barcodeReader.IsDetecting = false;

        // Обработка результата сканирования в основном потоке UI
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            // Предполагаем, что first.Value содержит ID предмета
            await ProcessScannedData(first.Value);

            // Перезапускаем сканирование после задержки
            await Task.Delay(5000);
            barcodeReader.IsDetecting = true;
        });
    }

    private async Task ProcessScannedData(string data)
    {
        if (int.TryParse(data, out int itemId))
        {
            using (var httpClient = new HttpClient())
            {
#if ANDROID            
                var response = await httpClient.GetAsync($"http://10.0.2.2:5283/api/Items/check/{itemId}/{CurrentClassroomId}");
#endif

#if WINDOWS
                var response = await httpClient.GetAsync($"https://localhost:7194/api/Items/check/{itemId}/{CurrentClassroomId}"); ;
#endif
                //var response = await httpClient.GetAsync($"https://localhost:7194/api/Items/check/{itemId}/{CurrentClassroomId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var item = JsonSerializer.Deserialize<Item>(json);
                    if (item != null)
                    {
                        ClassroomPageInstance?.UpdateItemCheckStatus(itemId, true);
                        await DisplayAlert("Сообщение", "Предмет соответствует кабинету!", "OK");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    bool userResponse = await DisplayAlert("Обновление", "Предмет не найден или не принадлежит указанному кабинету. Обновить?", "Да", "Нет");
                    if (userResponse)
                    {
                        await UpdateItem(itemId, httpClient);
                    }
                }
            }
        }
        else
        {
            await DisplayAlert("Ошибка", "Неверный формат данных QR-кода.", "OK");
        }
    }

    private async Task UpdateItem(int itemId, HttpClient httpClient)
    {

        var updateContent = new StringContent(
                 JsonSerializer.Serialize(new
                 {
                     NewClassroomId = CurrentClassroomId,
                     NewClassName = CurrentClassroomName // Замените эту строку актуальным именем класса
                 }),
                 Encoding.UTF8,
                 "application/json"
        );

#if ANDROID
            
            var updateResult = await httpClient.PutAsync($"http://10.0.2.2:5283/api/Items/update/{itemId}", updateContent);
#endif

#if WINDOWS
        var updateResult = await httpClient.PutAsync($"https://localhost:7194/api/Items/update/{itemId}", updateContent);
#endif

        //var updateResult = await httpClient.PutAsync($"https://localhost:7194/api/items/update/{itemId}", updateContent);

        if (updateResult.IsSuccessStatusCode)
        {
            await HandleSuccessfulUpdate(updateResult);
        }
        else
        {
            await DisplayAlert("Ошибка", "Не удалось обновить информацию о предмете.", "OK");
        }
    }

    private async Task HandleSuccessfulUpdate(HttpResponseMessage updateResult)
    {
        var responseContent = await updateResult.Content.ReadAsStringAsync();
        var item = JsonSerializer.Deserialize<Item>(responseContent);
        if (item != null)
        {
            ClassroomPageInstance?.AddNewItemAndUpdate(item);
            await DisplayAlert("Успех", "Информация о предмете обновлена.", "OK");
        }
        else
        {
            await DisplayAlert("Ошибка", "Обновленный предмет не был получен.", "OK");
        }
    }


}
