namespace GameOfLife.Models;

public partial class Game : IGame
{
  protected readonly SDL_Color CellColor  = new() { r = 0x0a, g = 0xbd, b = 0xe3, a = 0xff };
  protected readonly SDL_Color BgColor    = new() { r = 0x57, g = 0x65, b = 0x74, a = 0xff };
  protected readonly SDL_Color GridColor  = new() { r = 0x22, g = 0x2f, b = 0x3e, a = 0xff };
  protected readonly SDL_Color BlackColor = new() { r = 0x00, g = 0x00, b = 0x00, a = 0xff };
  protected readonly SDL_Color RedColor   = new() { r = 0xff, g = 0x00, b = 0x00, a = 0xff };
  protected readonly SDL_Color GreenColor = new() { r = 0xff, g = 0x00, b = 0x00, a = 0xff };
  protected readonly SDL_Color BlueColor  = new() { r = 0xff, g = 0x00, b = 0x00, a = 0xff };

  protected Dictionary<string, IntPtr> Fonts = new();

  protected SDL_Event   OriginalEvent;
  protected string      Title;
  protected GameOptions Options  = new();
  protected KeyData     Keyboard = new();
  protected MouseData   Mouse    = new();
  protected WindowData  Window   = new();
  protected ScreenData  Screen   = new();
  protected RunningData Running  = new();
  protected Renderer    Render;

  private IntPtr        _window;
  private uint          InitFlags    = SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_AUDIO;
  private IMG_InitFlags ImgInitFlags = IMG_INIT_JPG | IMG_INIT_PNG | IMG_INIT_TIF | IMG_INIT_WEBP;
  private IntPtr        _iconSurface;
  private Scene?        _scene;
  private Scene?        _nextScene;

  public Game(int width, int height)
  {
    Title = "Game of Life";

    Window.OriginalWidth  = Window.Width  = width;
    Window.OriginalHeight = Window.Height = height;

    Options.ShouldDrawGrid = true;
    Options.GridColumns    = 64;
    Options.GridRows       = 64;

    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "best");

    SDL_Init(InitFlags);
    IMG_Init(ImgInitFlags);
    TTF_Init();

    SDL_GetDisplayMode(0, 0, out var displayMode);
    Screen.Width  = displayMode.w;
    Screen.Height = displayMode.h;
    Screen.Fps    = displayMode.refresh_rate;

    Running.Fps        = Screen.Fps;
    Running.FrameIndex = 0;
    Running.IsRunning  = true;

    InitWindow();
    Render             = new Renderer(_window);
    UIElement.Renderer = Render.SDLRrenderer;
    InitFonts();

