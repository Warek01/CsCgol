namespace GameOfLife.Models;

class OptionsScene : Scene
{
  public Button   BackButton;
  public Checkbox DrawGridCheckbox;
  public Text     InfoText;

  public OptionsScene(SceneInitObject init) : base(init)
  {
    BackButton = new Button(Renderer, Fonts["Main-md"], "Go back")
    {
      Width           = 120,
      Height          = 50,
      X               = 30,
      Y               = 30,
      BackgroundColor = Colors["Button"]
    };

    BackButton.AddEventListener(Element.Event.MOUSE_DOWN, () => { Game.SetNextScene<MenuScene>(); });

    DrawGridCheckbox = new Checkbox(Renderer, Fonts["Main-md"], "Draw grid")
    {
      X         = 30,
      Y         = 110,
      IsChecked = Options.ShouldDrawGrid,
    };

    DrawGridCheckbox.AddEventListener(
      Element.Event.CHECKBOX_CHANGE,
      () => Options.ShouldDrawGrid = DrawGridCheckbox.IsChecked
    );

    InfoText = new Text(
      Renderer,
      Fonts["Main-md"],
      "Space to pause/unpause\nCtrl + LR arrows to modify speed\nShift + s to take screenshot\nQ to quit\nR to reset\nG to toggle grid",
      (uint)Window.Width - 60
    )
    {
      X         = 30,
      Y         = DrawGridCheckbox.Y + DrawGridCheckbox.Height + 30,
    };

    ElementManager
      .AddElement(DrawGridCheckbox)
      .AddElement(BackButton)
      .AddElement(InfoText);
  }

  public override void OnWindowResize()
  {
    InfoText.WrapWidth = (uint)Window.Width - 60;
  }
}
