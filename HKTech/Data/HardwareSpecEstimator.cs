using System.Text.RegularExpressions;

namespace HKTech.Data;

// ================================================================
// Ước tính BenchmarkScore (thang 0–3000) + TDP từ TÊN sản phẩm.
// Dùng cho dữ liệu scrape từ phongvu.vn (vốn để benchmark = 0),
// để FPS Estimator + Bottleneck Analyzer có số liệu thực tế.
//
// Score scale khớp với DbSeeder / ApiController.CalcFpsTable:
//   mid-range ~800-1200, high-end ~1500-2500, flagship ~2900.
// ================================================================
public static class HardwareSpecEstimator
{
    public record SpecEstimate(int BenchmarkScore, int TdpWatt);

    // Trả về null nếu không nhận diện được (không phải CPU/GPU gaming).
    public static SpecEstimate? Estimate(string categorySlug, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var n = name.ToLowerInvariant();

        return categorySlug switch
        {
            "cpu" => EstimateCpu(n),
            "gpu" => EstimateGpu(n),
            _     => null,
        };
    }

    // ── GPU ─────────────────────────────────────────────────────
    // Khớp pattern dài trước (vd "5070 ti" trước "5070").
    private static readonly (Regex Rx, int Score)[] GpuTable =
    {
        // NVIDIA RTX 50 series
        (R(@"rtx\s*5090"),          2950),
        (R(@"rtx\s*5080"),          2400),
        (R(@"rtx\s*5070\s*ti"),     2050),
        (R(@"rtx\s*5070"),          1650),
        (R(@"rtx\s*5060\s*ti"),     1300),
        (R(@"rtx\s*5060"),          1050),
        (R(@"rtx\s*5050"),           820),
        // NVIDIA RTX 40 series
        (R(@"rtx\s*4090"),          2900),
        (R(@"rtx\s*4080\s*super"),  2450),
        (R(@"rtx\s*4080"),          2350),
        (R(@"rtx\s*4070\s*ti\s*super"), 2000),
        (R(@"rtx\s*4070\s*ti"),     1900),
        (R(@"rtx\s*4070\s*super"),  1750),
        (R(@"rtx\s*4070"),          1500),
        (R(@"rtx\s*4060\s*ti"),     1150),
        (R(@"rtx\s*4060"),           950),
        // NVIDIA RTX 30 series
        (R(@"rtx\s*3090"),          2000),
        (R(@"rtx\s*3080"),          1700),
        (R(@"rtx\s*3070"),          1350),
        (R(@"rtx\s*3060\s*ti"),     1050),
        (R(@"rtx\s*3060"),           880),
        (R(@"rtx\s*3050"),           550),
        (R(@"rtx\s*2060"),           600),
        // NVIDIA Workstation (RTX PRO / ADA) — không phải gaming nhưng tránh để 0
        (R(@"pro\s*6000"),          2800),
        (R(@"pro\s*5000"),          2200),
        (R(@"pro\s*4500"),          1500),
        (R(@"pro\s*2000"),          1100),
        (R(@"4000\s*ada"),          1200),
        (R(@"2000\s*ada"),           900),
        // AMD Radeon RX
        (R(@"rx\s*7900"),           2300),
        (R(@"rx\s*7800"),           1700),
        (R(@"rx\s*7700"),           1450),
        (R(@"rx\s*7600"),            950),
        (R(@"rx\s*6700"),           1100),
        (R(@"rx\s*6600"),            850),
        (R(@"rx\s*6500\s*xt"),       450),
    };

    private static SpecEstimate? EstimateGpu(string n)
    {
        foreach (var (rx, score) in GpuTable)
        {
            if (rx.IsMatch(n))
                return new SpecEstimate(score, TdpForGpu(score));
        }
        // Fallback: thẻ RTX/RX lạ không khớp bảng → mid-range để không bằng 0
        if (Regex.IsMatch(n, @"\brtx\b|\bgtx\b|\brx\b|radeon"))
            return new SpecEstimate(1000, 200);
        return null;
    }

    private static int TdpForGpu(int score) => score switch
    {
        >= 2500 => 400,
        >= 2000 => 320,
        >= 1500 => 250,
        >= 1100 => 200,
        >= 800  => 160,
        _       => 130,
    };

    // ── CPU ─────────────────────────────────────────────────────
    private static SpecEstimate? EstimateCpu(string n)
    {
        // Intel Core Ultra (series 2xx)
        if (n.Contains("ultra"))
        {
            int baseScore =
                Regex.IsMatch(n, @"ultra\s*9") ? 2100 :
                Regex.IsMatch(n, @"ultra\s*7") ? 1500 :
                Regex.IsMatch(n, @"ultra\s*5") ?  950 : 1200;
            return new SpecEstimate(Clamp(baseScore), TdpForCpu(baseScore));
        }

        // Intel Core iX
        var iMatch = Regex.Match(n, @"\bi([3579])[- ]?(\d{4,5})");
        if (iMatch.Success)
        {
            int tier = int.Parse(iMatch.Groups[1].Value);
            string model = iMatch.Groups[2].Value;
            int gen = int.Parse(model.Substring(0, model.Length == 5 ? 2 : 1));

            double baseScore = tier switch { 9 => 1900, 7 => 1300, 5 => 800, _ => 430 };
            double genMul = gen switch { >= 14 => 1.15, 13 => 1.08, 12 => 1.00, 11 => 0.90, _ => 0.80 };
            if (Regex.IsMatch(n, @"\d+ks?\b") || n.Contains("k ") || n.EndsWith("k")) baseScore *= 1.08; // K/KS
            return new SpecEstimate(Clamp((int)(baseScore * genMul)), TdpForCpu((int)(baseScore * genMul)));
        }

        // AMD Ryzen
        var ryMatch = Regex.Match(n, @"ryzen\s*([3579])\s*(\d{4})?");
        if (ryMatch.Success)
        {
            int tier = int.Parse(ryMatch.Groups[1].Value);
            double baseScore = tier switch { 9 => 2100, 7 => 1400, 5 => 850, _ => 430 };

            if (ryMatch.Groups[2].Success)
            {
                int gen = ryMatch.Groups[2].Value[0] - '0';
                double genMul = gen switch { 9 => 1.18, 8 => 1.08, 7 => 1.00, 5 => 0.82, 3 => 0.62, _ => 0.80 };
                baseScore *= genMul;
            }
            if (n.Contains("x3d")) baseScore *= 1.14;
            // APU (G / GT) — đồ hoạ tích hợp, hiệu năng CPU thấp hơn
            if (Regex.IsMatch(n, @"\d+gt?\b") && !n.Contains("x3d")) baseScore = Math.Min(baseScore, 650);

            return new SpecEstimate(Clamp((int)baseScore), TdpForCpu((int)baseScore));
        }

        return null;
    }

    private static int TdpForCpu(int score) => score switch
    {
        >= 1800 => 170,
        >= 1200 => 125,
        >= 700  => 95,
        _       => 65,
    };

    private static int Clamp(int score) => Math.Clamp(score, 300, 2950);

    private static Regex R(string pattern) =>
        new(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
}
