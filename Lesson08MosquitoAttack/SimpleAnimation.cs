using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Lesson08MosquitoAttack;

public class SimpleAnimation
{
    private readonly Texture2D _texture;
    private readonly List<Rectangle> _frames;
    private readonly float _timePerFrame;

    private float _timer;
    private int _frameIndex;

    internal bool Looping { get; set; } = true;
    internal bool Paused { get; set; } = false;

    // Play animation backwards
    internal bool Reverse { get; set; } = false;

    internal bool DonePlayingOnce
    {
        get
        {
            bool finished = false;

            if (!Looping)
            {
                if (!Reverse)
                {
                    finished = _frameIndex == _frames.Count - 1;
                }
                else
                {
                    finished = _frameIndex == 0;
                }
            }

            return finished;
        }
    }

    internal Vector2 FrameDimensions
    {
        get
        {
            Vector2 dimensions = Vector2.Zero;
            if (_frames != null)
            {
                dimensions = _frames[0].Size.ToVector2();
            }
            return dimensions;
        }
    }

    internal SimpleAnimation(Texture2D texture, int frameWidth, int frameHeight, int frameCount, float framesPerSecond)
    {
        _texture = texture;

        _frames = new List<Rectangle>();
        for (int i = 0; i < frameCount; i++)
        {
            _frames.Add(new Rectangle(i * frameWidth, 0, frameWidth, frameHeight));
        }

        _timePerFrame = 1f / framesPerSecond;
    }

    internal void Update(GameTime gameTime)
    {
        bool shouldProcess = !Paused;
        if (shouldProcess)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool shouldAdvanceFrame = _timer >= _timePerFrame;
            if (shouldAdvanceFrame)
            {
                _timer -= _timePerFrame;

                if (!Reverse)
                {
                    _frameIndex++;
                    if (_frameIndex >= _frames.Count)
                    {
                        if (Looping)
                        {
                            _frameIndex = 0;
                        }
                        else
                        {
                            _frameIndex = _frames.Count - 1;
                        }
                    }
                }
                else
                {
                    _frameIndex--;
                    if (_frameIndex < 0)
                    {
                        if (Looping)
                        {
                            _frameIndex = _frames.Count - 1;
                        }
                        else
                        {
                            _frameIndex = 0;
                        }
                    }
                }
            }
        }

        // One exit point only
    }

    internal void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects effects)
    {
        spriteBatch.Draw(
            _texture,
            position,
            _frames[_frameIndex],
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            effects,
            0f
        );
    }
}