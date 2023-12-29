namespace GameOfLife.Models;

public class Game : IGame
{
  private readonly Dictionary<string, IntPtr>    _fonts  = new();
  private readonly Dictionary<string, SDL_Color> _colors = new();

  private uint          InitFlags    = SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_AUDIO;
  private IMG_InitFlags ImgInitFlags = IMG_INIT_JPG | IMG_INIT_PNG | IMG_INIT_TIF | IMG_INIT_WEBP;

  private Scene? _scene;
  private Scene? _nextScene;

  private readonly GameState _state;
  private readonly Renderer  _renderer;

  public Game(int width, int height)
  {
    _state = new GameState
    {
      Keyboard = new KeyState(),
      Mouse    = new MouseState(),
      Screen   = new ScreenState(),
      Window   = new WindowState(),
      Options  = new OptionsState(),
      Runtime  = new RuntimeState(),
    };

    _state.Window.Title          = "Game of Life";
    _state.Window.OriginalWidth  = _state.Window.Width  = width;
    _state.Window.OriginalHeight = _state.Window.Height = height;

    _state.Options.ShouldDrawGrid = true;
    _state.Options.GridColumns    = 64;
    _state.Options.GridRows       = 64;

    InitColors();

    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "best");

    SDL_Init(InitFlags);
    IMG_Init(ImgInitFlags);
    TTF_Init();

    SDL_GetDisplayMode(0, 0, out var displayMode);
    _state.Screen.Width       = displayMode.w;
    _state.Screen.Height      = displayMode.h;
    _state.Screen.RefreshRate = displayMode.refresh_rate;

    _state.Runtime.Fps        = _state.Screen.RefreshRate;
    _state.Runtime.FrameIndex = 0;
    _state.Runtime.IsRunning  = true;

    InitWindow();
    _renderer          = new Renderer(_state.Window.WindowPtr);
    UIElement.Renderer = _renderer.SDLRrenderer;
    InitFonts();

