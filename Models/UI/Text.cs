namespace GameOfLife.Models;

public class Text : Element
{
  public string TextValue { get; private set; }

  public uint WrapWidth
  {
    get => _wrapWidth;
    set
    {
      _wrapWidth = value;
      UpdateTexture();
    }
  }

  public Color Color { get; private set; } = new Color(0x000000FF);

  protected IntPtr Texture = IntPtr.Zero;
  protected IntPtr Font    = IntPtr.Zero;

  private uint _wrapWidth = 0;

  public Text(Renderer renderer) : base(renderer) { }

  public Text(Renderer renderer, IntPtr font, string text, uint wrapWidth = 0) : base(renderer)
  {
    TextValue = text;
    Font      = font;
    WrapWidth = wrapWidth;
    UpdateTexture();
  }

  public Text(Renderer renderer, IntPtr font) : base(renderer)
  {
    Font = font;
  }

  public void SetText(string text)
  {
    TextValue = text;
    UpdateTexture();
  }

  public void SetColor(Color color)
  {
    Color = color;
    UpdateTexture();
  }

  public override void Render()
  {
    Renderer.DrawTexture(Texture, Rect);
  }

  public override void Dispose()
  {
    SDL_DestroyTexture(Texture);
  }

  protected void UpdateTexture()
  {
    if (Texture != IntPtr.Zero)
      SDL_DestroyTexture(Texture);

    Texture = WrapWidth > 0
      ? Renderer.LoadTextTextureWrapped(Font, TextValue, Color, WrapWidth)
      : Renderer.LoadTextTexture(Font, TextValue, Color);

    SDL_QueryTexture(Texture, out uint _, out int _, out Rect.w, out Rect.h);
  }
}
