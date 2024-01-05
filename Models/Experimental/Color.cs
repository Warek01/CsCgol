namespace CsGame.Experimental;

public struct Color : IEquatable<Color>
{
  public byte R = 0x00;
  public byte G = 0x00;
  public byte B = 0x00;
  public byte A = 0xFF;

  public uint PackedValue = 0x000000FF;

  public Color() { }

  public Color(uint packedValue)
  {
    R = Convert.ToByte((packedValue >> 24) & 0xFF);
    G = Convert.ToByte((packedValue >> 16) & 0xFF);
    B = Convert.ToByte((packedValue >> 8) & 0xFF);
    A = Convert.ToByte(packedValue & 0xFF);

    PackedValue = packedValue;
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
    return a.PackedValue == b.PackedValue;
  }

  public static bool operator !=(Color a, Color b)
  {
    return a.PackedValue != b.PackedValue;
  }

  public static Color operator +(Color color, byte value)
  {
    var c = new Color(color.PackedValue);
    c.Lighten(value);
    return c;
  }

  public static Color operator -(Color color, byte value)
  {
    var c = new Color(color.PackedValue);
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
    return PackedValue == obj.PackedValue;
  }

  public override int GetHashCode()
  {
    return PackedValue.GetHashCode();
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
    PackedValue = 0U | (uint)R << 24 | (uint)G << 16 | (uint)B << 8 | A;
  }

  #region Predefined colors

  public static readonly Color Black;
  public static readonly Color Red;
  public static readonly Color Green;
  public static readonly Color Blue;
  public static readonly Color White;
  public static readonly Color Yellow;
  public static readonly Color Silver;
  public static readonly Color Gray;
  public static readonly Color Maroon;
  public static readonly Color Olive;
  public static readonly Color Lime;
  public static readonly Color Aqua;
  public static readonly Color Teal;
  public static readonly Color Navy;
  public static readonly Color Fuchsia;
  public static readonly Color Purple;

  static Color()
  {
    Black   = new Color(0x000000FF);
    Red     = new Color(0xFF0000FF);
    Green   = new Color(0x008000FF);
    Blue    = new Color(0x0000FFFF);
    White   = new Color(0xFFFFFFFF);
    Yellow  = new Color(0xFFFF00FF);
    Silver  = new Color(0xC0C0C0FF);
    Gray    = new Color(0x808080FF);
    Maroon  = new Color(0x800000FF);
    Olive   = new Color(0x808000FF);
    Lime    = new Color(0x00FF00FF);
    Aqua    = new Color(0x00FFFFFF);
    Teal    = new Color(0x008080FF);
    Navy    = new Color(0x000080FF);
    Fuchsia = new Color(0xFF00FFFF);
    Purple  = new Color(0x800080FF);
  }

  #endregion
}
