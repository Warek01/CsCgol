namespace GameOfLife.Models;

public abstract class Scene : IDisposable
{
  protected readonly KeyState     Keyboard;
  protected readonly MouseState   Mouse;
  protected readonly WindowState  Window;
  protected readonly Renderer     Renderer;
  protected readonly OptionsState Options;
  protected readonly RuntimeState Runtime;
  protected readonly IGame        Game;

  protected readonly Dictionary<string, IntPtr> Fonts;
  protected readonly Dictionary<string, Color>  Colors;

  protected readonly SceneElementManager ElementManager;

  private readonly SceneElementManager.EventManager  EventManager;
  private readonly Dictionary<SDL_EventType, Action> _eventDict;

  public Scene(SceneInitObject init)
  {
    Game     = init.Game;
    Keyboard = init.State.Keyboard;
    Mouse    = init.State.Mouse;
    Window   = init.State.Window;
    Options  = init.State.Options;
    Runtime  = init.State.Runtime;
    Renderer = init.Renderer;
    Fonts    = init.Fonts;
    Colors   = init.Colors;

    ElementManager = new SceneElementManager(Mouse);
    EventManager   = ElementManager.GetEventManager();

    _eventDict = new()
    {
      [SDL_KEYDOWN]         = OnKeyDown,
      [SDL_KEYUP]           = OnKeyUp,
      [SDL_MOUSEBUTTONDOWN] = OnMouseDown,
      [SDL_MOUSEBUTTONUP]   = OnMouseUp,
      [SDL_MOUSEMOTION]     = OnMouseMove,
    };
  }

  public void HandleEvent(SDL_Event e)
  {
    if (e.type == SDL_WINDOWEVENT)
      HandleWindowEvent(e);
    else if (_eventDict.ContainsKey(e.type))
      _eventDict[e.type]();
  }

  public virtual void OnRender()
  {
    EventManager.OnRender();
  }

  public virtual void OnMouseDown()
  {
    EventManager.OnMouseDown();
  }

  public virtual void OnMouseUp()
  {
    EventManager.OnMouseUp();
  }

  public virtual void OnMouseMove()
  {
    EventManager.OnMouseMove();
  }


  public virtual void OnWindowResize()     { }
  public virtual void OnKeyDown()          { }
  public virtual void OnKeyUp()            { }
  public virtual void OnToggleFullscreen() { }
  public virtual void OnFocusLost()        { }
  public virtual void OnFocusGain()        { }

  public virtual void Dispose() { }

  public override string ToString()
  {
    return GetType().Name;
  }

  private void HandleWindowEvent(SDL_Event e)
  {
    switch (e.window.windowEvent)
    {
      case SDL_WINDOWEVENT_RESIZED:
      case SDL_WINDOWEVENT_MAXIMIZED:
      case SDL_WINDOWEVENT_RESTORED:
        OnWindowResize();
        break;
      case SDL_WINDOWEVENT_FOCUS_LOST:
        OnFocusLost();
        break;
      case SDL_WINDOWEVENT_FOCUS_GAINED:
        OnFocusGain();
        break;
    }
  }
}
