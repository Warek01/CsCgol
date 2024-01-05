namespace CsGame.Models;

// Stores a single font of different sizes
public class Font : IDisposable
{
  public readonly IntPtr RegularLg;
  public readonly IntPtr RegularMd;
  public readonly IntPtr RegularSm;

  public Font(string file)
  {
    if (TTF_WasInit() == 0 && TTF_Init() != 0)
      throw new Exception($"Error initializing TTF: {TTF_GetError()}");

    string path = Path.Join("Assets", "Fonts", file);

    RegularLg = TTF_OpenFont(path, 36);
    RegularMd = TTF_OpenFont(path, 24);
    RegularSm = TTF_OpenFont(path, 16);
  }


  public void Dispose()
  {
    TTF_CloseFont(RegularLg);
    TTF_CloseFont(RegularMd);
    TTF_CloseFont(RegularSm);

    TTF_Quit();
  }
}
