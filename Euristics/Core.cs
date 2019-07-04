using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WinD = System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Euristics
{



    public class Core : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static Random rand = new Random();

        public static float Rand => (float)rand.NextDouble();


        public static Viewport PrimaryViewport;

        public Core()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Simplex.Init(GraphicsDevice);
            TargetElapsedTime = new TimeSpan(11110);
            base.Initialize();
        }



        Timer t = new Timer(100);
        Timer successpopup = new Timer(600);

        public static SpriteFont font;
        int totalgens;
        int successgens;

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("fonts//Xolonium");
            font.DefaultCharacter = ' ';
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Generate();

            t.OnFinish += () =>
            {
                Genetics.Evolve(generation, () => Rand * Rand * Rand * Rand);
                //t.Reset(false);
                t.Start();
            };

            t.Start();
        }

        List<ResultEntry> results = new List<ResultEntry>();
        ResultEntry last;
        Generation generation;

        public static int MaxGenes = 1000;

        void Generate()
        {
            if (generation != null && generation.genes.Count > MaxGenes)
            {
                results.Add(new ResultEntry(generation));
                if (!successpopup.IsRunning)
                {
                    successpopup.Reset(false);
                    successpopup.Start();
                    last = results[results.Count-1];
                }
                successgens++;
            }
            List<Gene> genes = new List<Gene>();
            for (int i = 0; i < 20; i++)
            {
                // [Energy : ResAmount : Speed] => Result
                genes.Add(new Gene(Rand * 20, Rand * 100, Rand * 100, 0));
            }
            generation = new Generation(genes);
            totalgens++;
        }








        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            PrimaryViewport = GraphicsDevice.Viewport;
            Control.Update();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            t.Update(gameTime.ElapsedGameTime.Milliseconds);
            successpopup.Update(gameTime.ElapsedGameTime.Milliseconds);
            if (generation.CanRestart)
                Generate();
            base.Update(gameTime);
        }









        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(40, 40, 40));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            {
                //successgens / totalgens
                spriteBatch.DrawFill(new Rectangle(0, 0, 225, 800), Color.Gray);
                if (totalgens > 0)
                {
                    spriteBatch.DrawString(font, $"Total   -   {totalgens} \nSuccess ({successgens})  -   {(1f * successgens / totalgens):0.000}", new Vector2(245, 10), Color.White);
                    int i = 0, j = 0;
                    ResultEntry hov = new ResultEntry();
                    foreach (var r in results)
                    {
                        if (i > 36)
                        {
                            j += 1;
                            i = 0;
                        }
                        var rc = new Rectangle(1 + 6 * i, 1 + 6 * j, 5, 5);
                        if (rc.Contains(Control.MousePos))
                        {
                            hov = r;
                        }

                        spriteBatch.DrawFill(rc, new Color(r.data[0] / 100, r.data[1] / 100, r.data[2] / 100));
                        i++;
                    }
                    float x = successpopup.Progress;
                    float y = (float)(-(x - 1) * (x + 1) / 0.5 / 2);
                    if (!last.Equals(default(ResultEntry)))
                        spriteBatch.DrawString(font, $"+S[{last.data[0]:0.00}   |   {last.data[1]:0.00}   |   {last.data[2]:0.00}]", new Vector2(15, 10 + 30 * y), last.col * (1 - y));

                    if (!hov.Equals(default(ResultEntry)))
                    {
                        Rectangle dr;
                        if (PrimaryViewport.Height - Control.MousePos.Y < 100)
                            dr = new Rectangle(Control.MousePos.ToPoint() - new Point(100, 100), new Point(100, 100));
                        else dr = new Rectangle(Control.MousePos.ToPoint(), new Point(100, 100));
                        spriteBatch.DrawFill(dr, new Color(hov.data[0] / 100, hov.data[1] / 100, hov.data[2] / 100));
                        spriteBatch.DrawString(font, $"E : {(hov.data[0]):0.000}\nR:{(hov.data[1]):0.000}\nS:{(hov.data[2]):0.000}", Control.MousePos + new Vector2(20), Color.White);
                    }
                }
            }
            spriteBatch.End();
            generation.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
