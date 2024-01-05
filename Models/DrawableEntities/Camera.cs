namespace CsGame.Models;

public class Camera : DrawableEntity
{
  public int ScrollSpeed;
  public int FastScrollSpeed;
  public int TargetX;
  public int TargetY;
  public int TargetEndX;
  public int TargetEndY;
  public int BoundX;
  public int BoundY;

  private List<DrawableEntity> _entities;

  public Camera(int x, int y, int w, int h, List<DrawableEntity> entities, Color? color = null)
    : base(x, y, w, h, color)
  {
    _entities = entities;
    UpdateTargetEnds();
  }

  public void Update()
  {
    UpdateSurface();

    foreach (var entity in _entities)
    {
      if (!IsEntityInBounds(entity)) continue;

      var unitDstRect = entity.SDL_Rect;
      unitDstRect.x -= TargetX;
      unitDstRect.y -= TargetY;
      SDL_BlitSurface(entity.Surface, IntPtr.Zero, Surface, ref unitDstRect);
    }
  }

  public void ScrollDown()
  {
    TargetY = Math.Min(TargetY + ScrollSpeed, BoundY - Height);
    UpdateTargetEnds();
  }

  public void ScrollUp()
  {
    TargetY = Math.Max(TargetY - ScrollSpeed, 0);
    UpdateTargetEnds();
  }

  public void ScrollRight()
  {
    TargetX = Math.Min(TargetX + ScrollSpeed, BoundX - Width);
    UpdateTargetEnds();
  }

  public void ScrollLeft()
  {
    TargetX = Math.Max(TargetX - ScrollSpeed, 0);
    UpdateTargetEnds();
  }

  public void ScrollTo(int x, int y)
  {
    TargetX = Math.Clamp(x, 0, BoundX - Width);
    TargetY = Math.Clamp(y, 0, BoundY - Height);
    UpdateTargetEnds();
  }

  public void FastScrollDown()
  {
    TargetY = Math.Min(TargetY + FastScrollSpeed, BoundY - Height);
    UpdateTargetEnds();
  }

  public void FastScrollUp()
  {
    TargetY = Math.Max(TargetY - FastScrollSpeed, 0);
    UpdateTargetEnds();
  }

  public void FastScrollRight()
  {
    TargetX = Math.Min(TargetX + FastScrollSpeed, BoundX - Width);
    UpdateTargetEnds();
  }

  public void FastScrollLeft()
  {
    TargetX = Math.Max(TargetX - FastScrollSpeed, 0);
    UpdateTargetEnds();
  }

  protected void UpdateTargetEnds()
  {
    TargetEndX = TargetX + Width;
    TargetEndY    = TargetY + Height;
  }

  protected bool IsEntityInBounds(DrawableEntity entity)
  {
    return entity.X < TargetEndX
      && TargetX < entity.EndX
      && entity.Y < TargetEndY
      && TargetY < entity.EndY;
  }
}
