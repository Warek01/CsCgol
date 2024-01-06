namespace CsGame.Models;

public class TestScene : Scene
{
  public Camera Camera;
  public World  World;

  public TestScene(SceneInitObject init) : base(init)
  {
    World = new World(1024, 1024);

    for (int row = 0; row < World.TileRows; row++)
    for (int col = 0; col < World.TileColumns; col++)
    {
      World.Tiles[row, col] = new Tile(row, col, (row + col) % 2 == 0 ? TileType.SAND : TileType.GRASS_DARK);
    }

    Camera = new Camera(0, 0, Screen.Width, Screen.Height, World)
    {
      ScrollSpeed     = 20,
      FastScrollSpeed = 40,
      TargetX         = 0,
      TargetY         = 0,
    };
  }

  public override void OnRender()
  {
    if (Mouse.X == 0)
      Camera.Scroll(Direction.LEFT);
    else if (Mouse.X == Screen.Width - 1)
      Camera.Scroll(Direction.RIGHT);

    if (Mouse.Y == 0)
      Camera.Scroll(Direction.UP);
    else if (Mouse.Y == Screen.Height - 1)
      Camera.Scroll(Direction.DOWN);

    Camera.Update();
    IntPtr cameraTexture = SDL_CreateTextureFromSurface(Renderer, Camera.Surface);
    var    cameraRect    = Camera.SDL_Rect;
    SDL_RenderCopy(Renderer, cameraTexture, IntPtr.Zero, ref cameraRect);
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

  public override void Dispose()
  {
    base.Dispose();

    foreach (var tile in World.Tiles)
      tile.Dispose();

    Camera.Dispose();

    Tile.ClearSharedSurfaces();
  }
}
