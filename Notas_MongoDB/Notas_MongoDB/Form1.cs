using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;

namespace Notas_MongoDB
{
    public partial class Form1 : Form
    {
        // Guardamos los TextBox globales para acceder desde otros métodos
        TextBox txtUsuario;
        TextBox txtPass;
        TextBox txtTitulo;
        TextBox txtContenido;
        TextBox txtTags;
        DataGridView dgvNotas; // DataGridView para mostrar las notas (recomendación del profe)

        string usuarioGlobal = "";
        string passGlobal = "";
        string notaIdActual = ""; 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CrearGroupBoxUsuario();
        }

        private void CrearGroupBoxUsuario()
        {
            // Crear GroupBox
            GroupBox group = new GroupBox();
            group.Text = "Iniciar o crear usuario";
            group.Size = new Size(300, 125);
            group.Location = new Point(20, 20);

            // Label Usuario
            Label lblU = new Label();
            lblU.Text = "Usuario:";
            lblU.Location = new Point(10, 25);
            lblU.Size = new Size(80, 20);

            // TextBox Usuario
            txtUsuario = new TextBox();
            txtUsuario.Location = new Point(125, 22);
            txtUsuario.Size = new Size(150, 20);

            // Label Contraseña
            Label lblP = new Label();
            lblP.Text = "Clave:";
            lblP.Location = new Point(10, 60);
            lblP.Size = new Size(80, 25);

            // TextBox Contraseña
            txtPass = new TextBox();
            txtPass.Location = new Point(125, 58);
            txtPass.Size = new Size(150, 20);
            txtPass.PasswordChar = '*';

            // Crear un botón para crear usuario
            Button btnCrear = new Button();
            btnCrear.Text = "Crear";
            btnCrear.Size = new Size(100, 30);
            btnCrear.Location = new Point(10, 90);
            btnCrear.Click += BtnCrear_Click;

            //Crear un botón para iniciar sesión
            Button btnIniciar = new Button();
            btnIniciar.Text = "Iniciar";
            btnIniciar.Size = new Size(100, 30);
            btnIniciar.Location = new Point(125, 90);
            btnIniciar.Click += BtnIniciar_Click;


            // Agregarlos al GroupBox
            group.Controls.Add(lblU);
            group.Controls.Add(txtUsuario);
            group.Controls.Add(lblP);
            group.Controls.Add(txtPass);
            group.Controls.Add(btnCrear);
            group.Controls.Add(btnIniciar);

            // Agregar al Form
            this.Controls.Add(group);
        }

