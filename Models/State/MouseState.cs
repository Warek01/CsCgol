namespace CsGame.Models;

// Represents mouse cursor
public class MouseState
{
  public int  X;
  public int  Y;
  // SDL_BUTTON_x
  public uint Button;
  public bool IsButtonPressed;
}
