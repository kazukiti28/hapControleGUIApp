using System;
using System.ComponentModel;
using System.Windows;

namespace hapControlGUIApp
{
    /// <summary>
    /// ProcessingSplash.xaml の相互作用ロジック
    /// </summary>
    public partial class ProcessingSplash : Window
    {
        BackgroundWorker _backgroundWorker = new BackgroundWorker();
        Action action;
        public bool complete { get; private set; }
        public bool close { get; private set; }

        public ProcessingSplash(string message, Action action)
        {
            InitializeComponent();
            this.action = action;
            DataContext = message;
            complete = false;
            close = false;

            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.RunWorkerAsync();
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (action != null)
                action.Invoke();
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //Cancelled
                complete = false;
                close = true;
            }
            else if (e.Error != null)
            {
                //Exception Thrown
                complete = false;
                close = true;
            }
            else
            {
                //Completed
                complete = true;
                close = true;
            }
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!close)
                e.Cancel = true;
        }
    }
}