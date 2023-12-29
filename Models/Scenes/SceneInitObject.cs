namespace GameOfLife.Models;

public class SceneInitObject
{
  public IGame                         Game;
  public GameState                     State;
  public Dictionary<string, IntPtr>    Fonts;
  public Dictionary<string, SDL_Color> Colors;
  public Renderer                      Renderer;
}
