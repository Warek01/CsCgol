namespace CsGame.Models;

public class Game : IDisposable
{
  public bool   FullscreenOnly = false;
  public int    Width          = 600;
  public int    Height         = 800;
  public int    ScreenIndex    = 0;
  public string Title          = string.Empty;
  public int    MinWidth       = 400;
  public int    MinHeight      = 400;

  private const SDL_WindowFlags WINDOW_FLAGS = SDL_WINDOW_SHOWN;

  private const SDL_WindowFlags FULLSCREEN_WINDOW_FLAGS =
    SDL_WINDOW_SHOWN | SDL_WINDOW_FULLSCREEN_DESKTOP | SDL_WINDOW_INPUT_FOCUS | SDL_WINDOW_INPUT_FOCUS;

  private const uint              INIT_FLAGS          = SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_EVENTS;
  private const IMG_InitFlags     IMG_INIT_FLAGS      = IMG_INIT_JPG | IMG_INIT_PNG | IMG_INIT_TIF | IMG_INIT_WEBP;
  private const SDL_RendererFlags RENDERER_FLAGS      = SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC;
  private const SDL_BlendMode     RENDERER_BLEND_MODE = SDL_BLENDMODE_BLEND;

  private readonly KeyState     _keyboard = new KeyState();
  private readonly MouseState   _mouse    = new MouseState();
  private readonly ScreenState  _screen   = new ScreenState();
  private readonly WindowState  _window   = new WindowState();
  private readonly OptionsState _options  = new OptionsState();
  private readonly RuntimeState _runtime  = new RuntimeState();

  private Scene?    _scene         = null;
  private IntPtr    _cursor        = IntPtr.Zero;
  private Scene?    _nextScene     = null;
  private bool      _wasInit       = false;
  private GameState _state         = null!;
  private IntPtr    _renderer      = IntPtr.Zero;
  private IntPtr    _windowSurface = IntPtr.Zero;
  private Font      _mainFont      = null!;


  public void Init()
  {
    if (_wasInit) return;
    _wasInit = true;

    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "best");
    SDL_Init(INIT_FLAGS);
    IMG_Init(IMG_INIT_FLAGS);

    int displayCount = SDL_GetNumVideoDisplays();
    ScreenIndex = Math.Min(ScreenIndex, displayCount);

    SDL_GetDisplayMode(ScreenIndex, 0, out var displayMode);

    _screen.DisplaysCount = displayCount;
    _screen.Index         = ScreenIndex;
    _screen.Width         = displayMode.w;
    _screen.Height        = displayMode.h;
    _screen.RefreshRate   = displayMode.refresh_rate;

    _window.Title          = Title;
    _window.Width          = FullscreenOnly ? displayMode.w : Width;
    _window.Height         = FullscreenOnly ? displayMode.h : Height;
    _window.IsFullscreen   = FullscreenOnly;
    _window.OriginalWidth  = _window.Width;
    _window.OriginalHeight = _window.Height;

    _runtime.Fps        = _screen.RefreshRate;
    _runtime.FrameIndex = 0;
    _runtime.IsRunning  = true;

    _state = new GameState
    {
      Keyboard = _keyboard,
      Mouse    = _mouse,
      Screen   = _screen,
      Window   = _window,
      Options  = _options,
      Runtime  = _runtime,
    };

    InitWindowAndRenderer();
    InitCursor();

    _mainFont = new Font("UbuntuMono-R.ttf");

