using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Naec_Checker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            } catch (System.InvalidOperationException)
            {
                
            }
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            var checker = new Checker(idInput.Text, this);
            MainButton.Content = "Stop";
            Thread t = new Thread(checker.ThreadTask);
            t.Start();
        }

        public void changeProgress(double num)
        {
            Progress.Text = string.Format("Progress {0}%", num);
        }
    }
}
