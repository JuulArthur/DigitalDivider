using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace DigitalDivider
{
    public class Number : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private int value;
        private Vector2 position;
        private Vector2 size = new Vector2(90, 90);
        private Texture2D numberTexture;
        private int textureId;
        private int textureId2;
        private float rotation = 0;
        private float rotation2 = 2 * (float)Math.PI;
        private Main game;
        private Vector2 destinationField;
        public bool goesToDestination = false;
        public DateTime dropTime;
        public Boolean snapToTouchPoint = false;
        public DateTime createdTime;

        public Number(Game game, int value, Vector2 position)
            : base(game)
        {
            this.value = value;
            this.position = position;
            this.game = (Main)game;
            createdTime = DateTime.Now;
        }

        public override void Initialize()
        {
			textureId = game.getRandomInt(4, 8);
			textureId2 = game.getRandomInt(0, 3);
			base.Initialize();
        }

		public void LoadContent()
		{
			numberTexture = Game.Content.Load<Texture2D>("Twirls");
			base.LoadContent();
		}

        public override void Update(GameTime gameTime)
        {
            rotation = (rotation > 2 * (float)Math.PI ? rotation + 0.05f - 2 * (float)Math.PI : rotation + 0.05f);
            rotation2 = (rotation2 < 0 ? rotation2 - 0.02f + 2 * (float)Math.PI : rotation2 - 0.02f);
            if (goesToDestination)
            {
                Vector2 delta = destinationField - position;
                position += 0.4f * delta;
                size *= 0.9f;
            }
            else
            {
                if (game.getNumberBeingDragged() != this)
                {
					this.position.X += (float)(game.getSpeed() * gameTime.ElapsedGameTime.TotalMilliseconds * 0.03);
                }
                else
                {
					float dX = game.getTouchPosition().X - 45 - position.X;
					float dY = game.getTouchPosition().Y - 45 - position.Y;
					float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
					position.X += (float)(snapToTouchPoint ? 1 : 0.5) * dX;
					position.Y += (float)(snapToTouchPoint ? 1 : 0.5) * dY;
					if (distance <= 45 && !snapToTouchPoint)
					{
						snapToTouchPoint = true;
					}
                }
            }
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(numberTexture, new Rectangle((int)position.X + 45, (int)position.Y + 45, (int)size.X, (int)size.Y),
                new Rectangle(90 * textureId, 0, 90, 90), Color.White, rotation, new Vector2(45, 45), SpriteEffects.None, 0);

            spriteBatch.Draw(numberTexture, new Rectangle((int)position.X + 45, (int)position.Y + 45, (int)size.X, (int)size.Y),
                new Rectangle(90 * textureId2, 0, 90, 90), Color.White, rotation2, new Vector2(45, 45), SpriteEffects.None, 0);

            if (!game.isGamePaused())
            {
                if (value < 10)
                {
                    spriteBatch.DrawString(game.font, value.ToString(), new Vector2(position.X + 32, position.Y + 15), Color.White);
                }
                else if (value < 100)
                {
                    spriteBatch.DrawString(game.mediumFont, value.ToString(), new Vector2(position.X + 28, position.Y + 23), Color.White);
                }
                else if (value < 1000)
                {
                    spriteBatch.DrawString(game.smallFont, value.ToString(), new Vector2(position.X + 24, position.Y + 25), Color.White);
                }
            }

            base.Draw(gameTime);
        }

        public Vector2 getPosition()
        {
            return this.position;
        }

        public int getValue()
        {
            return value;
        }

        public void setDestinationField(Vector2 destinationField)
        {
            this.destinationField = destinationField;
        }
    }
}
