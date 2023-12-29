namespace GameOfLife.Models;

public abstract class Element : IDisposable
{
  public enum Event
  {
    MOUSE_DOWN,
    MOUSE_UP,
    MOUSE_ENTER,
    MOUSE_LEAVE,
    MOUSE_MOVE,
  }

  public Renderer Renderer;

  public int X
  {
    get => Rect.x;
    set
    {
      Rect.x = value;
      OnRectUpdate();
    }
  }

  public int Y
  {
    get => Rect.y;
    set
    {
      Rect.y = value;
      OnRectUpdate();
    }
  }

  public int Width
  {
    get => Rect.w;
    set
    {
      Rect.w = value;
      OnRectUpdate();
    }
  }

  public int Height
  {
    get => Rect.h;
    set
    {
      Rect.h = value;
      OnRectUpdate();
    }
  }

  protected SDL_Rect Rect;

  private readonly Dictionary<Event, List<Action>> _eventsDict = new();

  public Element(Renderer renderer)
  {
    Renderer = renderer;
  }

  public abstract void Render();

  public abstract void Dispose();

  public void OnEvent(Event e)
  {
    if (_eventsDict.ContainsKey(e))
      foreach (Action callback in _eventsDict[e])
        callback();
  }

  public void AddEventListener(Event e, Action listener)
  {
    if (!_eventsDict.ContainsKey(e))
      _eventsDict[e] = new List<Action>();

    _eventsDict[e].Add(listener);
  }

  public void RemoveEventListener(Event e, Action listener)
  {
    if (!_eventsDict.ContainsKey(e))
      return;

    _eventsDict[e].Remove(listener);
  }

  public void RemoveAllListeners(Event e)
  {
    if (!_eventsDict.ContainsKey(e))
      return;

    _eventsDict.Remove(e);
  }

  public void RemoveAllListeners()
  {
    _eventsDict.Clear();
  }

  protected virtual void OnRectUpdate() { }
}
