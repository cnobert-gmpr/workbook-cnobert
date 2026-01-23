using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpriteFontPlus; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
using System.IO; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)

namespace Lesson05KeyboardInput;

public class InputGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

     private string _message = "Hi.";
     private SpriteFont _font;

     private KeyboardState _kbCurrentState, _kbPreviousState;

    public InputGame()
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
        //_font = Content.Load<SpriteFont>("SystemArialFont");

        #region students, don't look here
        //MacOS ONLY
         byte[] fontBytes = File.ReadAllBytes("Content/Tahoma.ttf");
        _font = TtfFontBaker.Bake(fontBytes, 30, 1024, 1024, new[] { CharacterRange.BasicLatin }).CreateSpriteFont(GraphicsDevice);
        #endregion
        
    }

    protected override void Update(GameTime gameTime)
    {
        _kbCurrentState = Keyboard.GetState();
        _message = "";

        #region arrow keys
        if(_kbCurrentState.IsKeyDown(Keys.Up))
        {
            _message += "Up ";
        }
        if(_kbCurrentState.IsKeyDown(Keys.Down))
        {
            _message += "Down ";
        }
        if(_kbCurrentState.IsKeyDown(Keys.Left))
        {
            _message += "Left ";
        }
        if(_kbCurrentState.IsKeyDown(Keys.Right))
        {
            _message += "Right ";
        }

        #endregion

        #region state of space bar key
        if(IsNewKeyPress(Keys.Space))
        {
            _message += "\n";
            _message += "Space Bar pressed \n";
            _message += "-----------------------------------------------------------\n";
            _message += "-----------------------------------------------------------\n";
            _message += "-----------------------------------------------------------\n";
            _message += "-----------------------------------------------------------\n";
            _message += "-----------------------------------------------------------\n";
            _message += "-----------------------------------------------------------\n";
            _message += "-----------------------------------------------------------\n";
            _message += "-----------------------------------------------------------\n";
            _message += "-----------------------------------------------------------\n";
            _message += "-----------------------------------------------------------\n";
            
        }
        else if (_kbCurrentState.IsKeyDown(Keys.Space))
        {
            _message += "\n";
            _message += "Space bar being held down.";
        }
        else if(_kbPreviousState.IsKeyDown(Keys.Space))
        {
            _message += "\n";
            _message += "**********************************************************\n";
            _message += "**********************************************************\n";
            _message += "**********************************************************\n";
            _message += "**********************************************************\n";
            _message += "**********************************************************\n";
            _message += "**********************************************************\n";
            _message += "**********************************************************\n";
            _message += "**********************************************************\n";
        }
        #endregion
        
        _kbPreviousState = _kbCurrentState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, _message, Vector2.Zero, Color.Wheat);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private bool IsNewKeyPress(Keys key)
    {
        bool returnValue = _kbPreviousState.IsKeyUp(key) && _kbCurrentState.IsKeyDown(key);
        return returnValue;
    }
}
