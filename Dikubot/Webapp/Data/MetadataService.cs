using Microsoft.JSInterop;

namespace Data;

public class MetadataService
{
    private readonly IJSRuntime _js;

    public MetadataService(IJSRuntime js)
    {
        _js = js;
    }
    
    public async void SetTitle(string title)
    {
        await _js.InvokeVoidAsync("setTitle", title);
    }

    public async void SetDescription(string description)
    {
        await _js.InvokeVoidAsync("setDescription", description);
    }
    
}