namespace GameOfLife.Models;

public partial class Game
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

      for (int row = 0; row < Options.GridRows; row++)
      {
        cells.Add(new List<bool>());
        newCells.Add(new List<bool>());

        for (int col = 0; col < Options.GridColumns; col++)
        {
          cells[row].Add(false);
          newCells[row].Add(false);
        }
      }
    }

    public void DrawGrid()
    {
      DrawGridCells();

      if (Options.ShouldDrawGrid)
        DrawGridGrid();
    }

    public void DrawGridGrid()
    {
      SDL_Color c = Render.GetDrawColor();
      Render.SetDrawColor(Game.GridColor);

      float rowsSpacing    = (float)Window.Height / Options.GridColumns;
      float columnsSpacing = (float)Window.Width / Options.GridRows;

      for (int i = 0; i < Options.GridRows; i++)
      {
        float y = rowsSpacing * i;
        Render.DrawLine(0, y, Window.Width, y);

        for (int j = 0; j < Options.GridColumns; j++)
        {
          float x = columnsSpacing * j;
          Render.DrawLine(x, 0, x, Window.Height);
        }
      }

      Render.SetDrawColor(c);
    }

    public void DrawGridCells()
    {
      SDL_Color initialColor = Render.GetDrawColor();
      Render.SetDrawColor(Game.CellColor);

      var cellRect = new SDL_FRect
      {
        w = CellWidth,
        h = CellHeight
      };

      for (int row = 0; row < Options.GridRows; row++)
      for (int col = 0; col < Options.GridColumns; col++)
      {
        if (!cells[row][col]) continue;

        cellRect.x = col * CellWidth;
        cellRect.y = row * CellHeight;

        Render.FillRect(cellRect);
      }

      Render.SetDrawColor(initialColor);
    }

    public void DisposeGrid()
    {
      cells.Clear();
      newCells.Clear();
    }

    public void UpdateCellSize()
    {
      CellWidth  = (float)Window.Width / Options.GridRows;
      CellHeight = (float)Window.Height / Options.GridColumns;
    }
  }
}
