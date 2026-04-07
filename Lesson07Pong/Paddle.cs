using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson07Pong;

public class Paddle
{
    private Rectangle _playAreaBoundingBox;

    private Texture2D _texture;
    private Vector2 _position, _direction, _dimensions;
    private float _speed;

    // a "derived property", meaning that we "create" (derive) it from other data members
    internal Rectangle BoundingBox
    {
        get
        {
            return new Rectangle(_position.ToPoint(), _dimensions.ToPoint());
        }
    }

    internal Vector2 Direction
    {
        set
        {
            //make sure that the paddle is never moved horizontally
            value.X = 0;
            _direction = value;
        }
    }

    internal void Initialize
        (Vector2 position, float speed, Vector2 dimensions, Rectangle playAreaBoundingBox)
    {
        _position = position;
        _speed = speed;
        _dimensions = dimensions;
        _direction = Vector2.Zero;
        _playAreaBoundingBox = playAreaBoundingBox;
    }

    internal void LoadContent(ContentManager content)
    {
        _texture = content.Load<Texture2D>("Paddle");
    }

    internal void Update(GameTime gameTime)
    {
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        _position += _direction * _speed * dt;

        if(_position.Y <= _playAreaBoundingBox.Top)
        {
            _position.Y = _playAreaBoundingBox.Top;
        }
        else if( (_position.Y + _dimensions.Y) >= _playAreaBoundingBox.Bottom)
        {
            _position.Y = _playAreaBoundingBox.Bottom - _dimensions.Y;
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        Rectangle paddleRectangle = new Rectangle(_position.ToPoint(), _dimensions.ToPoint());
        spriteBatch.Draw(_texture, paddleRectangle, Color.Azure);
    }
}