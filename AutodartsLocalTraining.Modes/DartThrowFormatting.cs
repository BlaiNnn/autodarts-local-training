namespace AutodartsLocalTraining.Modes;

public static class DartThrowFormatting
{
    public static string FormatName(this DartThrow dart)
    {
        // Autodarts reports Multiplier=0 for any throw that scores nothing at
        // all - a genuine miss, an undetected bounce-out we filled in
        // ourselves, or a hit in the outer "wire"/no-score ring around the
        // board. All of these should read as "Miss", not the nearby segment
        // number Autodarts happens to attach to them.
        if (dart.Multiplier == 0)
            return "Miss";

        if (dart.Number == 25 && dart.Multiplier == 2)
            return "Bull";

        return dart.Multiplier switch
        {
            2 => $"D{dart.Number}",
            3 => $"T{dart.Number}",
            _ => dart.Number.ToString()
        };
    }
}