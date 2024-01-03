namespace CsGame.Models;

// Manages all UI elements shown in the scene. Renders the elements, computes mouse events.
public class SceneElementManager
{
  public class EventManager
  {
    private readonly List<Element> _elements;
    private readonly MouseState    _mouse;

    private Element? _hoveredElement;
    private bool     _isDisabled = false;


    public EventManager(List<Element> elements, MouseState mouse)
    {
      _elements = elements;
      _mouse    = mouse;
    }

    public void OnRender()
    {
      foreach (var element in _elements)
        element.Render();
    }

    public void OnMouseDown()
    {
      if (_isDisabled) return;
      FindElementOnCoordinates(_mouse.X, _mouse.Y)?.InvokeEvent(UiEvent.MOUSE_DOWN);
    }

    public void OnMouseUp()
    {
      if (_isDisabled) return;
      FindElementOnCoordinates(_mouse.X, _mouse.Y)?.InvokeEvent(UiEvent.MOUSE_UP);
    }

    public void OnMouseMove()
    {
      if (_isDisabled) return;

      var element = FindElementOnCoordinates(_mouse.X, _mouse.Y);

      if (element != _hoveredElement)
      {
        _hoveredElement?.InvokeEvent(UiEvent.MOUSE_LEAVE);
        element?.InvokeEvent(UiEvent.MOUSE_ENTER);
        _hoveredElement = element;
      }

      element?.InvokeEvent(UiEvent.MOUSE_MOVE);
    }

    public Element? FindElementOnCoordinates(int x, int y)
    {
      return _elements.FirstOrDefault(
        element =>
          x >= element.X && x <= element.X + element.Width
          && y >= element.Y && y <= element.Y + element.Height
      );
    }

    public void Disable()
    {
      _isDisabled = true;
    }

    public void Enable()
    {
      _isDisabled = false;
    }
  }

  private readonly List<Element> _elements = new();
  private readonly EventManager  _eventManager;

  public SceneElementManager(MouseState mouse)
  {
    _eventManager = new EventManager(_elements, mouse);
  }

  public EventManager GetEventManager()
  {
    return _eventManager;
  }

  public SceneElementManager AddElement(Element element)
  {
    _elements.Add(element);
    return this;
  }

  public SceneElementManager RemoveElement(Element element)
  {
    _elements.Remove(element);
    return this;
  }

  // Disables mouse events
  public void DisableEvents()
  {
    _eventManager.Disable();
  }

  public void EnableEvents()
  {
    _eventManager.Enable();
  }
}
