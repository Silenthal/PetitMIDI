namespace PetitMIDI
{
    using System.Threading;
    using System.Windows;
    using PetitMIDI.MML;

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
            mmlThread = new Thread(() => { petitMML.Play(mml); });
            mmlThread.Start();
        }

        public void RunIt(object sender, RoutedEventArgs e)
        {
            var lines = new string[mainTextBox.LineCount];
            for (int line = 0; line < lines.Length; line++)
            {
                lines[line] = mainTextBox.GetLineText(line).Replace("\r","").Replace("\n","");
            }

            StartInterpret(string.Join("", lines));
            mainTextBox.Focus();
        }
    }
}