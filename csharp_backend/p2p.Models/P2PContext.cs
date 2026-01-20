using Microsoft.EntityFrameworkCore; 
namespace p2p.models
{
    /// <summary>
    /// Contexto de base de datos para la aplicación P2P.
    /// Hereda de DbContext y proporciona acceso a las entidades P2P en la base de datos SQLite.
    /// </summary>
    public class P2PContext : DbContext
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="P2PContext"/>.
        /// </summary>
        /// <param name="options">Las opciones de configuración del contexto DbContext.</param>
        public P2PContext(DbContextOptions<P2PContext> options) : base(options)
        {
        }

        /// <summary>
        /// Obtiene o establece el conjunto de entidades P2PItems en la base de datos.
        /// </summary>
        /// <value>
        /// Colección de objetos <see cref="P2PItems"/> que representan los dispositivos y elementos P2P.
        /// </value>
        public DbSet<P2PItems> P2PItems { get; set; } = null!;

        /// <summary>
        /// Configura el contexto de la base de datos.
        /// </summary>
        /// <param name="optionsBuilder">El constructor de opciones para configurar la conexión.</param>
        /// <remarks>
        /// Este método configura la conexión a SQLite y establece que las migraciones
        /// se encuentran en el ensamblado "p2p.api".
        /// </remarks>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DB.db",
                b => b.MigrationsAssembly("p2p.api")); // Your API project
        }
    }
}
