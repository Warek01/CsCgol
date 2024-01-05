namespace CsGame.Models;

[Flags]
public enum Direction
{
  NONE  = 0,
  UP    = 1,
  RIGHT = 2,
  DOWN  = 4,
  LEFT  = 8,

  UP_RIGHT   = UP | RIGHT,
  UP_LEFT    = UP | LEFT,
  DOWN_RIGHT = DOWN | RIGHT,
  DOWN_LEFT  = DOWN | LEFT,
}
