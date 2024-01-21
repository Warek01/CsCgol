namespace CsGame.Models;

public class Game : IGame
{
  public readonly Keyboard Keyboard = new Keyboard();
  public readonly Mouse    Mouse    = new Mouse();
  public readonly Screen   Screen   = new Screen();
  public readonly Window   Window   = new Window();
  public readonly Options  Options  = new Options();
  public readonly Runtime  Runtime  = new Runtime();

  public Font   MainFont;
  public nint   Renderer;
  public Config InitialConfig;

  private const SDL_WindowFlags WINDOW_FLAGS = SDL_WINDOW_SHOWN;

  private const SDL_WindowFlags FULLSCREEN_WINDOW_FLAGS =
    SDL_WINDOW_SHOWN | SDL_WINDOW_FULLSCREEN_DESKTOP | SDL_WINDOW_INPUT_FOCUS | SDL_WINDOW_INPUT_FOCUS;

  private const uint              INIT_FLAGS          = SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_EVENTS;
  private const IMG_InitFlags     IMG_INIT_FLAGS      = IMG_INIT_JPG | IMG_INIT_PNG | IMG_INIT_TIF | IMG_INIT_WEBP;
  private const SDL_RendererFlags RENDERER_FLAGS      = SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC;
  private const SDL_BlendMode     RENDERER_BLEND_MODE = SDL_BLENDMODE_BLEND;

  private Scene? _scene     = null;
  private nint   _cursor    = nint.Zero;
  private Scene? _nextScene = null;


  public Game()
  {
    ReadConfigFile();

    SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "best");
    SDL_Init(INIT_FLAGS);
    IMG_Init(IMG_INIT_FLAGS);

    int displayCount = SDL_GetNumVideoDisplays();
    Screen.Index         = Math.Min(InitialConfig.ScreenIndex, displayCount - 1);
    Screen.DisplaysCount = displayCount;
    SDL_GetDisplayMode(Screen.Index, 0, out var displayMode);
    Screen.Width       = displayMode.w;
    Screen.Height      = displayMode.h;
    Screen.RefreshRate = displayMode.refresh_rate;

    Window.Title          = "Test game";
    Window.Width          = InitialConfig.Window.FullscreenOnly ? displayMode.w : InitialConfig.Window.InitialWidth;
    Window.Height         = InitialConfig.Window.FullscreenOnly ? displayMode.h : InitialConfig.Window.InitialHeight;
    Window.IsFullscreen   = InitialConfig.Window.FullscreenOnly;
    Window.OriginalWidth  = Window.Width;
    Window.OriginalHeight = Window.Height;

    Runtime.Fps        = InitialConfig.Runtime.TargetFps == -1 ? Screen.RefreshRate : InitialConfig.Runtime.TargetFps;
    Runtime.FrameIndex = 0;
    Runtime.IsRunning  = true;
    Runtime.FrameTimer = new Stopwatch();

    InitWindowAndRenderer();
    InitCursor();

    MainFont = new Font("UbuntuMono-R.ttf");

    SetScene<TestScene>();

