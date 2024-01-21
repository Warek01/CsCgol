namespace CsGame.Models;

// Contains all states
public class GameState
{
  public Keyboard Keyboard = new();
  public Mouse    Mouse    = new();
  public Options  Options  = new();
  public Runtime  Runtime  = new();
  public Screen   Screen   = new();
  public Window   Window   = new();
}
