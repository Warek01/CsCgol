namespace GameOfLife.Models;

public struct Color
{
  public SDL_Color SDL_Color = new SDL_Color { r = 0x00, g = 0x00, b = 0x00, a = 0xFF };

  public byte R
  {
    get => SDL_Color.r;
    set => SDL_Color.r = value;
  }

  public byte G
  {
    get => SDL_Color.g;
    set => SDL_Color.g = value;
  }

  public byte B
  {
    get => SDL_Color.b;
    set => SDL_Color.b = value;
  }

  public byte A
  {
    get => SDL_Color.a;
    set => SDL_Color.a = value;
  }

  public Color() { }

  public Color(string code)
  {
    if (code.StartsWith('#'))
      code = code[1..];

    R = Convert.ToByte(code[0..2], 16);
    G = Convert.ToByte(code[2..4], 16);
    B = Convert.ToByte(code[4..6], 16);

    if (code.Length > 6)
      A = Convert.ToByte(code[6..8], 16);
  }

  public Color(byte r, byte g, byte b) =>
    (R, G, B) = (r, g, b);

  public Color(byte r, byte g, byte b, byte a) : this(r, g, b) =>
    A = a;

  public void Lighten(byte value)
  {
    R = (byte)Math.Min(R + value, 0xFF);
    G = (byte)Math.Min(G + value, 0xFF);
    B = (byte)Math.Min(B + value, 0xFF);
  }

  public void Darken(byte value)
  {
    R = (byte)Math.Max(R - value, 0x00);
    G = (byte)Math.Max(G - value, 0x00);
    B = (byte)Math.Max(B - value, 0x00);
  }
}
