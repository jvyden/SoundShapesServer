using Newtonsoft.Json;
using SoundShapesServer.Types.Albums;

namespace SoundShapesServer.Responses.Game.Albums;

public class LinerNoteResponse
{
    public LinerNoteResponse(FontType fontType, string text)
    {
        Font = fontType switch
        {
            FontType.Title => "title",
            FontType.Heading => "heading",
            FontType.Normal => "normal",
            _ => "normal"
        };
        
        Text = text;
    }

    [JsonProperty("fontType")] public string Font { get; set; }
    [JsonProperty("text")] public string Text { get; set; }
}