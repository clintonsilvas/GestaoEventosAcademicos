using Microsoft.EntityFrameworkCore;
namespace GestaoEventosAcademicos.Models
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            //associa os dados ao contexto
            Context context = app.ApplicationServices.GetRequiredService<Context>();
            //inserir os dados nas entidades do contexto
            context.Database.Migrate();
            //se o contexto estiver vazio
            if (!context.Eventos.Any())
            //inserir os produtos iniciais
            {
                //context.Administradores.AddRange(
                //    new Administrador { Nome = "Clinton Nogueira", Email = "clintonnogueirasilva@gmail.com", Senha = "123", Telefone = "35998779281" });

                context.Eventos.AddRange(
                    new Evento
                    {
                        Nome = "Join2025",
                        Local = "Unifenas",
                        Data = new DateTime(2025, 6, 15),
                        Duração = "4 horas",
                        CodigoParticipacao = "JOIN2025",
                        Capacidade = 200,
                        AdministradorID = 1
                    },
                    new Evento
                    {
                        Nome = "TechSummit",
                        Local = "São Paulo Expo",
                        Data = new DateTime(2025, 9, 10),
                        Duração = "8 horas",
                        CodigoParticipacao = "TECH2025",
                        Capacidade = 500,
                        AdministradorID = 1

                    });
                context.SaveChanges();
            }
        }
    }
}
