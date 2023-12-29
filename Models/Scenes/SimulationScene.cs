namespace GameOfLife.Models;

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
      GridFramesToSkipUntilUpdate = Runtime.Fps / value;
    }
  }

  public int GridFramesToSkipUntilUpdate;

  public SimulationScene(SceneInitObject init) : base(init)
  {
    GridFps = 10;
  }

  public override void OnRender()
  {
    DrawGrid();

    if (!IsPaused && Runtime.FrameIndex % GridFramesToSkipUntilUpdate == 0)
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
        OptionsState.ShouldDrawGrid = !OptionsState.ShouldDrawGrid;
        break;
      case SDLK_s:
      {
        if (!Keyboard.ShiftPressed) break;
        Renderer.Screenshot(0, 0, Window.Width, Window.Height);
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

    for (int row = 0; row < OptionsState.GridRows; row++)
    for (int col = 0; col < OptionsState.GridColumns; col++)
    {
      int count = CountNeighbors(row, col);

      NewCells[row][col] = (!Cells[row][col] && count == 3) || (Cells[row][col] && count is 2 or 3);

      if (NewCells[row][col] != Cells[row][col])
        updated = true;
    }

    for (int row = 0; row < OptionsState.GridRows; row++)
    for (int col = 0; col < OptionsState.GridColumns; col++)
    {
      Cells[row][col]    = NewCells[row][col];
      NewCells[row][col] = false;
    }

    return updated;
  }

  public int CountNeighbors(int row, int column)
  {
    int count = 0;

    // upper 3 cells
    if (row > 0)
    {
      if (Cells[row - 1][column]) count++;
      if (column > 0 && Cells[row - 1][column - 1]) count++;
      if (column < OptionsState.GridRows - 1 && Cells[row - 1][column + 1]) count++;
    }

    // left and right cells
    if (column > 0 && Cells[row][column - 1]) count++;
    if (column < OptionsState.GridColumns - 1 && Cells[row][column + 1]) count++;

    // lower 3 cells
    if (row < OptionsState.GridRows - 1)
    {
      if (Cells[row + 1][column]) count++;
      if (column > 0 && Cells[row + 1][column - 1]) count++;
      if (column < OptionsState.GridColumns - 1 && Cells[row + 1][column + 1]) count++;
    }

    return count;
  }

  public void DecreaseGridUpdateSpeed()
  {
    if (GridFps <= GridLowestFps) return;

    if (GridFps - GridFpsChangeFactor <= GridLowestFps)
      GridFps = GridLowestFps;
    else
      GridFps -= GridFpsChangeFactor;
  }

  public void IncreaseGridUpdateSpeed()
  {
    if (GridFps >= Runtime.Fps) return;

    if (GridFps == GridLowestFps)
      GridFps = GridFpsChangeFactor;
    else if (Runtime.Fps - GridFps < GridFpsChangeFactor)
      GridFps = Runtime.Fps;
    else
      GridFps += GridFpsChangeFactor;
  }
}
