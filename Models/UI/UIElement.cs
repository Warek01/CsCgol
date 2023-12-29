namespace GameOfLife.Models;

public class UIElement : IDisposable
{
  public static IntPtr Renderer;

  public int X
  {
    get => Rect.x;
    set => Rect.x = value;
  }

  public int Y
  {
    get => Rect.y;
    set => Rect.y = value;
  }

  public int Width
  {
    get => Rect.w;
    set => Rect.w = value;
  }

  public int Height
  {
    get => Rect.h;
    set => Rect.h = value;
  }

  protected SDL_Rect Rect;
  protected IntPtr   Texture;


  public void Load(string path)
  {
    Texture = IMG_LoadTexture(Renderer, path);
    SDL_QueryTexture(Texture, out uint _, out int _, out Rect.w, out Rect.h);
  }

  public void Load(IntPtr font, string text, SDL_Color color)
  {
    Texture = SDL_CreateTextureFromSurface(Renderer, TTF_RenderText_Blended(font, text, color));
    SDL_QueryTexture(Texture, out uint _, out int _, out Rect.w, out Rect.h);
  }

  public void Load(IntPtr font, string text)
  {
    var color = new SDL_Color { r = 0x0, g = 0x0, b = 0x0, a = 0xff };
    Load(font, text, color);
  }

  public void Render()
  {
    SDL_RenderCopy(Renderer, Texture, IntPtr.Zero, ref Rect);
  }

  public void Dispose()
  {
    SDL_DestroyTexture(Texture);
  }
}
