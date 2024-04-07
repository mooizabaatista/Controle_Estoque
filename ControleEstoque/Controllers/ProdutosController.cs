using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace ControleEstoque.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos.ToListAsync();
            return View(produtos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Produto produto)
        {
            ViewBag.Error = null;

            var produtos = await _context.Produtos.AsNoTracking().ToListAsync();

            if (produtos.Any(x => RemoveAccents(x.Nome.Trim().ToLower()) == RemoveAccents(produto.Nome.Trim().ToLower())))
            {
                ViewBag.Error = true;
                return View();
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            return View(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Produto produto, string senha = "")
        {
            ViewBag.ErrorSenha = null;

            if(senha.Trim().ToLower() == "danilo1986")
            {
                _context.Produtos.Update(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            } else
            {
                ViewBag.ErrorSenha = true;
                return View(produto);
            }
        }

        public async Task<IActionResult> Remove(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            return View(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id, string senha = "")
        {
            ViewBag.Error = null;

            var produto = await _context.Produtos.FindAsync(id);

            if (senha.Trim().ToLower() == "danilo1986")
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = true;
                return View(produto);
            }
        }


        private string RemoveAccents(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().ToLower();
        }


    }
}
