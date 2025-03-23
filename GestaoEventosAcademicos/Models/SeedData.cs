using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestaoEventosAcademicos.Models;
using System;

public static class SeedData
{
    public static void Initialize(ModelBuilder modelBuilder)
    {
        // Criar cursos
        var cursos = new List<Curso>
        {
            new Curso { CursoID = 1, Nome = "Engenharia de Software" },
            new Curso { CursoID = 2, Nome = "Ciência da Computação" },
            new Curso { CursoID = 3, Nome = "Administração" },
            new Curso { CursoID = 4, Nome = "Direito" },
            new Curso { CursoID = 5, Nome = "Medicina" }
        };

        modelBuilder.Entity<Curso>().HasData(cursos);

        // Criar administradores
        var administradores = new List<Administrador>
        {
            new Administrador { Id = Guid.NewGuid().ToString(), Nome = "Admin 1", Email = "admin1@email.com", UserName = "admin1@email.com", DataNascimento = new DateTime(1980, 5, 20), TipoUsuario = "Administrador", Telefone = "111111111" },
            new Administrador { Id = Guid.NewGuid().ToString(), Nome = "Admin 2", Email = "admin2@email.com", UserName = "admin2@email.com", DataNascimento = new DateTime(1975, 8, 15), TipoUsuario = "Administrador", Telefone = "222222222" },
            new Administrador { Id = Guid.NewGuid().ToString(), Nome = "Admin 3", Email = "admin3@email.com", UserName = "admin3@email.com", DataNascimento = new DateTime(1988, 10, 5), TipoUsuario = "Administrador", Telefone = "333333333" }
        };

        modelBuilder.Entity<Administrador>().HasData(administradores);

        // Criar participantes
        var participantes = new List<Participante>();
        for (int i = 1; i <= 10; i++)
        {
            participantes.Add(new Participante
            {
                Id = Guid.NewGuid().ToString(),
                Nome = $"Participante {i}",
                Email = $"participante{i}@email.com",
                UserName = $"participante{i}@email.com",
                DataNascimento = new DateTime(1995, i, i + 10),
                TipoUsuario = "Participante",
                Telefone = $"90000000{i}",
                CursoID = (i % 5) + 1 // Distribui entre os 5 cursos
            });
        }

        modelBuilder.Entity<Participante>().HasData(participantes);

        // Criar eventos
        var eventos = new List<Evento>();
        for (int i = 1; i <= 15; i++)
        {
            eventos.Add(new Evento
            {
                EventoID = i,
                Nome = $"Evento {i}",
                Local = $"Auditório {i}",
                Data = DateTime.Now.AddDays(i),
                Duração = $"{i} horas",
                CodigoParticipacao = $"EVT{i * 100}",
                Capacidade = 100,
                AdministradorID = administradores[i % 3].Id // Distribui entre os 3 administradores
            });
        }

        modelBuilder.Entity<Evento>().HasData(eventos);
    }
    public static async Task SeedUsers(UserManager<Usuario> userManager)
    {
        // Criar lista de administradores e participantes diretamente na memória
        var administradores = userManager.Users.OfType<Administrador>().ToList();
        var participantes = userManager.Users.OfType<Participante>().ToList();

        // Criar contas para administradores
        foreach (var admin in administradores)
        {
            var existe = await userManager.FindByEmailAsync(admin.Email);
            if (existe == null)
            {
                await userManager.CreateAsync(admin, "@Adm123");
            }
        }

        // Criar contas para participantes
        foreach (var participante in participantes)
        {
            var existe = await userManager.FindByEmailAsync(participante.Email);
            if (existe == null)
            {
                await userManager.CreateAsync(participante, "@User123");
            }
        }
    }

}