    if (InitialConfig.Window.InitialFullscreen)
      ToggleFullscreen();
  }

  public void Start()
  {
    while (Runtime.IsRunning)
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

      SdlUtils.SetDrawColor(Renderer, Color.Maroon);
      SDL_RenderClear(Renderer);
      _scene?.OnRender();
      SDL_RenderPresent(Renderer);
      Runtime.FrameIndex++;

      FrameSleep();
    }
  }

  public void Dispose()
  {
    _scene?.Dispose();
    _nextScene?.Dispose();

    if (_cursor != nint.Zero)
      SDL_FreeCursor(_cursor);

    SDL_FreeSurface(Window.IconSurface);

    SDL_DestroyWindow(Window.WindowPtr);
    SDL_Quit();
  }

  private void HandleEvent(SDL_Event e)
  {
    switch (e.type)
    {
      case SDL_QUIT:
        Runtime.IsRunning = false;
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
    object? instance = Activator.CreateInstance(typeof(TSceneClass), this);

    if (instance is null)
      throw new Exception("Error instantiating scene");

    return (Scene)instance;
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
        SDL_GetWindowSize(Window.WindowPtr, out Window.Width, out Window.Height);
        break;
      case SDL_WINDOWEVENT_TAKE_FOCUS:
      case SDL_WINDOWEVENT_FOCUS_GAINED:
        Window.IsFocused = true;
        break;
      case SDL_WINDOWEVENT_FOCUS_LOST:
        Window.IsFocused = false;
        break;
    }
  }

  private void HandleMouseEvent(SDL_Event e)
  {
    Mouse.X               = e.button.x;
    Mouse.Y               = e.button.y;
    Mouse.Button          = e.button.button;
    Mouse.IsButtonPressed = e.type == SDL_MOUSEBUTTONDOWN;
  }

  private void HandleKeyDown(SDL_Event e)
  {
    HandleKeyEvent(e);

    switch (e.key.keysym.sym)
    {
      case SDLK_q:
        Runtime.IsRunning = false;
        break;
      case SDLK_f:
        ToggleFullscreen();
        break;

      // TODO: temp
      case SDLK_ESCAPE:
        SDL_SetWindowMouseGrab(
          Window.WindowPtr,
          SDL_GetWindowMouseGrab(Window.WindowPtr) == SDL_TRUE ? SDL_FALSE : SDL_TRUE
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

  private void InitWindowAndRenderer()
  {
    SDL_WindowFlags windowFlags = InitialConfig.Window.FullscreenOnly ? FULLSCREEN_WINDOW_FLAGS : WINDOW_FLAGS;

    nint window = SDL_CreateWindow(
      Window.Title,
      SDL_WINDOWPOS_CENTERED_DISPLAY(Screen.Index),
      SDL_WINDOWPOS_CENTERED_DISPLAY(Screen.Index),
      Window.Width,
      Window.Height,
      windowFlags
    );

    Renderer = SDL_CreateRenderer(window, -1, RENDERER_FLAGS);
    SDL_SetRenderDrawBlendMode(Renderer, RENDERER_BLEND_MODE);

    SDL_SetWindowAlwaysOnTop(window, SDL_FALSE);

    if (InitialConfig.Window.MinWidth != -1 && InitialConfig.Window.MinHeight != -1)
      SDL_SetWindowMinimumSize(window, InitialConfig.Window.MinWidth, InitialConfig.Window.MinHeight);

    if (InitialConfig.Window.MaxWidth != -1 && InitialConfig.Window.MaxHeight != -1)
      SDL_SetWindowMaximumSize(window, InitialConfig.Window.MaxWidth, InitialConfig.Window.MaxHeight);

    if (InitialConfig.Window.FullscreenOnly)
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

    nint iconSurface = IMG_Load("Assets/Images/GameIcon.png");
    SDL_SetWindowIcon(window, iconSurface);

    Window.WindowPtr   = window;
    Window.IconSurface = iconSurface;
  }

  private void ToggleFullscreen()
  {
    if (InitialConfig.Window.FullscreenOnly) return;

    if (Window.IsFullscreen)
    {
      SDL_SetWindowFullscreen(Window.WindowPtr, 0);
      SDL_SetWindowSize(Window.WindowPtr, Window.OriginalWidth, Window.OriginalHeight);
      SDL_SetWindowPosition(
        Window.WindowPtr,
        SDL_WINDOWPOS_CENTERED_DISPLAY(Screen.Index),
        SDL_WINDOWPOS_CENTERED_DISPLAY(Screen.Index)
      );
      SDL_SetWindowBordered(Window.WindowPtr, SDL_TRUE);
      SDL_SetWindowMouseGrab(Window.WindowPtr, SDL_FALSE);

      Window.Width  = Window.OriginalWidth;
      Window.Height = Window.OriginalHeight;
    }
    else
    {
      SDL_SetWindowFullscreen(Window.WindowPtr, (uint)SDL_WINDOW_FULLSCREEN_DESKTOP);
      SDL_SetWindowBordered(Window.WindowPtr, SDL_FALSE);
      SDL_SetWindowMouseGrab(Window.WindowPtr, SDL_TRUE);

      Window.Width  = Screen.Width;
      Window.Height = Screen.Height;
    }

    Window.IsFullscreen = !Window.IsFullscreen;

    _scene?.OnToggleFullscreen();
  }

  private void InitCursor()
  {
    _cursor = SDL_CreateColorCursor(IMG_Load("Assets/Images/GameCursor.webp"), 0, 0);

    if (_cursor != nint.Zero)
      SDL_SetCursor(_cursor);

    SDL_WarpMouseInWindow(Window.WindowPtr, Window.Width / 2, Window.Height / 2);
  }

  private void FrameSleep()
  {
    if (Runtime.FrameTimer.ElapsedMilliseconds >= Runtime.FrameDelay) return;

    int delay = Convert.ToInt32(Math.Floor(Runtime.FrameDelay - Runtime.FrameTimer.ElapsedMilliseconds));
    Thread.Sleep(delay);
    Runtime.FrameTimer.Restart();
  }

  private void ReadConfigFile()
  {
    string  content = File.ReadAllText("Config.json", Encoding.UTF8);
    Config? config  = JsonSerializer.Deserialize<Config>(content);

    if (config is null)
      throw new ApplicationException("Config is null");

    InitialConfig = config;
  }
}
