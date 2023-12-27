namespace SDLTest.Models;

public partial class GameOfLife : IGame
{
  private static string Title = "Game of Life";

  private readonly uint          InitFlags    = SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_AUDIO;
  private readonly IMG_InitFlags ImgInitFlags = IMG_INIT_JPG | IMG_INIT_PNG | IMG_INIT_TIF | IMG_INIT_WEBP;

  private readonly Dictionary<string, nint> _fonts = new();
  private readonly SDL_DisplayMode          _displayMode;

  private SDL_Event _originalEvent;

  private readonly SDL_Color _cellColor  = new() { r = 0x0a, g = 0xbd, b = 0xe3, a = 0xff };
  private readonly SDL_Color _bgColor    = new() { r = 0x57, g = 0x65, b = 0x74, a = 0xff };
  private readonly SDL_Color _gridColor  = new() { r = 0x22, g = 0x2f, b = 0x3e, a = 0xff };
  private readonly SDL_Color _blackColor = new() { r = 0x00, g = 0x00, b = 0x00, a = 0xff };
  private readonly SDL_Color _redColor   = new() { r = 0xff, g = 0x00, b = 0x00, a = 0xff };
  private readonly SDL_Color _greenColor = new() { r = 0xff, g = 0x00, b = 0x00, a = 0xff };
  private readonly SDL_Color _blueColor  = new() { r = 0xff, g = 0x00, b = 0x00, a = 0xff };

  private nint _iconSurface;

  private Scene?     _scene;
  private Scene?     _nextScene;
  private nint       _window;
  private nint       _renderer;
  private int        _frameIndex;
  private KeyData    _keyData    = new();
  private MouseData  _mouseData  = new();
  private WindowData _windowData = new();
  private int        _fps;
  private int        _frameDelay;
  private bool       _isMainLoopRunning = true;
  private int        _gridColumns       = 64;
  private int        _gridRows          = 64;
  private bool       _shouldDrawGrid;

  public GameOfLife(int width, int height)
  {
    _windowData.OriginalWidth  = _windowData.Width  = width;
    _windowData.OriginalHeight = _windowData.Height = height;

    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "best");

    SDL_Init(InitFlags);
    IMG_Init(ImgInitFlags);
    TTF_Init();

    SDL_GetDisplayMode(0, 0, out _displayMode);

    InitWindow();
    InitFonts();

    _renderer = SDL_CreateRenderer(_window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

    UpdateFps(_displayMode.refresh_rate);

    _scene = new MenuScene();
    _scene.Init(this);
  }

  public void Start()
  {
    var timer = new Stopwatch();

    while (_isMainLoopRunning)
    {
      while (SDL_PollEvent(out _originalEvent) != 0)
      {
        HandleEvent(_originalEvent);
        _scene?.HandleEvent(_originalEvent);
      }

      SDLUtils.SetDrawColor(_renderer, _bgColor);
      SDL_RenderClear(_renderer);
      _scene?.OnRender();
      SDL_RenderPresent(_renderer);

      if (_nextScene != null)
      {
        _scene?.Dispose();

        _scene     = _nextScene;
        _nextScene = null;
      }

      if (timer.ElapsedMilliseconds < _frameDelay)
        Thread.Sleep(_frameDelay - (int)timer.ElapsedMilliseconds);

      _frameIndex = _frameIndex == _fps - 1
        ? 0
        : _frameIndex + 1;

      timer.Restart();
    }
  }

  public void Dispose()
  {
    if (_scene != null)
      _scene?.Dispose();

    TTF_CloseFont(_fonts["main-lg"]);
    TTF_CloseFont(_fonts["main-md"]);
    TTF_CloseFont(_fonts["main-sm"]);

    SDL_FreeSurface(_iconSurface);

    SDL_DestroyRenderer(_renderer);
    SDL_DestroyWindow(_window);
    SDL_Quit();
  }

  private void HandleEvent(SDL_Event e)
  {
    switch (e.type)
    {
      case SDL_QUIT:
        _isMainLoopRunning = false;
        break;
      case SDL_MOUSEBUTTONDOWN:
        HandleMouseDown(e);
        break;
      case SDL_MOUSEMOTION:
        HandleMouseMove(e);
        break;
      case SDL_MOUSEBUTTONUP:
        HandleMouseUp(e);
        break;
      case SDL_KEYDOWN:
        HandleKeyDown(e);
        break;
      case SDL_KEYUP:
        HandleKeyUp(e);
        break;
      case SDL_WINDOWEVENT:
        switch (e.window.windowEvent)
        {
          case SDL_WINDOWEVENT_RESIZED:
            _windowData.Width  = e.window.data1;
            _windowData.Height = e.window.data2;
            break;
          case SDL_WINDOWEVENT_MAXIMIZED:
          case SDL_WINDOWEVENT_RESTORED:
            SDL_GetWindowSize(_window, out _windowData.Width, out _windowData.Height);
            break;
        }

        break;
    }
  }

