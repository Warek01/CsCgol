namespace CsGame.Models;

public class Box : Element
{
  public bool  IsFilled = true;
  public Color Color    = new Color();

  public Box(nint renderer) : base(renderer) { }

  public override void Dispose() { }

  public override void Render()
  {
    Color initialColor = SdlUtils.GetDrawColor(Renderer);
    SdlUtils.SetDrawColor(Renderer, Color);

    if (IsFilled)
      SDL_RenderFillRect(Renderer, ref Rect);
    else
      SDL_RenderDrawRect(Renderer, ref Rect);

    SdlUtils.SetDrawColor(Renderer, initialColor);
  }
}
