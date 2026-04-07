
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson08MosquitoAttack;

public class FireBall : Projectile
{
    private SimpleAnimation _animation;

    internal override void Initialize(float speed, Rectangle gameBoundingBox)
    {
        base.Initialize(speed, gameBoundingBox);
        
        _dimensions = new Point(5, 17);
    }
    internal override  void LoadContent(ContentManager content)
    {
        _texture = content.Load<Texture2D>("FireBall");
        _animation = new SimpleAnimation(_texture, _dimensions.X, _dimensions.Y, 8, 8);
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
                }
                break;
            case State.NotFlying:
                break;
        }
        _animation.Update(gameTime);
    }
    internal override void Draw(SpriteBatch spriteBatch)
    {
        switch(_state)
        {
            case State.Flying:
                _animation.Draw(spriteBatch, _position, SpriteEffects.None);
                break;
            case State.NotFlying:
                break;
        }
    }

}