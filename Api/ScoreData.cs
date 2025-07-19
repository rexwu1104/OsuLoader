namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

namespace OsuLoader.Api
{
    public record ScoreData(
        int Score,
        float Accuracy,
        int Combo,
        ushort __300,
        ushort __100,
        ushort __50,
        ushort Perfect,
        ushort Good,
        ushort Misses);
}