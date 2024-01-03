namespace GameOfLife.Models;

public class Checkbox : Element
{
  public bool IsChecked
  {
    get => _isChecked;
    set
    {
      CheckboxClipRect.x = value == false ? 0 : 97;
      _isChecked         = value;
    }
  }

  protected IntPtr   CheckboxTexture;
  protected SDL_Rect CheckboxClipRect = new SDL_Rect { w = 78, h = 78, x = 0, y = 0 };
  protected SDL_Rect CheckboxRect     = new SDL_Rect { w = 78, h = 78, x = 0, y = 0 };
  protected Text     Text;

  private bool _isChecked;

  public Checkbox(Renderer renderer, IntPtr font, string text) : base(renderer)
  {
    Text           = new Text(renderer, font, text);
    CheckboxRect.h = Text.Height;
    CheckboxRect.w = Text.Height;
    Rect.h         = Text.Height;
    Rect.w         = Text.Width + Text.Height + 10;

    CheckboxTexture = Renderer.LoadTexture("Assets/Images/Checkbox.png");

    AddEventListener(UiEvent.MOUSE_DOWN, () =>
    {
      IsChecked = !IsChecked;
      InvokeEvent(UiEvent.CHANGE);
    });
  }

  public override void Dispose()
  {
    Renderer.DestroyTexture(CheckboxTexture);
  }

  public override void Render()
  {
    Renderer.DrawTexture(CheckboxTexture, CheckboxRect, CheckboxClipRect);
    Text.Render();
  }

  protected override void OnRectUpdate()
  {
    CheckboxRect.x = X;
    CheckboxRect.y = Y;
    Text.X         = X + CheckboxRect.w + 10;
    Text.Y         = Y;
  }
}
