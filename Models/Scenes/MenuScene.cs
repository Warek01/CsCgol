namespace CsGame.Models;

public class MenuScene : Scene
{
  public     Camera               Camera;
  public new List<DrawableEntity> Entities;

  public MenuScene(SceneInitObject init) : base(init)
  {
    Entities = new List<DrawableEntity>()
    {
      new Unit(0, 0, 30, 100, Color.Blue),
      new Unit(35, 40, 32, 32, Color.Brown),
      new Unit(90, 350, 64, 64, Color.Chartreuse),
      new Unit(1600, 1600, 100, 100, Color.Violet),
    };

    Camera = new Camera(0, 0, 400, 400, Entities, Color.Crimson)
    {
      ScrollSpeed     = 24,
      FastScrollSpeed = 48,
      TargetX         = 0,
      TargetY         = 0,
      BoundX          = 2000,
      BoundY          = 2000
    };
  }

  public override void OnRender()
  {
    Console.WriteLine($"{Camera.TargetX} {Camera.TargetY}");
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
        Camera.ScrollRight();
        break;
      case SDLK_LEFT:
        Camera.ScrollLeft();
        break;
      case SDLK_UP:
        Camera.ScrollUp();
        break;
      case SDLK_DOWN:
        Camera.ScrollDown();
        break;
    }
  }

  public override void Dispose()
  {
    base.Dispose();

    foreach (var entity in Entities)
      entity.Dispose();

    Camera.Dispose();
  }
}
