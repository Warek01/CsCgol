namespace CsGame.Models;

public interface IRectangleBounds
{
  public int X      { get; }
  public int Y      { get; }
  public int Width  { get; }
  public int Height { get; }
  public int EndX   { get; }
  public int EndY   { get; }
}
