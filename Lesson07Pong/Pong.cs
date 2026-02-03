using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson07Pong;

public class Pong : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int _WindowWidth = 750, _WindowHeight = 450, _BallWidthAndHeight = 21, _PlayAreaEdgeLineWidth = 12;

    private Texture2D _backgroundTexture, _ballTexture;

    private Vector2 _ballPosition, _ballDirection;
    private float _ballSpeed;

    // in c#, if we want to expose data to an outside object, we create a c# property
    // c# property: a special type of method that looks like a data member and gives access to private data
   
    // a c# property is a type of method ("getter" or "setter")
    // since it just repackages data, the below is called a "derived" property
    internal Rectangle PlayAreaBoundingBox
    {
        get => new Rectangle(0, 0, _WindowWidth, _WindowHeight);
    }

    public Pong()
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

        _ballPosition.X = 150;
        _ballPosition.Y = 195;

        _ballSpeed = 60;
        _ballDirection = new Vector2(-1, -1);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _backgroundTexture = Content.Load<Texture2D>("Court");
        _ballTexture = Content.Load<Texture2D>("Ball");
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        _ballPosition += _ballDirection * _ballSpeed * dt;

        // in-class exercise: make the ball bounce off of the right side as well
        if(_ballPosition.X <= PlayAreaBoundingBox.Left || _ballPosition.X + _BallWidthAndHeight >= PlayAreaBoundingBox.Right)
        {
            _ballDirection.X *= -1;
        }
        if(_ballPosition.Y <= PlayAreaBoundingBox.Top ||
           (_ballPosition.Y + _BallWidthAndHeight) >= PlayAreaBoundingBox.Bottom)
        {
            _ballDirection.Y *= -1;
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _WindowWidth, _WindowHeight), Color.White);

        Rectangle ballRectangle = new Rectangle((int) _ballPosition.X, (int) _ballPosition.Y, _BallWidthAndHeight, _BallWidthAndHeight);
        _spriteBatch.Draw(_ballTexture, ballRectangle, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
