using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Controllers
{
    public class EstoqueController : Controller
    {
        private readonly AppDbContext _context;

        public EstoqueController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var estoque = await _context.Estoque.Include(x => x.Produto).ToListAsync();

            return View(estoque);
        }

        public async Task<IActionResult> Create()
        {
            var produtos = await _context.Produtos
                               .Where(p => !_context.Estoque.Any(e => e.ProdutoId == p.Id))
                               .OrderBy(p => p.Nome)
                               .ToListAsync();

            ViewBag.Produtos = produtos.Select(x => new SelectListItem
            {
                Text = x.Nome,
                Value = x.Id.ToString()
            }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Estoque estoque)
        {
            estoque.EstoqueFinal = estoque.QtdInicial;
            _context.Estoque.Add(estoque);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var estoque = await _context.Estoque.FirstOrDefaultAsync(x => x.Id.Equals(id));

            var itemEstoque = await _context.Estoque
                  .Include(x => x.Produto)
                  .Where(x => x.Id == estoque.Id)
                  .FirstOrDefaultAsync();

            ViewBag.Produto = itemEstoque.Produto;

            return View(estoque);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Estoque estoque, string senha = "")
        {
            ViewBag.Error = null;
            ViewBag.ErrorSenha = null;

            if (senha.Trim().ToLower() == "danilo1986")
            {
                var movimentacoes = await _context.Movimentacoes.Where(x => x.ProdutoId == estoque.ProdutoId).ToListAsync();

                if (movimentacoes.Any())
                {
                    foreach (var movimentacao in movimentacoes)
                    {
                        if (movimentacao.TipoMovimentacao.ToUpper() == "ENTRADA")
                        {
                            estoque.QtdEntrada = movimentacao.Quantidade;
                            estoque.EstoqueFinal += movimentacao.Quantidade;
                        }

                        if (movimentacao.TipoMovimentacao.ToUpper() == "SAÍDA")
                        {
                            estoque.QtdSaida = movimentacao.Quantidade;
                            estoque.EstoqueFinal -= movimentacao.Quantidade;
                        }
                    }
                }

                estoque.EstoqueFinal += estoque.QtdInicial;

                _context.Estoque.Update(estoque);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                var itemEstoque = await _context.Estoque
                  .Include(x => x.Produto)
                  .Where(x => x.Id == estoque.Id)
                  .FirstOrDefaultAsync();

                ViewBag.Produto = itemEstoque.Produto;

                ViewBag.ErrorSenha = true;
                return View(estoque);
            }
        }

        public async Task<IActionResult> Remove(int id)
        {
            var itemEstoque = await _context.Estoque.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return View(itemEstoque);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id, string senha = "")
        {
            ViewBag.Error = null;

            var itemEstoque = await _context.Estoque.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            var itensEmMovimentacao = await _context.Movimentacoes.Where(x => x.ProdutoId == itemEstoque.ProdutoId).ToListAsync();

            if (senha.Trim().ToLower() == "danilo1986")
            {
                _context.Movimentacoes.RemoveRange(itensEmMovimentacao);
                _context.Estoque.Remove(itemEstoque);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = true;
                return View(itemEstoque);
            }
        }
    }
}
