namespace SDLTest.Models;

public partial class GameOfLife
{
  public class SimulationScene : GridDrawingScene
  {
    public static int GridFpsChangeFactor = 10;
    public static int GridLowestFps       = 1;

    public bool IsPaused;
    public int  GridFps = 10;
    public int  GridFramesToSkipUntilUpdate;

    public override void OnInit()
    {
      Name = "Simulation";
      UpdateGridFramesToSkipUntilUpdate();
    }

    public override void OnRender()
    {
      DrawGrid();

      if (!IsPaused && Game._frameIndex % GridFramesToSkipUntilUpdate == 0)
      {
        bool updated = UpdateGrid();

        if (!updated)
          Game.SetNextScene<EndScreenScene>();
      }
    }

    public override void OnWindowResize()
    {
      UpdateCellSize();
    }

    public override void OnToggleFullscreen()
    {
      UpdateCellSize();
    }

    public override void OnMouseDown()
    {
      if (Mouse.Button == SDL_BUTTON_LEFT)
        IsPaused = !IsPaused;
    }

    public override void OnKeyDown()
    {
      switch (Keyboard.Key)
      {
        case SDLK_SPACE:
          IsPaused = !IsPaused;
          break;
        case SDLK_LEFT:
          DecreaseGridUpdateSpeed();
          break;
        case SDLK_RIGHT:
          IncreaseGridUpdateSpeed();
          break;
        case SDLK_g:
          Game._shouldDrawGrid = !Game._shouldDrawGrid;
          break;
      }
    }

    public override void Dispose()
    {
      DisposeGrid();
    }

    public bool UpdateGrid()
    {
      bool updated = false;

      for (int row = 0; row < Game._gridRows; row++)
      for (int col = 0; col < Game._gridColumns; col++)
      {
        int count = CountNeighbors(row, col);

        newCells[row][col] = (!cells[row][col] && count == 3) || (cells[row][col] && count is 2 or 3);

        if (newCells[row][col] != cells[row][col])
          updated = true;
      }

      for (int row = 0; row < Game._gridRows; row++)
      for (int col = 0; col < Game._gridColumns; col++)
      {
        cells[row][col]    = newCells[row][col];
        newCells[row][col] = false;
      }

      return updated;
    }

    public int CountNeighbors(int row, int column)
    {
      int count = 0;

      // upper 3 cells
      if (row > 0)
      {
        if (cells[row - 1][column]) count++;
        if (column > 0 && cells[row - 1][column - 1]) count++;
        if (column < Game._gridRows - 1 && cells[row - 1][column + 1]) count++;
      }

      // left and right cells
      if (column > 0 && cells[row][column - 1]) count++;
      if (column < Game._gridColumns - 1 && cells[row][column + 1]) count++;

      // lower 3 cells
      if (row < Game._gridRows - 1)
      {
        if (cells[row + 1][column]) count++;
        if (column > 0 && cells[row + 1][column - 1]) count++;
        if (column < Game._gridColumns - 1 && cells[row + 1][column + 1]) count++;
      }

      return count;
    }

    public void UpdateGridFramesToSkipUntilUpdate()
    {
      Console.WriteLine($"Updated grid fps to {GridFps}");
      GridFramesToSkipUntilUpdate = Game._fps / GridFps;
    }

    public void DecreaseGridUpdateSpeed()
    {
      if (GridFps <= GridLowestFps) return;

      if (GridFps - GridFpsChangeFactor <= GridLowestFps)
        GridFps = GridLowestFps;
      else
        GridFps -= GridFpsChangeFactor;

      UpdateGridFramesToSkipUntilUpdate();
    }

    public void IncreaseGridUpdateSpeed()
    {
      if (GridFps >= Game._fps) return;

      if (GridFps == GridLowestFps)
        GridFps = GridFpsChangeFactor;
      else if (Game._fps - GridFps < GridFpsChangeFactor)
        GridFps = Game._fps;
      else
        GridFps += GridFpsChangeFactor;

      UpdateGridFramesToSkipUntilUpdate();
    }
  }
}
