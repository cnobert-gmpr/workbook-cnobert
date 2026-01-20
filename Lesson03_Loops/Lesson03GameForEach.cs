using System.Net.Http.Headers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson03_Loops;

public class Lesson03GameForEach : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;
    private Vector2 _position, _dimensions;
    private int _count;
    private float _spacing;

    private Rectangle[] _rectangles;

    public Lesson03GameForEach()
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

        _rectangles = new Rectangle[_count];
        for(int c = 0; c < _count; c++)
        {
            float x = _position.X + c * (_dimensions.X + _spacing);

            Rectangle rect = new Rectangle((int) x,(int) _position.Y, (int)_dimensions.X, (int)_dimensions.Y);
            
            _rectangles[c] = rect;
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        //creates a 1x1 pixel
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new []{Color.Khaki});
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        foreach(Rectangle r in _rectangles)
        {
           _spriteBatch.Draw(_pixel, r, Color.White);
        }


        

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
 