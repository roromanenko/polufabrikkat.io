using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polufabrikkat.Core.Models.Entities
{
	[Table("files")]
	public class File
	{
		[BsonId]
		public ObjectId Id { get; set; }
		public string FileName { get; set; }
		public string ContentType { get; set; }
		public DateTime Added { get; set; }
		public byte[] FileData { get; set; }
	}
}
