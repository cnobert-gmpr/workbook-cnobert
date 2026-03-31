using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson08MosquitoAttack;

public class CannonBall : Projectile
{

    private Texture2D _texture;
    private List<Vector2> _trailPositions;
    private float _trailTimer;
    private const float _TrailSpawnInterval = 0.15f;
    private const int _MaxTrailPositions = 12;

    internal Rectangle BoundingBox
    {
        get => new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
    }

    //"override" means "I'm hiding the parent method"
    internal override void Initialize(float speed, Rectangle gameBoundingBox)
    {
        // run the parent class' Initialize method
        base.Initialize(speed, gameBoundingBox);

        // run the CannonBall-specific code
        _trailPositions = new List<Vector2>();
        _trailTimer = 0;
    }
    internal override void LoadContent(ContentManager content)
    {
        _texture = content.Load<Texture2D>("CannonBall");
    }
    internal override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        switch(_state)
        {
            case State.Flying:
                _position += _direction * _speed * dt;
                if(!BoundingBox.Intersects(_gameBoundingBox))
                {
                    _state = State.NotFlying;
                    _trailPositions.Clear();
                }

                _trailTimer += dt;
                if(_trailTimer >= _TrailSpawnInterval)
                {
                    _trailTimer = 0;
                    _trailPositions.Insert(0, _position);
                    if(_trailPositions.Count > _MaxTrailPositions)
                    {
                        _trailPositions.RemoveAt(_trailPositions.Count - 1);
                    }
                }
                break;
            case State.NotFlying:
                break;
        }
    }
    internal override void Draw(SpriteBatch spriteBatch)
    {
        switch(_state)
        {
            case State.Flying:
                spriteBatch.Draw(_texture, _position, Color.White);
                DrawTrail(spriteBatch);
                break;
            case State.NotFlying:
                break;
        }
    }
    
    internal void DrawTrail(SpriteBatch spriteBatch)
    {
        for(int c = 0; c < _trailPositions.Count; c++)
        {
            // gets closer and closer to zero as the counter (c) increases (12/13, 11/13, 10/13, ...)
            float alpha = 1f - ((float)(c + 1) / (_trailPositions.Count + 1));
            // 1, 0.9, 0.8, 0.7
            float scale = 1f - (c * 0.1f);
            if(scale < 0.2f)
            {
                scale = 0.2f;
            }
            Vector2 drawPosition = _trailPositions[c];
            Vector2 origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            Vector2 centeredPosition = drawPosition + new Vector2(_texture.Width / 2, _texture.Height / 2);
            spriteBatch.Draw
            (
                _texture, centeredPosition, null,
                Color.Gray * (alpha * 0.5f), 0f, origin, scale, SpriteEffects.None, 0f
            );
        }
    }
    
    internal bool ProcessCollision(Rectangle boundingBox)
    {
        bool returnValue = false;
        if(BoundingBox.Intersects(boundingBox))
        {
            _state = State.NotFlying;
            _trailPositions.Clear();
            returnValue = true;
        }
        return returnValue;
    }
}