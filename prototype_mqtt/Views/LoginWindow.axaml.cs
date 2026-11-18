using Avalonia.Controls;
using prototype_mqtt.Models;
using prototype_mqtt.ViewModels;

namespace prototype_mqtt.Views;

public partial class LoginWindow : Window
{
    public DatabaseConfig? AuthenticatedConfig { get; private set; }

    public LoginWindow()
    {
        InitializeComponent();
        var vm = new DatabaseConfigViewModel();
        
        // Khi kiểm tra kết nối DB thành công -> Lưu cấu hình và đóng LoginWindow
        vm.OnLoginSuccess += config =>
        {
            AuthenticatedConfig = config;
            Close();
        };
        
        DataContext = vm;
    }
}