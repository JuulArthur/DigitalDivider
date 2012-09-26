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
    public class Field : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Texture2D fieldTexture;
        private int textureId = 0;
        private Vector2 position;
        private float rotation = 0;
        private Main game;

        public Field(Game game, int id, Vector2 p)
            : base(game)
        {
            this.game = (Main)game;
            this.textureId = id;
            position = p;
        }

        public void LoadContent()
        {
            fieldTexture = Game.Content.Load<Texture2D>("Fields");
            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            rotation = (float)(rotation > 2 * Math.PI ? rotation + game.getSpeed() * 0.01 - 2 * Math.PI : rotation + game.getSpeed()*0.01);
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(fieldTexture,
                new Rectangle((int)position.X+75, (int)position.Y+75, 150, 150),
                new Rectangle(150 * textureId, 0, 150, 150),
                Color.White,
                rotation, 
                new Vector2(75,75), 
                SpriteEffects.None, 
                0);

            base.Draw(gameTime);
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public int getWidth()
        {
            return fieldTexture.Width;
        }

        public int getHeight()
        {
            return fieldTexture.Height;
        }
    }
}
