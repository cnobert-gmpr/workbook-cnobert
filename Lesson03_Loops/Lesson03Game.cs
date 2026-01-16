using System.Net.Http.Headers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson03_Loops;

public class Lesson03Game : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;
    private Vector2 _position, _dimensions;
    private int _count;
    private float _spacing;

    public Lesson03Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _position = new Vector2(50, 200);
        _dimensions = new Vector2(60, 40);

        _count = 6; // number of rectangles
        _spacing = 10; //10 pixels apart

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        //creates a 1x1 pixel
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new []{Color.Firebrick});
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        for(int c = 0; c < _count; c++)
        {
            float x = _position.X + c * (_dimensions.X + _spacing);


            Rectangle rect = new Rectangle((int) x,(int) _position.Y, (int)_dimensions.X, (int)_dimensions.Y);

            _spriteBatch.Draw(_pixel, rect, Color.White);
        }

        

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
