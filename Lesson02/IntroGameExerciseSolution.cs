using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson02;

public class IntroGameExerciseSolution : Game
{
    //an object that represents the screen
    private GraphicsDeviceManager _graphics;
    //an object that batches up draw commands so that they can be sent
    //to the screen all at once
    private SpriteBatch _spriteBatch;

    private Texture2D _pixel;

    private float _xPosition = 100, _yPosition = 150;
    private float _speed = 150;

    //In-class Exercise: Add a second float variable named _slowSpeed and use it instead of _speed.
    private float _slowSpeed = 60;

    private int _width = 80, _height = 50;

    public IntroGameExerciseSolution()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        //new texture that is one pixel by one pixel
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new [] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        //In-class Exercise: Change the direction of movement so the rectangle moves downward instead of to the right.
        //In-class Exercise: Update both _xPosition and _yPosition so that the rectangle moves diagonally.
        _xPosition += _slowSpeed * dt;
        //_yPosition += _slowSpeed * seconds;

        //Stretch goal: Prevent the rectangle from moving beyond the right edge of the window.
        int windowWidth = _graphics.PreferredBackBufferWidth;
        float maxX = windowWidth - _width;

        if (_xPosition > maxX)
        {
            _xPosition = maxX;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Wheat);

        //all draw commands should always be with the spritebatch begin and end
        _spriteBatch.Begin();

        Rectangle rect = new Rectangle((int)_xPosition, (int)_yPosition, _width, _height);

        _spriteBatch.Draw(_pixel, rect, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}