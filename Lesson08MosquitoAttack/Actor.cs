using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson08MosquitoAttack;

public class Actor
{
    protected SimpleAnimation _animationAlive, _animationDying;

    protected Vector2 _position, _direction;
    protected float _speed;

    protected Rectangle _gameBoundingBox;

    internal Rectangle BoundingBox
    {
        get
        {
            return new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)_animationAlive.FrameDimensions.X,
                (int)_animationAlive.FrameDimensions.Y
            );
        }
    }
}