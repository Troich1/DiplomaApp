namespace MauiApp1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Установка светлой темы по умолчанию
            Application.Current.UserAppTheme = AppTheme.Light;

            MainPage = new AppShell();
        }
    }
}
