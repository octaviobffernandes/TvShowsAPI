using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TvShowsAPI.WebApi.Models
{
    /// <summary>
    /// Holds main information about a TV show
    /// </summary>
    public class Show
    {
        [BsonId]
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<CastMember> Cast { get; set; }
    }
}
