namespace GameOfLife.Models;

class MenuScene : Scene
{
  public Button PlayButton;
  public Button OptionsButton;
  public Text   CGOLText;

  public MenuScene(SceneInitObject init) : base(init)
  {
    CGOLText = new Text(Renderer, Fonts["Main-lg"], "Game of life");

    PlayButton = new Button(Renderer, Fonts["Main-md"], "Start")
    {
      BackgroundColor = Colors["Button"],
      Width           = 120,
      Height          = 50,
    };
    PlayButton.AddEventListener(Element.Event.MOUSE_DOWN, () => { Game.SetNextScene<EditScene>(); });
    PlayButton.AddEventListener(Element.Event.MOUSE_ENTER, () => { PlayButton.BackgroundColor.Lighten(0xF); });
    PlayButton.AddEventListener(Element.Event.MOUSE_LEAVE, () => { PlayButton.BackgroundColor.Darken(0xF); });

    OptionsButton = new Button(Renderer, Fonts["Main-md"], "Options")
    {
      BackgroundColor = Colors["Button"],
      Width           = PlayButton.Width,
      Height          = PlayButton.Height,
    };
    OptionsButton.AddEventListener(Element.Event.MOUSE_DOWN, () => { Game.SetNextScene<OptionsScene>(); });
    OptionsButton.AddEventListener(Element.Event.MOUSE_ENTER, () => { OptionsButton.BackgroundColor.Lighten(0xF); });
    OptionsButton.AddEventListener(Element.Event.MOUSE_LEAVE, () => { OptionsButton.BackgroundColor.Darken(0xF); });


    ElementManager
      .AddElement(PlayButton)
      .AddElement(OptionsButton)
      .AddElement(CGOLText);

    ComputeElementsPositions();
  }

  public override void OnKeyDown()
  {
    base.OnKeyDown();

    if (Keyboard.Key == SDLK_SPACE)
      Game.SetNextScene<EditScene>();
  }

  public override void OnWindowResize()
  {
    ComputeElementsPositions();
  }

  public void ComputeElementsPositions()
  {
    CGOLText.X = (Window.Width - CGOLText.Width) / 2;
    CGOLText.Y = (Window.Height - CGOLText.Height) / 2 - 80;

    PlayButton.Y = CGOLText.Y + CGOLText.Height + 30;
    PlayButton.X = (Window.Width - PlayButton.Width) / 2 - 75;

    OptionsButton.X = (Window.Width - OptionsButton.Width) / 2 + 75;
    OptionsButton.Y = PlayButton.Y;
  }
}
