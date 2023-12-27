namespace SDLTest.Models;

public partial class GameOfLife
{
  class EditScene : GridDrawingScene
  {
    public override void OnInit()
    {
      Name = "Edit";
      InitGrid();
    }

    public override void OnRender()
    {
      DrawGrid();
    }

    public override void OnKeyDown()
    {
      switch (Keyboard.Key)
      {
        case SDLK_SPACE:
          Game.SetNextScene<SimulationScene>();
          break;
        case SDLK_g:
          Game._shouldDrawGrid = !Game._shouldDrawGrid;
          break;
      }
    }

    public override void OnMouseDown()
    {
      UpdateCellOnCursor();
    }

    public override void OnMouseMove()
    {
      if (!Mouse.ButtonPressed) return;
      UpdateCellOnCursor();
    }

    public override void OnWindowResize()
    {
      UpdateCellSize();
    }

    public override void OnToggleFullscreen()
    {
      UpdateCellSize();
    }

    public void UpdateCellOnCursor()
    {
      int row    = (int)(Mouse.Y / CellHeight);
      int column = (int)(Mouse.X / CellWidth);

      cells[row][column] = Mouse.Button switch
      {
        SDL_BUTTON_LEFT or SDL_BUTTON_X2  => true,
        SDL_BUTTON_RIGHT or SDL_BUTTON_X1 => false,
        _                                 => cells[row][column]
      };
    }
  }
}
