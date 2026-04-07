using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson06Debugging;

public class DebuggingGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;
    private Vector2 _position;
    private Vector2 _dimensions;
    private float _speed;

    public DebuggingGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _position = new Vector2(60f, 80f);
        _dimensions = new Vector2(250f, 50f);

        _speed = 120f;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        Move(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        Rectangle rect = new Rectangle(
            (int)_position.X,
            (int)_position.Y,
            (int)_dimensions.Y,
            (int)_dimensions.X
        );

        _spriteBatch.Draw(_pixel, rect, Color.Black);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void Move(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _position.X += _speed * dt;

        base.Update(gameTime);
    }
}