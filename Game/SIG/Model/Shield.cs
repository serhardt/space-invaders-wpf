using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SIG.Model
{
    internal class Shield : Sprite
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Shield(string p_graphics, Brush p_foreground, Size p_pixelSize, Point p_position)
            : base(p_graphics, p_foreground, p_pixelSize, p_position)
        {
        }

        /// <summary>
        /// Is shield burnt by missile position ?
        /// </summary>
        /// <returns></returns>
        public bool CheckBurning(Missile p_missile)
        {
            bool isBurnt = false;

            Point posMissile = p_missile.GetPosition();
            double halfWidthMissile = p_missile.Bounds.Width / 2;

            Point posShield = this.GetPosition();
            double halfWidthShield = this.Bounds.Width / 2;
            double halfHeightShield = this.Bounds.Height / 2;

            if (this.Bounds.IntersectsWith(p_missile.Bounds))
            {
                // Check column if shield reached
                int column = (int)((posMissile.X - (posShield.X - halfWidthShield)) / this.CellSize.Width);
                if (column >= 0)
                {
                    for (int row = this.Patterns[Sequence[this.SequenceIndex]].Length - 1; row >= 0; row--)
                    {
                        string spriteData = this.Patterns[Sequence[this.SequenceIndex]][row];

                        if ((column < spriteData.Length) && (spriteData[column] == '1'))
                        {
                            this.Patterns[Sequence[this.SequenceIndex]][row] = spriteData.Substring(0, column) + '0' + spriteData.Substring(column + 1);
                            isBurnt = true;
                            break;
                        }
                    }

                }

                if (isBurnt)
                    this.Bitmaps[Sequence[this.SequenceIndex]] = GetBitmap(this.Patterns[Sequence[this.SequenceIndex]]);
            }

            return isBurnt;
        }

        /// <summary>
        /// Is shield exploded by bomb position ?
        /// </summary>
        /// <returns></returns>
        public bool CheckExplosion(Bomb p_bomb)
        {
            bool isExploded = false;

            Point posBomb = p_bomb.GetPosition();
            double halfWidthBomb = p_bomb.Bounds.Width / 2;

            Point posShield = this.GetPosition();
            double halfWidthShield = this.Bounds.Width / 2;
            double halfHeightShield = this.Bounds.Height / 2;

            if (this.Bounds.IntersectsWith(p_bomb.Bounds))
            {
                // Check column if shield reached
                int column = (int)((posBomb.X - (posShield.X - halfWidthShield)) / this.CellSize.Width) - 1;

                int damageCount = 0;
                for (int damageCol = 0; damageCol < 3; damageCol++)
                {
                    if (column + damageCol >= 0)
                    {
                        int damageRow = 0;

                        for (int row = 0; row < this.Patterns[Sequence[this.SequenceIndex]].Length; row++)
                        {
                            string spriteData = this.Patterns[Sequence[this.SequenceIndex]][row];

                            if (((column + damageCol) < spriteData.Length) && (spriteData[column + damageCol] == '1'))
                            {
                                isExploded = true;
                                damageRow++;
                                damageCount++;
                                this.Patterns[Sequence[this.SequenceIndex]][row] = spriteData.Substring(0, column + damageCol) + '0' + spriteData.Substring(column + damageCol + 1);

                                if (damageRow >= 2)
                                    break;

                                if (damageCount >= 5)
                                    break;
                            }
                        }
                    }

                    if (damageCount >= 6)
                        break;
                }

                if (damageCount > 0)
                    this.Bitmaps[Sequence[this.SequenceIndex]] = GetBitmap(this.Patterns[Sequence[this.SequenceIndex]]);
            }

            return isExploded;
        }
    }
}
