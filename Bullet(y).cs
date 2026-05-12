using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Bullet
    {
        public Vector2 p, v;
        public bool d;

        public Rectangle b => new Rectangle((int)p.X, (int)p.Y, 6, 6);

        public Bullet(Vector2 s, Vector2 t)
        {
            p = s;

            v = t - s;
            if (v != Vector2.Zero)
                v.Normalize();

            v *= 900f;
        }

        public void Update(float dt)
        {
            p += v * dt;

            if (p.X < -50 || p.X > 900 || p.Y < -50 || p.Y > 700)
                d = true;
        }
    }