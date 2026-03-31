using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson08MosquitoAttack;

public class Mosquito
{
    private const int NumFireBalls = 10, UpperRandomFiringRange = 200;
    private SimpleAnimation _animationAlive, _animationDying;

    private Vector2 _position;
    private Vector2 _direction;
    private float _speed;

    private Rectangle _gameBoundingBox;

    private enum State { Alive, Dying, Dead }
    private State _state;

    private FireBall[] _fireBalls;
    Random rng = new Random();

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

        _fireBalls = new FireBall[NumFireBalls];
        for(int c = 0; c < NumFireBalls; c++)
        {
            _fireBalls[c] = new FireBall();
            _fireBalls[c].Initialize(50, _gameBoundingBox);
        }
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

        foreach(FireBall fb in _fireBalls)
            fb.LoadContent(content);
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

                if(rng.Next(1, UpperRandomFiringRange) == 1)
                    Shoot();

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
        foreach(FireBall fb in _fireBalls)
            fb.Update(gameTime);
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
        foreach(FireBall fb in _fireBalls)
            fb.Draw(spriteBatch);
    }

    internal void Die()
    {
        _state = State.Dying;
        _animationDying.Looping = false;
    }

    internal void Shoot()
    {
        foreach(FireBall fb in _fireBalls)
        {
            if(fb.Launchable)
            {
                float fireBallPositionY = BoundingBox.Bottom;
                float fireBallPositionX = BoundingBox.Center.X - fb.BoundingBox.Width / 2;
                fb.Launch(new Vector2(fireBallPositionX, fireBallPositionY), new Vector2(0, 1));
                return; // or "break;"
            }
        }  
    }

}