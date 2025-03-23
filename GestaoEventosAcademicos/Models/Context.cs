using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoEventosAcademicos.Models
{
    public class Context : IdentityDbContext<Usuario>
    {
        /*o método contrutor usa o objeto options da superclasse
         para buscar as configurações de conexão com o BD */
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }
        /*Para que os modelos sejam mapeados como tabelas no BD,
         declare-os como DbSet*/
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Certificado> Certificados { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<Inscricao> Inscricoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction; // Ou DeleteBehavior.SetNull
            }
            SeedData.Initialize(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

    }
}
