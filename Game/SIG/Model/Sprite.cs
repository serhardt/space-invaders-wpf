using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SIG.Model
{
    internal class Sprite
    {
        private FormattedText m_text;
        private Point m_initialPosition;

        protected BitmapSource[] Bitmaps;
        protected string[][] Patterns;
        protected int[] Sequence;

        public Brush Foreground { get; private set; }
        public Size CellSize { get; private set; }
        public Rect Bounds { get; private set; }
        protected int SequenceIndex { get; set; }
        public Point Position { get; private set; }

        public bool IsAnimated { get; set; }

        /// <summary>
        /// Constructor 1
        /// </summary>
        public Sprite(string p_patterns, Brush p_foreground, Size p_pixelSize, Point p_position, int[] p_spriteSequence)
        {
            m_text = null;
            m_initialPosition = new Point(p_position.X, p_position.Y);
            this.Position = new Point(p_position.X, p_position.Y);
            this.Foreground = p_foreground;
            this.CellSize = p_pixelSize;

            this.IsAnimated = (p_spriteSequence != null) || (p_spriteSequence.Length == 1);

            SequenceIndex = 0;
            if ((p_spriteSequence != null) && (p_spriteSequence.Length > 0))
            {
                Sequence = p_spriteSequence;
            }
            else
            {
                Sequence = new int[] { 0 };
            }
            this.IsAnimated = Sequence.Length > 1;

            LoadPatterns(p_patterns.Split(','));
            Select(Sequence[SequenceIndex]);
        }

        /// <summary>
        /// Constructor 2
        /// </summary>
        public Sprite(string p_patterns, Brush p_foreground, Size p_pixelSize, Point p_position, int p_sequenceIndex = 0)
            : this(p_patterns, p_foreground, p_pixelSize, p_position, new int[] { p_sequenceIndex })
        {
        }

        /// <summary>
        /// Constructor 3
        /// </summary>
        public Sprite(string p_text, Brush p_foreground, Point p_position)
        {
            m_text = new FormattedText(p_text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, Game.TextTypeFace, Game.TextFontSize, p_foreground);
            m_initialPosition = p_position;
            this.Position = m_initialPosition;
            this.Foreground = p_foreground;
            this.IsAnimated = false;
            this.Bounds = new Rect(new Point(this.Position.X - m_text.Width / 2.0, this.Position.Y - m_text.Height / 2.0), new Size(m_text.Width, m_text.Height));
        }

        /// <summary>
        /// Load graphics contents
        /// </summary>
        private void LoadPatterns(string[] spritePatterns)
        {
            int spriteCount = spritePatterns.Length;
            if (spriteCount == 0) return;

            this.Bitmaps = new BitmapSource[spriteCount];
            this.Patterns = new string[spriteCount][];

            for (int i = 0; i < spriteCount; i++)
            {
                string[] spriteLines = spritePatterns[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                this.Patterns[i] = spriteLines;
                this.Bitmaps[i] = GetBitmap(spriteLines);
            }
        }

        /// <summary>
        /// Get bitmap from pattern
        /// </summary>
        /// <returns></returns>
        protected RenderTargetBitmap GetBitmap(string[] spriteLines)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            int columnCount = 0;
            int rowCount = spriteLines.Length;

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                for (int r = 0; r < rowCount; r++)
                {
                    if (columnCount < spriteLines[r].Length)
                        columnCount = spriteLines[r].Length;

                    for (int c = 0; c < columnCount; c++)
                    {
                        if (spriteLines[r][c] == '1')
                            drawingContext.DrawRectangle(this.Foreground, null, new Rect(
                                c * this.CellSize.Width,
                                r * this.CellSize.Height, this.CellSize.Width, this.CellSize.Height));
                    }
                }
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)(columnCount * this.CellSize.Width), (int)(rowCount * this.CellSize.Height), 96, 96, PixelFormats.Default);
            bitmap.Render(drawingVisual);
            bitmap.Freeze();

            return  bitmap;
        }

        /// <summary>
        /// Select sprite to draw
        /// </summary>
        protected void Select(int p_sequenceIndex)
        {
            this.SequenceIndex = p_sequenceIndex;
            int width = Bitmaps[Sequence[this.SequenceIndex]].PixelWidth;
            int height = Bitmaps[Sequence[this.SequenceIndex]].PixelHeight;
            this.Bounds = new Rect(new Point(this.Position.X - width / 2.0, this.Position.Y - height / 2.0), new Size(width, height));
        }

        /// <summary>
        /// Draw sprite
        /// </summary>
        public virtual void Draw(DrawingContext p_dc)
        {
            if (m_text != null)
            {
                p_dc.DrawText(m_text, new Point(Position.X - this.Bounds.Width / 2, Position.Y - this.Bounds.Height / 2));
                return;
            }

            p_dc.DrawImage(Bitmaps[Sequence[this.SequenceIndex]], this.Bounds);
        }

        /// <summary>
        /// Draw sprite
        /// </summary>
        /// <param name="dc"></param>
        public virtual void Draw(DrawingContext p_dc, bool p_forced) { }

        /// <summary>
        /// Get position of sprite
        /// </summary>
        public Point GetPosition()
        {
            return Position;
        }

        /// <summary>
        /// Get position of sprite
        /// </summary>
        public Point GetPosition(double p_offsetX, double p_offsetY)
        {
            return new Point(Position.X + p_offsetX, Position.Y + p_offsetY);
        }

        /// <summary>
        /// Set position of sprite
        /// </summary>
        public void SetPosition(Point p_position)
        {
            this.Position = p_position;
            this.Bounds = new Rect(new Point(p_position.X - this.Bounds.Width / 2.0, p_position.Y - this.Bounds.Height / 2.0), this.Bounds.Size);
        }

        /// <summary>
        /// Set position of sprite
        /// </summary>
        public void SetPosition(double p_x, double p_y)
        {
            this.Position = new Point(p_x, p_y);
            this.Bounds = new Rect(new Point(p_x - this.Bounds.Width / 2.0, p_y - this.Bounds.Height / 2.0), this.Bounds.Size);
        }

        /// <summary>
        /// Reset position of sprite to initial
        /// </summary>
        public void ResetPosition()
        {
            this.Position = m_initialPosition;
            this.Bounds = new Rect(new Point(m_initialPosition.X - this.Bounds.Width / 2.0, m_initialPosition.Y - this.Bounds.Height / 2.0), this.Bounds.Size);
        }
    }
}
