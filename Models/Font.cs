namespace CsGame.Models;

// Stores a single font of different sizes
public class Font : IDisposable
{
  public readonly IntPtr MainLg;
  public readonly IntPtr MainMd;
  public readonly IntPtr MainSm;

  public Font(string file)
  {
    if (TTF_Init() != 0)
      throw new Exception($"Error initializing TTF: {TTF_GetError()}");

    MainLg = TTF_OpenFont($"Assets/Fonts/{file}", 36);
    MainMd = TTF_OpenFont($"Assets/Fonts/{file}", 24);
    MainSm = TTF_OpenFont($"Assets/Fonts/{file}", 16);
  }


  public void Dispose()
  {
    TTF_CloseFont(MainLg);
    TTF_CloseFont(MainMd);
    TTF_CloseFont(MainSm);

    TTF_Quit();
  }
}
