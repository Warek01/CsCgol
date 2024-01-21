namespace CsGame.Models;

public static class SdlUtils
{
  public static void SetDrawColor(nint renderer, Color color)
  {
    SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
  }

  public static Color GetDrawColor(nint renderer)
  {
    SDL_GetRenderDrawColor(renderer, out byte r, out byte g, out byte b, out byte a);
    return Color.FromArgb(a, r, g, b);
  }

  public static void DrawRect(nint renderer, int x, int y, int width, int height)
  {
    var rect = new SDL_Rect { x = x, y = y, w = width, h = height };
    SDL_RenderDrawRect(renderer, ref rect);
  }

  public static void DrawRect(nint renderer, float x, float y, float width, float height)
  {
    var rect = new SDL_FRect { x = x, y = y, w = width, h = height };
    SDL_RenderDrawRectF(renderer, ref rect);
  }

  public static void FillRect(nint renderer, int x, int y, int width, int height)
  {
    var rect = new SDL_Rect { x = x, y = y, w = width, h = height };
    SDL_RenderFillRect(renderer, ref rect);
  }

  public static void FillRect(nint renderer, float x, float y, float width, float height)
  {
    var rect = new SDL_FRect { x = x, y = y, w = width, h = height };
    SDL_RenderFillRectF(renderer, ref rect);
  }

  public static void Screenshot(nint renderer, string filename, int x, int y, int width, int height)
  {
    var rect       = new SDL_Rect { x = x, y = y, w = width, h = height };
    var surfacePtr = SDL_CreateRGBSurfaceWithFormat(0, width, height, 24, SDL_PIXELFORMAT_RGBA8888);
    var surface    = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);

    SDL_RenderReadPixels(renderer, ref rect, SDL_PIXELFORMAT_RGB24, surface.pixels, surface.pitch);
    Marshal.StructureToPtr(surface, surfacePtr, true);

    IMG_SavePNG(surfacePtr, filename);
    SDL_FreeSurface(surfacePtr);
  }

  public static void Screenshot(nint renderer, int x, int y, int width, int height)
  {
    string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");

    if (!Directory.Exists(basePath))
      Directory.CreateDirectory(basePath);

    string filename = Path.Combine(basePath, $"{DateTime.Now:HH:mm:ss_dd-MM-yyyy}.png");

    Screenshot(renderer, filename, x, y, width, height);
  }

  public static uint ColorToSurfaceFormat(nint surfacePtr, Color color)
  {
    SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
    return SDL_MapRGBA(surface.format, color.R, color.G, color.B, color.A);
  }

  public static nint LoadTextSurface(nint font, string text, Color color)
  {
    return TTF_RenderUNICODE_Blended(font, text, ToSdlColor(color));
  }

  public static nint LoadTextSurfaceWrapped(nint font, string text, Color color, uint wrap)
  {
    return TTF_RenderUNICODE_Blended_Wrapped(font, text, ToSdlColor(color), wrap);
  }

  public static nint LoadTextTexture(nint renderer, nint font, string text, Color color)
  {
    return SDL_CreateTextureFromSurface(renderer, LoadTextSurface(font, text, color));
  }

  public static nint LoadTextTextureWrapped(nint renderer, nint font, string text, Color color, uint wrap)
  {
    return SDL_CreateTextureFromSurface(renderer, LoadTextSurfaceWrapped(font, text, color, wrap));
  }

  public static SDL_Color ToSdlColor(Color color)
  {
    return new SDL_Color { r = color.R, g = color.G, b = color.B, a = color.A };
  }

  public static SDL_Rect CreateRect(int x, int y, int w, int h)
  {
    return new SDL_Rect { x = x, y = y, w = w, h = h };
  }

  public static SDL_Rect CreateRect(IRectangleBounds r)
  {
    return new SDL_Rect { x = r.X, y = r.Y, w = r.Width, h = r.Height };
  }
}
