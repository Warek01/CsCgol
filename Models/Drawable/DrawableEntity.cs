namespace CsGame.Models;

public abstract class DrawableEntity
{
  public int X      { get; protected set; } = 0;
  public int Y      { get; protected set; } = 0;
  public int Width  { get; protected set; } = 0;
  public int Height { get; protected set; } = 0;
  public int EndX   { get; protected set; } = 0;
  public int EndY   { get; protected set; } = 0;


  public DrawableEntity() { }

  public DrawableEntity(int x, int y, int w, int h)
  {
    X      = x;
    Y      = y;
    Width  = w;
    Height = h;
    EndX   = x + w;
    EndY   = y + h;
  }

  public DrawableEntity(int x, int y) => (X, Y) = (x, y);

  public abstract DrawableEntity Render();
  public abstract DrawableEntity Render(DrawableEntity inBoundsEntity);

  public DrawableEntity SetX(int x)
  {
    X    = x;
    EndX = X + Width;
    UpdateEndCoordinates();
    return this;
  }

  public DrawableEntity SetWidth(int w)
  {
    Width = w;
    UpdateEndCoordinates();
    return this;
  }

  public DrawableEntity SetY(int y)
  {
    Y = y;
    UpdateEndCoordinates();
    return this;
  }

  public DrawableEntity SetHeight(int h)
  {
    Height = h;
    UpdateEndCoordinates();
    return this;
  }

  protected void UpdateEndCoordinates()
  {
    EndX = X + Width;
    EndY = Y + Height;
  }
}
