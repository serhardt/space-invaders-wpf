using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SIG.Model
{
    internal class Game
    {
        public const int TotalLivesOnStart = 3;

        public const string PatternInvader1 = "00011000 00111100 01111110 11011011 11111111 01011010 10000001 01000010, 00011000 00111100 01111110 11011011 11111111 00100100 01011010 10100101";
        public const string PatternInvader2 = "00100000100 00010001000 00111111100 01101110110 11111111111 10111111101 10100000101 00011011000, 00100000100 10010001001 10111111101 11101110111 01111111110 00111111100 00010001000 00100000100";
        public const string PatternInvader3 = "000011110000 011111111110 111111111111 111001100111 111111111111 000110011000 001101101100 110000000011, 000011110000 011111111110 111111111111 111001100111 111111111111 000110011000 011001100110 001100001100";
        public const string PatternPlayer = "0000001000000 0000011100000 0000011100000 0111111111110 1111111111111 1111111111111 1111111111111 1111111111111";
        public const string PatternSaucer = "00000111111100000 00011111111111000 00111111111111100 01101101101101110 11111111111111111 00011100110011100 00001000000001000, 00000111111100000 00011111111111000 00111111111111100 01110110110110110 11111111111111111 00011100110011100 00001000000001000";
        public const string PatternShield = "00000111111111111111100000 00001111111111111111110000 00011111111111111111111000 00111111111111111111111100 01111111111111111111111110 11111111111111111111111111 11111111111111111111111111 11111111111111111111111111 11111111111111111111111111 11111111111111111111111111 11111111111111111111111111 11111111110000001111111111 11111111100000000111111111 11111111000000000011111111 11111110000000000001111111";
        public const string PatternMissile = "1 1 1 1";
        public const string PatternExplosion = "0000001000000 0010001000100 0001000001000 0000100010000 0000000000000 1110000000111 0000000000000 0000100010000 0001000001000 0010001000100 0000001000000";
        public const string PatternBomb = "100 010 001 001 010 100 100, 001 010 100 100 010 001 001"; //100 100 010 001 001 010 100";

        public const int ScreenWidth = 640;
        public const int ScreenHeight = 480;
        public const int TextFontSize = 20;
        public const string TextFontName = "Arial";

        public static readonly Brush BrushGreen = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        public static readonly Brush BrushTextTitle = BrushGreen;
        public static readonly Brush BrushTextValue = Brushes.White;
        public static readonly Brush BrushTextGameOver = Brushes.Red;

        public static readonly Brush BackgroundBrush = Brushes.Black;
        public static readonly Pen BorderPen = new Pen(BrushGreen, 2.0);
        public static readonly Pen BoundPen = new Pen(Brushes.Yellow, 1.0);

        public static readonly Brush BrushPlayer = BrushGreen;
        public static readonly Brush BrushShield = BrushGreen;
        public static readonly Brush BrushSaucer = Brushes.Red;
        public static readonly Brush BrushInvader1 = BrushGreen;
        public static readonly Brush BrushInvader2 = Brushes.Turquoise;
        public static readonly Brush BrushInvader3 = Brushes.Orchid;
        public static readonly Brush BrushMissile = Brushes.White;
        public static readonly Brush BrushBomb = Brushes.White;

        public static readonly Point PosTitleScore;
        public static readonly Point PosTitleHiScore;
        public static readonly Point PosTitleLives;
        public static readonly Point PosTitleLevel;
        public static readonly Point PosTextLevelStart;
        public static readonly Point PosTextGameOver;
        public static readonly Point PosTextLevelCompleted;
        public static readonly Point PosTextRestart;
        public static readonly Point PosTextStartNewLevel;

        public static readonly Point PosValueScore;
        public static readonly Point PosValueHiScore;
        public static readonly Point PosValueLives;
        public static readonly Point PosValueLevel;

        public static readonly Typeface TextTypeFace = new Typeface(TextFontName);

        public static readonly FormattedText TitleScore = new FormattedText("SCORE:", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextTitle);
        public static readonly FormattedText TitleHiScore = new FormattedText("HI-SCORE:", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextTitle);
        public static readonly FormattedText TitleLives = new FormattedText("LIVES:", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextTitle);
        public static readonly FormattedText TitleLevel = new FormattedText("LEVEL:", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextTitle);
        public static readonly FormattedText TextLevelStart = new FormattedText("LEVEL START", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextValue);
        public static readonly FormattedText TextGameOver = new FormattedText("GAME OVER", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextGameOver);
        public static readonly FormattedText TextLevelCompleted = new FormattedText("LEVEL COMPLETED", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextTitle);
        public static readonly FormattedText TextRestart = new FormattedText("ENTER TO START NEW GAME", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextValue);
        public static readonly FormattedText TextStartNewLevel = new FormattedText("ENTER TO START NEXT LEVEL", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextValue);

        public uint HiScore { get; private set; }
        public Level Level { get; private set; }

        public static Rect Screen { get; private set; }
        public static Size PixelSize { get; private set; }

        private List<Player> m_playerLives;

        static Game()
        {
            Game.Screen = new Rect(0, 0, 640, 480);
            Game.PixelSize = new Size(3, 3);

            PosTextLevelStart = new Point((Game.Screen.Width - TextLevelStart.Width) / 2, (Game.Screen.Height - TextLevelStart.Height) / 2);
            PosTextGameOver = new Point((Game.Screen.Width - TextGameOver.Width) / 2, (Game.Screen.Height - TextGameOver.Height) / 2);
            PosTextLevelCompleted = new Point((Game.Screen.Width - TextLevelCompleted.Width) / 2, (Game.Screen.Height - TextLevelCompleted.Height) / 2);
            PosTextRestart = new Point((Game.Screen.Width - TextRestart.Width) / 2, (Game.Screen.Height - TextRestart.Height) / 2);
            PosTextStartNewLevel = new Point((Game.Screen.Width - TextStartNewLevel.Width) / 2, (Game.Screen.Height - TextStartNewLevel.Height) / 2);

            PosTitleScore = new Point(10, 5);
            PosTitleHiScore = new Point(468, 5);
            PosTitleLives = new Point(10, Game.Screen.Height - 30 + 4);
            PosTitleLevel = new Point(540, Game.Screen.Height - 30 + 4);

            PosValueScore = new Point(90, 5);
            PosValueHiScore = new Point(575, 5);
            PosValueLives = new Point(75, Game.Screen.Height - 30 + 4);
            PosValueLevel = new Point(611, Game.Screen.Height - 30 + 4);

            BrushGreen.Freeze();
            BrushTextTitle.Freeze();
            BrushTextValue.Freeze();
            BrushTextGameOver.Freeze();
            BackgroundBrush.Freeze();
            BorderPen.Freeze();
            BoundPen.Freeze();
            BrushPlayer.Freeze();
            BrushShield.Freeze();
            BrushSaucer.Freeze();
            BrushInvader1.Freeze();
            BrushInvader2.Freeze();
            BrushInvader3.Freeze();
            BrushMissile.Freeze();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Game(uint p_hiScore)
        {
            m_playerLives = new List<Player>();

            for (int i = 0; i < Game.TotalLivesOnStart; i++)
            {
                m_playerLives.Add(new Player(Game.PatternPlayer, Game.BrushPlayer, new Size(2, 2), new Point(110 + i * 35, Game.Screen.Height - 16)));
            }

            this.HiScore = p_hiScore;
            this.Level = new Level(Game.Screen.Size, Game.PixelSize);
        }

        /// <summary>
        /// Reset game
        /// </summary>
        public void Reset()
        {
            if (this.Level != null)
            {
                this.Level.Exit();
            }
            this.Level.Start(Game.TotalLivesOnStart);
        }

        /// <summary>
        /// Exit game
        /// </summary>
        public void Exit()
        {
            if (this.Level != null)
            {
                this.Level.Exit();
            }
        }

        /// <summary>
        /// Draw all sprites of game
        /// </summary>
        public void Draw(DrawingContext p_dc)
        {
            p_dc.DrawRectangle(BackgroundBrush, null, Screen);
            this.Level.Draw(p_dc);

            if (this.Level.Score > this.HiScore)
                this.HiScore = this.Level.Score;

            p_dc.DrawText(TitleScore, PosTitleScore);
            p_dc.DrawText(new FormattedText(this.Level.Score.ToString("00000"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextValue), PosValueScore);

            p_dc.DrawText(TitleHiScore, PosTitleHiScore);
            p_dc.DrawText(new FormattedText(this.HiScore.ToString("00000"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextValue), PosValueHiScore);

            p_dc.DrawText(TitleLives, PosTitleLives);
            p_dc.DrawText(new FormattedText(this.Level.Lives.ToString("0"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextValue), PosValueLives);

            for(int i = 0; i < this.Level.Lives; i++)
            {
                m_playerLives[i].Draw(p_dc);
            }

            p_dc.DrawText(TitleLevel, PosTitleLevel);
            p_dc.DrawText(new FormattedText(this.Level.Number.ToString("00"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TextTypeFace, TextFontSize, BrushTextValue), PosValueLevel);

            p_dc.DrawLine(BorderPen, new Point(0, Game.Screen.Height - 30), new Point(Game.Screen.Width, Game.Screen.Height - 30));
            p_dc.DrawRectangle(null, BorderPen, Screen);
        }
    }
}
