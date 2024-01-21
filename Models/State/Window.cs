namespace CsGame.Models;

public class Window
{
  public nint   WindowPtr;
  public string Title = null!;
  public int    Width;
  public int    Height;
  public int    OriginalWidth;
  public int    OriginalHeight;
  public bool   IsFullscreen;
  public nint   IconSurface;
  public bool   IsFocused;
}
