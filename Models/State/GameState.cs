namespace GameOfLife.Models;

// Contains all states
public class GameState
{
  public KeyState     Keyboard = new();
  public MouseState   Mouse    = new();
  public OptionsState Options  = new();
  public RuntimeState Runtime  = new();
  public ScreenState  Screen   = new();
  public WindowState  Window   = new();
}