    SetNextScene<TestScene>();
  }

  public void Start()
  {
    if (!_wasInit) Init();

    var timer = new Stopwatch();

    while (_runtime.IsRunning)
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

      if (timer.ElapsedMilliseconds < _runtime.FrameDelay)
        Thread.Sleep((int)Math.Floor(_runtime.FrameDelay - timer.ElapsedMilliseconds));

      _runtime.FrameIndex++;

      timer.Restart();
    }
  }

  public void Dispose()
  {
    _scene?.Dispose();
    _nextScene?.Dispose();

    if (_cursor != IntPtr.Zero)
      SDL_FreeCursor(_cursor);

    SDL_FreeSurface(_window.IconSurface);

    SDL_DestroyWindow(_window.WindowPtr);
    SDL_Quit();
  }

  private void HandleEvent(SDL_Event e)
  {
    switch (e.type)
    {
      case SDL_QUIT:
        _runtime.IsRunning = false;
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
        _window.Width  = e.window.data1;
        _window.Height = e.window.data2;
        break;
      case SDL_WINDOWEVENT_MAXIMIZED:
      case SDL_WINDOWEVENT_RESTORED:
        SDL_GetWindowSize(_window.WindowPtr, out _window.Width, out _window.Height);
        break;
    }
  }

  private void HandleMouseEvent(SDL_Event e)
  {
    _mouse.X               = e.button.x;
    _mouse.Y               = e.button.y;
    _mouse.Button          = e.button.button;
    _mouse.IsButtonPressed = e.type == SDL_MOUSEBUTTONDOWN;
  }

  private void HandleKeyDown(SDL_Event e)
  {
    HandleKeyEvent(e);

    switch (e.key.keysym.sym)
    {
      case SDLK_q:
        _runtime.IsRunning = false;
        break;
      case SDLK_f:
        ToggleFullscreen();
        break;

      // TODO: temp
      case SDLK_ESCAPE:
        SDL_SetWindowMouseGrab(
          _window.WindowPtr,
          SDL_GetWindowMouseGrab(_window.WindowPtr) == SDL_TRUE ? SDL_FALSE : SDL_TRUE
        );
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
    _keyboard.KeyPressed   = true;
    _keyboard.Key          = e.key.keysym.sym;
    _keyboard.AltPressed   = (state & KMOD_ALT) != 0;
    _keyboard.CtrlPressed  = (state & KMOD_CTRL) != 0;
    _keyboard.CapsPressed  = (state & KMOD_CAPS) != 0;
    _keyboard.ShiftPressed = (state & KMOD_SHIFT) != 0;
    _keyboard.GuiPressed   = (state & KMOD_GUI) != 0;
  }

  private void HandleMouseMove(SDL_Event e)
  {
    _mouse.X = e.motion.x;
    _mouse.Y = e.motion.y;
  }

  private void InitWindowAndRenderer()
  {
    SDL_WindowFlags windowFlags = FullscreenOnly ? FULLSCREEN_WINDOW_FLAGS : WINDOW_FLAGS;

    IntPtr window = SDL_CreateWindow(
      _window.Title,
      SDL_WINDOWPOS_CENTERED_DISPLAY(ScreenIndex),
      SDL_WINDOWPOS_CENTERED_DISPLAY(ScreenIndex),
      _window.Width,
      _window.Height,
      windowFlags
    );

    _windowSurface = SDL_GetWindowSurface(window);
    _renderer      = SDL_CreateRenderer(window, -1, RENDERER_FLAGS);
    SDL_SetRenderDrawBlendMode(_renderer, RENDERER_BLEND_MODE);

    SDL_SetWindowAlwaysOnTop(window, SDL_FALSE);
    SDL_SetWindowMinimumSize(window, _screen.Width, _screen.Height);
    SDL_SetWindowMaximumSize(window, _screen.Width, _screen.Height);

    if (FullscreenOnly)
    {
      SDL_SetWindowBordered(window, SDL_FALSE);
      SDL_SetWindowResizable(window, SDL_FALSE);
      SDL_SetWindowGrab(window, SDL_TRUE);
    }
    else
    {
      SDL_SetWindowBordered(window, SDL_TRUE);
      SDL_SetWindowResizable(window, SDL_TRUE);
    }

    IntPtr iconSurface = IMG_Load("Assets/Images/GameIcon.png");
    SDL_SetWindowIcon(window, iconSurface);

    _window.WindowPtr   = window;
    _window.IconSurface = iconSurface;
  }

  private void ToggleFullscreen()
  {
    if (FullscreenOnly) return;

    if (_window.IsFullscreen)
    {
      SDL_SetWindowFullscreen(_window.WindowPtr, 0);
      SDL_SetWindowSize(_window.WindowPtr, _window.OriginalWidth, _window.OriginalHeight);
      SDL_SetWindowPosition(
        _window.WindowPtr,
        SDL_WINDOWPOS_CENTERED_DISPLAY(ScreenIndex),
        SDL_WINDOWPOS_CENTERED_DISPLAY(ScreenIndex)
      );
      SDL_SetWindowBordered(_window.WindowPtr, SDL_TRUE);
      SDL_SetWindowMouseGrab(_window.WindowPtr, SDL_FALSE);

      _window.Width  = _window.OriginalWidth;
      _window.Height = _window.OriginalHeight;
    }
    else
    {
      SDL_SetWindowFullscreen(_window.WindowPtr, (uint)SDL_WINDOW_FULLSCREEN_DESKTOP);
      SDL_SetWindowBordered(_window.WindowPtr, SDL_FALSE);
      SDL_SetWindowMouseGrab(_window.WindowPtr, SDL_TRUE);

      _window.Width  = _screen.Width;
      _window.Height = _screen.Height;
    }

    _window.IsFullscreen = !_window.IsFullscreen;

    _scene?.OnToggleFullscreen();
  }

  private void InitCursor()
  {
    _cursor = SDL_CreateColorCursor(IMG_Load("Assets/Images/GameCursor.webp"), 0, 0);

    if (_cursor != IntPtr.Zero)
      SDL_SetCursor(_cursor);

    SDL_WarpMouseInWindow(_window.WindowPtr, _window.Width / 2, _window.Height / 2);
  }
}