        private void NotaActual()
        {
            // Crear GroupBox
            GroupBox group = new GroupBox();
            group.Text = "Nota";
            group.Size = new Size(450, 360);
            group.Location = new Point(600, 20);

            // Label Título
            Label lblT = new Label();
            lblT.Text = "Título:";
            lblT.Location = new Point(10, 25);
            lblT.Size = new Size(80, 20);

            // TextBox Título
            txtTitulo = new TextBox();
            txtTitulo.Location = new Point(100, 22);
            txtTitulo.Size = new Size(320, 20);

            // Label Contenido
            Label lblC = new Label();
            lblC.Text = "Contenido:";
            lblC.Location = new Point(10, 60);
            lblC.Size = new Size(80, 20);

            // TextBox Contenido (multilínea)
            txtContenido = new TextBox();
            txtContenido.Location = new Point(100, 57);
            txtContenido.Size = new Size(320, 170);
            txtContenido.Multiline = true;
            txtContenido.ScrollBars = ScrollBars.Vertical;

            // Label Tags
            Label lblTags = new Label();
            lblTags.Text = "Tags:";
            lblTags.Location = new Point(10, 240);
            lblTags.Size = new Size(80, 20);

            // TextBox Tags
            txtTags = new TextBox();
            txtTags.Location = new Point(100, 237);
            txtTags.Size = new Size(200, 20);

            // Botón Subir Nota
            Button btnSubirNota = new Button();
            btnSubirNota.Text = "Subir nota";
            btnSubirNota.Size = new Size(120, 35);
            btnSubirNota.Location = new Point(300, 265);
            btnSubirNota.Click += BtnSubirNota_Click;

            // Botn Eliminar Nota
            Button btnEliminarNota = new Button();
            btnEliminarNota.Text = "Eliminar nota";
            btnEliminarNota.Size = new Size(120, 35);
            btnEliminarNota.Location = new Point(160, 265);
            btnEliminarNota.Click += BtnEliminarNota_Click;

            // Botón Limpiar Nota
            Button btnLimpiarNota = new Button();
            btnLimpiarNota.Text = "Limpiar";
            btnLimpiarNota.Size = new Size(120, 35);
            btnLimpiarNota.Location = new Point(20, 265);
            btnLimpiarNota.Click += BtnLimpiarNota_Click;

            // Botón Crear TXT
            Button btnCrearTxt = new Button();
            btnCrearTxt.Text = "Guardar TXT";
            btnCrearTxt.Size = new Size(120, 35);
            btnCrearTxt.Location = new Point(160, 305);
            btnCrearTxt.Click += BtnCrearTxt_Click;

            // Agregar controles al GroupBox
            group.Controls.Add(lblT);
            group.Controls.Add(txtTitulo);
            group.Controls.Add(lblC);
            group.Controls.Add(txtContenido);
            group.Controls.Add(lblTags);
            group.Controls.Add(txtTags);
            group.Controls.Add(btnSubirNota);
            group.Controls.Add(btnEliminarNota);
            group.Controls.Add(btnLimpiarNota);
            group.Controls.Add(btnCrearTxt);

            // Agregar al Form
            this.Controls.Add(group);
        }