    SetNextScene<MenuScene>();
  }

  public void Start()
  {
    var timer = new Stopwatch();

    while (_state.Runtime.IsRunning)
    {
      if (_nextScene != null)
      {
        _scene?.Dispose();

        _scene     = _nextScene;
        _nextScene = null;
      }

      while (SDL_PollEvent(out var e) != 0)
      {
        HandleEvent(e);
        _scene?.HandleEvent(e);
      }

      _renderer.SetDrawColor(_colors["Background"]);
      _renderer.Clear();
      _scene?.OnRender();
      _renderer.Present();


      if (timer.ElapsedMilliseconds < _state.Runtime.FrameDelay)
        Thread.Sleep(_state.Runtime.FrameDelay - (int)timer.ElapsedMilliseconds);

      _state.Runtime.FrameIndex++;

      timer.Restart();
    }
  }

  public void Dispose()
  {
    _scene?.Dispose();
    _nextScene?.Dispose();

    foreach (var pair in _fonts)
      _renderer.DestroyFont(pair.Value);

    _renderer.Dispose();

    SDL_FreeSurface(_state.Window.IconSurface);

    SDL_DestroyWindow(_state.Window.WindowPtr);
    SDL_Quit();
  }

  private void HandleEvent(SDL_Event e)
  {
    switch (e.type)
    {
      case SDL_QUIT:
        _state.Runtime.IsRunning = false;
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

  public void SetNextScene<TSceneClass>() where TSceneClass : Scene
  {
    _nextScene = InstantiateScene<TSceneClass>();
  }

  public void SetScene<TSceneClass>() where TSceneClass : Scene
  {
    _scene = InstantiateScene<TSceneClass>();
  }

  private Scene InstantiateScene<TSceneClass>() where TSceneClass : Scene
  {
    var init = new SceneInitObject
    {
      Colors = _colors,
      Fonts  = _fonts,
      Game   = this,
      State  = _state,
      Renderer = _renderer,
    };

    object? instance = Activator.CreateInstance(typeof(TSceneClass), init);

    if (instance is null)
      throw new Exception("Error instantiating scene");

    return (Scene)instance;
  }

  private void InitFonts()
  {
    _fonts["main-lg"] = _renderer.LoadFont("Assets/Fonts/UbuntuMono-R.ttf", 36);
    _fonts["main-md"] = _renderer.LoadFont("Assets/Fonts/UbuntuMono-R.ttf", 24);
    _fonts["main-sm"] = _renderer.LoadFont("Assets/Fonts/UbuntuMono-R.ttf", 16);
  }

  private void HandleWindowEvent(SDL_Event e)
  {
    switch (e.window.windowEvent)
    {
      case SDL_WINDOWEVENT_RESIZED:
        _state.Window.Width  = e.window.data1;
        _state.Window.Height = e.window.data2;
        break;
      case SDL_WINDOWEVENT_MAXIMIZED:
      case SDL_WINDOWEVENT_RESTORED:
        SDL_GetWindowSize(_state.Window.WindowPtr, out _state.Window.Width, out _state.Window.Height);
        break;
    }
  }

  private void HandleMouseEvent(SDL_Event e)
  {
    _state.Mouse.X             = e.button.x;
    _state.Mouse.Y             = e.button.y;
    _state.Mouse.Button        = e.button.button;
    _state.Mouse.ButtonPressed = e.type == SDL_MOUSEBUTTONDOWN;
  }

  private void HandleKeyDown(SDL_Event e)
  {
    HandleKeyEvent(e);

    switch (e.key.keysym.sym)
    {
      case SDLK_q:
        _state.Runtime.IsRunning = false;
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
    _state.Keyboard.KeyPressed   = true;
    _state.Keyboard.Key          = e.key.keysym.sym;
    _state.Keyboard.AltPressed   = (state & KMOD_ALT) != 0;
    _state.Keyboard.CtrlPressed  = (state & KMOD_CTRL) != 0;
    _state.Keyboard.CapsPressed  = (state & KMOD_CAPS) != 0;
    _state.Keyboard.ShiftPressed = (state & KMOD_SHIFT) != 0;
    _state.Keyboard.GuiPressed   = (state & KMOD_GUI) != 0;
  }

  private void HandleMouseMove(SDL_Event e)
  {
    _state.Mouse.X = e.motion.x;
    _state.Mouse.Y = e.motion.y;
  }

  private void InitWindow()
  {
    IntPtr window = SDL_CreateWindow(
      _state.Window.Title,
      SDL_WINDOWPOS_CENTERED,
      SDL_WINDOWPOS_CENTERED,
      _state.Window.Width,
      _state.Window.Height,
      SDL_WINDOW_SHOWN
    );

    SDL_SetWindowMinimumSize(window, 400, 400);
    SDL_SetWindowMaximumSize(window, _state.Screen.Width, _state.Screen.Height);
    SDL_SetWindowBordered(window, SDL_TRUE);
    SDL_SetWindowAlwaysOnTop(window, SDL_FALSE);
    SDL_SetWindowResizable(window, SDL_TRUE);

    IntPtr iconSurface = IMG_Load("Assets/Images/GameIcon.png");
    SDL_SetWindowIcon(window, iconSurface);

    _state.Window.WindowPtr   = window;
    _state.Window.IconSurface = iconSurface;
  }

  private void ToggleFullscreen()
  {
    if (_state.Window.IsFullscreen)
    {
      SDL_SetWindowFullscreen(_state.Window.WindowPtr, 0);
      SDL_SetWindowSize(_state.Window.WindowPtr, _state.Window.OriginalWidth, _state.Window.OriginalHeight);
      SDL_SetWindowPosition(_state.Window.WindowPtr, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
      SDL_SetWindowBordered(_state.Window.WindowPtr, SDL_TRUE);

      _state.Window.Width  = _state.Window.OriginalWidth;
      _state.Window.Height = _state.Window.OriginalHeight;
    }
    else
    {
      SDL_SetWindowFullscreen(_state.Window.WindowPtr, (uint)SDL_WINDOW_FULLSCREEN_DESKTOP);
      SDL_SetWindowBordered(_state.Window.WindowPtr, SDL_FALSE);

      _state.Window.Width  = _state.Screen.Width;
      _state.Window.Height = _state.Screen.Height;
    }

    _state.Window.IsFullscreen = !_state.Window.IsFullscreen;

    _scene?.OnToggleFullscreen();
  }

  private void InitColors()
  {
    _colors["Grid"]       = new() { r = 0x22, g = 0x2f, b = 0x3e, a = 0xff };
    _colors["Cell"]       = new() { r = 0x0a, g = 0xbd, b = 0xe3, a = 0xff };
    _colors["Background"] = new() { r = 0x57, g = 0x65, b = 0x74, a = 0xff };
    _colors["Black"]      = new() { r = 0x00, g = 0x00, b = 0x00, a = 0xff };
    _colors["Red"]        = new() { r = 0xff, g = 0x00, b = 0x00, a = 0xff };
    _colors["Green"]      = new() { r = 0xff, g = 0x00, b = 0x00, a = 0xff };
    _colors["Blue"]       = new() { r = 0xff, g = 0x00, b = 0x00, a = 0xff };
  }
}
