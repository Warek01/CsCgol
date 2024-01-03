namespace CsGame.Models;

public class WindowState
{
  public IntPtr WindowPtr;
  public string Title;
  public int    Width;
  public int    OriginalWidth;
  public int    Height;
  public int    OriginalHeight;
  public bool   IsFullscreen;
  public IntPtr IconSurface;
}
