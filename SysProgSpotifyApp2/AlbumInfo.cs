using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysProgSpotifyApp
{
    internal class AlbumInfo
    {
        public string AlbumName { get; set; }
        public string ArtistName { get; set; }
        public string ReleaseDate { get; set; }
        public int TotalTracks { get; set; }
        public override string ToString()
        {
            return $"--------------------------------\nAlbumName: {AlbumName}\n---------------------------\nArtistName: {ArtistName}\nReleaseDate: {ReleaseDate} C\nTotalTracks: {TotalTracks}";
        }
    }
}
