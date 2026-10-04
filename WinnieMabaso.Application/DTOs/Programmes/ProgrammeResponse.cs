namespace WinnieMabaso.Application.DTOs.Programmes;

public class ProgrammeResponse
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int Capacity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? CreatedByUserId { get; set; }

    public string? UpdatedByUserId { get; set; }
}