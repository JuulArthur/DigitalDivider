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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Explosion : Microsoft.Xna.Framework.DrawableGameComponent
    {

        private Vector2 position;
        private Main game;
        Texture2D spriteStrip;
        int elapsedTime;
        int frameTime;
        int frameCount;
        int currentFrame;
        Color color;
        Rectangle sourceRect = new Rectangle();
        Rectangle destinationRect = new Rectangle();
        public int FrameWidth;
        public int FrameHeight;
        public bool active;
        public bool Looping;

        public Explosion(Game game, Vector2 position)
            : base(game)
        {
            this.position = position;
            active = true;
            FrameHeight = 125;
            FrameWidth = 125;
            frameTime = 30;
            frameCount = 15;
            color = Color.White;
            Looping = false;
            elapsedTime = 0;
            currentFrame = 0;
            this.game = (Main)game;
        }

        public void LoadContent()
        {
            spriteStrip = Game.Content.Load<Texture2D>("HitSprite2000");
            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (active == false)
                return;
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime > frameTime)
            {
                if (++currentFrame == frameCount)
                {
                    currentFrame = 16;
                    if (Looping == false) active = false;
                }
                elapsedTime = 0;
            }

            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            destinationRect = new Rectangle((int)position.X + 45 - (int)(FrameHeight * 0.5f), (int)position.Y + 45 - (int)(FrameHeight * 0.5f), FrameWidth, FrameHeight);
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);

            base.Draw(gameTime);
        }

        public bool deleter()
        {
            return !active;
        }
    }
}
