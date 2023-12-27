namespace SDLTest.Models;

public partial class GameOfLife
{
  public class GridDrawingScene : Scene
  {
    public static List<List<bool>> cells = new();
    public static List<List<bool>> newCells = new();

    public static float CellWidth;
    public static float CellHeight;

    public void InitGrid()
    {
      UpdateCellSize();

      for (int row = 0; row < Game._gridRows; row++)
      {
        cells.Add(new List<bool>());
        newCells.Add(new List<bool>());

        for (int col = 0; col < Game._gridColumns; col++)
        {
          cells[row].Add(false);
          newCells[row].Add(false);
        }
      }
    }

    public void DrawGrid()
    {
      DrawGridCells();

      if (Game._shouldDrawGrid)
        DrawGridGrid();
    }

    public void DrawGridGrid()
    {
      SDL_Color c = SDLUtils.GetDrawColor(Game._renderer);

      SDLUtils.SetDrawColor(Game._renderer, Game._gridColor);

      float rowsSpacing    = (float)Window.Height / Game._gridColumns;
      float columnsSpacing = (float)Window.Width / Game._gridRows;

      for (int i = 0; i < Game._gridRows; i++)
      {
        float y = rowsSpacing * i;
        SDL_RenderDrawLineF(Game._renderer, 0, y, Window.Width, y);

        for (int j = 0; j < Game._gridColumns; j++)
        {
          float x = columnsSpacing * j;
          SDL_RenderDrawLineF(Game._renderer, x, 0, x, Window.Height);
        }
      }

      SDLUtils.SetDrawColor(Game._renderer, c);
    }

    public void DrawGridCells()
    {
      SDL_Color initialColor = SDLUtils.GetDrawColor(Game._renderer);
      SDLUtils.SetDrawColor(Game._renderer, Game._cellColor);

      var cellRect = new SDL_FRect
      {
        w = CellWidth,
        h = CellHeight
      };

      for (int row = 0; row < Game._gridRows; row++)
      for (int col = 0; col < Game._gridColumns; col++)
      {
        if (!cells[row][col]) continue;

        cellRect.x = col * CellWidth;
        cellRect.y = row * CellHeight;

        SDL_RenderFillRectF(Game._renderer, ref cellRect);
      }

      SDLUtils.SetDrawColor(Game._renderer, initialColor);
    }

    public void DisposeGrid()
    {
      cells.Clear();
      newCells.Clear();
    }

    public void UpdateCellSize()
    {
      CellWidth  = (float)Window.Width / Game._gridRows;
      CellHeight = (float)Window.Height / Game._gridColumns;
    }
  }
}
