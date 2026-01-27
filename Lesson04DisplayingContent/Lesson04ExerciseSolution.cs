using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson04DisplayingContent;


public class Lesson04ExerciseSolution : Game
{
    private const int _WindowWidth = 640, _WindowHeight = 320;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _shipSprite, _backgroundSprite;

    private float _xPosition = 3, _yPosition = 3, _xVelocity = 150, _yVelocity = 150;
    private Vector2 _position, _velocity;

    private float _rotation = 0, _rotationSpeed = 6;

    public Lesson04ExerciseSolution()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = _WindowWidth;
        _graphics.PreferredBackBufferHeight = _WindowHeight;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _shipSprite = Content.Load<Texture2D>("Beetle");
        _backgroundSprite = Content.Load<Texture2D>("Station");
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        //_shipSprite.Bounds.Height

        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

        if( (_xPosition + _shipSprite.Bounds.Width) >= _WindowWidth || _xPosition <= 0) 
        {
            _xVelocity *= -1;
        }
        _xPosition += _xVelocity * dt;
        
        if( (_yPosition + _shipSprite.Bounds.Height) >= _WindowHeight || _yPosition <= 0) 
        {
            _yVelocity *= -1;
        }
        _yPosition += _yVelocity * dt;

        _rotation += _rotationSpeed * dt;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _spriteBatch.Draw(_backgroundSprite, Vector2.Zero, null, Color.White);

        Vector2 origin = new Vector2(_shipSprite.Width / 2f, _shipSprite.Height / 2f);

        Vector2 drawPos = new Vector2(
            _xPosition + origin.X,
            _yPosition + origin.Y
        );

        _spriteBatch.Draw(
            _shipSprite,
            drawPos,
            null,
            Color.White,
            _rotation,
            origin,
            1f,
            SpriteEffects.None,
            0f
        );
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
