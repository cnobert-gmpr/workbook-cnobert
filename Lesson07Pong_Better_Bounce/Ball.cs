using System; // === CHANGE: needed for Math.Cos/Math.Sin === CHANGE END
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson07Pong_Better_Bounce;

public class Ball
{
    private const float _CollisionTimerInterval = 0.05f; // === CHANGE: shorter cooldown; overlap-resolution handles most cases === CHANGE END

    // === CHANGE START: bounce tuning constants ===
    private const float _MaxBounceAngleDegrees = 60f;
    private const float _MinVerticalComponent = 0.20f; // prevents near-horizontal lasers
    private const float _PaddleInfluence = 0.25f; // how much paddle movement affects bounce
    // === CHANGE END ===

    private Texture2D _texture;
    private Vector2 _position, _dimensions, _direction;
    private float _speed, _collisionTimer;

    private Rectangle _playAreaBoundingBox;
    internal Rectangle BoundingBox
    {
        get => new Rectangle(_position.ToPoint(), _dimensions.ToPoint());
    }
    internal void Initialize (Vector2 position, Vector2 dimensions, Vector2 direction, float speed, Rectangle playAreaBoundingBox)
    {
        _position = position;
        _dimensions = dimensions;

        // === CHANGE START: normalize direction so diagonal is not faster than straight ===
        if(direction != Vector2.Zero)
        {
            _direction = Vector2.Normalize(direction);
        }
        else
        {
            _direction = new Vector2(-1, 0);
        }
        // === CHANGE END ===

        _speed = speed;
        _playAreaBoundingBox = playAreaBoundingBox;
    }

    internal void LoadContent(ContentManager content)
    {
        _texture = content.Load<Texture2D>("Ball");
    }

    internal void Update(GameTime gameTime)
    {
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

        _collisionTimer += dt;
        _position += _direction * _speed * dt;

        //bounce the ball off left and right sides
        if(_position.X <= _playAreaBoundingBox.Left || 
            _position.X + _dimensions.X >= _playAreaBoundingBox.Right)
        {
            _direction.X *= -1;
        }
        if(_position.Y <= _playAreaBoundingBox.Top || 
            _position.Y + _dimensions.Y >= _playAreaBoundingBox.Bottom)
        {
            _direction.Y *= -1;
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        Rectangle ballRectangle = new Rectangle((int) _position.X, (int) _position.Y, (int) _dimensions.X, (int) _dimensions.Y);

        spriteBatch.Draw(_texture, ballRectangle, Color.White);
    }

    // === CHANGE START: new overload that supports "realistic" paddle bounces ===
    internal void ProcessCollision(Paddle paddle)
    {
        Rectangle otherBoundingBox = paddle.BoundingBox;

        if(_collisionTimer >= _CollisionTimerInterval && BoundingBox.Intersects(otherBoundingBox))
        {
            _collisionTimer = 0;

            Rectangle intersection = Rectangle.Intersect(BoundingBox, otherBoundingBox);

            // If we hit the paddle on the side, calculate a bounce angle based on hit position.
            if(intersection.Width <= intersection.Height)
            {
                bool hitRightPaddle = BoundingBox.Center.X < otherBoundingBox.Center.X;

                // Resolve overlap so we don't "stick" inside the paddle.
                // (This removes most of the need for long collision timers.)
                if(hitRightPaddle)
                {
                    _position.X = otherBoundingBox.Left - _dimensions.X;
                }
                else
                {
                    _position.X = otherBoundingBox.Right;
                }

                float paddleCenterY = otherBoundingBox.Center.Y;
                float ballCenterY = BoundingBox.Center.Y;

                float halfPaddleHeight = otherBoundingBox.Height / 2f;

                float offset = 0f;
                if(halfPaddleHeight > 0f)
                {
                    offset = (ballCenterY - paddleCenterY) / halfPaddleHeight;
                }

                // Clamp to [-1, 1] so extreme intersections don't create insane angles.
                offset = MathHelper.Clamp(offset, -1f, 1f);

                float maxAngleRadians = MathHelper.ToRadians(_MaxBounceAngleDegrees);
                float angle = offset * maxAngleRadians;

                float xSign = hitRightPaddle ? -1f : 1f;

                Vector2 newDirection = new Vector2(
                    xSign * (float) Math.Cos(angle),
                    (float) Math.Sin(angle)
                );

                // Paddle movement influence (A choice).
                // Convert paddle Y velocity (pixels/sec) into a small additive Y component.
                newDirection.Y += (paddle.Velocity.Y / _speed) * _PaddleInfluence;

                // Clamp so the ball always has some vertical motion (A choice).
                if(Math.Abs(newDirection.Y) < _MinVerticalComponent)
                {
                    newDirection.Y = Math.Sign(newDirection.Y == 0f ? offset : newDirection.Y) * _MinVerticalComponent;
                }

                _direction = Vector2.Normalize(newDirection);
            }
            else
            {
                // Top/bottom collision with paddle - keep your simple invert.
                _direction.Y *= -1;

                // Resolve overlap vertically (optional but helps).
                if(BoundingBox.Center.Y < otherBoundingBox.Center.Y)
                {
                    _position.Y = otherBoundingBox.Top - _dimensions.Y;
                }
                else
                {
                    _position.Y = otherBoundingBox.Bottom;
                }
            }
        }
    }
    // === CHANGE END ===

    // === CHANGE START: keep old signature, but route it through the new logic
    // (So older lesson code still compiles if it calls ProcessCollision(Rectangle).)
    internal void ProcessCollision(Rectangle otherBoundingBox)
    {
        // NOTE: This fallback keeps your original behaviour (simple reflect).
        // If you want realistic bounces, call ProcessCollision(Paddle) instead.
        if(_collisionTimer >= _CollisionTimerInterval && BoundingBox.Intersects(otherBoundingBox))
        {
            _collisionTimer = 0;
            Rectangle intersection = Rectangle.Intersect(BoundingBox, otherBoundingBox);
            if(intersection.Width > intersection.Height)
            {
                _direction.Y *= -1;
            }
            else
            {
                _direction.X *= -1;
            }
        }
    }
    // === CHANGE END ===
}
