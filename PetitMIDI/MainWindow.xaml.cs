namespace PetitMIDI
{
    using PetitMIDI.MML;
    using System.Threading;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MMLPlayer petitMML;
        private Thread mmlThread;

        public MainWindow()
        {
            InitializeComponent();
            petitMML = new MMLPlayer();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (mmlThread != null)
            {
                mmlThread.Abort();
            }
            petitMML.Close();
        }

        private void StartInterpret(string mml)
        {
            if (mmlThread != null)
            {
                mmlThread.Abort();
            }
            mmlThread = new Thread(() => petitMML.Play(mml));
            mmlThread.Start();
        }

        public void RunIt(object sender, RoutedEventArgs e)
        {
            StartInterpret(mainTextBox.Text.Replace("\r", "").Replace("\n", ""));
            mainTextBox.Focus();
        }
    }
}