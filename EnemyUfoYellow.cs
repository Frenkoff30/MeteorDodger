using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeteorDodger
{
    public class EnemyUfoYellow
    {
        private Texture2D texture;
        public Vector2 Position;
        private float verticalSpeed;
        private float horizontalSpeed;
        private float direction = 1f;

        public EnemyUfoYellow(Texture2D texture, Vector2 startPosition, float verticalSpeed, float horizontalSpeed)
        {
            this.texture = texture;
            this.Position = startPosition;
            this.verticalSpeed = verticalSpeed;
            this.horizontalSpeed = horizontalSpeed;
        }

        public void Update()
        {
            Position.Y += verticalSpeed;
            Position.X += horizontalSpeed * direction;

            // Změna směru po dosažení okraje obrazovky
            if (Position.X <= 0 || Position.X + texture.Width >= 800)
                direction *= -1;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }

        public bool IsOffScreen(int screenHeight)
        {
            return Position.Y > screenHeight;
        }
    }
}
