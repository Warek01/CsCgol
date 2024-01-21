namespace CsGame.Models;

public class TestScene : Scene
{
  public Camera             Camera;
  public World              World;
  public MouseState         MouseState;
  public SelectionRectangle SelectionRectangle;

  public TestScene(Game game) : base(game)
  {
    SelectionRectangle = new SelectionRectangle();
    World              = new World(256, 256);

    for (int row = 0; row < World.TileRows; row++)
    for (int col = 0; col < World.TileColumns; col++)
    {
      World.Tiles[row, col] = new Tile(row, col, (row + col) % 2 == 0 ? TileType.SAND : TileType.GRASS_DARK);
    }

    Camera = new Camera(0, 0, Screen.Width, Screen.Height, World)
    {
      ScrollSpeed     = InitialConfig.Game.ScrollSpeed,
      FastScrollSpeed = InitialConfig.Game.FastScrollSpeed,
      TargetX         = 0,
      TargetY         = 0,
    };
  }

  public override void OnRender()
  {
    if (Window.IsFocused)
    {
      if (Mouse.X == 0)
        Camera.Scroll(Direction.LEFT);
      else if (Mouse.X == Screen.Width - 1)
        Camera.Scroll(Direction.RIGHT);

      if (Mouse.Y == 0)
        Camera.Scroll(Direction.UP);
      else if (Mouse.Y == Screen.Height - 1)
        Camera.Scroll(Direction.DOWN);
    }

    Camera.Update();
    nint cameraTexture = SDL_CreateTextureFromSurface(Renderer, Camera.SDL_Surface);
    var  cameraRect    = SdlUtils.CreateRect(Camera);

    SDL_RenderCopy(Renderer, cameraTexture, nint.Zero, ref cameraRect);
    SDL_DestroyTexture(cameraTexture);
  }

  public override void OnKeyDown()
  {
    base.OnKeyDown();

    switch (Keyboard.Key)
    {
      case SDLK_RIGHT:
        Camera.StartScroll(Direction.RIGHT);
        break;
      case SDLK_LEFT:
        Camera.StartScroll(Direction.LEFT);
        break;
      case SDLK_UP:
        Camera.StartScroll(Direction.UP);
        break;
      case SDLK_DOWN:
        Camera.StartScroll(Direction.DOWN);
        break;
    }
  }

  public override void OnKeyUp()
  {
    base.OnKeyUp();

    switch (Keyboard.Key)
    {
      case SDLK_RIGHT:
        Camera.StopScroll(Direction.RIGHT);
        break;
      case SDLK_LEFT:
        Camera.StopScroll(Direction.LEFT);
        break;
      case SDLK_UP:
        Camera.StopScroll(Direction.UP);
        break;
      case SDLK_DOWN:
        Camera.StopScroll(Direction.DOWN);
        break;
    }
  }

  public override void OnMouseDown()
  {
    if (MouseState == MouseState.DEFAULT) { }
  }

  public override void Dispose()
  {
    base.Dispose();

    foreach (var tile in World.Tiles)
      tile.Dispose();

    Camera.Dispose();

    Tile.ClearSharedSurfaces();
  }
}
