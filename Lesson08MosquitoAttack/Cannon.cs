using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson08MosquitoAttack;

public class Cannon
{
    private SimpleAnimation _animation;
    private Vector2 _position, _direction;
    private Point _dimensions;
    private float _speed;

    internal Vector2 Direction 
    { 
        set
        {
            value.Y = 0;
            _direction = value; 
            _animation.Reverse = _direction.X < 0;
        }
    }

    internal void Initialize(Vector2 position, float speed)
    {
        _position = position;
        _speed = speed;
    }

    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Cannon");
        _dimensions = new Point(texture.Width / 4, texture.Height);
        _animation = new SimpleAnimation(texture, _dimensions.X, _dimensions.Y, 4, 2f);
    }

    internal void Update(GameTime gameTime)
    {
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        _position += _speed * _direction * dt;

        if(_direction != Vector2.Zero)
            _animation.Update(gameTime);
    }
    
    internal void Draw(SpriteBatch spriteBatch)
    {
        if(_animation != null)
            _animation.Draw(spriteBatch, _position, SpriteEffects.None);
    }
}