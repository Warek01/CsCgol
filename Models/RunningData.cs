namespace GameOfLife.Models;

public class RunningData
{
  private int _fps;
  private int _frameIndex;

  public int Fps
  {
    get => _fps;
    set
    {
      _fps       = value;
      FrameDelay = 1000 / value;
    }
  }

  public int FrameIndex
  {
    get => _frameIndex;
    set => _frameIndex = value >= Fps ? 0 : value;
  }

  public int  FrameDelay;
  public bool IsRunning;
}
