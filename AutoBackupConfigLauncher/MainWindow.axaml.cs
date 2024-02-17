using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace AutoBackupConfigLauncher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeConfig();
    }

    private void InitializeConfig()
    {
        ClearView();
        
        string configPath = CheckForConfig();
        if (!string.IsNullOrEmpty(configPath))
        {
            ButtonPanel.Children.RemoveAll(ButtonPanel.Children);

            LoadButtons();
            LoadKeyValuePairs(configPath);
        }
        else
        {
            // Handle the case where no config path could be determined
            // MessageBox.Show("Please select a configuration file.");
            Button setConfigButton = new();
            setConfigButton.Name = "setConfigButton";
            setConfigButton.Content = "Configuration To Edit";
            setConfigButton.Click += OpenFileButton_Clicked;
            
            ButtonPanel.Children.Add(setConfigButton);
        }
    }

    private void ClearView()
    {
        StackPanelKey.Children.RemoveAll(StackPanelKey.Children);
        StackPanelValue.Children.RemoveAll(StackPanelValue.Children);
    }

    private string CheckForConfig()
    {
        return File.ReadAllText("ScriptConfigurationPath");
    }
    
    private void LoadKeyValuePairs(string configPath)
    {
    //     StackPanel keyPanel = StackPanelKey;
    //     StackPanel valuePanel = StackPanelValue;

        ConfigReader reader = new(configPath);
        Dictionary<string, object> configAsDictionary = reader.ConfigurationDictionary;
        
        DisplayConfig(configAsDictionary);
    }

    private void LoadButtons()
    {
        // setConfigButton
        Button setConfigButton = new();
        setConfigButton.Name = "setConfigButton";
        setConfigButton.Content = "Load";
        setConfigButton.Click += OpenFileButton_Clicked;
        
        // editConfig
        Button editConfigButton = new();
        editConfigButton.Name = "editConfigButton";
        editConfigButton.Content = "Edit";
        editConfigButton.Click += EditConfigButton_Clicked;
        
        // applyConfigButton
        Button applyConfigButton = new();
        applyConfigButton.Name = "applyConfigButton";
        applyConfigButton.Content = "Apply";
        applyConfigButton.Click += ApplyConfigButton_Clicked;
        
        // closeAppButton
        Button closeAppButton = new();
        closeAppButton.Name = "closeAppButton";
        closeAppButton.Content = "OK";
        closeAppButton.Click += OkButton_Clicked;
        
        // Loading the buttons
        ButtonPanel.Children.Add(setConfigButton);
        ButtonPanel.Children.Add(editConfigButton);
        ButtonPanel.Children.Add(applyConfigButton);
        ButtonPanel.Children.Add(closeAppButton);
    }

    private void DisplayConfig(Dictionary<string, object> configAsDictionary)
    {
        foreach (KeyValuePair<string, object> keyValuePair in configAsDictionary)
        {
            // Key: string
            TextBlock keyLabel = new();
            keyLabel.Text = keyValuePair.Key;
            
            StackPanelKey.Children.Add(keyLabel);
            
            // Value: object
            TextBlock valueLabel = new();
            valueLabel.Text = keyValuePair.Value.ToString();
            
            StackPanelValue.Children.Add(valueLabel);
        }
    }
    
    private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        TopLevel topLevel = GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        string configPath = File.ReadAllText("ScriptConfigurationPath");
        if (files.Count == 1)
        {
            configPath = files[0].Path.AbsolutePath;
        }
        
        File.WriteAllText("ScriptConfigurationPath", configPath);
        InitializeConfig();
    }

    private async void EditConfigButton_Clicked(object sender, RoutedEventArgs args)
    {
        List<TextBox> boxes = new();
        List<Control> labels = new();
        
        // Turn text labels into text boxes
        foreach (var valueLabel in StackPanelValue.Children)
        {
            if (valueLabel.GetType() == typeof(TextBlock))
            {
                // Make new textbox
                TextBox valueBox = new();
                valueBox.Name = valueLabel.Name;
                valueBox.Text = valueLabel.GetValue(TextBlock.TextProperty);

                boxes.Add(valueBox);
                labels.Add(valueLabel);
            }
        }

        foreach (var element in labels)
        {
            StackPanelValue.Children.Remove(element);
        }
        
        foreach (TextBox box in boxes)
        {
            // Add the box
            StackPanelValue.Children.Add(box);
        }
    }
    
    private async void ApplyConfigButton_Clicked(object sender, RoutedEventArgs args)
    {
        // Read from the text labels and apply to the toml
        Dictionary<string, string> newConfigValues = new();
        
        List<TextBlock> labels = new();
        List<Control> boxes = new();

        // Turn text labels into text boxes
        foreach (var box in StackPanelValue.Children)
        {
            if (box.GetType() == typeof(TextBox))
            {
                // Make new label
                TextBlock valueLabel = new();
                valueLabel.Name = box.Name;
                valueLabel.Text = box.GetValue(TextBlock.TextProperty);

                labels.Add(valueLabel);
                boxes.Add(box);
            }
        }

        foreach (var element in boxes)
        {
            StackPanelValue.Children.Remove(element);
        }

        foreach (TextBlock label in labels)
        {
            // Add the box
            StackPanelValue.Children.Add(label);
        }
        
        // TODO: Send new config data to TOML
        
    }
    
    private async void OkButton_Clicked(object sender, RoutedEventArgs args)
    {
        Close();
    }
}