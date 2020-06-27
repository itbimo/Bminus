namespace Bminus.Models
{
    public class Song
    {
        public string Title { get; set; }
        public Genre Genre { get; set; }
        public string Length { get; set; }
        public Artist Artist { get; set; }
    }

    public enum Genre
    {
        ElectronicDanceMusic,
        Rock,
        Jazz,
        Dubstep,
        RhythmAndBlues,
        Techno,
        Country,
        Electro,
        IndieRock,
        Pop
    }
}