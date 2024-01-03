namespace GameOfLife.Models;

public struct Color : IEquatable<Color>
{
  public byte R = 0x00;
  public byte G = 0x00;
  public byte B = 0x00;
  public byte A = 0xFF;

  private uint _packedValue = 0x000000FF;

  public Color() { }

  public Color(uint packedValue)
  {
    R = Convert.ToByte((packedValue >> 24) & 0xFF);
    G = Convert.ToByte((packedValue >> 16) & 0xFF);
    B = Convert.ToByte((packedValue >> 8) & 0xFF);
    A = Convert.ToByte(packedValue & 0xFF);

    _packedValue = packedValue;
  }

  public Color(byte r, byte g, byte b, byte a = 0xFF)
  {
    R = r;
    G = g;
    B = b;
    A = a;
    UpdatePackedValue();
  }

  public static bool operator ==(Color a, Color b)
  {
    return a._packedValue == b._packedValue;
  }

  public static bool operator !=(Color a, Color b)
  {
    return a._packedValue != b._packedValue;
  }

  public static Color operator +(Color color, byte value)
  {
    var c = new Color(color._packedValue);
    c.Lighten(value);
    return c;
  }
  public static Color operator -(Color color, byte value)
  {
    var c = new Color(color._packedValue);
    c.Darken(value);
    return c;
  }

  public SDL_Color SDL_Color()
  {
    return new SDL_Color { r = R, g = G, b = B, a = A };
  }

  public void Lighten(byte value)
  {
    R = Convert.ToByte(Math.Min(R + value, 0xFF));
    G = Convert.ToByte(Math.Min(G + value, 0xFF));
    B = Convert.ToByte(Math.Min(B + value, 0xFF));

    UpdatePackedValue();
  }

  public void Darken(byte value)
  {
    R = Convert.ToByte(Math.Max(R - value, 0));
    G = Convert.ToByte(Math.Max(G - value, 0));
    B = Convert.ToByte(Math.Max(B - value, 0));

    UpdatePackedValue();
  }

  public void Deconstruct(out byte r, out byte g, out byte b, out byte a)
  {
    r = R;
    g = G;
    b = B;
    a = A;
  }

  public bool Equals(Color obj)
  {
    return _packedValue == obj._packedValue;
  }

  public override int GetHashCode()
  {
    return _packedValue.GetHashCode();
  }

  public override bool Equals(object? obj)
  {
    return (obj is Color color) && Equals(color);
  }

  public override string ToString()
  {
    return $"#{R:x2}{G:x2}{B:x2}{A:x2}";
  }

  private void UpdatePackedValue()
  {
    _packedValue = 0U | (uint)R << 24 | (uint)G << 16 | (uint)B << 8 | A;
  }
}
