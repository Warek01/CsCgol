namespace GameOfLife.Models;

public class EndScreenScene : Scene
{
  public IntPtr   Texture;
  public SDL_Rect Rect;

  public EndScreenScene(SceneInitObject init) : base(init)
  {
    IntPtr surf = TTF_RenderText_Blended(
      Fonts["Main-lg"],
      "Game over",
      new SDL_Color { r = 0x00, g = 0x00, b = 0x00, a = 0xff }
    );

    Texture = Renderer.CreateTexture(surf);
    SDL_QueryTexture(Texture, out uint _, out int _, out Rect.w, out Rect.h);
    Rect.x = (Window.Width - Rect.w) / 2;
    Rect.y = (Window.Height - Rect.h) / 2;
  }

  public override void OnWindowResize()
  {
    Rect.x = (Window.Width - Rect.w) / 2;
    Rect.y = (Window.Height - Rect.h) / 2;
  }

  public override void Dispose()
  {
    SDL_DestroyTexture(Texture);
  }

  public override void OnRender()
  {
    Renderer.DrawTexture(Texture, Rect);
  }
}
