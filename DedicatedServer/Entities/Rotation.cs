namespace Minecraft.Entities;

public struct Rotation
{
    internal byte Value
    {
        get;
        set;
    }

    public float Degrees
    {
        get => Value * 360f / 256f;
        set => Value = Normalize(value);
    }

    public Rotation(byte value)
        => Value = value;

    public static implicit operator Rotation(float degree)
        => new(Normalize(Clamp(degree)));

    public static implicit operator float(Rotation angle)
        => angle.Degrees;

    public static byte Normalize(float value)
        => (byte)(value * 256f / 360f);

    public static float Clamp(float degrees)
    {
        float clamped = degrees % 360f;
        return clamped < 0f ? clamped + 360f : clamped;
    }
}
