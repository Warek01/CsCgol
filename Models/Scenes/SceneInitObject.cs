namespace GameOfLife.Models;

public class SceneInitObject
{
  public IGame                      Game;
  public GameState                  State;
  public Dictionary<string, IntPtr> Fonts;
  public Dictionary<string, Color>  Colors;
  public Renderer                   Renderer;
}