  private void SetNextScene<TSceneClass>() where TSceneClass : Scene, new()
  {
    _nextScene = new TSceneClass();
    _nextScene.Init(this);
    Console.WriteLine($"Next scene is {_nextScene.Name}");
  }

  private void UpdateFps(int fps)
  {
    _fps        = fps;
    _frameDelay = 1000 / fps;
  }

  private void InitFonts()
  {
    _fonts["main-lg"] = TTF_OpenFont("Assets/Fonts/UbuntuMono-R.ttf", 36);
    _fonts["main-md"] = TTF_OpenFont("Assets/Fonts/UbuntuMono-R.ttf", 24);
    _fonts["main-sm"] = TTF_OpenFont("Assets/Fonts/UbuntuMono-R.ttf", 16);
  }

  private void HandleKeyDown(SDL_Event e)
  {
    switch (e.key.keysym.sym)
    {
      case SDLK_LALT:
      case SDLK_RALT:
        _keyData.AltPressed = true;
        break;
      case SDLK_LCTRL:
      case SDLK_RCTRL:
        _keyData.CtrlPressed = true;
        break;
      case SDLK_LGUI:
      case SDLK_RGUI:
        _keyData.CmdPressed = true;
        break;
      case SDLK_LSHIFT:
      case SDLK_RSHIFT:
        _keyData.ShiftPressed = true;
        break;
      case SDLK_q:
        _isMainLoopRunning = false;
        break;
      case SDLK_f:
        ToggleFullscreen();
        break;
      default:
        _keyData.KeyPressed = true;
        _keyData.Key        = e.key.keysym.sym;
        break;
    }
  }

  private void HandleKeyUp(SDL_Event e)
  {
    switch (e.key.keysym.sym)
    {
      case SDLK_LALT:
      case SDLK_RALT:
        _keyData.AltPressed = false;
        break;
      case SDLK_LCTRL:
      case SDLK_RCTRL:
        _keyData.CtrlPressed = false;
        break;
      case SDLK_LGUI:
      case SDLK_RGUI:
        _keyData.CmdPressed = false;
        break;
      case SDLK_LSHIFT:
      case SDLK_RSHIFT:
        _keyData.ShiftPressed = false;
        break;
      default:
        _keyData.KeyPressed = false;
        _keyData.Key        = e.key.keysym.sym;
        break;
    }
  }

  private void HandleMouseDown(SDL_Event e)
  {
    _mouseData.X             = e.button.x;
    _mouseData.Y             = e.button.y;
    _mouseData.Button        = e.button.button;
    _mouseData.ButtonPressed = true;
  }

  private void HandleMouseUp(SDL_Event e)
  {
    _mouseData.X             = e.button.x;
    _mouseData.Y             = e.button.y;
    _mouseData.Button        = e.button.button;
    _mouseData.ButtonPressed = false;
  }

  private void HandleMouseMove(SDL_Event e)
  {
    _mouseData.X = e.motion.x;
    _mouseData.Y = e.motion.y;
  }

  private void InitWindow()
  {
    _window = SDL_CreateWindow(
      Title,
      SDL_WINDOWPOS_CENTERED,
      SDL_WINDOWPOS_CENTERED,
      _windowData.Width,
      _windowData.Height,
      SDL_WINDOW_SHOWN
    );

    SDL_SetWindowMinimumSize(_window, 400, 400);
    SDL_SetWindowMaximumSize(_window, _displayMode.w, _displayMode.h);
    SDL_SetWindowBordered(_window, SDL_TRUE);
    SDL_SetWindowAlwaysOnTop(_window, SDL_FALSE);
    SDL_SetWindowResizable(_window, SDL_TRUE);

    _iconSurface = IMG_Load("Assets/Images/GameIcon.png");
    SDL_SetWindowIcon(_window, _iconSurface);
  }

  private void ToggleFullscreen()
  {
    if (_windowData.IsFullscreen)
    {
      SDL_SetWindowFullscreen(_window, 0);
      SDL_SetWindowSize(_window, _windowData.OriginalWidth, _windowData.OriginalHeight);
      SDL_SetWindowPosition(_window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
      SDL_SetWindowBordered(_window, SDL_TRUE);

      _windowData.Width  = _windowData.OriginalWidth;
      _windowData.Height = _windowData.OriginalHeight;
    }
    else
    {
      SDL_SetWindowFullscreen(_window, (uint)SDL_WINDOW_FULLSCREEN_DESKTOP);
      SDL_SetWindowBordered(_window, SDL_FALSE);

      _windowData.Width  = _displayMode.w;
      _windowData.Height = _displayMode.h;
    }

    _windowData.IsFullscreen = !_windowData.IsFullscreen;

    _scene?.OnToggleFullscreen();
  }
}
