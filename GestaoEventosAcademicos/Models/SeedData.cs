using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestaoEventosAcademicos.Models;

public class SeedData
{
    public static async Task EnsurePopulated(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

        context.Database.Migrate();


        string[] roles = { "Administrador", "Participante" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }


        if (!context.Cursos.Any())
        {
            var cursos = new List<Curso>
            {
                new Curso { Nome = "Ciência da Computação" },
                new Curso { Nome = "Engenharia de Software" },
                new Curso { Nome = "Administração" },
                new Curso { Nome = "Direito" },
                new Curso { Nome = "Medicina" }
            };

            context.Cursos.AddRange(cursos);
            await context.SaveChangesAsync();
        }


        if (!userManager.Users.Any(u => u.TipoUsuario == "Administrador"))
        {
            var admin1 = new Administrador
            {
                UserName = "clintonnogueirasilva@gmail.com",
                Email = "clintonnogueirasilva@gmail.com",
                EmailConfirmed = true,
                Nome = "Clinton Nogueira Silva",
                Telefone = "3599877-9281",
                TipoUsuario = "Administrador",
                DataNascimento = new DateTime(1990, 1, 1),
            };
            await userManager.CreateAsync(admin1, "@Clinton123");
            await userManager.AddToRoleAsync(admin1, "Administrador");

        }
        if (!userManager.Users.Any(u => u.TipoUsuario == "Participante"))
        {
            var curso = context.Cursos.First();

            var participante1 = new Participante
            {
                UserName = "luizbaldini@gmail.com",
                Email = "luizbaldini@gmail.com",
                EmailConfirmed = true,
                Nome = "Luiz Felipe ",
                Telefone = "35977777777",
                TipoUsuario = "Participante",
                DataNascimento = new DateTime(2002, 10, 15),
                CursoID = curso.CursoID
            };
            await userManager.CreateAsync(participante1, "@Luiz123");
            await userManager.AddToRoleAsync(participante1, "Participante");
        }


        if (!context.Eventos.Any())
        {
            var administrador = userManager.Users.OfType<Administrador>().FirstOrDefault();
            if (administrador != null)
            {
                var eventos = new List<Evento>
                {
                    new Evento { Nome = "Palestra sobre IA", Local = "Auditório 1", Data = DateTime.Now.AddDays(10), Duração = "2h", CodigoParticipacao = "AI2025", Capacidade = 100, AdministradorID = administrador.Id },
                    new Evento { Nome = "Workshop de Programação", Local = "Sala 101", Data = DateTime.Now.AddDays(15), Duração = "3h", CodigoParticipacao = "DEV2025", Capacidade = 50, AdministradorID = administrador.Id }
                };

                context.Eventos.AddRange(eventos);
                await context.SaveChangesAsync();
            }
        }
    }
}
