using System.Text.Json.Serialization;

namespace CsGame.Models;

public class Config
{
  public class WindowConfig
  {
    public bool FullscreenOnly    { get; set; }
    public bool InitialFullscreen { get; set; }
    public int  InitialWidth      { get; set; }
    public int  InitialHeight     { get; set; }
    public int  MaxWidth          { get; set; }
    public int  MaxHeight         { get; set; }
    public int  MinWidth          { get; set; }
    public int  MinHeight         { get; set; }
  }

  public class RuntimeConfig
  {
    public int TargetFps { get; set; }
  }

  public class GameConfig
  {
    public int ScrollSpeed     { get; set; }
    public int FastScrollSpeed { get; set; }
  }

  public int           ScreenIndex { get; set; }
  public WindowConfig  Window      { get; set; } = null!;
  public RuntimeConfig Runtime     { get; set; } = null!;
  public GameConfig    Game        { get; set; } = null!;
}
