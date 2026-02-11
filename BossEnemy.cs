using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MeteorDodger
{
    public class BossEnemy
    {
        private Texture2D texture;
        private Texture2D laserTexture;
        public Vector2 Position;
        private float verticalSpeed = 0.5f;
        private float horizontalSpeed = 1.5f;
        private float direction = 1f;
        public int Width => texture.Width;
        public int Height => texture.Height;

        private double fireCooldown = 2.5;
        private double fireTimer = 0;

        public List<LaserProjectile> Projectiles { get; private set; }

        public BossEnemy(Texture2D texture, Texture2D laserTexture, Vector2 startPosition)
        {
            this.texture = texture;
            this.laserTexture = laserTexture;
            this.Position = startPosition;
            this.Projectiles = new List<LaserProjectile>();
        }

        public void Update(GameTime gameTime)
        {
            // Pohyb dolů a do stran
            Position.Y += verticalSpeed;
            Position.X += horizontalSpeed * direction;

            if (Position.X <= 0 || Position.X + texture.Width >= 800)
                direction *= -1;

            // Střelba
            fireTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (fireTimer >= fireCooldown)
            {
                fireTimer = 0;
                Vector2 laserPos = new Vector2(Position.X + texture.Width / 2 - laserTexture.Width / 2, Position.Y + texture.Height);
                Projectiles.Add(new LaserProjectile(laserTexture, laserPos, 4f));
            }

            // Aktualizace projektilů
            foreach (var p in Projectiles)
                p.Update();

            Projectiles.RemoveAll(p => p.IsOffScreen(600));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
            foreach (var p in Projectiles)
                p.Draw(spriteBatch);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }

        public bool IsOffScreen(int screenHeight)
        {
            return Position.Y > screenHeight;
        }
        public bool CanShoot()
        {
            return fireTimer == 0;
        }
    }
}
