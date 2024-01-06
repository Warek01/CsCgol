namespace CsGame.Models;

// Current screen (monitor)
public class ScreenState
{
  // Which monitor
  public int  Index;
  public int  RefreshRate;
  public int  Width;
  public int  Height;
  // SDL pixel format
  public uint PixelFormat;
  public int  DisplaysCount;
}
