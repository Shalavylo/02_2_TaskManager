using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _02_2_TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer _timer = null;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += _timer_Tick;
            _timer.Start();
            Refresh(null, null);
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            try
            {
                grid.ItemsSource = Process.GetProcesses();
                text.Text = "Processes update: " + DateTime.Now.ToLongTimeString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading processes:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void _timer_Tick(object? sender, EventArgs e)
        {
            Refresh(sender, null);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            int seconds = int.Parse(radioButton.Content.ToString());
            _timer.Interval = TimeSpan.FromSeconds(seconds);
            text.Text = "Update interval: " + seconds + " seconds";
        }

        private void Kill_Process(object sender, RoutedEventArgs e)
        {
            Process process = grid.SelectedItem as Process;
            if(process == null)
            {
                MessageBox.Show("Select a process first!", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                string processName = process.ProcessName;
                int processId = process.Id;

                MessageBoxResult result = MessageBox.Show(
                    $"Do you really want to terminate process?\n\n" +
                    $"Name: {processName}\n" +
                    $"PID:  {processId}",
                    "Kill Process",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if(result == MessageBoxResult.Yes)
                {
                    process.Kill();
                    process.WaitForExit();
                    text.Text = $"Process {processName} terminate";
                    Refresh(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot terminate this process.\n\n" +
                    ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Stop_timer(object sender, RoutedEventArgs e)
        {
            if(_timer.IsEnabled)
            {
                _timer.Stop();
                text.Text = "Automatic update stopped";
            }
            else
            {
                _timer.Stop();
                text.Text = "Automatic update started";
            }
        }

        private void Show_Details(object sender, RoutedEventArgs e)
        {
            Process process = grid.SelectedItem as Process;

            if(process == null)
            {
                MessageBox.Show("Select a process first!",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                string startTime;
                try
                {
                    startTime = process.StartTime.ToString();
                }
                catch
                {

                    startTime = "Access denied";
                }

                string priority;
                try
                {
                    priority = process.PriorityClass.ToString();
                }
                catch
                {
                    priority = "Access denied";
                }

                string cpuTime;
                try
                {
                    cpuTime = process.TotalProcessorTime.ToString();
                }
                catch
                {
                    cpuTime = "Access denied";
                }

                string details = $"Process Name: {process.ProcessName}\n" +
                    $"PID: {process.Id}" +
                    $"Priority: {priority}" +
                    $"Start Time: {startTime}" +
                    $"Total Process Time: {cpuTime}" +
                    $"Machine Name: {process.MachineName}";

                MessageBox.Show(details, "Process Details",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot get process information.\n\n" +
                    ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Start_Process(object sender, RoutedEventArgs e)
        {
            string path = input.Text.Trim();
            if(string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Enter path to .exe file!",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });

                text.Text = "Process started: " + path;
                Refresh(null, null);
            } 
            catch (Exception ex)
            {
                MessageBox.Show("Cannot start process.\n\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}