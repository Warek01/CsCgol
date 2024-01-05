namespace CsGame.Models;

public class Button : Element
{
  public Color BackgroundColor = Color.White;
  public Color TextColor       = Color.Black;

  protected SDL_Rect BackgroundRect = new SDL_Rect { x = 0, y = 0, w = 0, h = 0 };
  protected SDL_Rect TextRect       = new SDL_Rect { x = 0, y = 0, w = 0, h = 0 };
  protected IntPtr   BackgroundTexture;
  protected IntPtr   TextTexture;

  public Button(IntPtr renderer, IntPtr font, string text) : base(renderer)
  {
    LoadText(font, text);
  }

  public void LoadBackground(string path)
  {
    BackgroundTexture = IMG_Load(path);
    SDL_QueryTexture(BackgroundTexture, out uint _, out int _, out BackgroundRect.w, out BackgroundRect.h);
  }

  public void LoadText(IntPtr font, string text)
  {
    TextTexture = SdlUtils.LoadTextTexture(Renderer, font, text, TextColor);
    SDL_QueryTexture(TextTexture, out uint _, out int _, out TextRect.w, out TextRect.h);
  }

  public override void Render()
  {
    Color initialColor = SdlUtils.GetDrawColor(Renderer);
    SdlUtils.SetDrawColor(Renderer, BackgroundColor);
    SDL_RenderFillRect(Renderer, ref Rect);
    // Renderer.DrawTexture(BackgroundTexture, Rect);
    // Renderer.DrawTexture(TextTexture, TextRect);
    SdlUtils.SetDrawColor(Renderer, initialColor);
  }

  public void AdjustSizeAuto()
  {
    Rect.w = Math.Max(BackgroundRect.w, TextRect.w);
    Rect.h = Math.Max(BackgroundRect.h, TextRect.w);
    OnRectUpdate();
  }

  public void AdjustSizeToText()
  {
    Rect.w = TextRect.w;
    Rect.h = TextRect.h;
    OnRectUpdate();
  }

  public void AdjustSizeToBackground()
  {
    Rect.w = BackgroundRect.w;
    Rect.h = BackgroundRect.h;
    OnRectUpdate();
  }

  public override void Dispose()
  {
    SDL_DestroyTexture(BackgroundTexture);
    SDL_DestroyTexture(TextTexture);
  }

  protected override void OnRectUpdate()
  {
    TextRect.x = Rect.x + (Rect.w - TextRect.w) / 2;
    TextRect.y = Rect.y + (Rect.h - TextRect.h) / 2;
  }
}
