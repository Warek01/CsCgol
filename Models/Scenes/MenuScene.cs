namespace GameOfLife.Models;

public partial class Game
{
  class MenuScene : Scene
  {
    public readonly UIElement CGOLText = new();

    public override void OnInit()
    {
      Name = "Menu";

      CGOLText.Load(Game.Fonts["main-lg"], "Game of Life");

      ComputeRects();
    }

    public override void OnWindowResize()
    {
      ComputeRects();
    }

    public override void OnToggleFullscreen()
    {
      ComputeRects();
    }

    public override void Dispose() { }

    public override void OnRender()
    {
      CGOLText.Render();
    }

    public override void OnKeyDown()
    {
      if (Keyboard.Key == SDLK_SPACE)
        Game.SetNextScene<EditScene>();
    }

    public override void OnMouseDown()
    {
      if (Mouse.Button == SDL_BUTTON_LEFT)
        Game.SetNextScene<EditScene>();
    }

    public void ComputeRects()
    {
      CGOLText.X = (Window.Width - CGOLText.Width) / 2;
      CGOLText.Y = (Window.Height - CGOLText.Height) / 2;
    }
  }
}
