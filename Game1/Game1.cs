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
            ResourceLoader rsldr = new ResourceLoader("./Content/init.lua");
            mainHero = rsldr.Persons[0];
            scn = rsldr.CreateStage(spriteBatch,"main");
            uc = new UserControl();
            uc.onKeydown += keyDowned;
            uc.onMouseLeftDown += mouseMoved;
            IsMouseVisible = true;
            scn.CalibrateCollisions();
            scn.Reindex();
            mainHero.onCollision += onCollision;
            mainHero.onMove += onActorMoved;
            mainHero.speed = 0.5f;
            mainHero.RotateSpeed = 100;
            scn.window.X = 100;
            scn.window.Y = 100;
            scn.onMapOut += onSomebodyMapout;
            //mainHero.Rotate((float)(Math.PI*2/3));
            //makeMoveTest(mainHero);
        }
        void onSomebodyMapout(Actor act)
        {
            act.Stop();
            if (act.position.X < 0) act.position.X = 0;
            if (act.position.Y < 0) act.position.Y = 0;
            if (act.position.X + act.Width > scn.Background.WidthPix) act.position.X = scn.Background.WidthPix - act.Width;
            if (act.position.Y + act.Height > scn.Background.HeightPix) act.position.Y = scn.Background.HeightPix - act.Height;
        }
        void onActorMoved(Actor act)
        {
            int newX = 0, newY = 0;
            if (ActorPositionInScreen(act).X > 500) newX += 5;
            if (ActorPositionInScreen(act).X < 100) newX -= 5;
            if (ActorPositionInScreen(act).Y < 100) newY -= 5;
            if (ActorPositionInScreen(act).Y > 300) newY += 5;
            if (scn.window.X + newX >= 0) scn.window.X += newX;
            if (scn.window.Y + newY >=0) scn.window.Y +=newY;
        }
        Vector2 ActorPositionInScreen(Actor act)
        {
            return (act.position - scn.window.Location.ToVector2());
        }
        void makeMoveTest(Actor a)
        {
            List<Vector2> points = new List<Vector2>();
            for (int i=0;i<500;i+=10)
                for (int j = 0; j < 500; j+=10)
                {
                    if (scn.TryCollision(a,new Vector2(i,j)))
                        points.Add(new Vector2(i,j));
                }
        }
        void onCollision(Object a,Object b)
        {

        }
        void keyDowned(KeyboardState ks)
        {
            int k = 5;
            Vector2 curPos = mainHero.position;
            float rotateAngle = 0;
            if (mainHero.State == ActorState.Move) return;
            if (ks.IsKeyDown(Keys.W)) { curPos.Y -= k; rotateAngle = (float)Math.PI * 3 / 2; }
            if (ks.IsKeyDown(Keys.A)) { curPos.X -= k; rotateAngle = (float)Math.PI; }
            if (ks.IsKeyDown(Keys.S)) { curPos.Y += k; rotateAngle = (float)Math.PI / 2; }
            if (ks.IsKeyDown(Keys.D)) { curPos.X += k; rotateAngle = 0; }
            mainHero.Rotate(rotateAngle);
            if ((curPos != mainHero.position) && !(scn.TryCollision(mainHero, curPos)))
                mainHero.Move(curPos);
        }
        int counter = 0;
        void mouseMoved(Vector2 c)
        {
            
         /*   if (counter==2)
                mainHero.Rotate((float)Math.PI / 2);
            if (counter == 1)
                mainHero.Rotate((float)Math.PI);
            if (counter == 0)
                mainHero.Rotate((float)Math.PI * 3 / 2);
            if (counter == 3)
                mainHero.Rotate((float)Math.PI *2);
            counter++;*/
            if (counter == 4) counter = 0;
            //if (!(scn.Background.TryCollision(mainHero, mainHero.position, c)))
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
