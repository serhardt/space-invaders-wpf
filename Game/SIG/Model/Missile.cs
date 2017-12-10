using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SIG.Model
{
    internal class Missile : Sprite
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Missile(string p_graphics, Brush p_foreground, Size p_pixelSize, Point p_position)
            : base(p_graphics, p_foreground, p_pixelSize, p_position)
        {
        }

        /// <summary>
        /// Is missile reached by bomb ?
        /// </summary>
        /// <returns></returns>
        public bool CheckExplosion(Bomb p_bomb)
        {
            return this.Bounds.IntersectsWith(p_bomb.Bounds);
        }
    }
}
