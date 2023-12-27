namespace SDLTest.Models;

public partial class GameOfLife
{
  class MenuScene : Scene
  {
    public nint     Texture;
    public SDL_Rect Rect;

    public override void OnInit()
    {
      Name = "Menu";

      nint surf = TTF_RenderText_Blended(
        Game._fonts["main-lg"],
        "Game of Life",
        new SDL_Color { r = 0x00, g = 0x00, b = 0x00, a = 0xff }
      );

      Texture = SDL_CreateTextureFromSurface(Game._renderer, surf);
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
      SDL_RenderCopy(Game._renderer, Texture, 0, ref Rect);
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
  }
}
