namespace CsGame.Models;

// Stores a single font of different sizes
public class Font : IDisposable
{
  public readonly IntPtr MainLg;
  public readonly IntPtr MainMd;
  public readonly IntPtr MainSm;

  public Font(string file)
  {
    if (TTF_WasInit() == 0 && TTF_Init() != 0)
      throw new Exception($"Error initializing TTF: {TTF_GetError()}");

    string path = Path.Join("Assets", "Fonts", file);

    MainLg = TTF_OpenFont(path, 36);
    MainMd = TTF_OpenFont(path, 24);
    MainSm = TTF_OpenFont(path, 16);
  }


  public void Dispose()
  {
    TTF_CloseFont(MainLg);
    TTF_CloseFont(MainMd);
    TTF_CloseFont(MainSm);

    TTF_Quit();
  }
}
