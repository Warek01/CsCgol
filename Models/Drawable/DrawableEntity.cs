namespace CsGame.Models;

public abstract class DrawableEntity : IDisposable, IRectangleBounds, ISdlSurfaceProvider
{
  public int    X           { get; protected set; } = 0;
  public int    Y           { get; protected set; } = 0;
  public int    Width       { get; protected set; } = 0;
  public int    Height      { get; protected set; } = 0;
  public int    EndX        { get; protected set; } = 0;
  public int    EndY        { get; protected set; } = 0;
  public Color  Color       { get; protected set; } = Color.Transparent;
  public nint SDL_Surface { get; protected set; } = nint.Zero;

  public DrawableEntity() { }

  public DrawableEntity(int x, int y, int w, int h, Color? color = null)
  {
    X        = x;
    Y        = y;
    Width    = w;
    Height   = h;
    Color    = color ?? Color.Transparent;
    EndX     = X + Width;
    EndY     = Y + Height;
  }

  public virtual bool ContainsInBounds(int x, int y)
  {
    return x >= X && x <= EndX && y >= Y && y <= EndY;
  }

  public virtual bool ContainsInBounds(IRectangleBounds d)
  {
    return d.X < EndX
      && X < d.EndX
      && d.Y < EndY
      && Y < d.EndY;
  }

  public static bool ContainsInBounds(IRectangleBounds d1, IRectangleBounds d2)
  {
    return d1.X < d2.EndX
      && d2.X < d1.EndX
      && d1.Y < d2.EndY
      && d2.Y < d1.EndY;
  }

  public virtual void SetX(int x)
  {
    X    = x;
    EndX = X + Width;
    Update();
  }

  public virtual void SetWidth(int w)
  {
    Width = w;
    Update();
  }

  public virtual void SetY(int y)
  {
    Y = y;
    Update();
  }

  public virtual void SetHeight(int h)
  {
    Height = h;
    Update();
  }

  public virtual void SetBackgroundColor(Color color)
  {
    Color = color;
    Update();
  }

  public virtual void Dispose()
  {
    SDL_FreeSurface(SDL_Surface);
    SDL_Surface = nint.Zero;
  }


  public virtual void Update()
  {
    UpdateRect();
    UpdateSurface();
  }

  public virtual void UpdateRect()
  {
    EndX     = X + Width;
    EndY     = Y + Height;
  }

  public virtual void UpdateSurface()
  {
    if (SDL_Surface != nint.Zero)
      SDL_FreeSurface(SDL_Surface);

    SDL_Surface = SDL_CreateRGBSurfaceWithFormat(0, Width, Height, 32, SDL_PIXELFORMAT_RGBA8888);

    var surfaceRect = new SDL_Rect { x = 0, y = 0, w = Width, h = Height };
    SDL_FillRect(SDL_Surface, ref surfaceRect, SdlUtils.ColorToSurfaceFormat(SDL_Surface, Color));
  }
}
