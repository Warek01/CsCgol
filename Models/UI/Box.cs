namespace CsGame.Models;

public class Box : Element
{
  public bool  IsFilled = true;
  public Color Color    = new Color();

  public Box(Renderer renderer) : base(renderer) { }

  public override void Dispose() { }

  public override void Render()
  {
    Color initialColor = Renderer.GetDrawColor();
    Renderer.SetDrawColor(Color);

    if (IsFilled)
      Renderer.FillRect(Rect);
    else
      Renderer.DrawRect(Rect);

    Renderer.SetDrawColor(initialColor);
  }
}
