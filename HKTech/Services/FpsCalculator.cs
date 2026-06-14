namespace HKTech.Services;

// ================================================================
// Ước tính FPS 10 game từ BenchmarkScore của CPU + GPU.
// Dùng chung cho ApiController (Tự Build PC) và PrebuiltController
// (chi tiết PC build sẵn). BenchmarkScore scale 0–3000.
// ================================================================
public static class FpsCalculator
{
    public record FpsRow(string Game, int Fps1080, int Fps1440, int Fps4k);

    // (tên game, trọng số CPU, trọng số GPU, FPS tối đa ở 1080p Ultra)
    private static readonly (string Name, float CpuW, float GpuW, int Max1080)[] Games =
    {
        ("Valorant",         0.55f, 0.45f, 600),
        ("CS2",              0.60f, 0.40f, 500),
        ("GTA V",            0.40f, 0.60f, 200),
        ("Cyberpunk 2077",   0.25f, 0.75f, 150),
        ("Elden Ring",       0.35f, 0.65f, 180),
        ("Minecraft",        0.70f, 0.30f, 400),
        ("Red Dead 2",       0.35f, 0.65f, 160),
        ("FC 25",            0.50f, 0.50f, 300),
        ("Hogwarts Legacy",  0.30f, 0.70f, 120),
        ("The Witcher 3",    0.35f, 0.65f, 220),
    };

    private const float ScoreBase = 2500f;

    public static List<FpsRow> Calc(int cpuScore, int gpuScore)
    {
        return Games.Select(g =>
        {
            var weighted = Math.Min(cpuScore * g.CpuW + gpuScore * g.GpuW, ScoreBase);
            var ratio    = weighted / ScoreBase;
            var f1080    = (int)Math.Round(ratio * g.Max1080);
            var f1440    = (int)Math.Round(f1080 * 0.65);
            var f4k      = (int)Math.Round(f1080 * 0.37);
            return new FpsRow(g.Name, f1080, f1440, f4k);
        }).ToList();
    }

    public static string Badge(int cpuScore, int gpuScore) => ((cpuScore + gpuScore) / 2) switch
    {
        < 450  => "BUDGET BUILD",
        < 900  => "MID-RANGE BUILD",
        < 1600 => "HIGH-END GAMING",
        _      => "ENTHUSIAST",
    };
}
