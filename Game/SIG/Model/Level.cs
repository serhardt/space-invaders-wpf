using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SIG.Model
{
    internal enum GameStatus
    {
        Starting,
        Started,
        Stopped,
        LevelCompleted,
        Exiting,
        Exited
    }

    internal class Level
    {
        private static readonly object m_locker = new object();

        private double m_invaderHorizontalStep;
        private double m_invaderVerticalStep;
        private double m_saucerHorizontalStep;
        private double m_playerHorizontalStep;
        private double m_missileVerticalStep;
        private double m_bombVerticalStep;

        private Task m_taskLoop;
        private List<Sprite> m_sprites;
        private Key m_keyPressed;
        private Dictionary<FormattedText, Point> m_middleTexts;
        private FormattedText m_displayedMiddleText;
        private Random m_randomNumber;

        public uint Number { get; private set; }
        public uint Score { get; private set; }
        public int Lives { get; private set; }
        private Size GameSize { get; set; }
        private Size PixelSize { get; set; }
        private Player Player { get; set; }
        private Saucer Saucer { get; set; }
        private Missile Missile { get; set; }
        private Explosion EnemyExplosion { get; set; }
        private Explosion PlayerExplosion { get; set; }
        public GameStatus Status { get; set; }

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Level(Size p_gameSize, Size p_pixelSize)
        {
            m_invaderHorizontalStep = 0;
            m_invaderVerticalStep = 0;
            m_saucerHorizontalStep = 0;
            m_playerHorizontalStep = 0;
            m_missileVerticalStep = 0;
            m_bombVerticalStep = 0;

            m_taskLoop = null;
            m_middleTexts = new Dictionary<FormattedText, Point>()
            {
                { Game.TextLevelStart, Game.PosTextLevelStart },
                { Game.TextGameOver, Game.PosTextGameOver },
                { Game.TextLevelCompleted, Game.PosTextLevelCompleted },
                { Game.TextRestart, Game.PosTextRestart },
                { Game.TextStartNewLevel, Game.PosTextStartNewLevel },
            };
            m_displayedMiddleText = null;
            m_keyPressed = Key.None;
            m_randomNumber = new Random();
            m_sprites = new List<Sprite>();

            this.Status = GameStatus.Starting;
            this.Number = 0;
            this.Score = 0;
            this.Lives = 0;
            this.GameSize = p_gameSize;
            this.PixelSize = p_pixelSize;

            this.Reset();
        }

        #endregion

        #region Drawing game objects

        /// <summary>
        /// Draw all sprites of level
        /// </summary>
        public void Draw(DrawingContext p_dc)
        {
            lock(m_locker)
            {
                foreach (Sprite sprite in m_sprites)
                {
                    if ((sprite is Player) && (this.Status == GameStatus.Stopped))
                        sprite.Draw(p_dc, true);
                    else
                        sprite.Draw(p_dc);
                }

                if (this.Missile != null)
                {
                    this.Missile.Draw(p_dc);
                }

                if (this.EnemyExplosion != null)
                {
                    this.EnemyExplosion.Draw(p_dc);
                }

                if (this.PlayerExplosion != null)
                {
                    this.PlayerExplosion.Draw(p_dc);
                }

                if (m_displayedMiddleText != null)
                {
                    if (m_middleTexts.ContainsKey(m_displayedMiddleText))
                        p_dc.DrawText(m_displayedMiddleText, m_middleTexts[m_displayedMiddleText]);
                }

                //p_dc.DrawLine(Game.BorderPen, new Point(0, 350), new Point(Game.Screen.Width, 350)); // Bomb
                //p_dc.DrawLine(Game.BorderPen, new Point(0, 400), new Point(Game.Screen.Width, 400)); // Game over
            }
        }

        #endregion

        #region Key pressed management

        /// <summary>
        /// Press a key
        /// </summary>
        public void PressKey(Key p_inputKey)
        {
            m_keyPressed = p_inputKey;
        }

        /// <summary>
        /// Release current key
        /// </summary>
        public void ReleaseKey()
        {
            m_keyPressed = Key.None;
        }

        /// <summary>
        /// Handle input key
        /// </summary>
        private void HandleInputKey()
        {
            if (this.Status == GameStatus.Started)
            {
                if (m_keyPressed == Key.Left)
                {
                    if (this.Player != null)
                    {
                        Point position = this.Player.GetPosition();

                        if (position.X > 20)
                            this.Player.SetPosition(position.X - m_playerHorizontalStep, position.Y);
                    }
                }
                else if (m_keyPressed == Key.Right)
                {
                    if (this.Player != null)
                    {
                        Point position = this.Player.GetPosition();

                        if (position.X < 610)
                            this.Player.SetPosition(position.X + m_playerHorizontalStep, position.Y);
                    }
                }
                else if (m_keyPressed == Key.Space)
                {
                    if ((this.Missile == null) && (this.Player != null))
                    {
                        this.Missile = CreateMissile(this.Player.GetPosition());
                    }
                }
            }
        }

        #endregion

        #region Creating/Destroying sprite

        /// <summary>
        /// Create a shield
        /// </summary>
        private Shield CreateShield(Point p_position)
        {
            Shield sprite = new Shield(Game.PatternShield, Game.BrushShield, this.PixelSize, p_position);
            m_sprites.Add(sprite);

            return sprite;
        }

        /// <summary>
        /// Create an invader
        /// </summary>
        private Invader CreateInvader(Point p_position, int p_type, int[] p_sequence)
        {
            Invader sprite;

            switch (p_type)
            {
                default:
                case 1: sprite = new Invader(Game.PatternInvader1, Game.BrushInvader1, this.PixelSize, p_position, p_sequence, 30); break;
                case 2: sprite = new Invader(Game.PatternInvader2, Game.BrushInvader2, this.PixelSize, p_position, p_sequence, 20); break;
                case 3: sprite = new Invader(Game.PatternInvader3, Game.BrushInvader3, this.PixelSize, p_position, p_sequence, 10); break;
            }

            m_sprites.Add(sprite);

            return sprite;
        }

        /// <summary>
        /// Create a saucer
        /// </summary>
        private Saucer CreateSaucer(Point p_position)
        {
            Saucer sprite = new Saucer(Game.PatternSaucer, Game.BrushSaucer, this.PixelSize, p_position, new int[] { 0, 1 }, (uint)m_randomNumber.Next(5, 15) * 10);
            this.Saucer = sprite;
            m_sprites.Add(sprite);

            return sprite;
        }

        /// <summary>
        /// Create a player
        /// </summary>
        private Player CreatePlayer(Point p_position)
        {
            Player sprite = new Player(Game.PatternPlayer, Game.BrushPlayer, this.PixelSize, p_position);
            this.Player = sprite;
            m_sprites.Add(sprite);

            return sprite;
        }

        /// <summary>
        /// Create a missile
        /// </summary>
        private Missile CreateMissile(Point p_position)
        {
            return new Missile(Game.PatternMissile, Game.BrushMissile, this.PixelSize, p_position);
        }

        /// <summary>
        /// Create a bomb
        /// </summary>
        private Bomb CreateBomb(Point p_position, int[] p_sequence)
        {
            Bomb sprite = new Bomb(Game.PatternBomb, Game.BrushBomb, this.PixelSize, p_position, p_sequence);
            m_sprites.Add(sprite);

            return sprite;
        }

        /// <summary>
        /// Destroy an invader
        /// </summary>
        private void DestroyInvader(Invader p_invader)
        {
            m_sprites.Remove(p_invader);
        }

        /// <summary>
        /// Destroy a saucer
        /// </summary>
        /// <returns></returns>
        private void DestroySaucer()
        {
            m_sprites.Remove(this.Saucer);
            this.Saucer = null;
        }

        /// <summary>
        /// Destroy an enemy
        /// </summary>
        private void DestroyEnemy(IEnemy p_enemy)
        {
            this.Score += p_enemy.Points;

            if (p_enemy is Invader)
            {
                Invader invader = p_enemy as Invader;

                DestroyInvader(invader);
                this.EnemyExplosion = new Explosion(Game.PatternExplosion, invader.Foreground, this.PixelSize, invader.GetPosition(), 20);
            }
            else if (p_enemy is Saucer)
            {
                Saucer saucer = p_enemy as Saucer;

                DestroySaucer();
                this.EnemyExplosion = new Explosion(saucer.Points, saucer.Foreground, saucer.GetPosition(), 80);
            }

        }

        /// <summary>
        /// Destroy a player
        /// </summary>
        /// <returns></returns>
        private void DestroyPlayer()
        {
            m_sprites.Remove(this.Player);
            this.Player = null;
        }

        /// <summary>
        /// Destroy a missile
        /// </summary>
        /// <returns></returns>
        private void DestroyMissile()
        {
             this.Missile = null;
        }

        #endregion

        #region Middle text management

        /// <summary>
        /// Display in the middle of screen
        /// </summary>
        private void SetDisplayedMiddleText(FormattedText p_formattedTextToDiplay)
        {
            m_displayedMiddleText = p_formattedTextToDiplay;
        }

        /// <summary>
        /// Clear display in the middle of screen
        /// </summary>
        private void ResetDisplayedMiddleText()
        {
            m_displayedMiddleText = null;
        }

        /// <summary>
        /// Is text displayed in the middle of screen ?
        /// </summary>
        private bool IsDisplayedMiddleText()
        {
            return m_displayedMiddleText != null;
        }

        #endregion

        #region Level game actions

        /// <summary>
        /// Start level of game
        /// </summary>
        public void Start(int p_lives)
        {
            // Level is starting...
            m_taskLoop = Task.Factory.StartNew((Action)(() =>
            {
                int counter = 300;
                this.Status = GameStatus.Stopped;
                if (this.Player == null)
                {
                    CreatePlayer(new Point(320, 430));
                }
                this.Player.BecomeInvincible();

                while ((this.Status != GameStatus.Exiting) && (m_keyPressed != Key.Escape))
                {
                    GameStatus savedStatus = this.Status;

                    #region Game restart loop

                    while ((this.Status != GameStatus.Exiting) && (m_keyPressed != Key.Enter) && (m_keyPressed != Key.Escape))
                    {
                        lock (m_locker)
                        {
                            // Text
                            if (counter >= 300)
                            {
                                if (this.Status == GameStatus.LevelCompleted)
                                    SetDisplayedMiddleText(Game.TextStartNewLevel);
                                else
                                    SetDisplayedMiddleText(Game.TextRestart);
                            }

                            // Saucer
                            if (counter % 10 == 0)
                            {
                                MoveSaucer();
                                AnimateSaucer();
                            }

                            // Invaders
                            if (counter % 20 == 0)
                            {
                                AnimateInvaders();
                            }

                            // Bombs
                            if (counter % 3 == 0)
                            {
                                AnimateBombs();
                                MoveBombs();
                            }

                            // Missile
                            MoveMissile();

                            // Explosion of enemy
                            if (this.EnemyExplosion != null)
                            {
                                if (this.EnemyExplosion.VisibilityCounter > 0)
                                {
                                    this.EnemyExplosion.VisibilityCounter--;
                                }
                                else
                                {
                                    this.EnemyExplosion = null;
                                }
                            }

                            // Explosion of player
                            if (this.PlayerExplosion != null)
                            {
                                if (this.PlayerExplosion.VisibilityCounter > 0)
                                {
                                    this.PlayerExplosion.VisibilityCounter--;
                                }
                                else
                                {
                                    this.PlayerExplosion = null;
                                }
                            }

                            // Keyboard
                            HandleInputKey();
                            Thread.Sleep(10);
                            counter++;
                        }
                    }

                    #endregion

                    #region Game level loop

                    if (savedStatus == GameStatus.LevelCompleted)
                    {
                        this.Number++;
                    }
                    else
                    {
                        this.Number = 1;
                        this.Score = 0;
                    }

                    counter = 0;

                    this.Reset();
                    this.Lives = p_lives;
                    this.Status = GameStatus.Started;
                    this.Player.BecomeInvincible();
                    SetDisplayedMiddleText(Game.TextLevelStart);

                    int speedInvader = 21 - (int)this.Number;
                    int speedSaucer = 11 - (int)this.Number / 5;
                    int speedBomb = 4 - (int)this.Number / 10;

                    while ((this.Status != GameStatus.Exiting) && (this.Status == GameStatus.Started))
                    {
                        lock(m_locker)
                        {
                            // Text
                            if ((IsDisplayedMiddleText()) && (counter >= 300))
                            {
                                ResetDisplayedMiddleText();
                            }

                            // Saucer
                            if (counter % speedSaucer == 0)
                            {
                                AnimateSaucer();
                                MoveSaucer();
                            }

                            // Invaders
                            if (counter % speedInvader == 0)
                            {
                                AnimateInvaders();
                                MoveInvaders();
                            }

                            // Bombs
                            if (counter % speedBomb == 0)
                            {
                                AnimateBombs();
                                MoveBombs();
                            }

                            // Missile
                            MoveMissile();

                            // Explosion of enemy
                            if (this.EnemyExplosion != null)
                            {
                                if (this.EnemyExplosion.VisibilityCounter > 0)
                                {
                                    this.EnemyExplosion.VisibilityCounter--;
                                }
                                else
                                {
                                    this.EnemyExplosion = null;
                                }
                            }

                            // Invicibility of player
                            if ((this.Player != null) && this.Player.IsInvincible)
                            {
                                this.Player.DecreaseInvincibility();
                            }

                            // Explosion of player
                            if (this.PlayerExplosion != null)
                            {
                                if (this.PlayerExplosion.VisibilityCounter > 0)
                                {
                                    this.PlayerExplosion.VisibilityCounter--;
                                }
                                else
                                {
                                    this.PlayerExplosion = null;

                                    if (this.Lives == 0)
                                    {
                                        this.Status = GameStatus.Stopped;
                                    }
                                    else
                                    {
                                        CreatePlayer(new Point(320, 430));
                                        this.Player.BecomeInvincible();
                                    }
                                }
                            }

                            // Keyboard
                            HandleInputKey();
                            if (m_keyPressed == Key.Space)
                            {
                                m_keyPressed = Key.None;
                            }
                            else if (m_keyPressed == Key.Escape)
                            {
                                m_keyPressed = Key.None;
                                this.Status = GameStatus.Stopped;
                            }

                            Thread.Sleep(10);
                            counter++;
                        }
                    }

                    if (this.Status == GameStatus.LevelCompleted)
                        SetDisplayedMiddleText(Game.TextLevelCompleted);
                    else if (this.Status == GameStatus.Stopped)
                        SetDisplayedMiddleText(Game.TextGameOver);

                    if (this.Player != null)
                        this.Player.BecomeNormal();
                    counter = 0;

                    #endregion
                }

                this.Status = GameStatus.Exited;
            }));

        }

        /// <summary>
        /// Exit level of game
        /// </summary>
        public void Exit()
        {
            if (this.Status == GameStatus.Starting) return;
            if (this.Status == GameStatus.Exited) return;

            this.Status = GameStatus.Exiting;
            while (this.Status == GameStatus.Exiting) { Thread.Sleep(100); }
        }

        /// <summary>
        /// Reset game
        /// </summary>
        private void Reset()
        {
            m_invaderHorizontalStep = this.PixelSize.Width;
            m_invaderVerticalStep = this.PixelSize.Height * this.Number;
            m_saucerHorizontalStep = this.PixelSize.Width * 3;
            m_playerHorizontalStep = this.PixelSize.Width * 1;
            m_missileVerticalStep = this.PixelSize.Height * 3;
            m_bombVerticalStep = this.PixelSize.Height;

            lock (m_locker)
            {
                m_sprites.Clear();

                double x = 120;

                for (int i = 0; i < 11; i++)
                {
                    double y = 80;

                    CreateInvader(new Point(x + i * 40, y), 1, new int[] { 0, 1 });
                    y += 30;

                    CreateInvader(new Point(x + i * 40, y), 2, new int[] { 1, 0 });
                    y += 30;

                    CreateInvader(new Point(x + i * 40, y), 2, new int[] { 0, 1 });
                    y += 30;

                    CreateInvader(new Point(x + i * 40, y), 3, new int[] { 1, 0 });
                    y += 30;

                    CreateInvader(new Point(x + i * 40, y), 3, new int[] { 0, 1 });
                    y += 30;
                }

                CreateSaucer(new Point(-30, 50));

                for (int i = 0; i < 4; i++)
                {
                    CreateShield(new Point(135 + i * 120, 390));
                }

                CreatePlayer(new Point(320, 430));

                this.Missile = null;
                this.EnemyExplosion = null;
                this.PlayerExplosion = null;
            }
        }

        #endregion

        #region Animation of sprites

        /// <summary>
        /// Animate all sprites of enemies
        /// </summary>
        private void AnimateInvaders()
        {
            foreach (Invader invader in m_sprites.Where(s => s is Invader))
            {
                invader.Animate();
            }
        }

        /// <summary>
        /// Animate all sprites of bombs
        /// </summary>
        private void AnimateBombs()
        {
            foreach (Bomb bomb in m_sprites.Where(b => b is Bomb))
            {
                bomb.Animate();
            }
        }

        /// <summary>
        /// Animate all sprites of enemies
        /// </summary>
        private void AnimateSaucer()
        {
            if (this.Saucer != null)
            {
                this.Saucer.Animate();
            }
        }

        #endregion

        #region Moving sprite objects

        /// <summary>
        /// Do moving saucer
        /// </summary>
        private void MoveSaucer()
        {
            // Do movement of saucer
            if (this.Saucer != null)
            {
                Point position = this.Saucer.GetPosition();
                if (position.X < 670)
                {
                    this.Saucer.SetPosition(position.X + m_saucerHorizontalStep, position.Y);
                }
                else
                {
                    if (m_randomNumber.Next(150) == 4)
                    {
                        this.Saucer.ResetPosition();
                    }
                }
            }
        }

        /// <summary>
        /// Do moving missile
        /// </summary>
        private void MoveMissile()
        {
            // Do movement of missile
            if (this.Missile != null)
            {
                Point posMissile = this.Missile.GetPosition();
                this.Missile.SetPosition(posMissile.X, posMissile.Y - m_missileVerticalStep);
                posMissile = this.Missile.GetPosition();

                // Check if missile exits of screen
                if (posMissile.Y <= 0)
                {
                    DestroyMissile();
                }
                else
                {
                    // Check if missile exploses bomb
                    Bomb explodedBomb = null;

                    foreach (Bomb bomb in m_sprites.Where(s => s is Bomb))
                    {
                        if (this.Missile.CheckExplosion(bomb))
                        {
                            explodedBomb = bomb;
                            break;
                        }
                    }

                    if (explodedBomb != null)
                    {
                        // Remove bomb from sprite list
                        m_sprites.Remove(explodedBomb);
                        DestroyMissile();
                    }
                    else
                    {
                        // Check if missile reaches enemy
                        IEnemy reachedEnemy = null;

                        foreach (IEnemy enemy in m_sprites.Where(s => s is IEnemy))
                        {
                            if ((enemy as Sprite).Bounds.IntersectsWith(this.Missile.Bounds))
                            {
                                DestroyEnemy(enemy);
                                DestroyMissile();
                                reachedEnemy = enemy;
                                break;
                            }
                        }

                        if (reachedEnemy == null)
                        {
                            // Check if missile reaches shield
                            foreach (Shield shield in m_sprites.Where(s => s is Shield))
                            {
                                if (shield.CheckBurning(this.Missile))
                                {
                                    DestroyMissile();
                                    break;
                                }

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Do moving invaders
        /// </summary>
        private void MoveInvaders()
        {
            if (this.Status == GameStatus.Stopped)
            {
                return;
            }

            double verticalStep = 0;

            // Check if an invader reaches edge of screen
            var invaders = m_sprites.Where(s => s is Invader).ToList();

            if (invaders.Count == 0)
            {
                // Level completed
                this.Status = GameStatus.LevelCompleted;
                return;
            }

            m_invaderHorizontalStep = Math.Sign(m_invaderHorizontalStep) * this.PixelSize.Width * (1d + 3d * (1d - invaders.Count / 55d));

            foreach (Invader invader in invaders)
            {
                Point position = invader.GetPosition();
                if (((position.X > 615) && (m_invaderHorizontalStep > 0)) || ((position.X < 15) && (m_invaderHorizontalStep < 0)))
                {
                    m_invaderHorizontalStep = -m_invaderHorizontalStep;
                    verticalStep = m_invaderVerticalStep;
                    break;
                }
            }

            // Do movement of invaders and create new bombs
            int bombCount = m_sprites.Count(s => s is Bomb);

            foreach (Invader invader in invaders)
            {
                Point position = invader.GetPosition();
                invader.SetPosition(position.X + m_invaderHorizontalStep, position.Y + verticalStep);

                // Check if invader reaches earth and game over...
                if (position.Y > 400)
                {
                    // Stop level
                    this.Status = GameStatus.Stopped;

                    // Player dies
                    this.Lives = 0;

                    this.PlayerExplosion = new Explosion(Game.PatternExplosion, this.Player.Foreground, this.PixelSize, this.Player.GetPosition(), 80);
                    DestroyPlayer();

                    return;
                }

                // A bomb needs to be launched ? (based on y position of invader, bomb count limit, level number, and random number at each cycle loop)
                if ((position.Y < 350) && (bombCount < 15 + this.Number * 2) && (m_randomNumber.Next(150) == 5))
                {
                    CreateBomb(position, new int[] { 0, 1 });
                }
            }

            // Check if a bomb reaches a shield, a missile or the player
            var shields = m_sprites.Where(s => s is Shield).ToList();
            var bombs = m_sprites.Where(s => s is Bomb).ToList();

            foreach (Bomb bomb in bombs)
            {
                foreach (Shield shield in shields)
                {
                    if (shield.CheckExplosion(bomb))
                    {
                        // Remove bomb from sprite list
                        m_sprites.Remove(bomb);
                        break;
                    }
                }

                if ((this.Missile != null) && this.Missile.CheckExplosion(bomb))
                {
                    // Remove bomb from sprite list
                    m_sprites.Remove(bomb);
                    DestroyMissile();
                    break;
                }

                if ((this.Player != null) && !this.Player.IsInvincible && this.Player.CheckExplosion(bomb))
                {
                    // Remove exploded bomb from sprite list (to not be drawn furthermore)
                    m_sprites.Remove(bomb);

                    // Player dies
                    this.Lives--;

                    this.PlayerExplosion = new Explosion(Game.PatternExplosion, this.Player.Foreground, this.PixelSize, this.Player.GetPosition(), 80);
                    DestroyPlayer();
                }
            }
        }

        /// <summary>
        /// Do moving bombs
        /// </summary>
        private void MoveBombs()
        {
            List<Bomb> explodedBombs = null;

            // Do movement of bombs
            foreach (Bomb bomb in m_sprites.Where(s => s is Bomb))
            {
                Point position = bomb.GetPosition();
                if (position.Y > Game.Screen.Height)
                {
                    if (explodedBombs == null)
                        explodedBombs = new List<Bomb>();

                    explodedBombs.Add(bomb);
                }
                else bomb.SetPosition(position.X, position.Y + m_bombVerticalStep);
            }

            // Remove exploded bombs from game
            if (explodedBombs != null)
            {
                foreach (Bomb bomb in explodedBombs)
                {
                    m_sprites.Remove(bomb);
                }
            }
        }

        #endregion
    }
}
