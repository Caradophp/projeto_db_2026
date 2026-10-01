namespace projeto.Models;

public class ManagementListViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SearchPlaceholder { get; set; } = string.Empty;
    public string AddLabel { get; set; } = string.Empty;
    public IReadOnlyList<string> Columns { get; set; } = Array.Empty<string>();
}