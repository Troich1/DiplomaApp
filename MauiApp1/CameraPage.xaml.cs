
using Microsoft.Maui.Controls;
using MySqlConnector;
using System.Diagnostics;
using System.Text.Json;
using ZXing.Net.Maui.Controls;
using ZXing.QrCode.Internal;

namespace MauiApp1;

public partial class CameraPage : ContentPage
{

    public CameraPage()
    {
        InitializeComponent();

        barcodeReader.IsDetecting = true;



        barcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {

            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = true,

        };

        

    }
    
    private void barcodeReader_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        
        var first = e.Results.FirstOrDefault();
        if (first == null) return;

        barcodeReader.IsDetecting = false;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert("QR-код отсканирован", first.Value, "OK");


            // Получаем ID предмета из QR-кода
            int itemId = int.Parse(first.Value); // Убедитесь, что QR-код содержит только ID

            // Вызываем API для получения информации о предмете
            var item = await GetItemFromApi(itemId);

            if (item != null)
            {
                // Открываем ItemPage и передаем ей информацию о предмете
                await Navigation.PushAsync(new ItemPage(item));
            }
            else
            {
                await DisplayAlert("Ошибка", "Предмет не найден.", "OK");
            }

            await Task.Delay(5000);

            barcodeReader.IsDetecting = true;
        });
    }

    // Этот метод вызывает API и возвращает информацию о предмете
    private async Task<Item> GetItemFromApi(int itemId)
    {


        var httpClient = new HttpClient();
#if ANDROID

        var response = await httpClient.GetAsync($"http://10.0.2.2:5283/api/Items/{itemId}");
#endif

#if WINDOWS
                var response = await httpClient.GetAsync($"https://localhost:7194/api/Items/{itemId}");
#endif
        //var response = await httpClient.GetAsync($"https://localhost:7194/api/Items/{itemId}");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<Item>(json);
            return item;
        }

        return null;
    }
}



     