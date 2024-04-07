using ControleEstoque.Data;
using ControleEstoque.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.Controllers
{
    public class MovimentacoesController : Controller
    {
        private readonly AppDbContext _context;

        public MovimentacoesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movimentacoes = await _context.Movimentacoes.Include(x => x.Produto).Include(x => x.FrenteServico).ToListAsync();
            return View(movimentacoes);
        }

        public async Task<IActionResult> Create()
        {
            var produtos = await _context.Produtos
                               .Where(p => _context.Estoque.Any(e => e.ProdutoId == p.Id))
                               .OrderBy(p => p.Nome)
                               .ToListAsync();

            var frenteServicos = await _context.FrenteServicos
                               .OrderBy(p => p.Nome)
                               .ToListAsync();

            ViewBag.Produtos = produtos.Select(x => new SelectListItem
            {
                Text = x.Nome,
                Value = x.Id.ToString()
            }).ToList();

            ViewBag.FrenteServicos = frenteServicos.Select(x => new SelectListItem
            {
                Text = x.Nome,
                Value = x.Id.ToString()
            }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movimentacao movimentacao)
        {
            var itemDoEstoque = await _context.Estoque.FirstOrDefaultAsync(x => x.ProdutoId == movimentacao.ProdutoId);

            if (movimentacao.TipoMovimentacao.ToUpper() == "ENTRADA")
            {
                itemDoEstoque.QtdEntrada += movimentacao.Quantidade;
            }
            else if (movimentacao.TipoMovimentacao.ToUpper() == "SAÍDA")
            {
                if (movimentacao.Quantidade > (itemDoEstoque.QtdInicial + itemDoEstoque.QtdEntrada) - itemDoEstoque.QtdSaida)
                {
                    ViewBag.Error = true;

                    var produtos = await _context.Produtos
                               .Where(p => _context.Estoque.Any(e => e.ProdutoId == p.Id))
                               .OrderBy(p => p.Nome)
                               .ToListAsync();

                    var frenteServicos = await _context.FrenteServicos
                               .OrderBy(p => p.Nome)
                               .ToListAsync();

                    ViewBag.Produtos = produtos.Select(x => new SelectListItem
                    {
                        Text = x.Nome,
                        Value = x.Id.ToString()
                    }).ToList();

                    ViewBag.FrenteServicos = frenteServicos.Select(x => new SelectListItem
                    {
                        Text = x.Nome,
                        Value = x.Id.ToString()
                    }).ToList();

                    return View(movimentacao);
                }

                itemDoEstoque.QtdSaida += movimentacao.Quantidade;
            }

            itemDoEstoque.EstoqueFinal = (itemDoEstoque.QtdInicial + itemDoEstoque.QtdEntrada) - itemDoEstoque.QtdSaida;

            // Atualizar o objeto Estoque no contexto
            _context.Estoque.Update(itemDoEstoque);

            _context.Movimentacoes.Add(movimentacao);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var produtos = await _context.Produtos
                               .Where(p => _context.Estoque.Any(e => e.ProdutoId == p.Id))
                               .OrderBy(p => p.Nome)
                               .ToListAsync();

            var frenteServicos = await _context.FrenteServicos
                       .OrderBy(p => p.Nome)
                       .ToListAsync();

            ViewBag.Produtos = produtos.Select(x => new SelectListItem
            {
                Text = x.Nome,
                Value = x.Id.ToString()
            }).ToList();

            ViewBag.FrenteServicos = frenteServicos.Select(x => new SelectListItem
            {
                Text = x.Nome,
                Value = x.Id.ToString()
            }).ToList();

            var movimentacao = await _context.Movimentacoes.FindAsync(id);
            return View(movimentacao);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movimentacao movimentacao, string senha = "")
        {
            ViewBag.ErrorSenha = null;

            if (senha.Trim().ToLower() == "danilo1986")
            {
                var itemDoEstoque = await _context.Estoque.AsNoTracking().FirstOrDefaultAsync(x => x.ProdutoId == movimentacao.ProdutoId);

                var movimentacaoExistente = await _context.Movimentacoes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == movimentacao.Id);

                if (movimentacaoExistente != null)
                {
                    _context.Movimentacoes.Remove(movimentacaoExistente);
                }

                if (movimentacao.TipoMovimentacao.ToUpper() == "SAÍDA" && movimentacao.Quantidade > (itemDoEstoque.QtdInicial + itemDoEstoque.QtdEntrada) - itemDoEstoque.QtdSaida + movimentacaoExistente.Quantidade)
                {
                    ViewBag.Error = true;

                    var produtos = await _context.Produtos
                                     .Where(p => _context.Estoque.Any(e => e.ProdutoId == p.Id))
                                     .OrderBy(p => p.Nome)
                                     .ToListAsync();

                    var frenteServicos = await _context.FrenteServicos
                               .OrderBy(p => p.Nome)
                               .ToListAsync();

                    ViewBag.Produtos = produtos.Select(x => new SelectListItem
                    {
                        Text = x.Nome,
                        Value = x.Id.ToString()
                    }).ToList();

                    ViewBag.FrenteServicos = frenteServicos.Select(x => new SelectListItem
                    {
                        Text = x.Nome,
                        Value = x.Id.ToString()
                    }).ToList();

                    return View(movimentacao);
                }

                _context.Movimentacoes.Add(movimentacao);

                await _context.SaveChangesAsync();

                var movimentacoes = await _context.Movimentacoes.AsNoTracking().Where(m => m.ProdutoId == movimentacao.ProdutoId).ToListAsync();

                var totalEntrada = movimentacoes.Where(m => m.TipoMovimentacao.ToUpper() == "ENTRADA").Sum(m => m.Quantidade);
                var totalSaida = movimentacoes.Where(m => m.TipoMovimentacao.ToUpper() == "SAÍDA").Sum(m => m.Quantidade);

                itemDoEstoque.QtdEntrada = totalEntrada;
                itemDoEstoque.QtdSaida = totalSaida;
                itemDoEstoque.EstoqueFinal = itemDoEstoque.QtdInicial + itemDoEstoque.QtdEntrada - itemDoEstoque.QtdSaida;

                _context.Estoque.Update(itemDoEstoque);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                var produtos = await _context.Produtos
                                    .Where(p => _context.Estoque.Any(e => e.ProdutoId == p.Id))
                                    .OrderBy(p => p.Nome)
                                    .ToListAsync();

                var frenteServicos = await _context.FrenteServicos
                           .OrderBy(p => p.Nome)
                           .ToListAsync();

                ViewBag.Produtos = produtos.Select(x => new SelectListItem
                {
                    Text = x.Nome,
                    Value = x.Id.ToString()
                }).ToList();

                ViewBag.FrenteServicos = frenteServicos.Select(x => new SelectListItem
                {
                    Text = x.Nome,
                    Value = x.Id.ToString()
                }).ToList();
                ViewBag.ErrorSenha = true;
                return View(movimentacao);
            }
        }

        public async Task<IActionResult> Remove(int id)
        {
            var movimentacao = await _context.Movimentacoes.FindAsync(id);
            return View(movimentacao);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id, string senha = "")
        {
            var movimentacao = await _context.Movimentacoes.FindAsync(id);

            ViewBag.Error = true;

            if (senha.Trim().ToLower() == "danilo1986")
            {
                // Verifica se a movimentação existe
                if (movimentacao == null)
                {
                    return NotFound();
                }

                // Recupera o item de estoque relacionado à movimentação
                var itemDoEstoque = await _context.Estoque.FirstOrDefaultAsync(x => x.ProdutoId == movimentacao.ProdutoId);

                if (itemDoEstoque == null)
                {
                    _context.Movimentacoes.Remove(movimentacao);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Verifica o tipo de movimentação
                    if (movimentacao.TipoMovimentacao.ToUpper() == "ENTRADA")
                    {
                        // Reduz a quantidade de entrada do estoque
                        itemDoEstoque.QtdEntrada -= movimentacao.Quantidade;
                    }
                    else if (movimentacao.TipoMovimentacao.ToUpper() == "SAÍDA")
                    {
                        // Reduz a quantidade de saída do estoque
                        itemDoEstoque.QtdSaida -= movimentacao.Quantidade;
                    }

                    // Atualiza o estoque final
                    itemDoEstoque.EstoqueFinal = itemDoEstoque.QtdInicial + itemDoEstoque.QtdEntrada - itemDoEstoque.QtdSaida;

                    _context.Estoque.Update(itemDoEstoque);

                    // Remove a movimentação do contexto
                    _context.Movimentacoes.Remove(movimentacao);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View(movimentacao);
            }
        }
    }
}
