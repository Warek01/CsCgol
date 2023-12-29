namespace GameOfLife.Models;

public partial class Game
{
  public abstract class Scene : IDisposable
  {
    public string Name = "Anonymous";

    protected Game        Game     = null!;
    protected KeyData     Keyboard = null!;
    protected MouseData   Mouse    = null!;
    protected WindowData  Window   = null!;
    protected Renderer    Render   = null!;
    protected GameOptions Options  = null!;
    protected RunningData Running  = null!;

    private Dictionary<SDL_EventType, Action> _eventDict = null!;

    public void Init(Game game)
    {
      Game     = game;
      Keyboard = game.Keyboard;
      Mouse    = game.Mouse;
      Window   = game.Window;
      Render   = game.Render;
      Options  = game.Options;
      Running  = Game.Running;

      _eventDict = new()
      {
        [SDL_KEYDOWN]         = OnKeyDown,
        [SDL_KEYUP]           = OnKeyUp,
        [SDL_MOUSEBUTTONDOWN] = OnMouseDown,
        [SDL_MOUSEBUTTONUP]   = OnMouseUp,
        [SDL_MOUSEMOTION]     = OnMouseMove,
      };

      OnInit();
    }

    public void HandleEvent(SDL_Event e)
    {
      if (e.type == SDL_WINDOWEVENT)
        HandleWindowEvent(e);
      else if (_eventDict.ContainsKey(e.type))
        _eventDict[e.type]();
    }

    public virtual void OnInit()             { }
    public virtual void OnRender()           { }
    public virtual void OnMouseDown()        { }
    public virtual void OnMouseUp()          { }
    public virtual void OnMouseMove()        { }
    public virtual void OnKeyDown()          { }
    public virtual void OnKeyUp()            { }
    public virtual void OnToggleFullscreen() { }
    public virtual void OnWindowResize()     { }
    public virtual void OnFocusLost()        { }
    public virtual void OnFocusGain()        { }

    public virtual void Dispose() { }

    public override string ToString()
    {
      return Name;
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
}
