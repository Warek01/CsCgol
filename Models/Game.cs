namespace CsGame.Models;

public class Game : IDisposable
{
  private uint          InitFlags    = SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_AUDIO;
  private IMG_InitFlags ImgInitFlags = IMG_INIT_JPG | IMG_INIT_PNG | IMG_INIT_TIF | IMG_INIT_WEBP;

  private Scene? _scene;
  private Scene? _nextScene;

  private GameState _state;
  private IntPtr    _renderer;
  private IntPtr    _windowSurface;
  private Font      _mainFont;

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

    _state.Window.Title          = "GAME NAME";
    _state.Window.OriginalWidth  = _state.Window.Width  = width;
    _state.Window.OriginalHeight = _state.Window.Height = height;

    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "best");
    SDL_Init(InitFlags);
    IMG_Init(ImgInitFlags);

    _state.Screen.Index = 0;
    SDL_GetDisplayMode(_state.Screen.Index, 0, out var displayMode);
    _state.Screen.Width       = displayMode.w;
    _state.Screen.Height      = displayMode.h;
    _state.Screen.RefreshRate = displayMode.refresh_rate;

    _state.Runtime.Fps        = _state.Screen.RefreshRate;
    _state.Runtime.FrameIndex = 0;
    _state.Runtime.IsRunning  = true;

    InitWindowAndRenderer();
    SDL_SetRenderDrawBlendMode(_renderer, SDL_BLENDMODE_BLEND);

    _mainFont = new Font("UbuntuMono-R.ttf");

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

      SdlUtils.SetDrawColor(_renderer, Color.Maroon);
      SDL_RenderClear(_renderer);
      _scene?.OnRender();
      SDL_RenderPresent(_renderer);

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
      Game     = this,
      MainFont = _mainFont,
      State    = _state,
      Renderer = _renderer,
    };

    object? instance = Activator.CreateInstance(typeof(TSceneClass), init);

    if (instance is null)
      throw new Exception("Error instantiating scene");

    return (Scene)instance;
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
    _state.Mouse.X               = e.button.x;
    _state.Mouse.Y               = e.button.y;
    _state.Mouse.Button          = e.button.button;
    _state.Mouse.IsButtonPressed = e.type == SDL_MOUSEBUTTONDOWN;
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

  private void InitWindowAndRenderer()
  {
    IntPtr window = SDL_CreateWindow(
      _state.Window.Title,
      SDL_WINDOWPOS_CENTERED,
      SDL_WINDOWPOS_CENTERED,
      _state.Window.Width,
      _state.Window.Height,
      SDL_WINDOW_SHOWN
    );

    _windowSurface = SDL_GetWindowSurface(window);
    _renderer      = SDL_CreateRenderer(window, 0, SDL_RENDERER_ACCELERATED);

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
}
