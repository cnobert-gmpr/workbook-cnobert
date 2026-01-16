using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson02; //like a folder for classes

public class IntroGame : Game
{
    //an object that represents the screen
    private GraphicsDeviceManager _graphics;
    //an object that batches up the draw calls for efficiency
    private SpriteBatch _spriteBatch;

    private Texture2D _pixel;

    private float _xPosition = 100, _yPosition = 150;
    private float _speed = 150;
    private int _width = 80, _height = 50;

    //a C# constructor always has the same name as the class that it contructs
    public IntroGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    //Initialize and LoadContent will each be run once when we launch the game
    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        //creates a 1x1 pixel
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new []{Color.White});
    
        
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        //multiply _speed by the time that has passed since the last call to update
        //in case there has been lag
        _xPosition += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        Rectangle rect = new Rectangle((int)_xPosition, (int)_yPosition, _width, _height);
        
        _spriteBatch.Draw(_pixel, rect, Color.Wheat);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
