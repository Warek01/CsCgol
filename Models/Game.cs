namespace CsGame.Models;

public class Game : IDisposable
{
  public bool   FullscreenOnly  { get; init; } = false;
  public int    Width       { get; init; } = 600;
  public int    Height      { get; init; } = 800;
  public int    ScreenIndex { get; init; } = 0;
  public string Title       { get; init; } = string.Empty;
  public int    MinWidth    { get; init; } = 400;
  public int    MinHeight   { get; init; } = 400;

  private const SDL_WindowFlags WINDOW_FLAGS = SDL_WINDOW_SHOWN;

  private const SDL_WindowFlags FULLSCREEN_WINDOW_FLAGS =
    SDL_WINDOW_SHOWN | SDL_WINDOW_FULLSCREEN_DESKTOP | SDL_WINDOW_INPUT_FOCUS | SDL_WINDOW_INPUT_FOCUS;

  private const uint              INIT_FLAGS          = SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_EVENTS;
  private const IMG_InitFlags     IMG_INIT_FLAGS      = IMG_INIT_JPG | IMG_INIT_PNG | IMG_INIT_TIF | IMG_INIT_WEBP;
  private const SDL_RendererFlags RENDERER_FLAGS      = SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC;
  private const SDL_BlendMode     RENDERER_BLEND_MODE = SDL_BLENDMODE_BLEND;

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

    var keyboard = new KeyState();
    var mouse    = new MouseState();
    var screen   = new ScreenState();
    var window   = new WindowState();
    var options  = new OptionsState();
    var runtime  = new RuntimeState();

    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "best");
    SDL_Init(INIT_FLAGS);
    IMG_Init(IMG_INIT_FLAGS);
    SDL_GetDisplayMode(ScreenIndex, 0, out var displayMode);

    screen.Index       = ScreenIndex;
    screen.Width       = displayMode.w;
    screen.Height      = displayMode.h;
    screen.RefreshRate = displayMode.refresh_rate;

    window.Title          = Title;
    window.Width          = FullscreenOnly ? displayMode.w : Width;
    window.Height         = FullscreenOnly ? displayMode.h : Height;
    window.IsFullscreen   = FullscreenOnly;
    window.OriginalWidth  = window.Width;
    window.OriginalHeight = window.Height;

    runtime.Fps        = screen.RefreshRate;
    runtime.FrameIndex = 0;
    runtime.IsRunning  = true;

    _state = new GameState
    {
      Keyboard = keyboard,
      Mouse    = mouse,
      Screen   = screen,
      Window   = window,
      Options  = options,
      Runtime  = runtime,
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

    if (_cursor != IntPtr.Zero)
      SDL_FreeCursor(_cursor);

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
    SDL_WindowFlags windowFlags = FullscreenOnly ? FULLSCREEN_WINDOW_FLAGS : WINDOW_FLAGS;

    IntPtr window = SDL_CreateWindow(
      _state.Window.Title,
      SDL_WINDOWPOS_CENTERED_DISPLAY(ScreenIndex),
      SDL_WINDOWPOS_CENTERED_DISPLAY(ScreenIndex),
      _state.Window.Width,
      _state.Window.Height,
      windowFlags
    );

    _windowSurface = SDL_GetWindowSurface(window);
    _renderer      = SDL_CreateRenderer(window, -1, RENDERER_FLAGS);
    SDL_SetRenderDrawBlendMode(_renderer, RENDERER_BLEND_MODE);

    if (FullscreenOnly)
    {
      SDL_SetWindowMinimumSize(window, _state.Screen.Width, _state.Screen.Height);
      SDL_SetWindowMaximumSize(window, _state.Screen.Width, _state.Screen.Height);
      SDL_SetWindowBordered(window, SDL_FALSE);
      SDL_SetWindowAlwaysOnTop(window, SDL_TRUE);
      SDL_SetWindowResizable(window, SDL_FALSE);
      SDL_SetWindowMouseGrab(window, SDL_TRUE);
    }
    else
    {
      SDL_SetWindowMinimumSize(window, MinWidth, MinHeight);
      SDL_SetWindowMaximumSize(window, _state.Screen.Width, _state.Screen.Height);
      SDL_SetWindowBordered(window, SDL_TRUE);
      SDL_SetWindowAlwaysOnTop(window, SDL_FALSE);
      SDL_SetWindowResizable(window, SDL_TRUE);
    }


    IntPtr iconSurface = IMG_Load("Assets/Images/GameIcon.png");
    SDL_SetWindowIcon(window, iconSurface);

    _state.Window.WindowPtr   = window;
    _state.Window.IconSurface = iconSurface;
  }

  private void ToggleFullscreen()
  {
    if (FullscreenOnly) return;

    if (_state.Window.IsFullscreen)
    {
      SDL_SetWindowFullscreen(_state.Window.WindowPtr, 0);
      SDL_SetWindowSize(_state.Window.WindowPtr, _state.Window.OriginalWidth, _state.Window.OriginalHeight);
      SDL_SetWindowPosition(
        _state.Window.WindowPtr,
        SDL_WINDOWPOS_CENTERED_DISPLAY(ScreenIndex),
        SDL_WINDOWPOS_CENTERED_DISPLAY(ScreenIndex)
      );
      SDL_SetWindowBordered(_state.Window.WindowPtr, SDL_TRUE);
      SDL_SetWindowMouseGrab(_state.Window.WindowPtr, SDL_FALSE);

      _state.Window.Width  = _state.Window.OriginalWidth;
      _state.Window.Height = _state.Window.OriginalHeight;
    }
    else
    {
      SDL_SetWindowFullscreen(_state.Window.WindowPtr, (uint)SDL_WINDOW_FULLSCREEN_DESKTOP);
      SDL_SetWindowBordered(_state.Window.WindowPtr, SDL_FALSE);
      SDL_SetWindowMouseGrab(_state.Window.WindowPtr, SDL_TRUE);

      _state.Window.Width  = _state.Screen.Width;
      _state.Window.Height = _state.Screen.Height;
    }

    _state.Window.IsFullscreen = !_state.Window.IsFullscreen;

    _scene?.OnToggleFullscreen();
  }

  private void InitCursor()
  {
    _cursor = SDL_CreateColorCursor(IMG_Load("Assets/Images/GameCursor.webp"), 0, 0);

    if (_cursor != IntPtr.Zero)
      SDL_SetCursor(_cursor);

    SDL_WarpMouseInWindow(_state.Window.WindowPtr, _state.Window.Width / 2, _state.Window.Height / 2);
  }
}
