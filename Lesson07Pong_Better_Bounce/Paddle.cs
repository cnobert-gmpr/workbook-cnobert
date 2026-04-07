using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson07Pong_Better_Bounce;

public class Paddle
{
    private Texture2D _texture;
    private Vector2 _position, _dimensions, _direction;
    private float _speed;

    private Rectangle _playAreaBoundingBox;

    // === CHANGE START: track velocity so ball can be influenced by paddle movement ===
    internal Vector2 Velocity
    {
        get;
        private set;
    }
    // === CHANGE END ===

    internal Rectangle BoundingBox
    {
        get
        {
            return new Rectangle(_position.ToPoint(), _dimensions.ToPoint());
        }
    }

    internal Vector2 Direction 
    { 
        set => _direction = value; 
    }

    internal void Initialize (Vector2 position, Vector2 dimensions, float speed, Rectangle playAreaBoundingBox)
    {
        _position = position;
        _dimensions = dimensions;
        _direction = Vector2.Zero;
        _speed = speed;
        _playAreaBoundingBox = playAreaBoundingBox;

        // === CHANGE START: init velocity tracking ===
        Velocity = Vector2.Zero;
        // === CHANGE END ===
    }

    internal void LoadContent(ContentManager content)
    {
        _texture = content.Load<Texture2D>("Paddle");
    }

    internal void Update(GameTime gameTime)
    {
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

        // === CHANGE START: compute velocity (pixels/sec) ===
        Vector2 startPosition = _position;
        // === CHANGE END ===

        _position += _direction * _speed * dt;

        if(_position.Y <= _playAreaBoundingBox.Top)
        {
            _position.Y = _playAreaBoundingBox.Top;
        }
        else if( (_position.Y + _dimensions.Y) >= _playAreaBoundingBox.Bottom)
        {
            _position.Y = _playAreaBoundingBox.Bottom - _dimensions.Y;
        }

        // === CHANGE START: finalize velocity for this frame ===
        if(dt > 0f)
        {
            Velocity = (_position - startPosition) / dt;
        }
        else
        {
            Velocity = Vector2.Zero;
        }

        // === CHANGE END ===
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        Rectangle paddleRectangle = new Rectangle((int) _position.X, (int) _position.Y, (int) _dimensions.X, (int) _dimensions.Y);

        spriteBatch.Draw(_texture, paddleRectangle, Color.Azure);
    }
}
