using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson08MosquitoAttack;

public class Mosquito
{
    private SimpleAnimation _animationAlive, _animationDying;

    private Vector2 _position;
    private Vector2 _direction;
    private float _speed;

    private Rectangle _gameBoundingBox;

    private enum State { Alive, Dying, Dead }
    private State _state;

    internal Rectangle BoundingBox
    {
        get
        {
            return new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)_animationAlive.FrameDimensions.X,
                (int)_animationAlive.FrameDimensions.Y
            );
        }
    }
    
    internal bool Alive { get => _state == State.Alive; }
    
    internal void Initialize(Vector2 position, float speed, Vector2 direction, Rectangle gameBoundingBox)
    {
        _position = position;
        _speed = speed;
        _direction = direction;
        _gameBoundingBox = gameBoundingBox;
        _state = State.Alive;
    }
    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Mosquito");

        _animationAlive = 
            new SimpleAnimation(texture, texture.Width / 11, texture.Height, 11, 8f);
        _animationAlive.Paused = false;

        texture = content.Load<Texture2D>("Poof");
        _animationDying = 
            new SimpleAnimation(texture, texture.Width / 8, texture.Height, 8, 4);
    }
    internal void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        switch(_state)
        {
            case State.Alive:
                _position += _direction * _speed * dt;
                if(BoundingBox.Left < _gameBoundingBox.Left || BoundingBox.Right > _gameBoundingBox.Right)
                {
                    _direction.X *= -1;
                }
                _animationAlive.Update(gameTime);
                break;
            case State.Dying:
                _animationDying.Update(gameTime);
                if(_animationDying.DonePlayingOnce)
                {
                    _state = State.Dead;
                }
                break;
            case State.Dead:
                break;
        }
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        switch(_state)
        {
            case State.Alive:
                _animationAlive.Draw(spriteBatch, _position, SpriteEffects.None);
                break;
            case State.Dying:
                _animationDying.Draw(spriteBatch, _position, SpriteEffects.None);
                break;
            case State.Dead:
                break;
        }
    }

    internal void Die()
    {
        _state = State.Dying;
        _animationDying.Looping = false;
    }

}