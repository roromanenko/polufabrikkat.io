using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
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
		public byte[] FileData { get; set; }
	}
}
