namespace CsGame.Models;

public class World
{
  // No. of columns of tiles
  public readonly int TileColumns;

  // No. of rows of tiles
  public readonly int TileRows;

  public readonly int Width;
  public readonly int Height;

  public DrawableEntity[,] Tiles;

  public World(int rows, int columns)
  {
    TileColumns = columns;
    TileRows    = rows;
    Width       = columns * Tile.WIDTH;
    Height      = rows * Tile.HEIGHT;

    Tiles = new DrawableEntity[rows, columns];

    for (int row = 0; row < TileRows; row++)
    for (int col = 0; col < TileColumns; col++)
      Tiles[row, col] = new Tile(row, col, TileType.WATER_DEEP);
  }
}
