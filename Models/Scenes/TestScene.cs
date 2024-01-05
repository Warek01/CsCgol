namespace CsGame.Models;

public class TestScene : Scene
{
  public Camera Camera;
  public World  World;

  Color GetRandomColor(Random random)
  {
    return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
  }

  public TestScene(SceneInitObject init) : base(init)
  {
    World = new World(10000, 10000);

    var random = new Random();
    for (int i = 0; i < 100; i++)
    {
      // Generate random values for position, width, height, and color
      int   x           = random.Next(0, 10001);  // Random x position between 0 and 10000
      int   y           = random.Next(0, 10001);  // Random y position between 0 and 10000
      int   width       = random.Next(0, 257);    // Random width between -256 and 256
      int   height      = random.Next(0, 257);    // Random height between -256 and 256
      Color randomColor = GetRandomColor(random); // Get a random color

      // Create a new Unit with random parameters and add it to the list
      World.Entities.Add(new Unit(x, y, Math.Abs(width), Math.Abs(height), randomColor));
    }

    Camera = new Camera(0, 0, Screen.Width, Screen.Height, World, Color.Crimson)
    {
      ScrollSpeed     = 24,
      FastScrollSpeed = 48,
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

    foreach (var entity in World.Entities)
      entity.Dispose();

    Camera.Dispose();
  }
}
