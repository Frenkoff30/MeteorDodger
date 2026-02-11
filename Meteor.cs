using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeteorDodger
{
    public class Meteor
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed;

        public Meteor(Texture2D texture, Vector2 startPosition, float speed)
        {
            this.texture = texture;
            this.position = startPosition;
            this.speed = speed;
        }

        public void Update()
        {
            position.Y += speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public bool IsOffScreen(int screenHeight)
        {
            return position.Y > screenHeight;
        }
        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }
    }
}
