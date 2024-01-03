namespace CsGame.Models;

// Abstract scene representing a particular global game state like menu, ingame etc.
public abstract class Scene : IDisposable
{
  protected readonly KeyState     Keyboard;
  protected readonly MouseState   Mouse;
  protected readonly WindowState  Window;
  protected readonly Renderer     Renderer;
  protected readonly OptionsState Options;
  protected readonly RuntimeState Runtime;
  protected readonly IGame        Game;
  protected readonly Font         MainFont;
  protected readonly ScreenState  Screen;

  protected readonly SceneElementManager ElementManager;

  private readonly SceneElementManager.EventManager  _eventManager;
  private readonly Dictionary<SDL_EventType, Action> _eventDict;

  public Scene(SceneInitObject init)
  {
    (Game, Window, Mouse, Keyboard, Runtime, Options, Screen, Renderer, MainFont) = init;

    ElementManager = new SceneElementManager(Mouse);
    _eventManager   = ElementManager.GetEventManager();

    _eventDict = new Dictionary<SDL_EventType, Action>
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
    else if (_eventDict.TryGetValue(e.type, out Action value))
            value.Invoke();
  }

  public virtual void OnRender()
  {
    _eventManager.OnRender();
  }

  public virtual void OnMouseDown()
  {
    _eventManager.OnMouseDown();
  }

  public virtual void OnMouseUp()
  {
    _eventManager.OnMouseUp();
  }

  public virtual void OnMouseMove()
  {
    _eventManager.OnMouseMove();
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
