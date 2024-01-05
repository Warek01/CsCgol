namespace CsGame.Models;

// Object that is passed to constructor of a scene
public struct SceneInitObject
{
  public Game      Game;
  public GameState State;
  public IntPtr    Renderer;
  public Font      MainFont;

  public void Deconstruct(
    out Game         game,
    out WindowState  window,
    out MouseState   mouse,
    out KeyState     keyboard,
    out RuntimeState runtime,
    out OptionsState options,
    out ScreenState  screen,
    out IntPtr       renderer,
    out Font         mainFont
  )
  {
    game     = Game;
    window   = State.Window;
    mouse    = State.Mouse;
    keyboard = State.Keyboard;
    runtime  = State.Runtime;
    options  = State.Options;
    screen   = State.Screen;
    renderer = Renderer;
    mainFont = MainFont;
  }
}
