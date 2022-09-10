using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using XP3;

namespace XP3PackAvalonia
{
    public partial class MainWindow : Window
    {
        private string Inpath = default!;
        private string Outpath = default!;
        public MainWindow()
        {
            InitializeComponent();

            Browse_In.Click += Browse_inputFolder;
            Browse_Out.Click += Browse_outputFolder;
            Run_button.Click += Run_start;
        }
        private async void Browse_inputFolder(object? sender, RoutedEventArgs e)
        {
            OpenFolderDialog fdlg = new()
            {
                Title = "Browse Input Folder"
            };

            var result = await fdlg.ShowAsync(this);

            if (result != null)
            {
                Inpath = result;
                Input_text.Text = result;
                Outpath = result + ".xp3";
                Output_Text.Text = Outpath;
                Run_button.IsEnabled = true;
            }
        }
        private async void Browse_outputFolder(object? sender, RoutedEventArgs e)
        {
            SaveFileDialog fdlg = new()
            {
                Title = "Browse Output File",
            };
            fdlg.Filters?.Add(new FileDialogFilter() { Name = "xp3 files", Extensions = { "xp3" } });

            var result = await fdlg.ShowAsync(this);

            if (result != null)
            {
                Outpath = result;
                Output_Text.Text = result;
            }
        }
        private void Run_start(object? sender, RoutedEventArgs e)
        {
            Status_text.Text = "Packing files";
            Task PCK = Task.Factory.StartNew(Packing);
            PCK.GetAwaiter().OnCompleted(() =>
            {
                Status_text.Text = "Done";
            });
        }
        private void Packing()
        {
            XP3ArchiveWriter.Write(Inpath, Outpath);
        }
    }
}
