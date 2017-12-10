using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SIG.Model
{
    internal class Explosion : Sprite
    {
        public int VisibilityCounter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Explosion(string p_graphics, Brush p_foreground, Size p_pixelSize, Point p_position, int p_visibilityCounter)
            : base(p_graphics, p_foreground, p_pixelSize, p_position)
        {
            this.VisibilityCounter = p_visibilityCounter;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Explosion(uint p_points, Brush p_foreground, Point p_position, int p_visibilityCounter)
            : base(p_points.ToString(), p_foreground, p_position)
        {
            this.VisibilityCounter = p_visibilityCounter;
        }
    }
}
