// ParallaxingBackground.cs
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DigitalDivider
{
	public class ParallaxingBackground : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Texture2D texture;
        private Vector2 position1;
        private Vector2 position2;
        private Main game;
        private float speedMultiplier;

		public ParallaxingBackground(Game game)
            : base(game)
        {
            this.game = (Main)game;
        }

		public void LoadContent(String texturePath)
		{
			texture = game.Content.Load<Texture2D>(texturePath);
			base.LoadContent();
		}


        public void Initialize(int yPos, float speedMultiplier)
        {
            position1 = new Vector2(0, yPos);
            position2 = new Vector2(-800, yPos);
            this.speedMultiplier = speedMultiplier;
			base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            position1.X = (position1.X >= 800 ? position2.X - 800 : position1.X) + game.getSpeed() * speedMultiplier * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.03f;
			position2.X = (position2.X >= 800 ? position1.X - 800 : position2.X) + game.getSpeed() * speedMultiplier * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.03f;
        }

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position1, Color.White);
            spriteBatch.Draw(texture, position2, Color.White);
        }
    }
}
