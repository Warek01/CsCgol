namespace GameOfLife.Models;

class MenuScene : Scene
{
  public Button PlayButton;
  public Text   CGOLText;

  public MenuScene(SceneInitObject init) : base(init)
  {
    CGOLText   = new Text(Renderer, Fonts["Main-lg"], "Game of life");
    CGOLText.X = (Window.Width - CGOLText.Width) / 2;
    CGOLText.Y = (Window.Height - CGOLText.Height) / 2;

    PlayButton = new(Renderer);
    PlayButton.LoadText(Fonts["Main-md"], "Start");
    PlayButton.SetTextSize();
    PlayButton.BackgroundColor.a =  0xAA;
    PlayButton.Width             += 50;
    PlayButton.AddEventListener(Element.Event.MOUSE_DOWN, () => { Game.SetNextScene<EditScene>(); });
    PlayButton.AddEventListener(Element.Event.MOUSE_ENTER, () => { PlayButton.BackgroundColor.a = 0xFF; });
    PlayButton.AddEventListener(Element.Event.MOUSE_LEAVE, () => { PlayButton.BackgroundColor.a = 0xAA; });
    PlayButton.Y = CGOLText.Y + CGOLText.Height + 30;
    PlayButton.X = (Window.Width - PlayButton.Width) / 2;

    ElementManager
      .AddElement(PlayButton)
      .AddElement(CGOLText);
  }

  public override void OnKeyDown()
  {
    base.OnKeyDown();

    if (Keyboard.Key == SDLK_SPACE)
      Game.SetNextScene<EditScene>();
  }
}
