using System;

namespace TvShowsAPI.WebApi.Models
{
    /// <summary>
    /// Hold personal information about an actor/actress acting as a cast member on a show
    /// </summary>
    public class CastMember
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; }
    }
}
