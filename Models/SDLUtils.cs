namespace SDLTest.Models;

public static class SDLUtils
{
  public static SDL_Color GetDrawColor(nint renderer)
  {
    SDL_GetRenderDrawColor(renderer, out byte r, out byte g, out byte b, out byte a);
    return new SDL_Color { r = r, g = g, b = b, a = a };
  }

  public static void SetDrawColor(nint renderer, SDL_Color c)
  {
    SDL_SetRenderDrawColor(renderer, c.r, c.g, c.b, c.a);
  }
}