        private void BtnSubirNota_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioGlobal))
                {
                    MessageBox.Show("Primero debes iniciar sesión.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTitulo.Text))
                {
                    MessageBox.Show("El título no puede estar vacío.");
                    return;
                }

                // Obtener el usuario actual
                var filtroUsuario = Builders<Usuario>.Filter.Eq(u => u.Nombre, usuarioGlobal);
                var usuario = ConectMongo.Usuarios.Find(filtroUsuario).FirstOrDefault();

                if (usuario == null)
                {
                    MessageBox.Show("Usuario no encontrado :(");
                    return;
                }

                // Si hay un ID actual, intentar actualizar esa nota
                if (!string.IsNullOrEmpty(notaIdActual) && ObjectId.TryParse(notaIdActual, out ObjectId idNota))
                {
                    var filtroNota = Builders<Nota>.Filter.Eq(n => n.Id, idNota);
                    var notaExistente = ConectMongo.Notas.Find(filtroNota).FirstOrDefault();

                    if (notaExistente != null)
                    {
                        // Actualizar nota existente
                        var actualizacion = Builders<Nota>.Update
                            .Set(n => n.Titulo, txtTitulo.Text)
                            .Set(n => n.Contenido, txtContenido.Text)
                            .Set(n => n.FechaCreacion, DateTime.Now)
                            .Set(n => n.Tags, txtTags.Text);

                        ConectMongo.Notas.UpdateOne(filtroNota, actualizacion);
                        MessageBox.Show("Nota actualizada");
                    }
                    else
                    {
                        // Si el ID no existe en BD, crear una nueva
                        var nuevaNota = new Nota
                        {
                            UsuarioId = usuario.Id.ToString(),
                            Titulo = txtTitulo.Text,
                            Contenido = txtContenido.Text,
                            FechaCreacion = DateTime.Now,
                            Tags = txtTags.Text
                        };

                        ConectMongo.Notas.InsertOne(nuevaNota);
                        MessageBox.Show("Nueva nota creada: " + txtTitulo.Text);
                    }
                }
                else
                {
                    // Crear nueva nota (si no hay ID guardado)
                    CrearNota(
                        usuario.Id.ToString(),
                        txtTitulo.Text,
                        txtContenido.Text,
                        txtTags.Text);
                }

                // Refrescar la tabla y limpiar el ID actual
                MostrarNotasUsuario(usuario.Id.ToString());
                BtnLimpiarNota_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al subir la nota: " + ex.Message);
            }
        }

        private void BtnEliminarNota_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioGlobal))
                {
                    MessageBox.Show("Debes iniciar sesión antes de eliminar notas.");
                    return;
                }

                if (string.IsNullOrEmpty(notaIdActual))
                {
                    MessageBox.Show("Selecciona una nota para eliminarla.");
                    return;
                }

                var confirmacion = MessageBox.Show(
                    "¿Seguro que quieres eliminar esta nota?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.No)
                    return;

                // Llamamos a la función existente
                EliminarNota(notaIdActual);

                // Limpiar campos de nota
                notaIdActual = "";
                txtTitulo.Clear();
                txtContenido.Clear();
                txtTags.Clear();

                // Refrescar el DataGridView
                var filtroUsuario = Builders<Usuario>.Filter.Eq(u => u.Nombre, usuarioGlobal);
                var usuario = ConectMongo.Usuarios.Find(filtroUsuario).FirstOrDefault();
                if (usuario != null)
                {
                    MostrarNotasUsuario(usuario.Id.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar la nota: " + ex.Message);
            }
        }

        private void BtnLimpiarNota_Click(object sender, EventArgs e)
        {
            txtTitulo.Clear();
            txtContenido.Clear();
            txtTags.Clear();
            notaIdActual = "";
        }

        private void BtnCrear_Click(object sender, EventArgs e)
        {
            usuarioGlobal = txtUsuario.Text;
            passGlobal = txtPass.Text;
            CrearUsuario(usuarioGlobal, passGlobal);
        }

        private void BtnIniciar_Click(object sender, EventArgs e)
        {
            usuarioGlobal = txtUsuario.Text;
            passGlobal = txtPass.Text;
            IniciarSesion(usuarioGlobal, passGlobal);
        }

        private void MostrarNotasUsuario(string usuarioId)
        {
            try
            {
                // Buscar todas las notas del usuario en la BD
                var filtroNotas = Builders<Nota>.Filter.Eq(n => n.UsuarioId, usuarioId);
                var notasUsuario = ConectMongo.Notas.Find(filtroNotas).ToList();

                // Buscar el nombre del usuario
                var filtroUsuario = Builders<Usuario>.Filter.Eq(u => u.Id, ObjectId.Parse(usuarioId));
                var usuario = ConectMongo.Usuarios.Find(filtroUsuario).FirstOrDefault();

                // Si no existe el DataGridView, lo creamos una vez
                if (dgvNotas == null)
                {
                    dgvNotas = new DataGridView();
                    dgvNotas.Location = new Point(20, 160);
                    dgvNotas.Size = new Size(550, 200);
                    dgvNotas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvNotas.ReadOnly = true;
                    dgvNotas.AllowUserToAddRows = false;

                    // Asignar el evento CellClick
                    dgvNotas.CellClick += dgvNotas_CellClick;

                    this.Controls.Add(dgvNotas);
                }

                var datosFiltrados = notasUsuario.Select(n => new
                {
                    ID_Nota = n.Id.ToString(), 
                    ContenidoCompleto = n.Contenido,

                    Usuario = usuario != null ? usuario.Nombre : "(Desconocido)",
                    Título = n.Titulo,
                    Fecha = n.FechaCreacion.ToString("yyyy-MM-dd HH:mm"),
                    Tags = n.Tags
                }).ToList();

                // Asignar los datos al DataGridView
                dgvNotas.DataSource = datosFiltrados;

                // Ocultar las columnas internas
                if (dgvNotas.Columns.Contains("ID_Nota"))
                {
                    dgvNotas.Columns["ID_Nota"].Visible = false; // No se mostrará el ID (El usuario no lo necesita)
                }
                if (dgvNotas.Columns.Contains("ContenidoCompleto"))
                {
                    dgvNotas.Columns["ContenidoCompleto"].Visible = false;
                }
                CrearBuscadorNotas();
                dgvNotas.Refresh();

                if (notasUsuario.Count == 0)
                {
                    MessageBox.Show("Este usuario no tiene notas registradas.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las notas: " + ex.Message);
            }
        }

        private void dgvNotas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar que se haya hecho clic en una fila (no en la parte de arriba)
            if (e.RowIndex < 0 || dgvNotas.Rows.Count == 0)
            {
                return;
            }

            try
            {
                // Obtener la fila seleccionada
                DataGridViewRow filaSeleccionada = dgvNotas.Rows[e.RowIndex];

                // Leer los valores de las celdas
                string titulo = filaSeleccionada.Cells["Título"].Value.ToString();
                string contenido = filaSeleccionada.Cells["ContenidoCompleto"].Value.ToString(); 
                string tags = filaSeleccionada.Cells["Tags"].Value.ToString();

                // Cargar los datos en los TextBoxes de NotaActual
                txtTitulo.Text = titulo;
                txtContenido.Text = contenido;
                txtTags.Text = tags;
                notaIdActual = filaSeleccionada.Cells["ID_Nota"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la nota seleccionada: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrearBuscadorNotas()
        {
            // Label del buscador
            Label lblBuscar = new Label();
            lblBuscar.Text = "Buscar título:";
            lblBuscar.Location = new Point(20, 400);
            lblBuscar.Size = new Size(80, 20);

            // TextBox para ingresar texto de búsqueda
            TextBox txtBuscar = new TextBox();
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Location = new Point(110, 400);
            txtBuscar.Size = new Size(300, 20);

            // Botón para realizar la búsqueda
            Button btnBuscar = new Button();
            btnBuscar.Text = "Buscar";
            btnBuscar.Size = new Size(100, 25);
            btnBuscar.Location = new Point(420, 400);

            // Evento click del botón
            btnBuscar.Click += (s, e) =>
            {

                var textoBusqueda = txtBuscar.Text.Trim();

                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    // Si no se escribió nada, mostrar todas las noas del usuario
                    var filtroUsuario = Builders<Usuario>.Filter.Eq(u => u.Nombre, usuarioGlobal);
                    var usuario = ConectMongo.Usuarios.Find(filtroUsuario).FirstOrDefault();
                    if (usuario != null)
                        MostrarNotasUsuario(usuario.Id.ToString());
                    return;
                }

                try
                {
                    var filtroUsuario = Builders<Usuario>.Filter.Eq(u => u.Nombre, usuarioGlobal);
                    var usuario = ConectMongo.Usuarios.Find(filtroUsuario).FirstOrDefault();

                    if (usuario == null)
                    {
                        MessageBox.Show("Usuario no encontrado.");
                        return;
                    }

                    // Buscar por título
                    var filtroNotas = Builders<Nota>.Filter.And(
                        Builders<Nota>.Filter.Eq(n => n.UsuarioId, usuario.Id.ToString()),
                        Builders<Nota>.Filter.Regex(n => n.Titulo, new BsonRegularExpression(textoBusqueda, "i"))
                    );

                    var notasFiltradas = ConectMongo.Notas.Find(filtroNotas).ToList();

                    // Crear lista de datos para mostrar
                    var datosFiltrados = notasFiltradas.Select(n => new
                    {
                        ID_Nota = n.Id.ToString(),
                        ContenidoCompleto = n.Contenido,
                        Usuario = usuario.Nombre,
                        Título = n.Titulo,
                        Fecha = n.FechaCreacion.ToString("yyyy-MM-dd HH:mm"),
                        Tags = n.Tags
                    }).ToList();

                    dgvNotas.DataSource = datosFiltrados;

                    if (dgvNotas.Columns.Contains("ID_Nota"))
                        dgvNotas.Columns["ID_Nota"].Visible = false;
                    if (dgvNotas.Columns.Contains("ContenidoCompleto"))
                        dgvNotas.Columns["ContenidoCompleto"].Visible = false;

                    if (notasFiltradas.Count == 0)
                        MessageBox.Show("No se encontraron notas con ese título.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar notas: " + ex.Message);
                }
            };

            // Agregar al formulario
            this.Controls.Add(lblBuscar);
            this.Controls.Add(txtBuscar);
            this.Controls.Add(btnBuscar);
        }

        private void CrearUsuario(string usuario, string pass)
        {
            var existe = Builders<Usuario>.Filter.Eq(u => u.Nombre, usuarioGlobal);
            var usuarioExistente = ConectMongo.Usuarios.Find(existe).FirstOrDefaultAsync().Result;

            if (usuarioExistente != null)
            {
                MessageBox.Show("El nombre de usuario ya está en uso, elige otro.");
            }
            else
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(usuarioGlobal) || string.IsNullOrWhiteSpace(passGlobal))
                    {
                        MessageBox.Show("El nombre de usuario y la contraseña no pueden estar vacíos.");
                        return;
                    }

                    // Encriptar contraseña antes de guardar, se va a mandar la contraseña encriptada
                    string passEncriptada = EncriptarSHA256(passGlobal);

                    var nuevoUsuario = new Usuario
                    {
                        Nombre = usuarioGlobal,
                        Contraseania = passEncriptada
                    };

                    ConectMongo.Usuarios.InsertOneAsync(nuevoUsuario);
                    MessageBox.Show("Usuario creado exitosamente.");
                    IniciarSesion(usuarioGlobal, passGlobal);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al crear el usuario: " + ex.Message);
                }
            }
        }

        private void IniciarSesion(string usuario, string pass) //Se desencripta la contraseña de la BD y se utiliza como el texto que el usuario escribió
        {
            string passEncriptada = EncriptarSHA256(passGlobal);

            var filtro = Builders<Usuario>.Filter.Eq(u => u.Nombre, usuarioGlobal) &
                         Builders<Usuario>.Filter.Eq(u => u.Contraseania, passEncriptada);

            var usuarioEncontrado = ConectMongo.Usuarios.Find(filtro).FirstOrDefaultAsync().Result;

            if (usuarioEncontrado != null)
            {
                MessageBox.Show("Inicio de sesión exitoso.");
                MostrarNotasUsuario(usuarioEncontrado.Id.ToString());
                NotaActual();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.");
            }
        }

        private void CrearNota(string usuario, string titulo, string contenido, string tags)
        {
            try
            {
                var nota = new Nota
                {
                    UsuarioId = usuario,
                    Titulo = titulo,
                    Contenido = contenido,
                    FechaCreacion = DateTime.Now,
                    Tags = tags
                };
                ConectMongo.Notas.InsertOne(nota);
                MessageBox.Show("Nueva nota creada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la nota: " + ex.Message);

            }
        }

        private void EliminarNota(string notaId)
        {
            // Validar que el ID de la nota sea valido
            if (!ObjectId.TryParse(notaId, out ObjectId objectId))
            {
                MessageBox.Show("ID de nota inválido. Debe ser un ObjectId válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                var filtro = Builders<Nota>.Filter.Eq(n => n.Id, objectId);


                var resultado =  ConectMongo.Notas.DeleteOneAsync(filtro);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la nota: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCrearTxt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTitulo.Text))
                {
                    MessageBox.Show("No se puede crear un archivo sin título.");
                    return;
                }

                CrearTxt();
                MessageBox.Show("Archivo TXT creado en el escritorio.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear el archivo TXT: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrearTxt()
        {
            string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // Crear ruta completa del archivo
            string texto = txtContenido.Text;
            string tags = txtTags.Text;
            string escritor = txtUsuario.Text;
            string fecha = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string contenido = $"Escritor: {escritor}\nFecha: {fecha}\nTags: {tags}\n\n{texto}";

            string titulo = string.Join("_", txtTitulo.Text.Split(Path.GetInvalidFileNameChars()));
            string titulobueno = $"{titulo}_{escritor}_{fecha}";

            string rutaArchivo = Path.Combine(escritorio, titulobueno+".txt");
            // Crear o reemplazar el archivo
            File.WriteAllText(rutaArchivo, contenido);
        }

        private string EncriptarSHA256(string texto) // Para las contraseñas
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(texto);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

    }
}
