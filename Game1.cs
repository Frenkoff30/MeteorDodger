using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MeteorDodger
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D[] backgroundTextures;
        private Texture2D currentBackground;
        private Texture2D playerTexture, meteorBrownTexture, meteorGreyTexture;
        private Texture2D starTexture, lifeTexture;
        private Texture2D shieldIconTexture, shieldEffectTexture;
        private Texture2D ufoGreenTexture, ufoYellowTexture;
        private Texture2D bossTexture, bossLaserTexture;
        private Texture2D bluePillTexture;
        private SpriteFont font;

        private Player player;
        private List<Meteor> meteors;
        private List<Star> stars;
        private List<Shield> shields;
        private List<EnemyUfo> ufoGreenList;
        private List<EnemyUfoYellow> ufoYellowList;
        private List<BossEnemy> bossEnemies;
        private List<LaserProjectile> bossProjectiles;
        private List<BluePill> bluePills;

        private Random random;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        private enum GameState { Menu, Playing, GameOver, Win }
        private GameState currentGameState = GameState.Menu;

        private int starsCollected = 0;
        private int starsToNextLevel = 5;
        private int currentLevel = 1;
        private int lives = 5;
        private bool isShieldActive = false;
        private double shieldTimer = 0;

        private float playTimeSeconds = 0f;
        private ProgressData progress;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            meteors = new List<Meteor>();
            stars = new List<Star>();
            shields = new List<Shield>();
            bluePills = new List<BluePill>();
            ufoGreenList = new List<EnemyUfo>();
            ufoYellowList = new List<EnemyUfoYellow>();
            bossEnemies = new List<BossEnemy>();
            bossProjectiles = new List<LaserProjectile>();
            random = new Random();
            progress = ProgressManager.Load();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTextures = new Texture2D[]
            {
                Content.Load<Texture2D>("purple"),
                Content.Load<Texture2D>("blue"),
                Content.Load<Texture2D>("darkPurple"),
                Content.Load<Texture2D>("black")
            };
            currentBackground = backgroundTextures[0];

            playerTexture = Content.Load<Texture2D>("playerShip1_blue");
            meteorBrownTexture = Content.Load<Texture2D>("meteorBrown_big1");
            meteorGreyTexture = Content.Load<Texture2D>("meteorGrey_big1");
            starTexture = Content.Load<Texture2D>("star_gold");
            lifeTexture = Content.Load<Texture2D>("playerLife1_blue");
            shieldIconTexture = Content.Load<Texture2D>("shield_silver");
            shieldEffectTexture = Content.Load<Texture2D>("shield2");
            ufoGreenTexture = Content.Load<Texture2D>("ufoGreen");
            ufoYellowTexture = Content.Load<Texture2D>("ufoYellow");
            bluePillTexture = Content.Load<Texture2D>("pill_blue");
            bossTexture = Content.Load<Texture2D>("enemyRed5");
            bossLaserTexture = Content.Load<Texture2D>("laserRed15");
            font = Content.Load<SpriteFont>("DefaultFont");
        }

        private void StartGame()
        {
            player = new Player(playerTexture, new Vector2(400, 500));
            meteors.Clear();
            stars.Clear();
            shields.Clear();
            bluePills.Clear();
            ufoGreenList.Clear();
            ufoYellowList.Clear();
            bossEnemies.Clear();
            bossProjectiles.Clear();
            starsCollected = 0;
            currentLevel = 1;
            lives = 5;
            currentBackground = backgroundTextures[0];
            isShieldActive = false;
            shieldTimer = 0;
            currentGameState = GameState.Playing;
        }

        protected override void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();
            if (currentKeyboardState.IsKeyDown(Keys.Escape)) Exit();

            if (currentGameState == GameState.Playing)
                playTimeSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (currentGameState)
            {
                case GameState.Menu:
                    if (currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
                        StartGame();
                    break;

                case GameState.Playing:
                    player.Update();

                    if (random.NextDouble() < 0.015)
                    {
                        Texture2D selected = random.Next(2) == 0 ? meteorBrownTexture : meteorGreyTexture;
                        meteors.Add(new Meteor(selected, new Vector2(random.Next(0, 800 - selected.Width), -selected.Height), 2 + (float)random.NextDouble() * 2 + currentLevel));
                    }

                    if (random.NextDouble() < 0.005)
                        stars.Add(new Star(starTexture, new Vector2(random.Next(0, 800 - starTexture.Width), -starTexture.Height), 2f));

                    if (random.NextDouble() < 0.0005)
                        shields.Add(new Shield(shieldIconTexture, new Vector2(random.Next(0, 800 - shieldIconTexture.Width), -shieldIconTexture.Height), 2f));

                    if (random.NextDouble() < 0.0005)
                        bluePills.Add(new BluePill(bluePillTexture, new Vector2(random.Next(0, 800 - bluePillTexture.Width), -bluePillTexture.Height), 2f));

                    if (currentLevel >= 2 && random.NextDouble() < 0.004)
                        ufoGreenList.Add(new EnemyUfo(ufoGreenTexture, new Vector2(random.Next(0, 800 - ufoGreenTexture.Width), -ufoGreenTexture.Height), 1.5f));

                    if (currentLevel >= 3 && random.NextDouble() < 0.003)
                        ufoYellowList.Add(new EnemyUfoYellow(ufoYellowTexture, new Vector2(random.Next(0, 800 - ufoYellowTexture.Width), -ufoYellowTexture.Height), 1.2f, 1f));

                    if (currentLevel == 4 && random.NextDouble() < 0.0015)
                        bossEnemies.Add(new BossEnemy(bossTexture, bossLaserTexture, new Vector2(random.Next(0, 800 - bossTexture.Width), 20)));

                    meteors.RemoveAll(m => { m.Update(); return CheckCollisionOrOffscreen(m); });
                    stars.RemoveAll(s => { s.Update(); return HandleStar(s); });
                    shields.RemoveAll(sh => { sh.Update(); return HandleShield(sh); });
                    bluePills.RemoveAll(p => { p.Update(); return HandleBluePill(p); });
                    ufoGreenList.RemoveAll(u => { u.Update(); return CheckCollisionOrOffscreen(u); });
                    ufoYellowList.RemoveAll(u => { u.Update(); return CheckCollisionOrOffscreen(u); });

                    bossEnemies.ForEach(b =>
                    {
                        b.Update(gameTime);
                        if (b.CanShoot())
                        {
                            var projPos = new Vector2(b.Position.X + b.Width / 2 - bossLaserTexture.Width / 2, b.Position.Y + b.Height);
                            bossProjectiles.Add(new LaserProjectile(bossLaserTexture, projPos, 4f));
                        }
                    });

                    bossEnemies.RemoveAll(b => CheckCollisionOrOffscreen(b));
                    bossProjectiles.RemoveAll(p => { p.Update(); return CheckCollisionOrOffscreen(p); });

                    if (isShieldActive)
                    {
                        shieldTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                        if (shieldTimer <= 0) isShieldActive = false;
                    }
                    break;

                case GameState.GameOver:
                    if (currentKeyboardState.IsKeyDown(Keys.R) && previousKeyboardState.IsKeyUp(Keys.R))
                        currentGameState = GameState.Menu;
                    break;

                case GameState.Win:
                    if (playTimeSeconds < progress.BestTimeInSeconds)
                        progress.BestTimeInSeconds = playTimeSeconds;

                    progress.UnlockedLevels[Math.Clamp(currentLevel - 1, 0, progress.UnlockedLevels.Length - 1)] = true;
                    ProgressManager.Save(progress);

                    if (currentKeyboardState.IsKeyDown(Keys.R) && previousKeyboardState.IsKeyUp(Keys.R))
                        currentGameState = GameState.Menu;
                    break;
            }

            previousKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }

        private bool CheckCollisionOrOffscreen(dynamic obj)
        {
            if (player.GetBounds().Intersects(obj.GetBounds()))
            {
                if (!isShieldActive)
                {
                    lives--;
                    if (lives <= 0) currentGameState = GameState.GameOver;
                }
                return true;
            }
            return obj.IsOffScreen(600);
        }

        private bool HandleStar(Star s)
        {
            if (player.GetBounds().Intersects(s.GetBounds()))
            {
                starsCollected++;
                if (starsCollected >= starsToNextLevel)
                {
                    currentLevel++;
                    starsCollected = 0;
                    if (currentLevel <= 4)
                        currentBackground = backgroundTextures[currentLevel - 1];
                    else
                        currentGameState = GameState.Win;
                }
                return true;
            }
            return s.IsOffScreen(600);
        }

        private bool HandleShield(Shield sh)
        {
            if (player.GetBounds().Intersects(sh.GetBounds()))
            {
                isShieldActive = true;
                shieldTimer = 5;
                return true;
            }
            return sh.IsOffScreen(600);
        }

        private bool HandleBluePill(BluePill p)
        {
            if (player.GetBounds().Intersects(p.GetBounds()))
            {
                lives++;
                return true;
            }
            return p.IsOffScreen(600);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            for (int x = 0; x < 800; x += currentBackground.Width)
                for (int y = 0; y < 600; y += currentBackground.Height)
                    _spriteBatch.Draw(currentBackground, new Vector2(x, y), Color.White);

            if (currentGameState == GameState.Menu)
            {
                _spriteBatch.DrawString(font, "Meteor Dodger", new Vector2(280, 200), Color.White);
                _spriteBatch.DrawString(font, "Press ENTER to Play", new Vector2(260, 300), Color.White);
                _spriteBatch.DrawString(font, $"Best Time: {progress.BestTimeInSeconds:0.00}s", new Vector2(270, 360), Color.Yellow);
            }
            else if (currentGameState == GameState.Playing)
            {
                meteors.ForEach(m => m.Draw(_spriteBatch));
                stars.ForEach(s => s.Draw(_spriteBatch));
                shields.ForEach(sh => sh.Draw(_spriteBatch));
                bluePills.ForEach(p => p.Draw(_spriteBatch));
                ufoGreenList.ForEach(u => u.Draw(_spriteBatch));
                ufoYellowList.ForEach(u => u.Draw(_spriteBatch));
                bossEnemies.ForEach(b => b.Draw(_spriteBatch));
                bossProjectiles.ForEach(p => p.Draw(_spriteBatch));

                if (isShieldActive)
                {
                    Vector2 shieldPos = new Vector2(
                        player.Position.X + (player.Width - shieldEffectTexture.Width) / 2,
                        player.Position.Y + (player.Height - shieldEffectTexture.Height) / 2
                    );
                    _spriteBatch.Draw(shieldEffectTexture, shieldPos, Color.White);
                }

                player.Draw(_spriteBatch);
                _spriteBatch.DrawString(font, $"Stars: {starsCollected}/{starsToNextLevel}", new Vector2(10, 10), Color.Yellow);
                _spriteBatch.DrawString(font, $"Level: {currentLevel}", new Vector2(10, 30), Color.LightGreen);

                for (int i = 0; i < lives; i++)
                {
                    int x = 790 - (i + 1) * (lifeTexture.Width + 5);
                    _spriteBatch.Draw(lifeTexture, new Vector2(x, 10), Color.White);
                }
            }
            else if (currentGameState == GameState.GameOver)
            {
                _spriteBatch.DrawString(font, "Game Over - Press R to Restart", new Vector2(220, 300), Color.White);
            }
            else if (currentGameState == GameState.Win)
            {
                _spriteBatch.DrawString(font, "You Win! - Press R to Restart", new Vector2(240, 260), Color.LightGreen);
                _spriteBatch.DrawString(font, $"Your Time: {playTimeSeconds:0.00}s", new Vector2(280, 300), Color.LightBlue);
                _spriteBatch.DrawString(font, $"Best Time: {progress.BestTimeInSeconds:0.00}s", new Vector2(280, 340), Color.Yellow);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
