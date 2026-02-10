using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson07Pong;

public class Ball
{
    private Rectangle _playAreaBoundingBox;

    private Texture2D _texture;
    private Vector2 _position, _direction, _dimensions;
    private float _speed;

    // _ball.Initialize(new Vector2(150, 195), 60, new Vector2(21, 21), new Vector2(-1, -1));
    internal void Initialize
        (Vector2 position, float speed, Vector2 dimensions, Vector2 direction, Rectangle playAreaBoundingBox)
    {
        _position = position;
        _speed = speed;
        _dimensions = dimensions;
        _direction = direction;
        _playAreaBoundingBox = playAreaBoundingBox;
    }

    internal void LoadContent(ContentManager content)
    {
        _texture = content.Load<Texture2D>("Ball");
    }

    internal void Update(GameTime gameTime)
    {
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        _position += _direction * _speed * dt;

        //bounce the ball off left and right sides
        if(_position.X <= _playAreaBoundingBox.Left || 
            _position.X + _dimensions.X >= _playAreaBoundingBox.Right)
        {
            _direction.X *= -1;
        }
        //in-class exercise: make the ball bounce off of the top and bottom of the play area bounding box
        if(_position.Y <= _playAreaBoundingBox.Top || 
            _position.Y + _dimensions.Y >= _playAreaBoundingBox.Bottom)
        {
            _direction.Y *= -1;
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        Rectangle ballRectangle = 
            new Rectangle((int) _position.X, (int) _position.Y, (int) _dimensions.X, (int) _dimensions.Y);
        
        spriteBatch.Draw(_texture, ballRectangle, Color.White);
    }
}