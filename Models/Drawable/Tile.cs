namespace CsGame.Models;

public enum TileType
{
  GRASS_LIGHT,
  GRASS_DARK,
  SAND,
  WATER_SHALLOW,
  WATER_DEEP
}

public class Tile : DrawableEntity
{
  public const int WIDTH  = 32;
  public const int HEIGHT = 32;

  public readonly TileType Type;

  protected static int RefCount = 0;

  protected static readonly Dictionary<TileType, nint> SharedSurfacesMap = new();

  protected static readonly Dictionary<TileType, Color> ColorMap = new()
  {
    [TileType.WATER_DEEP]    = Color.DarkBlue,
    [TileType.WATER_SHALLOW] = Color.LightBlue,
    [TileType.GRASS_LIGHT]   = Color.LimeGreen,
    [TileType.GRASS_DARK]    = Color.DarkGreen,
    [TileType.SAND]          = Color.PaleGoldenrod
  };

  public Tile(int row, int col, TileType type)
  {
    RefCount++;
    Type     = type;
    Color    = ColorMap[type];
    Width    = WIDTH;
    Height   = HEIGHT;
    X        = col * WIDTH;
    Y        = row * HEIGHT;

    CreateNewSharedSurface();
  }

  protected void CreateNewSharedSurface()
  {
    UpdateRect();

    if (SharedSurfacesMap.TryGetValue(Type, out nint value))
    {
      SDL_Surface = value;
      return;
    }

    SDL_Surface = SDL_CreateRGBSurfaceWithFormat(0, WIDTH, HEIGHT, 32, SDL_PIXELFORMAT_RGBA8888);
    SharedSurfacesMap[Type] = SDL_Surface;

    uint color = SdlUtils.ColorToSurfaceFormat(SDL_Surface, ColorMap[Type]);
    SDL_FillRect(SDL_Surface, nint.Zero, color);
  }

  public static void ClearSharedSurfaces()
  {
    foreach (var pair in SharedSurfacesMap)
      SDL_FreeSurface(pair.Value);
  }

  public override void Dispose() { }
}
