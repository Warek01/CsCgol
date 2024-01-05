namespace CsGame.Models;

public abstract class DrawableEntity : IDisposable
{
  public int X      { get; protected set; }
  public int Y      { get; protected set; }
  public int Width  { get; protected set; }
  public int Height { get; protected set; }
  public int EndX   { get; protected set; }
  public int EndY   { get; protected set; }

  public SDL_Rect SDL_Rect { get; protected set; }
  public Color    Color    { get; protected set; }
  public IntPtr   Surface  { get; protected set; }

  public DrawableEntity(int x, int y, int w, int h, Color? color = null)
  {
    X      = x;
    Y      = y;
    Width  = w;
    Height = h;
    Color  = color ?? Color.Transparent;

    UpdateSurface();
  }

  public void SetX(int x)
  {
    X    = x;
    EndX = X + Width;
    UpdateSurface();
  }

  public void SetWidth(int w)
  {
    Width = w;
    UpdateSurface();
  }

  public void SetY(int y)
  {
    Y = y;
    UpdateSurface();
  }

  public void SetHeight(int h)
  {
    Height = h;
    UpdateSurface();
  }

  public void Dispose()
  {
    SDL_FreeSurface(Surface);
  }

  public void SetBackgroundColor(Color color)
  {
    Color = color;
    UpdateSurface();
  }

  protected void UpdateSurface()
  {
    Surface = SDL_CreateRGBSurfaceWithFormat(0, Width, Height, 32, SDL_PIXELFORMAT_RGBA8888);

    EndX     = X + Width;
    EndY     = Y + Height;
    SDL_Rect = new SDL_Rect { x = X, y = Y, w = Width, h = Height };

    var rect = new SDL_Rect { x = 0, y = 0, w = Width, h = Height };
    SDL_FillRect(Surface, ref rect, SdlUtils.ColorToSurfaceFormat(Surface, Color));
  }
}
