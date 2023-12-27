namespace SDLTest.Models;

public class KeyData
{
  public bool         KeyPressed   = false;
  public bool         ShiftPressed = false;
  public bool         CtrlPressed  = false;
  public bool         CmdPressed   = false;
  public bool         AltPressed   = false;
  public SDL_Keycode? Key          = null;
};
