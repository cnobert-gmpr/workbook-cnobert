using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson08MosquitoAttack;

public class CannonBall
{
    private Vector2 _position, _direction;
    private float _speed;
    private Texture2D _texture;
    private Rectangle _gameBoundingBox;

    private enum State { Flying, NotFlying }
    private State _state = State.NotFlying;

    internal Rectangle BoundingBox
    {
        get => new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
    }
    internal void Initialize(float speed, Rectangle gameBoundingBox)
    {
        _position = new Vector2(50, 300);
        _direction = new Vector2(0, -1);
        _speed = speed;
        _gameBoundingBox = gameBoundingBox;
    }
    internal void LoadContent(ContentManager content)
    {
        _texture = content.Load<Texture2D>("CannonBall");
    }
    internal void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        switch(_state)
        {
            case State.Flying:
                _position += _direction * _speed * dt;
                break;
            case State.NotFlying:
                break;
        }
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        switch(_state)
        {
            case State.Flying:
                spriteBatch.Draw(_texture, _position, Color.White);
                break;
            case State.NotFlying:
                break;
        }
    }
    internal void Launch(Vector2 position, Vector2 direction)
    {
        _position = position;
        _direction = direction;
        _state = State.Flying;
    }
}