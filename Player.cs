using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MeteorDodger
{
    public class Player
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed = 5f;

        public Player(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            this.position = startPosition;
        }

        public void Update()
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                position.X -= speed;
            if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                position.X += speed;
            if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
                position.Y -= speed;
            if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
                position.Y += speed;

            position.X = MathHelper.Clamp(position.X, 0, 800 - texture.Width);
            position.Y = MathHelper.Clamp(position.Y, 0, 600 - texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public Vector2 Position => position;
        public int Width => texture.Width;
        public int Height => texture.Height;
    }



}
