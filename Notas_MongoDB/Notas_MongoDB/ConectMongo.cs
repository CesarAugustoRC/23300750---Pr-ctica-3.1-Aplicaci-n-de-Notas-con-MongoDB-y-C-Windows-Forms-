using MongoDB.Driver;

namespace Notas_MongoDB
{
    public static class ConectMongo
    {
        private static readonly string connectionString =
            $"mongodb+srv://<db_user>:<db_password>@clusternotas.embegqi.mongodb.net/?appName=ClusterNotas\r\n";
        // Ejemplo con usuario ya creado: mongodb+srv://a23300750_db_user:<db_password>@clusternotas.embegqi.mongodb.net/?appName=ClusterNotas
        private static readonly MongoClient client = new MongoClient(connectionString);

        // Nombre de la base de datos
        private static readonly IMongoDatabase db = client.GetDatabase("BD_Notas");

        // Colecciones
        public static IMongoCollection<Usuario> Usuarios = db.GetCollection<Usuario>("usuarios");
        public static IMongoCollection<Nota> Notas = db.GetCollection<Nota>("notas");
    }
}