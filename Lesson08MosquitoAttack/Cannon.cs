using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson08MosquitoAttack;

public class Cannon : Actor
{
    private const int _NumProjectiles = 5;

    private Point _dimensions;

    private Projectile[] _projectiles;

    internal Vector2 Direction 
    { 
        set
        {
            value.Y = 0;
            _direction = value; 
            _animationAlive.Reverse = _direction.X < 0;
        }
    }
    
    internal void Initialize(Vector2 position, float speed, Rectangle gameBoundingBox)
    {
        _position = position;
        _speed = speed;
        _gameBoundingBox = gameBoundingBox;

        _projectiles = new Projectile[_NumProjectiles];

        _projectiles[0] = new CannonBall();
        _projectiles[1] = new FireBall();
        _projectiles[2] = new FireBall();
        _projectiles[3] = new CannonBall();
        _projectiles[4] = new CannonBall();

        for(int c = 0; c < _NumProjectiles; c++)
        {
            _projectiles[c].Initialize(50, _gameBoundingBox);
        } 
    }

    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Cannon");
        _dimensions = new Point(texture.Width / 4, texture.Height);
        _animationAlive = new SimpleAnimation(texture, _dimensions.X, _dimensions.Y, 4, 2f);
        foreach(Projectile p in _projectiles)
            p.LoadContent(content);
    }
    internal void Update(GameTime gameTime)
    {
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        _position += _speed * _direction * dt;

        if(_direction != Vector2.Zero)
            _animationAlive.Update(gameTime);

        foreach(Projectile p in _projectiles)
            p.Update(gameTime);
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        if(_animationAlive != null)
            _animationAlive.Draw(spriteBatch, _position, SpriteEffects.None);
        foreach(Projectile p in _projectiles)
            p.Draw(spriteBatch);
    }

    internal void Shoot()
    {
        foreach(Projectile p in _projectiles)
        {
            if(p.Launchable)
            {
                float projectilePositionY = BoundingBox.Top - p.BoundingBox.Height;
                float projectilesPositionX = BoundingBox.Center.X - p.BoundingBox.Width / 2;
                p.Launch(new Vector2(projectilesPositionX, projectilePositionY), new Vector2(0, -1));
                //we have found one to launch, time to abort
                return; // or "break;"
            }
        }  
    }

    internal bool ProcessCollision(Rectangle boundingBox)
    {
        foreach(Projectile p in _projectiles)
        {
            if(p.ProcessCollision(boundingBox))
                return true;
        }
        return false;
    }
}