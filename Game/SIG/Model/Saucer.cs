using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SIG.Model
{
    internal class Saucer : Sprite, IEnemy
    {
        private uint m_points;

        /// <summary>
        /// Constructor
        /// </summary>
        public Saucer(string p_graphics, Brush p_foreground, Size p_pixelSize, Point p_position, int[] p_spriteSequence, uint p_points)
            : base(p_graphics, p_foreground, p_pixelSize, p_position, p_spriteSequence)
        {
            m_points = p_points;
        }

        public uint Points => m_points;

        /// <summary>
        /// Animate enemy sprite
        /// </summary>
        public void Animate()
        {
            if (SequenceIndex < Sequence.Length - 1)
            {
                SequenceIndex++;
            }
            else
            {
                SequenceIndex = 0;
            }
            //LoadGraphics(SequenceIndex);
            Select(SequenceIndex);
        }
    }
}
