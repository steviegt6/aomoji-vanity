namespace AomojiVanity.API.Networking.Models;

/// <summary>
///     A struct that represents a byte that can be accessed as a bit array
///     represented through boolean values.
/// </summary>
/// <seealso cref="Terraria.BitsByte"/>
public struct BitFlags {
    private byte b0;

    public bool this[int index] {
        get => (b0 & (1 << index)) != 0;

        set {
            if (value)
                b0 |= (byte) (1 << index);
            else
                b0 &= (byte) ~(1 << index);
        }
    }
}
