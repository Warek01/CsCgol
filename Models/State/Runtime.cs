namespace CsGame.Models;

public class Runtime
{
  private int _fps;
  private int _frameIndex;

  public int Fps
  {
    get => _fps;
    set
    {
      _fps       = value;
      FrameDelay = (float)1000 / value;
    }
  }

  public int FrameIndex
  {
    get => _frameIndex;
    set => _frameIndex = value >= Fps ? 0 : value;
  }

  public float     FrameDelay;
  public bool      IsRunning;
  public Stopwatch FrameTimer;
}
