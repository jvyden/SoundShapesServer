using SoundShapesServer.Types.Albums;

namespace SoundShapesServer.Responses.Api.Albums;

public class ApiAlbumResponse
{
    public ApiAlbumResponse(GameAlbum album)
    {
        Id = album.Id;
        Artist = album.Artist;
        Name = album.Name;
        LinerNotes = album.LinerNotes;
    }

    public string Id { get; set; }
    public string Artist { get; set; }
    public string Name { get; set; }
    public string LinerNotes { get; set; }
}