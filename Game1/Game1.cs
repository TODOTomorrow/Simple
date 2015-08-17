using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    public class GameException : System.Exception
    {
        public String why;
        public int level;
        public const int LevelCatastrophic = 10;
        public const int LevelWarning = 5;
        public const int LevelInfo = 0;
        public GameException(String why = "No errors", int level = LevelCatastrophic)
        {
            this.why = why;
            this.level = level;
        }
    }
    public class Debugger
    {
        string fileName;
        Game g;
        public Debugger(String fileName, Game g)
        {
            this.fileName = fileName;
            System.IO.File.WriteAllText(fileName, "");
            this.g = g;
        }
        public void Log(String text, int level = GameException.LevelInfo)
        {
            System.IO.File.WriteAllText(fileName, System.IO.File.ReadAllText(fileName) +
                "\n[" + DateTime.Now.ToString() + "] " + text);
        }
        public void Log(GameException e)
        {
            System.IO.File.WriteAllText(fileName, System.IO.File.ReadAllText(fileName) +
               "\n[" + DateTime.Now.ToString() + "] " + e.why);
            if (e.level >= GameException.LevelCatastrophic)
                g.Exit();
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        UserControl uc;
        public static Debugger dbg;
        Stage scn;
        Actor mainHero;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Tile.g = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            dbg = new Debugger("DebugInfo.txt", this);
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
            ResourceLoader rsldr = new ResourceLoader("./Content/main.bgs");
            mainHero = rsldr.Persons[0];
            scn = rsldr.CreateStage(spriteBatch,"main");
            uc = new UserControl();
            uc.onKeydown += keyDowned;
            uc.onMouseMove += mouseMoved;
            IsMouseVisible = true;
        }

        void keyDowned(KeyboardState ks)
        {
            int k = 10;
            Vector2 curPos = mainHero.position;
            if (mainHero.State == ActorState.Move) return;
            if (ks.IsKeyDown(Keys.W))   curPos.Y-=k;
            if (ks.IsKeyDown(Keys.A))   curPos.X-=k;
            if (ks.IsKeyDown(Keys.S))   curPos.Y+=k;
            if (ks.IsKeyDown(Keys.D))   curPos.X+=k;

            if ((curPos != mainHero.position) && !(scn.Background.TryCollision(mainHero, curPos)))
                mainHero.Move(curPos);
        }
        void mouseMoved(Vector2 c)
        {
            mainHero.Rotate(c);
        }
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            uc.Update(gameTime);
            scn.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            scn.Draw();
            base.Draw(gameTime);
        }
    }
}
