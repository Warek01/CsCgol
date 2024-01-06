namespace CsGame.Models;

public interface IDrawable : IDisposable
{
  public IntPtr Surface { get; }
  public int    X       { get; }
  public int    Y       { get; }
}
