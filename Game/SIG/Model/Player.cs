using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SIG.Model
{
    internal class Player : Sprite
    {
        private int m_invicibilityCounter;
        private bool m_wasDrawn;

        public bool IsInvincible { get { return m_invicibilityCounter > 0; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Player(string p_graphics, Brush p_foreground, Size p_pixelSize, Point p_position)
            : base(p_graphics, p_foreground, p_pixelSize, p_position)
        {
            m_invicibilityCounter = 0;
            m_wasDrawn = false;
        }

        /// <summary>
        /// Set player to become invincible
        /// </summary>
        public void BecomeInvincible()
        {
            m_invicibilityCounter = 200;
        }

        /// <summary>
        /// Decrease player invincibility
        /// </summary>
        public void DecreaseInvincibility()
        {
            m_invicibilityCounter--;
        }

        /// <summary>
        /// Set player to become normal (not invincible)
        /// </summary>
        public void BecomeNormal()
        {
            m_invicibilityCounter = 0;
        }

        /// <summary>
        /// Is player reached by bomb ?
        /// </summary>
        public bool CheckExplosion(Bomb p_bomb)
        {
            return this.Bounds.IntersectsWith(p_bomb.Bounds);
        }

        /// <summary>
        /// Draw player with managing invicibility animation
        /// </summary>
        public override void Draw(DrawingContext p_dc, bool p_forced)
        {
            if (p_forced)
                base.Draw(p_dc);
            else
                this.Draw(p_dc);
        }

        /// <summary>
        /// Draw player with managing invicibility animation
        /// </summary>
        public override void Draw(DrawingContext p_dc)
        {
            if (m_invicibilityCounter == 0)
            {
                base.Draw(p_dc);
                return;
            }

            if (!m_wasDrawn)
            {
                base.Draw(p_dc);
            }

            m_wasDrawn = !m_wasDrawn;
        }
    }
}
