namespace CsGame.Models;

public class KeyState
{
  public bool         KeyPressed   = false;
  public bool         ShiftPressed = false;
  public bool         CtrlPressed  = false;
  public bool         GuiPressed   = false;
  public bool         AltPressed   = false;
  public bool         CapsPressed  = false;
  public SDL_Keycode? Key          = null;
};
