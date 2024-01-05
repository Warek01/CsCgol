namespace CsGame.Models;

public class World
{
  public enum Tile
  {
    GRASS_LIGHT,
    GRASS_DARK,
    SAND,
    WATER_SHALLOW,
    WATER_DEEP
  }

  public readonly int Width;
  public readonly int Height;

  public List<DrawableEntity> Entities = new();

  public World(int w, int h)
  {
    Width  = w;
    Height = h;

    for (int i = 0; i < 200; i++)
    for (int j = 0; j < 200; j++)
      Entities.Add(new Unit(i * 20, j * 20, 20, 20, Color.DarkGreen));
  }
}
