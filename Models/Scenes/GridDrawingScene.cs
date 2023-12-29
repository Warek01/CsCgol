namespace GameOfLife.Models;

public class GridDrawingScene : Scene
{
  public static List<List<bool>> Cells    = new();
  public static List<List<bool>> NewCells = new();

  public static float CellWidth;
  public static float CellHeight;

  public GridDrawingScene(SceneInitObject init) : base(init) { }

  public void InitGrid()
  {
    UpdateCellSize();

    for (int row = 0; row < OptionsState.GridRows; row++)
    {
      Cells.Add(new List<bool>());
      NewCells.Add(new List<bool>());

      for (int col = 0; col < OptionsState.GridColumns; col++)
      {
        Cells[row].Add(false);
        NewCells[row].Add(false);
      }
    }
  }

  public void DrawGrid()
  {
    DrawGridCells();

    if (OptionsState.ShouldDrawGrid)
      DrawGridGrid();
  }

  public void DrawGridGrid()
  {
    SDL_Color c = Renderer.GetDrawColor();
    Renderer.SetDrawColor(Colors["Grid"]);

    float rowsSpacing    = (float)Window.Height / OptionsState.GridRows;
    float columnsSpacing = (float)Window.Width / OptionsState.GridColumns;

    for (int i = 0; i < OptionsState.GridRows; i++)
    {
      float y = rowsSpacing * i;
      Renderer.DrawLine(0, y, Window.Width, y);

      for (int j = 0; j < OptionsState.GridColumns; j++)
      {
        float x = columnsSpacing * j;
        Renderer.DrawLine(x, 0, x, Window.Height);
      }
    }

    Renderer.SetDrawColor(c);
  }

  public void DrawGridCells()
  {
    SDL_Color initialColor = Renderer.GetDrawColor();
    Renderer.SetDrawColor(Colors["Cell"]);

    var cellRect = new SDL_FRect
    {
      w = CellWidth,
      h = CellHeight
    };

    for (int row = 0; row < OptionsState.GridRows; row++)
    for (int col = 0; col < OptionsState.GridColumns; col++)
    {
      if (!Cells[row][col]) continue;

      cellRect.x = col * CellWidth;
      cellRect.y = row * CellHeight;

      Renderer.FillRect(cellRect);
    }

    Renderer.SetDrawColor(initialColor);
  }

  public void DisposeGrid()
  {
    Cells.Clear();
    NewCells.Clear();
  }

  public void UpdateCellSize()
  {
    CellWidth  = (float)Window.Width / OptionsState.GridColumns;
    CellHeight = (float)Window.Height / OptionsState.GridRows;
  }
}
