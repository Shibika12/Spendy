using ShibikaKhatri22068731CW1.Services.Interface;
using ShibikaKhatri22068731CW1.Model;
namespace ShibikaKhatri22068731CW1.Services;

public class TagService : ITagService
{
    private readonly string _tagsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "custom_tags.txt");
    private readonly List<string> _defaultTags = new List<string>
    {
        "Yearly", "Monthly", "Food", "Drinks", "Clothes", "Gadgets",
        "Miscellaneous", "Fuel", "Rent", "EMI", "Party"
    };
    public async Task<List<string>> GetCustomTagsAsync()
    {
        try
        {
            if (!File.Exists(_tagsFilePath))
            {
                return new List<string>();
            }

            var tags = await File.ReadAllLinesAsync(_tagsFilePath);
            return tags.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading custom tags: {ex.Message}");
            return new List<string>();
        }
    }
    public async Task<List<string>> GetAllTagsAsync()
    {
        var customTags = await GetCustomTagsAsync();
        var allTags = new List<string>(_defaultTags);
        allTags.AddRange(customTags);
        return allTags;
    }
    public async Task AddCustomTagAsync(string tag)
    {
        var customTags = await GetCustomTagsAsync();
        if (!customTags.Contains(tag) && !_defaultTags.Contains(tag))
        {
            customTags.Add(tag);
            await SaveCustomTagsAsync(customTags);
        }
    }
    private async Task SaveCustomTagsAsync(List<string> tags)
    {
        await File.WriteAllLinesAsync(_tagsFilePath, tags);
    }
    public async Task DeleteCustomTagAsync(string tag)
    {
        if (_defaultTags.Contains(tag))
        {
            throw new InvalidOperationException("Cannot delete default tags");
        }
        var customTags = await GetCustomTagsAsync();
        if (customTags.Contains(tag))
        {
            customTags.Remove(tag);
            await SaveCustomTagsAsync(customTags);
        }
    }
}