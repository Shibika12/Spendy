namespace ShibikaKhatri22068731CW1.Services.Interface;

public interface ITagService
{
    Task<List<string>> GetAllTagsAsync();
    Task AddCustomTagAsync(string tag);
    Task DeleteCustomTagAsync(string tag);
    Task<List<string>> GetCustomTagsAsync();
}