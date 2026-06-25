using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HKTech.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(150)]
    public string? FullName { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    // Navigation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<PcBuild> PcBuilds { get; set; } = new List<PcBuild>();
}
