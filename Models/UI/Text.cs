namespace GameOfLife.Models;

public class Text : Element
{
  #region Public properties

  public string TextValue { get; private set; }

  public SDL_Color Color { get; private set; } = new SDL_Color { r = 0x00, g = 0x00, b = 0x00, a = 0xFF };

  #endregion

  #region Protected properties

  protected IntPtr Texture = IntPtr.Zero;
  protected IntPtr Font    = IntPtr.Zero;

  #endregion

  #region Private properties

  #endregion

  #region Constructors

  public Text(Renderer renderer) : base(renderer) { }

  public Text(Renderer renderer, IntPtr font, string text) : base(renderer)
  {
    TextValue = text;
    Font      = font;
    UpdateTexture();
  }

  public Text(Renderer renderer, IntPtr font) : base(renderer)
  {
    Font = font;
  }

  #endregion

  #region Public methods

  public void SetText(string text)
  {
    TextValue = text;
    UpdateTexture();
  }

  public void SetColor(SDL_Color color)
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

  #endregion

  #region Protected methods

  protected void UpdateTexture()
  {
    if (Texture != IntPtr.Zero)
      SDL_DestroyTexture(Texture);

    Texture = Renderer.LoadTextTexture(Font, TextValue, Color);
    SDL_QueryTexture(Texture, out uint _, out int _, out Rect.w, out Rect.h);
  }

  #endregion

  #region Private methods

  #endregion
}
