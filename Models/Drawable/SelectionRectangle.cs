namespace CsGame.Models;

public class SelectionRectangle : DrawableEntity
{
  public SelectionRectangle()
  {
    X       = 0;
    Y       = 0;
    Width   = 0;
    Height  = 0;

    Color = Color.FromArgb(0x50, Color.Lime);
  }
}
