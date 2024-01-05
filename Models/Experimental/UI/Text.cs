namespace CsGame.Models;

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

  public Color Color { get; private set; } = Color.Black;

  protected IntPtr Texture = IntPtr.Zero;
  protected IntPtr Font    = IntPtr.Zero;

  private uint _wrapWidth = 0;

  public Text(IntPtr renderer) : base(renderer) { }

  public Text(IntPtr renderer, IntPtr font, string text, uint wrapWidth = 0) : base(renderer)
  {
    TextValue = text;
    Font      = font;
    WrapWidth = wrapWidth;
    UpdateTexture();
  }

  public Text(IntPtr renderer, IntPtr font) : base(renderer)
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
    SDL_RenderCopy(Renderer, Texture, IntPtr.Zero, ref Rect);
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
      ? SdlUtils.LoadTextTextureWrapped(Renderer, Font, TextValue, Color, WrapWidth)
      : SdlUtils.LoadTextTexture(Renderer, Font, TextValue, Color);

    SDL_QueryTexture(Texture, out uint _, out int _, out Rect.w, out Rect.h);
  }
}