    _scene = new MenuScene();
    _scene.Init(this);
  }

  public void Start()
  {
    var timer = new Stopwatch();

    while (Running.IsRunning)
    {
      while (SDL_PollEvent(out OriginalEvent) != 0)
      {
        HandleEvent(OriginalEvent);
        _scene?.HandleEvent(OriginalEvent);
      }

      Render.SetDrawColor(BgColor);
      Render.Clear();
      _scene?.OnRender();
      Render.Present();

      if (_nextScene != null)
      {
        _scene?.Dispose();

        _scene     = _nextScene;
        _nextScene = null;
      }

      if (timer.ElapsedMilliseconds < Running.FrameDelay)
        Thread.Sleep(Running.FrameDelay - (int)timer.ElapsedMilliseconds);

      Running.FrameIndex++;

      timer.Restart();
    }
  }

  public void Dispose()
  {
    _scene?.Dispose();
    _nextScene?.Dispose();

    foreach (var pair in Fonts)
      Render.DestroyFont(pair.Value);

    Render.Dispose();

    SDL_FreeSurface(_iconSurface);

    SDL_DestroyWindow(_window);
    SDL_Quit();
  }

  private void HandleEvent(SDL_Event e)
  {
    switch (e.type)
    {
      case SDL_QUIT:
        Running.IsRunning = false;
        break;
      case SDL_MOUSEBUTTONDOWN:
      case SDL_MOUSEBUTTONUP:
        HandleMouseEvent(e);
        break;
      case SDL_MOUSEMOTION:
        HandleMouseMove(e);
        break;
      case SDL_KEYDOWN:
        HandleKeyDown(e);
        break;
      case SDL_KEYUP:
        HandleKeyUp(e);
        break;
      case SDL_WINDOWEVENT:
        HandleWindowEvent(e);
        break;
    }
  }

  private void SetNextScene<TSceneClass>() where TSceneClass : Scene, new()
  {
    _nextScene = new TSceneClass();
    _nextScene.Init(this);
  }

  private void InitFonts()
  {
    Fonts["main-lg"] = Render.LoadFont("Assets/Fonts/UbuntuMono-R.ttf", 36);
    Fonts["main-md"] = Render.LoadFont("Assets/Fonts/UbuntuMono-R.ttf", 24);
    Fonts["main-sm"] = Render.LoadFont("Assets/Fonts/UbuntuMono-R.ttf", 16);
  }

  private void HandleWindowEvent(SDL_Event e)
  {
    switch (e.window.windowEvent)
    {
      case SDL_WINDOWEVENT_RESIZED:
        Window.Width  = e.window.data1;
        Window.Height = e.window.data2;
        break;
      case SDL_WINDOWEVENT_MAXIMIZED:
      case SDL_WINDOWEVENT_RESTORED:
        SDL_GetWindowSize(_window, out Window.Width, out Window.Height);
        break;
    }
  }

  private void HandleMouseEvent(SDL_Event e)
  {
    Mouse.X             = e.button.x;
    Mouse.Y             = e.button.y;
    Mouse.Button        = e.button.button;
    Mouse.ButtonPressed = e.type == SDL_MOUSEBUTTONDOWN;
  }

  private void HandleKeyDown(SDL_Event e)
  {
    HandleKeyEvent(e);

    switch (e.key.keysym.sym)
    {
      case SDLK_q:
        Running.IsRunning = false;
        break;
      case SDLK_f:
        ToggleFullscreen();
        break;
      case SDLK_r:
        SetNextScene<MenuScene>();
        break;
    }
  }

  private void HandleKeyUp(SDL_Event e)
  {
    HandleKeyEvent(e);
  }

  private void HandleKeyEvent(SDL_Event e)
  {
    SDL_Keymod state = SDL_GetModState();
    Keyboard.KeyPressed   = true;
    Keyboard.Key          = e.key.keysym.sym;
    Keyboard.AltPressed   = (state & KMOD_ALT) != 0;
    Keyboard.CtrlPressed  = (state & KMOD_CTRL) != 0;
    Keyboard.CapsPressed  = (state & KMOD_CAPS) != 0;
    Keyboard.ShiftPressed = (state & KMOD_SHIFT) != 0;
    Keyboard.GuiPressed   = (state & KMOD_GUI) != 0;
  }

  private void HandleMouseMove(SDL_Event e)
  {
    Mouse.X = e.motion.x;
    Mouse.Y = e.motion.y;
  }

  private void InitWindow()
  {
    _window = SDL_CreateWindow(
      Title,
      SDL_WINDOWPOS_CENTERED,
      SDL_WINDOWPOS_CENTERED,
      Window.Width,
      Window.Height,
      SDL_WINDOW_SHOWN
    );

    SDL_SetWindowMinimumSize(_window, 400, 400);
    SDL_SetWindowMaximumSize(_window, Screen.Width, Screen.Height);
    SDL_SetWindowBordered(_window, SDL_TRUE);
    SDL_SetWindowAlwaysOnTop(_window, SDL_FALSE);
    SDL_SetWindowResizable(_window, SDL_TRUE);

    _iconSurface = IMG_Load("Assets/Images/GameIcon.png");
    SDL_SetWindowIcon(_window, _iconSurface);
  }

  private void ToggleFullscreen()
  {
    if (Window.IsFullscreen)
    {
      SDL_SetWindowFullscreen(_window, 0);
      SDL_SetWindowSize(_window, Window.OriginalWidth, Window.OriginalHeight);
      SDL_SetWindowPosition(_window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
      SDL_SetWindowBordered(_window, SDL_TRUE);

      Window.Width  = Window.OriginalWidth;
      Window.Height = Window.OriginalHeight;
    }
    else
    {
      SDL_SetWindowFullscreen(_window, (uint)SDL_WINDOW_FULLSCREEN_DESKTOP);
      SDL_SetWindowBordered(_window, SDL_FALSE);

      Window.Width  = Screen.Width;
      Window.Height = Screen.Height;
    }

    Window.IsFullscreen = !Window.IsFullscreen;

    _scene?.OnToggleFullscreen();
  }
}
