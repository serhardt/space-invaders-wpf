using SIG.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace SIG
{
    /// <summary>
    /// Logique d'interaction pour GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl
    {
        private readonly object m_locker = new object();
        private readonly string m_hiScoreDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd('\\') + @"\.sig\";
        private readonly string m_hiScoreFileName = ".his";
        private DispatcherTimer m_timerGame;
        private Key m_keyPressed;
        private Game m_game;
        private uint m_hiScore;

        // Constructor
        public GameControl()
        {
            InitializeComponent();

            m_game = new Game(m_hiScore);

            m_timerGame = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            m_timerGame.Tick += OnTimerGameTick;
        }

        /// <summary>
        /// On loaded
        /// </summary>
        private void OnLoaded(object p_sender, RoutedEventArgs p_e)
        {
            // Load hi-score from file
            m_hiScore = 0;

            try
            {
                if (!Directory.Exists(m_hiScoreDirectory))
                    Directory.CreateDirectory(m_hiScoreDirectory);

                if (File.Exists(m_hiScoreDirectory + m_hiScoreFileName))
                {
                    m_hiScore = BitConverter.ToUInt32(File.ReadAllBytes(m_hiScoreDirectory + m_hiScoreFileName), 0);
                }
            }
            catch { /* Nothing done */ }

            m_game.Reset();
            m_timerGame.Start();

            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.KeyDown += OnKeyDown;
                window.KeyUp += OnKeyUp;
            }
        }

        /// <summary>
        /// On unloaded
        /// </summary>
        private void OnUnloaded(object p_sender, RoutedEventArgs p_e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.KeyDown -= OnKeyDown;
                window.KeyUp -= OnKeyUp;
            }

            m_timerGame.Stop();
            m_game.Exit();

            // Save hi-score to file
            try
            {
                File.WriteAllBytes(m_hiScoreDirectory + m_hiScoreFileName, BitConverter.GetBytes(m_game.HiScore));
            }
            catch { /* Nothing done */ }
        }

        /// <summary>
        /// On timer game tick
        /// </summary>
        private void OnTimerGameTick(object p_sender, EventArgs p_e)
        {
            InvalidateVisual();
        }

        /// <summary>
        /// On render visual
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext p_dc)
        {
            base.OnRender(p_dc);

            if (m_game != null)
                m_game.Draw(p_dc);
        }

        /// <summary>
        /// On key down
        /// </summary>
        private void OnKeyDown(object p_sender, KeyEventArgs p_e)
        {
            if ((p_e.Key == Key.Space) && (m_keyPressed == Key.Space))
                return;

            if (m_game != null)
            {
                m_game.Level.PressKey(p_e.Key);
                m_keyPressed = p_e.Key;
            }
        }

        /// <summary>
        /// On key up
        /// </summary>
        private void OnKeyUp(object p_sender, KeyEventArgs p_e)
        {
            if (m_game != null)
            {
                m_game.Level.ReleaseKey();
                m_keyPressed = Key.None;
            }
        }
    }
}
