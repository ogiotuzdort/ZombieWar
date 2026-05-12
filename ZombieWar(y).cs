using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ZombieWar
{
    public class Game1 : Game
    {
        GraphicsDeviceManager g;
        SpriteBatch sb;
        Texture2D px;

        Vector2 p = new Vector2(400, 300);
        float ps = 320f;

        List<Bullet> b = new();
        List<Enemy> e = new();
        List<Weapon> w = new();

        int wi;
        float st, sp;
        int php = 100, pmax = 100;

        Random r = new Random();

        public Game1()
        {
            g = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            g.PreferredBackBufferWidth = 800;
            g.PreferredBackBufferHeight = 600;
        }

        protected override void LoadContent()
        {
            sb = new SpriteBatch(GraphicsDevice);

            px = new Texture2D(GraphicsDevice, 1, 1);
            px.SetData(new[] { Color.White });

            w.Add(new Weapon("P", .25f, 900f, 1, 1));
            w.Add(new Weapon("S", .08f, 1100f, 1, 1));
            w.Add(new Weapon("SG", .6f, 700f, 6, 1));
        }

        protected override void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            var k = Keyboard.GetState();
            var m = Mouse.GetState();

            if (k.IsKeyDown(Keys.Escape)) Exit();

            Vector2 mv = Vector2.Zero;

            if (k.IsKeyDown(Keys.W)) mv.Y--;
            if (k.IsKeyDown(Keys.S)) mv.Y++;
            if (k.IsKeyDown(Keys.A)) mv.X--;
            if (k.IsKeyDown(Keys.D)) mv.X++;

            if (mv != Vector2.Zero) mv.Normalize();

            p += mv * ps * dt;
            p = Vector2.Clamp(p, Vector2.Zero, new Vector2(750, 550));

            if (k.IsKeyDown(Keys.D1)) wi = 0;
            if (k.IsKeyDown(Keys.D2)) wi = 1;
            if (k.IsKeyDown(Keys.D3)) wi = 2;

            if (k.IsKeyDown(Keys.F))
            {
                w.Clear();
                w.Add(new Weapon("P", .25f, 900f, 1, 1));
                w.Add(new Weapon("S", .08f, 1100f, 1, 1));
                w.Add(new Weapon("SG", .6f, 700f, 6, 1));
            }

            if (w.Count == 0)
                w.Add(new Weapon("P", .25f, 900f, 1, 1));

            st += dt;

            if (m.LeftButton == ButtonState.Pressed && st > w[wi].r)
            {
                Vector2 t = new Vector2(m.X, m.Y);

                for (int i = 0; i < w[wi].p; i++)
                {
                    float spread = (float)(r.NextDouble() - 0.5) * 0.25f;

                    Vector2 dir = t - p;
                    dir = Rot(dir, spread);

                    Vector2 start = p + new Vector2(20, 20);

                    b.Add(new Bullet(start, p + dir));
                }

                st = 0;
            }

            for (int i = b.Count - 1; i >= 0; i--)
            {
                b[i].Update(dt);
                if (b[i].d) b.RemoveAt(i);
            }

            sp += dt;

            if (sp > 1.2f)
            {
                e.Add(new Enemy(new Vector2(r.Next(800), -20)));
                sp = 0;
            }

            foreach (var x in e)
            {
                x.Update(dt, p);
                if (Vector2.Distance(p, x.p) < 25)
                    php -= 20;
            }

            for (int i = b.Count - 1; i >= 0; i--)
            {
                for (int j = e.Count - 1; j >= 0; j--)
                {
                    if (b[i].b.Intersects(e[j].b))
                    {
                        e[j].h--;
                        b[i].d = true;

                        if (e[j].h <= 0)
                            e.RemoveAt(j);

                        break;
                    }
                }
            }

            b.RemoveAll(x => x.d);

            base.Update(gt);
        }

        Vector2 Rot(Vector2 v, float a)
        {
            float c = (float)Math.Cos(a);
            float s = (float)Math.Sin(a);
            return new Vector2(v.X * c - v.Y * s, v.X * s + v.Y * c);
        }

        protected override void Draw(GameTime gt)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20));

            sb.Begin();

            DrawBar(new Vector2(10, 10), 200, 20, php, pmax, Color.Lime);

            sb.Draw(px, new Rectangle((int)p.X, (int)p.Y, 40, 40), Color.Lime);

            foreach (var x in e)
            {
                sb.Draw(px, x.b, Color.DarkRed);
                DrawBar(new Vector2(x.p.X, x.p.Y - 8), 40, 5, x.h, 3, Color.Red);
            }

            foreach (var x in b)
                sb.Draw(px, x.b, Color.Yellow);

            sb.End();
        }

        void DrawBar(Vector2 pos, int w, int h, int v, int m, Color c)
        {
            float f = MathHelper.Clamp((float)v / m, 0, 1);

            sb.Draw(px, new Rectangle((int)pos.X, (int)pos.Y, w, h), Color.DarkGray);
            sb.Draw(px, new Rectangle((int)pos.X, (int)pos.Y, (int)(w * f), h), c);
        }
    }

    

    public class Enemy
    {
        public Vector2 p;
        public int h = 3;

        public Rectangle b => new Rectangle((int)p.X, (int)p.Y, 40, 40);

        public Enemy(Vector2 p)
        {
            this.p = p;
        }

        public void Update(float dt, Vector2 pl)
        {
            Vector2 d = pl - p;

            if (d != Vector2.Zero)
                d.Normalize();

            p += d * 120f * dt;
        }
    }

    public class Weapon
    {
        public string n;
        public float r;
        public float s;
        public int p;
        public int d;

        public Weapon(string n, float r, float s, int p, int d)
        {
            this.n = n;
            this.r = r;
            this.s = s;
            this.p = p;
            this.d = d;
        }
    }
}