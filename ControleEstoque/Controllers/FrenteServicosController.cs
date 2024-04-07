using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Controllers
{
    public class FrenteServicosController : Controller
    {
        private readonly AppDbContext _context;

        public FrenteServicosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var frenteServicos = await _context.FrenteServicos.ToListAsync();
            return View(frenteServicos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FrenteServico frenteServico)
        {
            _context.FrenteServicos.Add(frenteServico);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var frenteServico = await _context.FrenteServicos.FindAsync(id);
            return View(frenteServico);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FrenteServico frenteServico, string senha = "")
        {
            ViewBag.ErrorSenha = null;

            if (senha.Trim().ToLower() == "danilo1986")
            {
                _context.FrenteServicos.Update(frenteServico);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorSenha = true;
                return View(frenteServico);
            }
        }

        public async Task<IActionResult> Remove(int id)
        {
            var frenteServico = await _context.FrenteServicos.FindAsync(id);
            return View(frenteServico);
        }


        [HttpPost]
        public async Task<IActionResult> Remove(int id, string senha = "")
        {

            ViewBag.Error = null;

            var frenteServico = await _context.FrenteServicos.FindAsync(id);

            if (senha.Trim().ToLower() == "danilo1986")
            {
                _context.FrenteServicos.Remove(frenteServico);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = true;
                return View(frenteServico);
            }
        }
    }
}
