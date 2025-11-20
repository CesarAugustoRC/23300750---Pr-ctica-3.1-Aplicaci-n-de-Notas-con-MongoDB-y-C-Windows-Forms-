using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Notas_MongoDB
{
    public class Usuario
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Nombre")]
        public string Nombre { get; set; }

        [BsonElement("Contraseña")]
        public string Contraseania { get; set; }
    }

    public class Nota
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("UsuarioId")]
        public string UsuarioId { get; set; }

        [BsonElement("Titulo")]
        public string Titulo { get; set; }

        [BsonElement("Contenido")]
        public string Contenido { get; set; }

        [BsonElement("FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [BsonElement("Tags")]
        public string Tags { get; set; }
    }
}
