using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Devices;
using Microsoft.Advertising.Mobile.Xna;

namespace DigitalDivider
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static AdGameComponent adGameComponent;
        static DrawableAd bannerAd;
        VibrateController vibratecontroller;

        //Gamestate
        public enum GameState { menu, inGame, credits, highscore, tutorial1, tutorial2, tutorial3, tutorial4, tutorial5, gameOver };
        GameState currentGameState;


        //Fonts
        public SpriteFont font;
        public SpriteFont mediumFont;
        public SpriteFont bigFont;
        public SpriteFont smallFont;

        //Textures
        private Texture2D BG;
        private Texture2D splashLogo;
        private Texture2D menuBg;
        private Texture2D tutorialImage1;
        private Texture2D tutorialImage2;
        private Texture2D tutorialImage3;
        private Texture2D tutorialImage4;
        private Texture2D tutorialImage5;
        private Texture2D gameOverImage;
        private Texture2D creditsImage;
        private Texture2D highscoresImage;
        private Texture2D soundOn;
        private Texture2D soundOff;
        private Texture2D resume;

        //Objects
        private ParallaxingBackground backgroundTop;
        private ParallaxingBackground backgroundMid;
        private ParallaxingBackground backgroundBot;
        private Field field2;
        private Field field3;
        private Field field5;
        private Field field7;
        private Field otherField;
        private Field redField;
        private List<Number> numbers;
        private Number numberBeingDragged;
        private List<Explosion> explosions;
        private Preservervation preservation;
        private List<String> preservationList;

        //Sound
        private Song music;
        private SoundEffect acceptedNumber;
        private SoundEffect lose;
        private SoundEffect menuSelect;
        private SoundEffect specialAbility;
        private bool soundActivated;
        public bool zuneOn = false;

        //Game variables
        private bool loadingStarted = false;
        private bool loadingComplete = false;
        private float targetScore;
        private float scoreCounter;
        private float speed;
        private bool gamePaused;
        private List<Highscore> highscores;
        private String nameInput;
        public String input;

        //Time variables
        private DateTime lastNumber;
        private DateTime gameStart;
        private DateTime gameOverTime;
        private TimeSpan timeBetweenNumbers;
        private DateTime pauseTime;

        //Vectors
        private Vector2 touchPosition;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Create an adGameComponent for this game
            AdGameComponent.Initialize(this, "bb378d06-64e7-4214-97a7-a895b66d073f");
            adGameComponent = AdGameComponent.Current;
            Components.Add(adGameComponent);

            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 480;
            this.graphics.IsFullScreen = true;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            if (Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Playing)
            {
                Guide.BeginShowMessageBox("Zune music is playing", "Choose music choise",
                        new List<string> { "Game", "Zune" }, 0, MessageBoxIcon.Error,
                        asyncResult =>
                        {
                            int? returned = Guide.EndShowMessageBox(asyncResult);
                            if (returned == 0)
                            {
                                Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                                zuneOn = false;
                                MediaPlayer.Play(music);
                            }
                            else
                            {
                                zuneOn = true;
                            }
                        }, null);

            }

            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Tap;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            currentGameState = GameState.menu;
            backgroundTop = new ParallaxingBackground(this);
            backgroundMid = new ParallaxingBackground(this);
            backgroundBot = new ParallaxingBackground(this);

            backgroundTop.Initialize(121, 1f);
            backgroundMid.Initialize(142, 0.66f);
            backgroundBot.Initialize(174, 0.33f);

            field2 = new Field(this, 0, new Vector2(192, 0));
            field3 = new Field(this, 1, new Vector2(459, 0));
            field5 = new Field(this, 2, new Vector2(75, 330));
            field7 = new Field(this, 3, new Vector2(325, 330));
            otherField = new Field(this, 4, new Vector2(575, 330));
            redField = new Field(this, 5, new Vector2(725, 165));
            numbers = new List<Number>();
            explosions = new List<Explosion>();

            vibratecontroller = VibrateController.Default;

            initPreservation();

            gamePaused = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            splashLogo = Content.Load<Texture2D>("splashLogo");
        }

        private void ContinueLoad(GameTime gameTime)
        {
            loadingStarted = true;
            ForceDraw(gameTime);

            // Create a banner ad for the game.
            bannerAd = adGameComponent.CreateAd("83468", new Rectangle(0, 400, GraphicsDevice.Viewport.Bounds.Width, 80));
            bannerAd.Keywords = "math, mathematics, divide, game, prime number, even number, numbers, the human calculator, count";
            bannerAd.Visible = false;

            font = Content.Load<SpriteFont>("font");
            mediumFont = Content.Load<SpriteFont>("mediumFont");
            bigFont = Content.Load<SpriteFont>("bigFont");
            smallFont = Content.Load<SpriteFont>("smallFont");

            BG = Content.Load<Texture2D>("BG");
            menuBg = Content.Load<Texture2D>("menuBg");
            tutorialImage1 = Content.Load<Texture2D>("tutorialImage1");
            tutorialImage2 = Content.Load<Texture2D>("tutorialImage2");
            tutorialImage3 = Content.Load<Texture2D>("tutorialImage3");
            tutorialImage4 = Content.Load<Texture2D>("tutorialImage4");
            tutorialImage5 = Content.Load<Texture2D>("tutorialImage5");
            gameOverImage = Content.Load<Texture2D>("gameOverImage");
            creditsImage = Content.Load<Texture2D>("creditsImage");
            highscoresImage = Content.Load<Texture2D>("highscoresImage");
            soundOn = Content.Load<Texture2D>("soundOn");
            soundOff = Content.Load<Texture2D>("soundOff");
            resume = Content.Load<Texture2D>("resume");

            //to avoid lag when the first number and first explosion is initialized
            Content.Load<Texture2D>("Twirls");
            Content.Load<Texture2D>("HitSprite2000");

            field2.LoadContent();
            field3.LoadContent();
            field5.LoadContent();
            field7.LoadContent();
            otherField.LoadContent();
            redField.LoadContent();
            backgroundTop.LoadContent("TrailTop");
            backgroundMid.LoadContent("TrailMid");
            backgroundBot.LoadContent("TrailBot");

            music = Content.Load<Song>("sound/music");
            acceptedNumber = Content.Load<SoundEffect>("sound/accepted_number");
            lose = Content.Load<SoundEffect>("sound/lose");
            menuSelect = Content.Load<SoundEffect>("sound/menu_select");
            specialAbility = Content.Load<SoundEffect>("sound/special_ability");

            playMusic(false);

            loadingComplete = true;
        }

        private void ForceDraw(GameTime gameTime)
        {
            BeginDraw();
            Draw(gameTime);
            EndDraw();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (!loadingStarted)
            {
                ContinueLoad(gameTime);
            }



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                if (currentGameState == GameState.menu)
                {
                    this.Exit();
                }
                else
                {
                    numbers.Clear();
                    explosions.Clear();
                    numberBeingDragged = null;
                    currentGameState = GameState.menu;
                }
            }

            TouchCollection touches = TouchPanel.GetState();

            switch (currentGameState)
            {
                case GameState.credits:
                    {
                        TargetElapsedTime = TimeSpan.FromMilliseconds(500);	//decrease framerate to save battery
                        while (TouchPanel.IsGestureAvailable)
                        {
                            GestureSample gs = TouchPanel.ReadGesture();
                            if (gs.GestureType == GestureType.Tap)
                            {
                                currentGameState = GameState.menu;
                            }
                        }
                        break;
                    }
                case GameState.highscore:
                    {
                        TargetElapsedTime = TimeSpan.FromMilliseconds(300);	//decrease framerate to save battery
                        while (TouchPanel.IsGestureAvailable)
                        {
                            GestureSample gs = TouchPanel.ReadGesture();
                            if (gs.GestureType == GestureType.Tap)
                            {
                                currentGameState = GameState.menu;
                            }
                        }
                        break;
                    }
                case GameState.inGame:
                    {
                        TargetElapsedTime = TimeSpan.FromTicks(333333); //standard framerate
                        inGameUpdate(gameTime, touches);
                        break;
                    }
                case GameState.menu:
                    {
                        TargetElapsedTime = TimeSpan.FromMilliseconds(100);	//decrease framerate to save battery
                        while (TouchPanel.IsGestureAvailable)
                        {
                            GestureSample gs = TouchPanel.ReadGesture();
                            if (gs.GestureType == GestureType.Tap)
                            {
                                if (gs.Position.X > 65)
                                {
                                    if (gs.Position.X > 650 && gs.Position.Y > 330 && !zuneOn && MediaPlayer.GameHasControl)
                                    {
                                        SoundOnOff();
                                    }
                                    else if (gs.Position.X < 305 && gs.Position.Y > 130 && gs.Position.Y < 206)
                                    {
                                        newGame(true);
                                        if (isSoundAllowed()) { menuSelect.Play(); }
                                    }
                                    else if (gs.Position.X < 325 && gs.Position.Y >= 206 && gs.Position.Y < 265)
                                    {
                                        currentGameState = GameState.tutorial1;
                                        if (isSoundAllowed()) { menuSelect.Play(); }
                                    }
                                    else if (gs.Position.X < 355 && gs.Position.Y >= 265 && gs.Position.Y < 334)
                                    {
                                        currentGameState = GameState.highscore;
                                        if (isSoundAllowed()) { menuSelect.Play(); }
                                    }
                                    else if (gs.Position.X < 280 && gs.Position.Y >= 334 && gs.Position.Y < 400)
                                    {
                                        currentGameState = GameState.credits;
                                        if (isSoundAllowed()) { menuSelect.Play(); }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case GameState.tutorial1:
                    {
                        TargetElapsedTime = TimeSpan.FromMilliseconds(300);	//decrease framerate to save battery

                        while (TouchPanel.IsGestureAvailable)
                        {
                            GestureSample gs = TouchPanel.ReadGesture();
                            if (gs.GestureType == GestureType.Tap)
                            {
                                currentGameState = GameState.tutorial2;
                            }
                        }
                        break;
                    }
                case GameState.tutorial2:
                    {
                        TargetElapsedTime = TimeSpan.FromMilliseconds(300);	//decrease framerate to save battery

                        while (TouchPanel.IsGestureAvailable)
                        {
                            GestureSample gs = TouchPanel.ReadGesture();
                            if (gs.GestureType == GestureType.Tap)
                            {
                                currentGameState = GameState.tutorial3;
                            }
                        }
                        break;
                    }
                case GameState.tutorial3:
                    {
                        TargetElapsedTime = TimeSpan.FromMilliseconds(300);	//decrease framerate to save battery

                        while (TouchPanel.IsGestureAvailable)
                        {
                            GestureSample gs = TouchPanel.ReadGesture();
                            if (gs.GestureType == GestureType.Tap)
                            {
                                currentGameState = GameState.tutorial4;
                            }
                        }
                        break;

                    }
                case GameState.tutorial4:
                    {
                        TargetElapsedTime = TimeSpan.FromMilliseconds(300);	//decrease framerate to save battery

                        while (TouchPanel.IsGestureAvailable)
                        {
                            GestureSample gs = TouchPanel.ReadGesture();
                            if (gs.GestureType == GestureType.Tap)
                            {
                                currentGameState = GameState.tutorial5;
                            }
                        }
                        break;
                    }
                case GameState.tutorial5:
                    {
                        TargetElapsedTime = TimeSpan.FromMilliseconds(300);	//decrease framerate to save battery

                        while (TouchPanel.IsGestureAvailable)
                        {
                            GestureSample gs = TouchPanel.ReadGesture();
                            if (gs.GestureType == GestureType.Tap)
                            {
                                currentGameState = GameState.menu;
                            }
                        }
                        break;

                    }
                case GameState.gameOver:
                    {
                        TargetElapsedTime = TimeSpan.FromTicks(666666);	//decrease framerate to save battery

                        while (TouchPanel.IsGestureAvailable)
                        {
                            GestureSample gs = TouchPanel.ReadGesture();
                            if (gs.GestureType == GestureType.Tap)
                            {
                                if (gs.Position.X < 180 && gs.Position.Y < 90)
                                {
                                    newGame(false);
                                    currentGameState = GameState.menu;
                                }
                                else if (gs.Position.Y < 400)
                                {
                                    newGame(true);
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        //ingenting spesielt skjer
                        break;
                    }

            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            if (loadingComplete)
            {
                switch (currentGameState)
                {
                    case GameState.credits:
                        {
                            spriteBatch.Draw(creditsImage, new Vector2(), Color.White);
                            break;
                        }
                    case GameState.highscore:
                        {
                            spriteBatch.Draw(highscoresImage, new Vector2(), Color.White);
                            for (int i = 0; i < highscores.Count; i++)
                            {
                                spriteBatch.DrawString(mediumFont, (i + 1).ToString() + ".", new Vector2(90, 118 + i * 30), Color.White);
                                spriteBatch.DrawString(mediumFont, highscores[i].getName(), new Vector2(143, 118 + i * 30), Color.White);
                                spriteBatch.DrawString(mediumFont, highscores[i].getScore().ToString(), new Vector2(500, 118 + i * 30), Color.White);
                            }
                            break;
                        }
                    case GameState.inGame:
                        {
                            spriteBatch.Draw(BG, new Vector2(), Color.White);

                            backgroundMid.Draw(gameTime, spriteBatch);
                            backgroundBot.Draw(gameTime, spriteBatch);
                            backgroundTop.Draw(gameTime, spriteBatch);
                            field2.Draw(gameTime, spriteBatch);
                            field3.Draw(gameTime, spriteBatch);
                            field5.Draw(gameTime, spriteBatch);
                            field7.Draw(gameTime, spriteBatch);
                            otherField.Draw(gameTime, spriteBatch);
                            redField.Draw(gameTime, spriteBatch);
                            spriteBatch.DrawString(bigFont, "2", new Vector2(252, 37), Color.White);
                            spriteBatch.DrawString(bigFont, "3", new Vector2(519, 37), Color.White);
                            spriteBatch.DrawString(bigFont, "5", new Vector2(132, 367), Color.White);
                            spriteBatch.DrawString(bigFont, "7", new Vector2(385, 367), Color.White);

                            foreach (Number number in numbers)
                            {
                                number.Draw(gameTime, spriteBatch);
                            }

                            foreach (Explosion explosion in explosions)
                            {
                                explosion.Draw(gameTime, spriteBatch);
                            }


                            spriteBatch.DrawString(mediumFont, "Score: " + (int)scoreCounter, new Vector2(), Color.White);

                            break;
                        }
                    case GameState.menu:
                        {
                            spriteBatch.Draw(menuBg, new Vector2(), Color.White);
                            if (!zuneOn && MediaPlayer.GameHasControl)
                            {
                                if (isSoundAllowed())
                                {
                                    spriteBatch.Draw(soundOn, new Vector2(725, 405), Color.White);
                                }
                                else
                                {
                                    spriteBatch.Draw(soundOff, new Vector2(725, 405), Color.White);
                                }
                            }
                            if (gamePaused)
                            {
                                spriteBatch.Draw(resume, new Vector2(72, 136), Color.White);
                            }
                            break;
                        }
                    case GameState.tutorial1:
                        {
                            spriteBatch.Draw(tutorialImage1, new Vector2(), Color.White);
                            break;
                        }
                    case GameState.tutorial2:
                        {
                            spriteBatch.Draw(tutorialImage2, new Vector2(), Color.White);
                            break;
                        }
                    case GameState.tutorial3:
                        {
                            spriteBatch.Draw(tutorialImage3, new Vector2(), Color.White);
                            break;
                        }
                    case GameState.tutorial4:
                        {
                            spriteBatch.Draw(tutorialImage4, new Vector2(), Color.White);
                            break;
                        }
                    case GameState.tutorial5:
                        {
                            spriteBatch.Draw(tutorialImage5, new Vector2(), Color.White);
                            break;
                        }
                    case GameState.gameOver:
                        {
                            spriteBatch.Draw(gameOverImage, new Vector2(), Color.White);
                            spriteBatch.DrawString(bigFont, ((int)scoreCounter).ToString(), new Vector2(373, 214), Color.White);
                            break;
                        }

                    default:
                        {
                            //????
                            break;
                        }
                }
            }
            else
            {
                spriteBatch.Draw(splashLogo, new Vector2(125, 115), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public int getRandomInt(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private void playMusic(bool force)
        {
            if (!MediaPlayer.GameHasControl && !force)
            {
                return;
            }
            if (soundActivated)
            {
                try
                {
                    if (MediaPlayer.State == MediaState.Paused)
                    {
                        MediaPlayer.Resume();
                    }
                    else
                    {
                        MediaPlayer.Play(music);
                        MediaPlayer.IsRepeating = true;
                    }
                }
                catch { }
            }
            else
            {
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Pause();
                }
            }
        }

        private void pauseMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
            }
        }

        private void setNewSpeed()
        {
            speed = (float)((scoreCounter < 100 ? 1 : (float)Math.Pow((0.5 * Math.Log10(scoreCounter)), 2)) + DateTime.Now.Subtract(gameStart).TotalSeconds * 0.01f);
        }

        private void setTimeBetweenNumbers()
        {
            timeBetweenNumbers = TimeSpan.FromMilliseconds(getRandomInt((int)(500 + 1500 / (speed * 0.5)), (int)(700 + 6000 / (speed * 0.7))));
        }

        private int getNextNumberValue()
        {
            return getRandomInt(1, (int)(9 * Math.Pow(1.5, speed)));
        }

        private Number getNextNumber()
        {
            return new Number(this, getNextNumberValue(), new Vector2(-90, getRandomInt(150, 240)));
        }

        private void initGameOver(TimeSpan timeToGameOver)
        {
            if (timeToGameOver.TotalMilliseconds > 0)
            {
                gameOverTime = DateTime.Now.Add(timeToGameOver);
            }
            else
            {
                gameOver();
            }
        }

        private void gameOver()
        {
            setHighscores();
            currentGameState = GameState.gameOver;
            bannerAd.Visible = true;
            if (isSoundAllowed()) { lose.Play(); }
            numbers.Clear();
            explosions.Clear();
            numberBeingDragged = null;

            vibratecontroller.Start(TimeSpan.FromMilliseconds(200));

        }
        private void newGame(bool goToGame)
        {
            if (goToGame)
            {
                currentGameState = GameState.inGame;
            }
            bannerAd.Visible = false;
            targetScore = 0;
            scoreCounter = targetScore;
            speed = 1;
            lastNumber = DateTime.Now;
            gameStart = DateTime.Now;
            gameOverTime = DateTime.Now.AddYears(1);
            setTimeBetweenNumbers();
        }

        private float distanceToNumberCenter(Vector2 fromPoint, Vector2 numberPoint)
        {
            return (float)Math.Sqrt(Math.Pow((numberPoint.X + 45 - fromPoint.X), 2) + Math.Pow((numberPoint.Y + 45 - fromPoint.Y), 2));
        }

        private void sendNumberToField()
        {
            numberBeingDragged.goesToDestination = true;
            numberBeingDragged.dropTime = DateTime.Now;
            int destinationFieldValue;
            if (numberBeingDragged.getPosition().Y > 195)
            {
                if (numberBeingDragged.getPosition().X > 490)
                {
                    destinationFieldValue = 10;
                    numberBeingDragged.setDestinationField(new Vector2(605, 360));
                }
                else if (numberBeingDragged.getPosition().X > 230)
                {
                    destinationFieldValue = 7;
                    numberBeingDragged.setDestinationField(new Vector2(355, 360));
                }
                else
                {
                    destinationFieldValue = 5;
                    numberBeingDragged.setDestinationField(new Vector2(105, 360));
                }
            }
            else
            {
                if (numberBeingDragged.getPosition().X > 355)
                {
                    numberBeingDragged.setDestinationField(new Vector2(490, 30));
                    destinationFieldValue = 3;
                }
                else
                {
                    numberBeingDragged.setDestinationField(new Vector2(220, 30));
                    destinationFieldValue = 2;
                }
            }
            int numberValue = numberBeingDragged.getValue();
            if (destinationFieldValue == 2 && numberValue % 2 == 0
                || destinationFieldValue == 3 && numberValue % 3 == 0
                || destinationFieldValue == 5 && numberValue % 5 == 0
                || destinationFieldValue == 7 && numberValue % 7 == 0)
            {
                if (isSoundAllowed()) { acceptedNumber.Play(); }

                targetScore += 2.5f * destinationFieldValue * speed * (float)(Math.Log(0.6f * numberValue) + 0.02f * numberValue);
            }
            else if (destinationFieldValue == 10)
            {
                if (numberValue % 2 == 0
                    || numberValue % 3 == 0
                    || numberValue % 5 == 0
                    || numberValue % 7 == 0)
                {
                    initGameOver(TimeSpan.FromMilliseconds(250));
                }
                else
                {
                    if (isSoundAllowed()) { acceptedNumber.Play(); }
                    targetScore += 25f * speed * (float)(Math.Log(0.6f * (numberValue < 2 ? 2 : numberValue)) + 0.02f * numberValue);
                }
            }
            else
            {
                initGameOver(TimeSpan.FromMilliseconds(250));
            }

            numberBeingDragged = null;
        }

        public Vector2 getTouchPosition()
        {
            return touchPosition;
        }

        public float getSpeed()
        {
            return speed;
        }

        private void inGameUpdate(GameTime gameTime, TouchCollection touches)
        {
            scoreCounter += 0.05f * (targetScore - scoreCounter);
            setNewSpeed();

            if (DateTime.Now.Subtract(gameOverTime).TotalMilliseconds > 0)
            {
                gameOver();
                return;
            }

            if (DateTime.Now.Subtract(lastNumber).TotalMilliseconds > (numbers.Count > 0 ? timeBetweenNumbers.TotalMilliseconds : timeBetweenNumbers.TotalMilliseconds * 0.01))
            {
                setTimeBetweenNumbers();
                lastNumber = DateTime.Now;
                Number newNumber = getNextNumber();
                numbers.Add(newNumber);
                newNumber.LoadContent();
                newNumber.Initialize();
                targetScore += (float)(9 * speed + 0.5 * Math.Pow(Math.Log10(DateTime.Now.Subtract(gameStart).TotalMilliseconds), 2));
            }

            int k = 0;
            while (k < numbers.Count)
            {
                numbers[k].Update(gameTime);
                if (DateTime.Now.Subtract(numbers[k].dropTime).TotalMilliseconds > 250 && numbers[k].goesToDestination)
                {
                    Explosion explosion = new Explosion(this, numbers[k].getPosition());
                    explosion.LoadContent();
                    explosions.Add(explosion);
                    numbers.RemoveAt(k);
                }
                else
                {
                    if (distanceToNumberCenter(new Vector2((float)(redField.getPosition().X + 75), (float)(redField.getPosition().Y + 75)), numbers[k].getPosition()) < 100 && numberBeingDragged != numbers[k])
                    {
                        initGameOver(TimeSpan.FromMilliseconds(0));
                    }
                    k++;
                }
            }

            // get any gestures that are ready.
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gs = TouchPanel.ReadGesture();
                if (gs.GestureType == GestureType.FreeDrag)
                {
                    touchPosition = gs.Position;
                    if (numberBeingDragged == null)
                    {
                        if (numbers.Count > 0)
                        {
                            float shortestDistance = distanceToNumberCenter(touchPosition, numbers[0].getPosition());
                            numberBeingDragged = numbers[0];
                            for (int i = 1; i < numbers.Count; i++)
                            {
                                float distance = distanceToNumberCenter(touchPosition, numbers[i].getPosition());
                                if (distance < shortestDistance)
                                {
                                    shortestDistance = distance;
                                    numberBeingDragged = numbers[i];
                                }
                            }
                            if (numberBeingDragged.goesToDestination)
                            {
                                numberBeingDragged = null;
                            }
                        }
                    }
                }
            }

            //Touch released
            if (touches.Count == 0 && numberBeingDragged != null)
            {
                if (numberBeingDragged.getPosition().Y < 150 || numberBeingDragged.getPosition().Y > 240)
                {
                    sendNumberToField();
                }
                else
                {
                    numberBeingDragged.snapToTouchPoint = false;
                    numberBeingDragged = null;
                }
            }

            field2.Update(gameTime);
            field3.Update(gameTime);
            field5.Update(gameTime);
            field7.Update(gameTime);
            otherField.Update(gameTime);
            redField.Update(gameTime);
            backgroundTop.Update(gameTime);
            backgroundMid.Update(gameTime);
            backgroundBot.Update(gameTime);

            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].deleter()) { explosions.RemoveAt(i); }
            }
        }

        public Number getNumberBeingDragged()
        {
            return this.numberBeingDragged;
        }

        public bool isGamePaused()
        {
            return gamePaused;
        }

        private void SoundOnOff()
        {
            soundActivated = !soundActivated;
            playMusic(true);
            preservationList[0] = soundActivated ? "sound1" : "sound0";
            preserveData();
        }

        private void initPreservation()
        {
            highscores = new List<Highscore>();

            preservation = new Preservervation();
            preservationList = new List<String>();
            preservation.loadData();
            preservationList = preservation.getData();

            if (preservationList.Count >= 12)
            {
                String test = preservationList[0];
                bool testbool = test == "sound1";
                soundActivated = (preservationList[0] == "sound1");
                nameInput = preservationList[1];
                for (int i = 2; i < 12; i++)
                {
                    if (preservationList[i] != "")
                    {
                        int colonIndex = preservationList[i].LastIndexOf(':');
                        int highscoreScore = Convert.ToInt32(preservationList[i].Substring(colonIndex + 1));
                        String highscoreName = preservationList[i].Substring(0, colonIndex);
                        highscores.Add(new Highscore(highscoreName, highscoreScore, DateTime.Now));
                    }
                }
            }
            else
            {
                soundActivated = true;
                preservationList.Add("sound1");
                nameInput = "";
                for (int i = 0; i < 11; i++)
                {
                    preservationList.Add("");
                }
                preserveData();
            }
        }

        private void preserveData()
        {
            preservation.setData(preservationList);
            preservation.saveData();
        }

        private void setHighscores()
        {
            bool addHighscore = false;
            if (highscores.Count == 10)
            {
                if ((int)scoreCounter > highscores[highscores.Count - 1].getScore())
                {
                    addHighscore = true;
                }
            }
            else
            {
                addHighscore = true;
            }

            if (!Guide.IsVisible && addHighscore)
            {
                Guide.BeginShowKeyboardInput(PlayerIndex.One, "New highscore: " + ((int)scoreCounter).ToString(), "Enter your name", nameInput, delegate(IAsyncResult result)
                {
                    nameInput = Guide.EndShowKeyboardInput(result);
                    preservationList[1] = nameInput;
                    highscores.Add(new Highscore(nameInput, (int)scoreCounter, DateTime.Now));
                    highscores.Sort();
                    if (highscores.Count > 10) { highscores.RemoveAt(10); }
                    for (int i = 0; i < highscores.Count; i++)
                    {
                        preservationList[i + 2] = highscores[i].ToString();
                    }
                    preserveData();
                }, null);
            }
        }

        private bool isSoundAllowed()
        {
            return soundActivated && MediaPlayer.GameHasControl;
        }
    }
}
