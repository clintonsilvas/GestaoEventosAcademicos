using GestaoEventosAcademicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoEventosAcademicos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CursoController : Controller
    {
        public Context context;
        public CursoController(Context ctx)
        {
            context = ctx;
        }
        public IActionResult Index()
        {            
            var cursos = context.Cursos.Include(c => c.Participantes);
            return View(cursos);
        }
        public IActionResult Create()
        {            
            return View();
        }

        [HttpPost]
        public IActionResult Create(Curso curso)
        {
            context.Add(curso);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
         public IActionResult Edit(int id)
        {
            var curso = context.Cursos.Find(id);
            if (curso == null)
            {
                return NotFound();
            }
            
            return View(curso);
        }

        [HttpPost]
        public IActionResult Edit(Curso curso)
        {
            //avisa a EF que o registro será modificado
            context.Entry(curso).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var curso = context.Cursos.Find(id);

            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        [HttpPost]
        public IActionResult Delete(Curso curso)
        {
            context.Cursos.Remove(curso);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult AlunosCurso(int id)
        {
            var curso = context.Cursos
                                .Include(c => c.Participantes)                                  
                                .FirstOrDefault(c => c.CursoID == id);           

            return View(curso);
        }

    }
}
