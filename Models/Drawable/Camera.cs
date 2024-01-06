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

  public Camera(int x, int y, int w, int h, World world)
    : base(x, y, w, h, Color.White)
  {
    _world  = world;
    Surface = SDL_CreateRGBSurfaceWithFormat(0, Width, Height, 32, SDL_PIXELFORMAT_RGBA8888);

    Update();
  }

  public void Update()
  {
    UpdateScroll();
    UpdateTargetEnds();
    Draw();
  }

  public void Draw()
  {
    int rowFrom = Math.Max(TargetY / Tile.HEIGHT - 1, 0);
    int rowTo   = Math.Min(TargetEndY / Tile.HEIGHT + 1, _world.TileRows - 1);
    int colFrom = Math.Max(TargetX / Tile.WIDTH - 1, 0);
    int colTo   = Math.Min(TargetEndX / Tile.WIDTH + 1, _world.TileColumns - 1);

    for (int row = rowFrom; row < rowTo; row++)
    for (int col = colFrom; col < colTo; col++)
    {
      var tile = _world.Tiles[row, col];

      if (!IsEntityInBounds(tile)) continue;

      var unitDstRect = tile.SDL_Rect;
      unitDstRect.x -= TargetX;
      unitDstRect.y -= TargetY;
      SDL_BlitSurface(tile.Surface, IntPtr.Zero, Surface, ref unitDstRect);
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
