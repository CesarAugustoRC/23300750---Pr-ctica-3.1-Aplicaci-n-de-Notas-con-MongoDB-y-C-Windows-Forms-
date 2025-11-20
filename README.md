1. Para la instalación solo es necesario clonar el repositorio.
   {En el archivo ConectMongo.cs está la cadena de conexión con MongoDB Atlas, es necesario editarla con los datos del usuario y contraseña.
   También se puede reemplazar todo el string si se quiere utilizar otra BD}

2. Capturas de funcionamiento
<img width="921" height="424" alt="image" src="https://github.com/user-attachments/assets/b3714253-bbfd-47ee-afa5-10fd32bf3998" />
<img width="921" height="424" alt="image" src="https://github.com/user-attachments/assets/4a98d23b-60ed-4983-8abb-4a7dbc29cd93" />
<img width="921" height="424" alt="image" src="https://github.com/user-attachments/assets/075673cb-a63d-4ce5-bdad-bc785b5aa866" />
<img width="921" height="424" alt="image" src="https://github.com/user-attachments/assets/0eb3a890-c6e1-4c55-9b0a-91db2756381f" />
<img width="921" height="425" alt="image" src="https://github.com/user-attachments/assets/05f6b7ca-ae25-495f-bc79-305a42201d03" />
<img width="921" height="421" alt="image" src="https://github.com/user-attachments/assets/4e110c09-8b09-43ae-85c6-29df6507094d" />
<img width="921" height="653" alt="image" src="https://github.com/user-attachments/assets/7efd10b8-ff09-4d61-b3c9-3bef4155f7ad" />

3. Ejemplo de configuración para la cadena de conexión a MongoDB : mongodb+srv://<db_user>:<db_password>@clusternotas.embegqi.mongodb.net/?appName=ClusterNotas

Ejemplo con código (ConectMongo.cs): 

using MongoDB.Driver;

namespace Notas_MongoDB
{
    public static class ConectMongo
    {
        private static readonly string connectionString =
            $"mongodb+srv://<db_user>:<db_password>@clusternotas.embegqi.mongodb.net/?appName=ClusterNotas";
        // Ejemplo con usuario ya creado: mongodb+srv://a23300750_db_user:<db_password>@clusternotas.embegqi.mongodb.net/?appName=ClusterNotas
        private static readonly MongoClient client = new MongoClient(connectionString);

        // Nombre de la base de datos
        private static readonly IMongoDatabase db = client.GetDatabase("BD_Notas");

        // Colecciones
        public static IMongoCollection<Usuario> Usuarios = db.GetCollection<Usuario>("usuarios");
        public static IMongoCollection<Nota> Notas = db.GetCollection<Nota>("notas");
    }
}


