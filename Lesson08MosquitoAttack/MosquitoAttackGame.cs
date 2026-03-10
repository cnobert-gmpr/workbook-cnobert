using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;

namespace Lesson08MosquitoAttack;

public class MosquitoAttackGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int _WindowWidth = 550, _WindowHeight = 400, _NumMosquitoes = 10;

    private Texture2D _background;
    private SpriteFont _font;
    private string _message = "";

    private Cannon _cannon;
    private Mosquito[] _mosquitoes;

    private KeyboardState _kbCurrentState, _kbPreviousState;

    private enum GameState {Playing, Paused, Over}
    private GameState _gameState = GameState.Playing;

    private Rectangle BoundingBox
    {
        get { return new Rectangle(0, 0, _WindowWidth, _WindowHeight); }
    }

    public MosquitoAttackGame()
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

        _cannon = new Cannon();
        _cannon.Initialize(new Vector2(50, 325), 150);

        _mosquitoes = new Mosquito[_NumMosquitoes];
        for(int c = 0; c < _NumMosquitoes; c++)
        {
            _mosquitoes[c] = new Mosquito();
        }
        
        Random random = new Random();
        foreach(Mosquito mosquito in _mosquitoes)
        {
            int direction = random.Next(1, 3);
            if(direction == 2)
                direction = -1;
            // direction = random.Next(1, 3) == 2 ? -1: 1;
            int xPosition = random.Next(1, _WindowWidth - 50);
            int speed = random.Next(150, 251);
            int yPosition = random.Next(1, 151);
            mosquito.Initialize(new Vector2(xPosition, yPosition), speed, new Vector2(direction, 0), BoundingBox);
        }
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _background = Content.Load<Texture2D>("Background");

        _cannon.LoadContent(Content);

        foreach(Mosquito mosquito in _mosquitoes)
        {
            mosquito.LoadContent(Content);
        }

        #region students, don't look here
        //MacOS ONLY
         byte[] fontBytes = File.ReadAllBytes("Content/Tahoma.ttf");
        _font = TtfFontBaker.Bake(fontBytes, 30, 1024, 1024, new[] { CharacterRange.BasicLatin }).CreateSpriteFont(GraphicsDevice);
        #endregion

    }

    protected override void Update(GameTime gameTime)
    {
        _kbCurrentState = Keyboard.GetState();
        switch(_gameState)
        {
            case GameState.Playing:
                if(_kbCurrentState.IsKeyDown(Keys.A))
                    _cannon.Direction = new Vector2(-1, 0);
                else if(_kbCurrentState.IsKeyDown(Keys.D))
                    _cannon.Direction  = new Vector2(1, 0);
                else
                    _cannon.Direction  = Vector2.Zero;
                _cannon.Update(gameTime);
                foreach(Mosquito mosquito in _mosquitoes)
                {
                    mosquito.Update(gameTime);
                }

                if(Pressed(Keys.P))
                {
                    _gameState = GameState.Paused;
                    _message = "Game Paused, press P to start playing.";
                }

                break;
            case GameState.Paused:
                if(Pressed(Keys.P))
                {
                    _gameState = GameState.Playing;
                }
                break;
            case GameState.Over:
                break;
        }
        _kbPreviousState = _kbCurrentState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        switch(_gameState)
        {
            case GameState.Playing:
                _spriteBatch.Draw(_background, Vector2.Zero, Color.White);
                _cannon.Draw(_spriteBatch);
                foreach(Mosquito mosquito in _mosquitoes)
                {
                    mosquito.Draw(_spriteBatch);
                }
                
                break;
            case GameState.Paused:
                _spriteBatch.Draw(_background, Vector2.Zero, Color.Silver);
                _spriteBatch.DrawString(_font, _message, new Vector2(10, 135), Color.White);
                _cannon.Draw(_spriteBatch);  
                foreach(Mosquito mosquito in _mosquitoes)
                {
                    mosquito.Draw(_spriteBatch);
                }          
                break;
            case GameState.Over:
                break;
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private bool Pressed(Keys key)
    {
        // a new key press if it is down now, but was "up" 1/60 of a second ago
        return _kbCurrentState.IsKeyDown(key) && _kbPreviousState.IsKeyUp(key);
    }
}
