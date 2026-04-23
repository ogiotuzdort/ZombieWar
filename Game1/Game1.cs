using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ZombieWar
{
    public class Bullet
    {
        public Vector2 Position;
        public Vector2 Direction; 
        public float Speed = 800f;
        public bool IsExpired = false;

        public Bullet(Vector2 startPosition, Vector2 targetPosition)
        {
            Position = startPosition;
            Direction = targetPosition - startPosition;
            if (Direction != Vector2.Zero) Direction.Normalize();
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Direction * Speed * deltaTime;

            // Ekran sınırları kontrolü
            if (Position.X < 0 || Position.X > 800 || Position.Y < 0 || Position.Y > 600)
                IsExpired = true;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, 10, 10), Color.Yellow);
        }
    }

    public class Enemy
    {
        public Vector2 Position;
        public float Speed;
        public int Health;
        public int Size;
        public Color EnemyColor;

        public Enemy(Vector2 startPosition, bool isBoss)
        {
            Position = startPosition;
            if (isBoss)
            {
                Speed = 50f;     
                Health = 10;     
                Size = 100;      
                EnemyColor = Color.DarkRed;
            }
            else
            {
                Speed = 120f;    
                Health = 1;      
                Size = 40;       
                EnemyColor = Color.Brown;
            }
        }

        public void Update(GameTime gameTime, Vector2 playerPos)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 direction = playerPos - Position;
            if (direction != Vector2.Zero) direction.Normalize();
            Position += direction * Speed * deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, Size, Size), EnemyColor);
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Vector2 _playerPos = new Vector2(400, 300);
        float _playerSpeed = 300f;
        
        List<Bullet> _bullets = new List<Bullet>();
        List<Enemy> _enemies = new List<Enemy>();
        
        Texture2D _pixel;
        float _shootTimer = 0;
        float _spawnTimer = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            _enemies.Add(new Enemy(new Vector2(100, 100), false)); // Normal Zombi
            _enemies.Add(new Enemy(new Vector2(700, 50), true));   // Boss
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.W)) _playerPos.Y -= _playerSpeed * deltaTime;
            if (kState.IsKeyDown(Keys.S)) _playerPos.Y += _playerSpeed * deltaTime;
            if (kState.IsKeyDown(Keys.A)) _playerPos.X -= _playerSpeed * deltaTime;
            if (kState.IsKeyDown(Keys.D)) _playerPos.X += _playerSpeed * deltaTime;

            var mState = Mouse.GetState();
            _shootTimer += deltaTime;
            if (mState.LeftButton == ButtonState.Pressed && _shootTimer > 0.2f)
            {
                _bullets.Add(new Bullet(_playerPos + new Vector2(20, 20), new Vector2(mState.X, mState.Y)));
                _shootTimer = 0;
            }

            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(gameTime);
                if (_bullets[i].IsExpired) _bullets.RemoveAt(i);
            }

            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime, _playerPos);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20)); // Karanlık atmosfer
            _spriteBatch.Begin();

            foreach (var enemy in _enemies) enemy.Draw(_spriteBatch, _pixel);

            foreach (var bullet in _bullets) bullet.Draw(_spriteBatch, _pixel);

            _spriteBatch.Draw(_pixel, new Rectangle((int)_playerPos.X, (int)_playerPos.Y, 50, 50), Color.LimeGreen);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}