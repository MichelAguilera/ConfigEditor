using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Tomlyn;

namespace AutoBackupConfigLauncher;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Added code:
        InitialSetup();
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    
    // Added code:
    public void InitialSetup()
    {
        if (!File.Exists("ScriptConfigurationPath"))
        {
            File.WriteAllText(path: "ScriptConfigurationPath", contents: string.Empty);
        }
    }
}