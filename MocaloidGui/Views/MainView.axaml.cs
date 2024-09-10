using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System.IO;
using Tmds.DBus.Protocol;

namespace MocaloidGui.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        ActionWorker.Error += ActionWorker_Error;
        ActionWorker.Started += ActionWorker_Started;
        ActionWorker.Completed += ActionWorker_Completed;
        UpdateUI();
    }

    private void ActionWorker_Completed(bool obj)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            UpdateUI();
            StatusTextBlock.Text = "Status:Done";
        });
    }

    private void ActionWorker_Started(string obj)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            StatusTextBlock.Text = "Status:Running -" + obj;
            StatusTextBlock.Background = Brushes.Blue;
            CleanButton.IsEnabled = false;
            BuildButton.IsEnabled = false;
        });
    }

    private void ActionWorker_Error(string obj)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            StatusTextBlock.Text = "Status:Error -" + obj;
            StatusTextBlock.Background = Brushes.Red;
        });
    }

    [System.Obsolete]
    private async void OnInputVBSelectButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Title = "Select the Vocaloid Daisy Database Index File";
        dialog.AllowMultiple = false;
        dialog.Filters.Add(new FileDialogFilter() { Extensions = new System.Collections.Generic.List<string>() { "ddi" }, Name = "Daisy Database Index File" });
        var result = await dialog.ShowAsync((Window)this.VisualRoot);
        if (result != null && result.Length > 0)
        {
            InputVBPath.Text = result[0];
            UpdateUI();
        }
    }

    [System.Obsolete]
    private async void OnOutputFolderSelectButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog();
        dialog.Title = "Select Mocaloid Output Directory";
        dialog.ToFolderPickerOpenOptions().AllowMultiple = false;
        var result = await dialog.ShowAsync((Window)this.VisualRoot);
        if (!string.IsNullOrEmpty(result))
        {
            OutputFolderPath.Text = result;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        CleanButton.IsEnabled = false;
        BuildButton.IsEnabled = false;
        string status = "Ready";
        if (InputVBPath.Text==null || !File.Exists(InputVBPath.Text)) { status = "Must Select a Vocaloid Voice Bank"; }
        else if (OutputFolderPath.Text == null || !Directory.Exists(OutputFolderPath.Text)) { status = "Must Select a Exists Folder for Output"; }
        else if (OutputFolderPath.Text != null && File.Exists(Path.Combine(OutputFolderPath.Text, "character.txt"))) { status = "Utau VoiceBank is Exists"; }
        else if (OutputFolderPath.Text != null && File.Exists(Path.Combine(OutputFolderPath.Text, "oto.ini"))) { status = "Utau VoiceBank is Exists"; }

        if (status == "Ready")
        {
            StatusTextBlock.Text = "Status:Ready";
            StatusTextBlock.Background = Brushes.Green;
            BuildButton.IsEnabled = true;
        }
        else if(status == "Utau VoiceBank is Exists")
        {
            StatusTextBlock.Text = "Status:"+status;
            StatusTextBlock.Background = Brushes.Yellow;
            CleanButton.IsEnabled = true;
            BuildButton.IsEnabled = true;
        }
        else
        {
            StatusTextBlock.Text = "Status:" + status;
            StatusTextBlock.Background = Brushes.Red;
        }
    }

    private void OnBuildButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ActionWorker.StartBuildMocaloid(InputVBPath.Text, OutputFolderPath.Text, IsGCM==null?false:(bool)IsGCM.IsChecked,selLang.SelectedIndex-1);
    }
    private void OnCleanButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ActionWorker.StartCleanMocaloid(OutputFolderPath.Text);
    }
}
