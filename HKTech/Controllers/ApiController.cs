// ================================================================
// HKTECH — ApiController.cs
// Phase 4: Smart Build Intelligence REST API
// POST /api/compatibility  — Kiểm tra tương thích 4 điểm
// POST /api/performance    — Bottleneck % + FPS table 10 game
// ================================================================

using HKTech.Data;
using HKTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ApiController(ApplicationDbContext db) => _db = db;

        // ================================================================
        // POST /api/compatibility
        // Body: { "cpu": 1, "mainboard": 2, "ram": 3, "gpu": 4, ... }
        // Returns: 4 kiểm tra tương thích
        // ================================================================
        [HttpPost("compatibility")]
        public async Task<IActionResult> Compatibility([FromBody] Dictionary<string, int> slots)
        {
            if (slots == null || !slots.Any())
                return Ok(EmptyCompatResult());

            var ids      = slots.Values.Distinct().ToList();
            var products = await _db.Products
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var sp = slots
                .Where(kvp => products.ContainsKey(kvp.Value))
                .ToDictionary(kvp => kvp.Key, kvp => products[kvp.Value]);

            return Ok(new
            {
                socket = CheckSocket(sp),
                ram    = CheckRam(sp),
                psu    = CheckPsu(sp),
                form   = CheckForm(sp),
            });
        }

        // ================================================================
        // POST /api/performance
        // Body: { "cpuId": 1, "gpuId": 2 }
        // Returns: bottleneck % + bảng FPS 10 game
        // ================================================================
        [HttpPost("performance")]
        public async Task<IActionResult> Performance([FromBody] PerformanceRequest req)
        {
            if (req.CpuId <= 0 || req.GpuId <= 0)
                return BadRequest(new { error = "cpuId và gpuId không hợp lệ." });

            var cpu = await _db.Products.FindAsync(req.CpuId);
            var gpu = await _db.Products.FindAsync(req.GpuId);
            if (cpu == null || gpu == null) return NotFound();

            var cpuScore = cpu.BenchmarkScore;
            var gpuScore = gpu.BenchmarkScore;

            // ── Bottleneck ──────────────────────────────────────────────
            int cpuPct = 50, gpuPct = 50, bottleneck = 0;
            string bottleneckHint = "* Chọn CPU & GPU để xem phân tích.";

            if (cpuScore > 0 && gpuScore > 0)
            {
                var total = cpuScore + gpuScore;
                cpuPct     = (int)Math.Round((double)cpuScore / total * 100);
                gpuPct     = 100 - cpuPct;
                bottleneck = Math.Abs(cpuPct - gpuPct);

                bottleneckHint = bottleneck switch
                {
                    <= 10 => "✅ CPU và GPU cân bằng tốt! Build tối ưu.",
                    <= 25 when cpuPct < gpuPct
                          => $"⚠ CPU đang hơi kìm hãm GPU ({bottleneck}%). Cân nhắc nâng CPU.",
                    <= 25 => $"⚠ GPU đang hơi kìm hãm CPU ({bottleneck}%). Cân nhắc nâng GPU.",
                    _ when cpuPct < gpuPct
                          => $"❌ CPU đang kìm hãm GPU nghiêm trọng ({bottleneck}%). Nên nâng CPU.",
                    _     => $"❌ GPU đang kìm hãm CPU nghiêm trọng ({bottleneck}%). Nên nâng GPU.",
                };
            }

            // ── FPS Table ───────────────────────────────────────────────
            var fpsTable = CalcFpsTable(cpuScore, gpuScore);

            // ── Build badge ─────────────────────────────────────────────
            var avgScore = (cpuScore + gpuScore) / 2;
            var badge = avgScore switch
            {
                < 200  => "BUDGET BUILD",
                < 500  => "MID-RANGE BUILD",
                < 800  => "HIGH-END GAMING",
                _      => "ENTHUSIAST",
            };

            return Ok(new
            {
                cpuPct,
                gpuPct,
                bottleneck,
                bottleneckHint,
                fpsTable,
                badge,
                cpuScore,
                gpuScore,
            });
        }

        // ================================================================
        // COMPATIBILITY HELPERS
        // ================================================================
        private static object EmptyCompatResult() => new
        {
            socket = new CompatResult(null, "⬜ Socket CPU / Mainboard"),
            ram    = new CompatResult(null, "⬜ RAM Type (DDR4/DDR5)"),
            psu    = new CompatResult(null, "⬜ PSU Wattage vs TDP"),
            form   = new CompatResult(null, "⬜ Form Factor (ATX/mATX/ITX)"),
        };

        private static CompatResult CheckSocket(Dictionary<string, Product> sp)
        {
            if (!sp.TryGetValue("cpu", out var cpu) || !sp.TryGetValue("mainboard", out var mb))
                return new(null, "⬜ Socket CPU / Mainboard");

            if (string.IsNullOrEmpty(cpu.Socket) || string.IsNullOrEmpty(mb.Socket))
                return new(null, "Socket: Không có dữ liệu");

            return cpu.Socket == mb.Socket
                ? new(true,  $"Socket {cpu.Socket} ↔ {mb.Socket}: Tương thích ✓")
                : new(false, $"Socket {cpu.Socket} ↔ {mb.Socket}: KHÔNG khớp ✗");
        }

        private static CompatResult CheckRam(Dictionary<string, Product> sp)
        {
            if (!sp.TryGetValue("ram", out var ram) || !sp.TryGetValue("mainboard", out var mb))
                return new(null, "⬜ RAM Type (DDR4/DDR5)");

            if (string.IsNullOrEmpty(ram.RamType) || string.IsNullOrEmpty(mb.RamType))
                return new(null, "RAM Type: Không có dữ liệu");

            return ram.RamType == mb.RamType
                ? new(true,  $"{ram.RamType} ↔ MB {mb.RamType}: Tương thích ✓")
                : new(false, $"RAM {ram.RamType} ↔ MB hỗ trợ {mb.RamType}: KHÔNG khớp ✗");
        }

        private static CompatResult CheckPsu(Dictionary<string, Product> sp)
        {
            if (!sp.TryGetValue("psu", out var psu))
                return new(null, "⬜ PSU Wattage vs TDP");

            // Cần ít nhất CPU hoặc GPU (2 linh kiện ngốn điện nhất) để kết luận có nghĩa
            if (!sp.ContainsKey("cpu") && !sp.ContainsKey("gpu"))
                return new(null, "⚠ Chọn CPU hoặc GPU để kiểm tra công suất");

            // Với PSU, TdpWatt lưu công suất đầu ra (W)
            var psuWatt  = psu.TdpWatt;
            var totalTdp = sp.Where(kvp => kvp.Key != "psu").Sum(kvp => kvp.Value.TdpWatt);

            if (totalTdp == 0)
                return new(null, $"PSU {psuWatt}W: Chưa đủ dữ liệu TDP");

            // Cần 20% headroom an toàn
            var needed = (int)Math.Ceiling(totalTdp * 1.2);

            return psuWatt >= needed
                ? new(true,  $"PSU {psuWatt}W ≥ {needed}W ({totalTdp}W + 20%): Đủ công suất ✓")
                : new(false, $"PSU {psuWatt}W < {needed}W cần ({totalTdp}W + 20%): Thiếu công suất ✗");
        }

        private static CompatResult CheckForm(Dictionary<string, Product> sp)
        {
            if (!sp.TryGetValue("case", out var pcCase) || !sp.TryGetValue("mainboard", out var mb))
                return new(null, "⬜ Form Factor (ATX/mATX/ITX)");

            if (string.IsNullOrEmpty(pcCase.FormFactor) || string.IsNullOrEmpty(mb.FormFactor))
                return new(null, "Form Factor: Không có dữ liệu");

            // ATX case chứa được ATX/mATX/ITX; mATX chứa mATX/ITX; ITX chỉ chứa ITX
            var fits = (pcCase.FormFactor, mb.FormFactor) switch
            {
                ("ATX",  _)      => true,
                ("mATX", "mATX") => true,
                ("mATX", "ITX")  => true,
                ("ITX",  "ITX")  => true,
                _                => false,
            };

            return fits
                ? new(true,  $"Case {pcCase.FormFactor} + MB {mb.FormFactor}: Phù hợp ✓")
                : new(false, $"Case {pcCase.FormFactor} không chứa được MB {mb.FormFactor} ✗");
        }

        // ================================================================
        // FPS TABLE CALCULATION
        // BenchmarkScore scale giả định 0–3000
        // (mid-range ~700-1000, high-end ~1500-2500)
        // ================================================================
        private static List<FpsRow> CalcFpsTable(int cpuScore, int gpuScore)
        {
            // (tên game, trọng số CPU, trọng số GPU, FPS tối đa ở 1080p Ultra)
            var games = new (string Name, float CpuW, float GpuW, int Max1080)[]
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

            // Chuẩn hóa về 0-1 (2500 = điểm chuẩn max)
            const float ScoreBase = 2500f;

            return games.Select(g =>
            {
                var weighted = Math.Min(cpuScore * g.CpuW + gpuScore * g.GpuW, ScoreBase);
                var ratio    = weighted / ScoreBase;
                var f1080    = (int)Math.Round(ratio * g.Max1080);
                var f1440    = (int)Math.Round(f1080 * 0.65);
                var f4k      = (int)Math.Round(f1080 * 0.37);
                return new FpsRow(g.Name, f1080, f1440, f4k);
            }).ToList();
        }

        // ================================================================
        // VALUE OBJECTS
        // ================================================================
        private record CompatResult(bool? Ok, string Message);
        private record FpsRow(string Game, int Fps1080, int Fps1440, int Fps4k);
    }

    public class PerformanceRequest
    {
        public int CpuId { get; set; }
        public int GpuId { get; set; }
    }
}
