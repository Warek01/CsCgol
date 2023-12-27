namespace SDLTest.Models;

public partial class GameOfLife
{
  public abstract class Scene : IDisposable
  {
    public string Name = "Anonymous";

    protected GameOfLife Game     = null!;
    protected KeyData    Keyboard = null!;
    protected MouseData  Mouse    = null!;
    protected WindowData Window   = null!;

    private Dictionary<SDL_EventType, Action> _eventDict = null!;

    public void Init(GameOfLife game)
    {
      Game     = game;
      Keyboard = game._keyData;
      Mouse    = game._mouseData;
      Window   = game._windowData;

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
      {
        switch (e.window.windowEvent)
        {
          case SDL_WINDOWEVENT_RESIZED:
          case SDL_WINDOWEVENT_MAXIMIZED:
          case SDL_WINDOWEVENT_RESTORED:
            OnWindowResize();
            break;
        }
      }
      else if (_eventDict.ContainsKey(e.type))
        _eventDict[e.type]();
    }

    public virtual void Dispose()            { }
    public virtual void OnInit()             { }
    public virtual void OnRender()           { }
    public virtual void OnMouseDown()        { }
    public virtual void OnMouseUp()          { }
    public virtual void OnMouseMove()        { }
    public virtual void OnKeyDown()          { }
    public virtual void OnKeyUp()            { }
    public virtual void OnToggleFullscreen() { }
    public virtual void OnWindowResize()     { }

    public override string ToString()
    {
      return Name;
    }
  }
}
