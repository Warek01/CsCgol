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
    PlayButton.AddEventListener(UiEvent.MOUSE_DOWN, () => Game.SetNextScene<EditScene>());
    PlayButton.AddEventListener(UiEvent.MOUSE_ENTER, () => PlayButton.BackgroundColor += 0xF);
    PlayButton.AddEventListener(UiEvent.MOUSE_LEAVE, () => PlayButton.BackgroundColor -= 0xF);

    OptionsButton = new Button(Renderer, Fonts["Main-md"], "Options")
    {
      BackgroundColor = Colors["Button"],
      Width           = PlayButton.Width,
      Height          = PlayButton.Height,
    };
    OptionsButton.AddEventListener(UiEvent.MOUSE_DOWN, () => Game.SetNextScene<OptionsScene>());
    OptionsButton.AddEventListener(UiEvent.MOUSE_ENTER, () => OptionsButton.BackgroundColor += 0xF);
    OptionsButton.AddEventListener(UiEvent.MOUSE_LEAVE, () => OptionsButton.BackgroundColor -= 0xF);


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
