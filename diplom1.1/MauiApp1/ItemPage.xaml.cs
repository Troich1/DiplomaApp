using ZXing;
using ZXing.Net.Maui;
using ZXing.QrCode;
using SkiaSharp;
using ZXing.Windows.Compatibility;
using ZXing.Common;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using System.IO;
using QRCoder;


namespace MauiApp1;

public partial class ItemPage : ContentPage
{
    public Item Data { get; private set; }

    public ItemPage(Item data)
    {
        InitializeComponent();
        Title = "Подробнее";

    
        Data = data ?? throw new ArgumentNullException(nameof(data));
        BindingContext = Data;
 
        Info();

        qrCodeImage.Source = GenerateQRCodeImageSource(Data.Id.ToString());


    }

    void Info()
    {

        var labelId = new Label { Text = Data.Id.ToString() };
        infoId.Children.Add(labelId);

        var labelName = new Label { Text = Data.Name ?? "Недоступно" };
        infoName.Children.Add(labelName);

        var labelNameType = new Label { Text = Data.NameType ?? "Недоступно" };
        infoNameType.Children.Add(labelNameType);

        var labelQuantity = new Label { Text = Data.Quantity.ToString() };
        infoQuantity.Children.Add(labelQuantity);

        var labelPrice = new Label { Text = Data.Price.ToString("F2") };
        infoPrice.Children.Add(labelPrice);

        var labelState = new Label { Text = Data.State ?? "Неизвестно" };
        infoState.Children.Add(labelState);

        var ClassNameLabel = new Label { Text = Data.ClassName ?? "Неизвестно" };
        infoClassName.Children.Add(ClassNameLabel);

        var labelInventoryCode = new Label { Text = Data.InventoryCode.ToString() };
        infoInventoryCode.Children.Add(labelInventoryCode);

        var labelFactoryCode = new Label { Text = Data.FactoryCode.ToString() };
        infoFactoryCode.Children.Add(labelFactoryCode);

        var labelIdClass = new Label { Text = Data.IdClass.ToString() };
        infoIdClass.Children.Add(labelIdClass);

        //var labelIdType = new Label { Text = Data.IdType.ToString() };
        //infoIdType.Children.Add(labelIdType);
    }

    private async void OnCopyButtonClicked(object sender, EventArgs e)
    {
        await SaveAndCopyQRCodeToClipboard(Data.Id.ToString());
    }

    private async void OnUpdateButtonClicked(object sender, EventArgs e)
    {        
            var newPage = new ItemStatusUpdate(Data);
            await Navigation.PushAsync(newPage);
                
    }


    private async Task SaveAndCopyQRCodeToClipboard(string qrCodeText)
    {
        // Генерация QR-кода
        var qrCodeImageSource = GenerateQRCodeImageSource(qrCodeText);
        var stream = await ((StreamImageSource)qrCodeImageSource).Stream(CancellationToken.None);

        // Использование MemoryStream для создания байтового массива из потока
        byte[] qrCodeBytes;
        using (var memoryStream = new MemoryStream())
        {
            await stream.CopyToAsync(memoryStream);
            qrCodeBytes = memoryStream.ToArray();
        }

        // Сохранение изображения в файл
        var folderPath = FileSystem.Current.CacheDirectory;
        var filePath = Path.Combine(folderPath, "qr_code.png");
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.WriteAsync(qrCodeBytes, 0, qrCodeBytes.Length);
        }

        // Копирование пути к файлу в буфер обмена
        //await Clipboard.SetTextAsync(filePath);

        // Показать уведомление об успешном копировании
        //await DisplayAlert("Успешно", "Путь к файлу QR-кода скопирован в буфер обмена.", "OK");

        // Добавленный код для обмена QR-кодом
        string fileName = "shared-qr-code.png";
        string fullPath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllBytesAsync(fullPath, qrCodeBytes);

        // Показать опции обмена на каждой платформе
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Сгенерированный QR-код",
            File = new ShareFile(fullPath)
        });
    }

    public ImageSource GenerateQRCodeImageSource(string content)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
        PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qRCode.GetGraphic(20);

        // Преобразование в ImageSource
        return ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
    }

  



}

