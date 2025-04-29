using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de endereços no sistema SoliGest.
    /// Fornece operações CRUD para a entidade Address.
    /// </summary>
    public class AddressesController : Controller
    {
        private readonly SoliGestServerContext _context;

        /// <summary>
        /// Construtor do controlador de endereços.
        /// </summary>
        /// <param name="context">Contexto da base de dados do servidor SoliGest.</param>
        public AddressesController(SoliGestServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de todos os endereços.
        /// </summary>
        /// <returns>Vista com a lista de endereços.</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Address.ToListAsync());
        }

        /// <summary>
        /// Obtém os detalhes de um endereço específico.
        /// </summary>
        /// <param name="id">ID do endereço.</param>
        /// <returns>Vista com os detalhes do endereço ou NotFound se não existir.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Address
                .FirstOrDefaultAsync(m => m.Id == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        /// <summary>
        /// Apresenta a vista para criação de um novo endereço.
        /// </summary>
        /// <returns>Vista de criação.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processa os dados submetidos para criar um novo endereço.
        /// </summary>
        /// <param name="address">Objeto Address preenchido pelo utilizador.</param>
        /// <returns>Redireciona para o Index se bem-sucedido, caso contrário retorna a mesma vista.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Line1,Line2,PostalCode,Locality,City,Country")] Address address)
        {
            if (ModelState.IsValid)
            {
                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(address);
        }

        /// <summary>
        /// Apresenta a vista de edição para um endereço específico.
        /// </summary>
        /// <param name="id">ID do endereço a editar.</param>
        /// <returns>Vista de edição ou NotFound se não existir.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Address.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }

        /// <summary>
        /// Processa os dados submetidos para atualizar um endereço existente.
        /// </summary>
        /// <param name="id">ID do endereço a atualizar.</param>
        /// <param name="address">Objeto Address atualizado.</param>
        /// <returns>Redireciona para o Index se bem-sucedido, caso contrário retorna a mesma vista.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Line1,Line2,PostalCode,Locality,City,Country")] Address address)
        {
            if (id != address.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(address);
        }

        /// <summary>
        /// Apresenta a vista de confirmação de remoção de um endereço.
        /// </summary>
        /// <param name="id">ID do endereço a remover.</param>
        /// <returns>Vista de confirmação ou NotFound se não existir.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Address
                .FirstOrDefaultAsync(m => m.Id == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        /// <summary>
        /// Confirma e executa a remoção de um endereço do sistema.
        /// </summary>
        /// <param name="id">ID do endereço a remover.</param>
        /// <returns>Redireciona para o Index após remoção.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.Address.FindAsync(id);
            if (address != null)
            {
                _context.Address.Remove(address);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um endereço com o ID especificado existe.
        /// </summary>
        /// <param name="id">ID do endereço.</param>
        /// <returns>True se existir, False caso contrário.</returns>
        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.Id == id);
        }
    }
}
