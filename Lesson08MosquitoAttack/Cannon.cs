using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson08MosquitoAttack;

public class Cannon
{
    private const int _NumCannonBalls = 10;

    private SimpleAnimation _animation;
    private Vector2 _position, _direction;
    private Point _dimensions;
    private float _speed;
    private Rectangle _gameBoundingBox;

    private CannonBall[] _cannonBalls;

    internal Vector2 Direction 
    { 
        set
        {
            value.Y = 0;
            _direction = value; 
            _animation.Reverse = _direction.X < 0;
        }
    }

    internal Rectangle BoundingBox
    {
        get
        {
            return new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)_animation.FrameDimensions.X,
                (int)_animation.FrameDimensions.Y
            );
        }
    }
    internal void Initialize(Vector2 position, float speed, Rectangle gameBoundingBox)
    {
        _position = position;
        _speed = speed;
        _gameBoundingBox = gameBoundingBox;

        _cannonBalls = new CannonBall[_NumCannonBalls];
        for(int c = 0; c < _NumCannonBalls; c++)
        {
            _cannonBalls[c] = new CannonBall();
            _cannonBalls[c].Initialize(50, _gameBoundingBox);
        } 
    }

    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Cannon");
        _dimensions = new Point(texture.Width / 4, texture.Height);
        _animation = new SimpleAnimation(texture, _dimensions.X, _dimensions.Y, 4, 2f);
        foreach(CannonBall c in _cannonBalls)
            c.LoadContent(content);
    }
    internal void Update(GameTime gameTime)
    {
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        _position += _speed * _direction * dt;

        if(_direction != Vector2.Zero)
            _animation.Update(gameTime);

        foreach(CannonBall c in _cannonBalls)
            c.Update(gameTime);
    }
    
    internal void Draw(SpriteBatch spriteBatch)
    {
        if(_animation != null)
            _animation.Draw(spriteBatch, _position, SpriteEffects.None);
        foreach(CannonBall c in _cannonBalls)
            c.Draw(spriteBatch);
    }

    internal void Shoot()
    {
        foreach(CannonBall c in _cannonBalls)
        {
            if(c.Launchable)
            {
                float cannonBallPositionY = BoundingBox.Top - c.BoundingBox.Height;
                float cannonBallPositionX = BoundingBox.Center.X - c.BoundingBox.Width / 2;
                c.Launch(new Vector2(cannonBallPositionX, cannonBallPositionY), new Vector2(0, -1));
                //we have found one to launch, time to abort
                return; // or "break;"
            }
        }  
    }

    internal bool ProcessCollision(Rectangle boundingBox)
    {
        foreach(CannonBall c in _cannonBalls)
        {
            if(c.ProcessCollision(boundingBox))
                return true;
        }
        return false;
    }
}