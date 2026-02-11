using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeteorDodger
{
    public class BluePill
    {
        private Texture2D texture;
        public Vector2 Position;
        private float speed;

        public BluePill(Texture2D texture, Vector2 startPosition, float speed)
        {
            this.texture = texture;
            this.Position = startPosition;
            this.speed = speed;
        }

        public void Update()
        {
            Position.Y += speed;
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
