namespace CsGame.Models;

public class Camera : DrawableEntity
{
  public int       ScrollSpeed;
  public int       FastScrollSpeed;
  public int       TargetX;
  public int       TargetY;
  public int       TargetEndX;
  public int       TargetEndY;
  public Direction ScrollingDirection = Direction.NONE;
  public bool      IsFastScrolling;

  private World _world;

  public Camera(int x, int y, int w, int h, World world, Color? color = null)
    : base(x, y, w, h, color)
  {
    _world = world;
    UpdateTargetEnds();
  }

  public void Update()
  {
    UpdateScroll();
    UpdateSurface();

    foreach (var entity in _world.Entities)
    {
      if (!IsEntityInBounds(entity)) continue;

      var unitDstRect = entity.SDL_Rect;
      unitDstRect.x -= TargetX;
      unitDstRect.y -= TargetY;
      SDL_BlitSurface(entity.Surface, IntPtr.Zero, Surface, ref unitDstRect);
    }
  }

  public void ScrollTo(int x, int y)
  {
    TargetX = Math.Clamp(x, 0, _world.Width - Width);
    TargetY = Math.Clamp(y, 0, _world.Height - Height);
    UpdateTargetEnds();
  }

  protected void UpdateTargetEnds()
  {
    TargetEndX = TargetX + Width;
    TargetEndY = TargetY + Height;
  }

  public void Scroll(Direction direction)
  {
    StartScroll(direction);
    UpdateScroll();
    StopScroll(direction);
  }

  public void StartScroll(Direction direction)
  {
    ScrollingDirection |= direction;
  }

  public void StopScroll(Direction direction)
  {
    ScrollingDirection &= ~direction;
  }

  protected void UpdateScroll()
  {
    if (ScrollingDirection == Direction.NONE) return;

    int scrollSpeed = IsFastScrolling ? FastScrollSpeed : ScrollSpeed;

    if ((ScrollingDirection & Direction.UP) != 0)
      TargetY = Math.Max(TargetY - scrollSpeed, 0);
    else if ((ScrollingDirection & Direction.DOWN) != 0)
      TargetY = Math.Min(TargetY + scrollSpeed, _world.Height - Height);

    if ((ScrollingDirection & Direction.LEFT) != 0)
      TargetX = Math.Max(TargetX - scrollSpeed, 0);
    else if ((ScrollingDirection & Direction.RIGHT) != 0)
      TargetX = Math.Min(TargetX + scrollSpeed, _world.Width - Width);

    UpdateTargetEnds();
  }

  protected bool IsEntityInBounds(DrawableEntity entity)
  {
    return entity.X < TargetEndX
      && TargetX < entity.EndX
      && entity.Y < TargetEndY
      && TargetY < entity.EndY;
  }
}
