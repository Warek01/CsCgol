namespace GameOfLife.Models;

public partial class Game
{
  public class SimulationScene : GridDrawingScene
  {
    private int _gridFps;

    public static int GridFpsChangeFactor = 10;
    public static int GridLowestFps       = 1;

    public bool IsPaused;

    public int GridFps
    {
      get => _gridFps;
      set
      {
        _gridFps                    = value;
        GridFramesToSkipUntilUpdate = Running.Fps / value;
      }
    }

    public int GridFramesToSkipUntilUpdate;

    public override void OnInit()
    {
      Name    = "Simulation";
      GridFps = 10;
    }

    public override void OnRender()
    {
      DrawGrid();

      if (!IsPaused && Running.FrameIndex % GridFramesToSkipUntilUpdate == 0)
      {
        bool updated = UpdateGrid();

        if (!updated)
          Game.SetNextScene<EndScreenScene>();
      }
    }

    public override void OnFocusLost()
    {
      IsPaused = true;
    }

    public override void OnFocusGain()
    {
      IsPaused = false;
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
          Options.ShouldDrawGrid = !Options.ShouldDrawGrid;
          break;
        case SDLK_s:
        {
          if (!Keyboard.ShiftPressed) break;
          Render.Screenshot(0, 0, Window.Width, Window.Height);
          break;
        }
      }
    }

    public override void Dispose()
    {
      DisposeGrid();
    }

    public bool UpdateGrid()
    {
      bool updated = false;

      for (int row = 0; row < Options.GridRows; row++)
      for (int col = 0; col < Options.GridColumns; col++)
      {
        int count = CountNeighbors(row, col);

        newCells[row][col] = (!cells[row][col] && count == 3) || (cells[row][col] && count is 2 or 3);

        if (newCells[row][col] != cells[row][col])
          updated = true;
      }

      for (int row = 0; row < Options.GridRows; row++)
      for (int col = 0; col < Options.GridColumns; col++)
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
        if (column < Options.GridRows - 1 && cells[row - 1][column + 1]) count++;
      }

      // left and right cells
      if (column > 0 && cells[row][column - 1]) count++;
      if (column < Options.GridColumns - 1 && cells[row][column + 1]) count++;

      // lower 3 cells
      if (row < Options.GridRows - 1)
      {
        if (cells[row + 1][column]) count++;
        if (column > 0 && cells[row + 1][column - 1]) count++;
        if (column < Options.GridColumns - 1 && cells[row + 1][column + 1]) count++;
      }

      return count;
    }

    // public void UpdateGridFramesToSkipUntilUpdate()
    // {
    //   Console.WriteLine($"Grid FPS = {GridFps}");
    //   GridFramesToSkipUntilUpdate = Running.Fps / GridFps;
    // }

    public void DecreaseGridUpdateSpeed()
    {
      if (GridFps <= GridLowestFps) return;

      if (GridFps - GridFpsChangeFactor <= GridLowestFps)
        GridFps = GridLowestFps;
      else
        GridFps -= GridFpsChangeFactor;

      // UpdateGridFramesToSkipUntilUpdate();
    }

    public void IncreaseGridUpdateSpeed()
    {
      if (GridFps >= Running.Fps) return;

      if (GridFps == GridLowestFps)
        GridFps = GridFpsChangeFactor;
      else if (Running.Fps - GridFps < GridFpsChangeFactor)
        GridFps = Running.Fps;
      else
        GridFps += GridFpsChangeFactor;

      // UpdateGridFramesToSkipUntilUpdate();
    }
  }
}
