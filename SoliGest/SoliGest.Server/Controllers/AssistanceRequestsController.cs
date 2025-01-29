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
    public class AssistanceRequestsController : Controller
    {
        private readonly SoliGestServerContext _context;

        public AssistanceRequestsController(SoliGestServerContext context)
        {
            _context = context;
        }

        // GET: AssistanceRequests
        public async Task<IActionResult> Index()
        {
            return View(await _context.AssistanceRequest.ToListAsync());
        }

        // GET: AssistanceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistanceRequest = await _context.AssistanceRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assistanceRequest == null)
            {
                return NotFound();
            }

            return View(assistanceRequest);
        }

        // GET: AssistanceRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AssistanceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RequestDate,ResolutionDate")] AssistanceRequest assistanceRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assistanceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(assistanceRequest);
        }

        // GET: AssistanceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistanceRequest = await _context.AssistanceRequest.FindAsync(id);
            if (assistanceRequest == null)
            {
                return NotFound();
            }
            return View(assistanceRequest);
        }

        // POST: AssistanceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RequestDate,ResolutionDate")] AssistanceRequest assistanceRequest)
        {
            if (id != assistanceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assistanceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssistanceRequestExists(assistanceRequest.Id))
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
            return View(assistanceRequest);
        }

        // GET: AssistanceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistanceRequest = await _context.AssistanceRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assistanceRequest == null)
            {
                return NotFound();
            }

            return View(assistanceRequest);
        }

        // POST: AssistanceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assistanceRequest = await _context.AssistanceRequest.FindAsync(id);
            if (assistanceRequest != null)
            {
                _context.AssistanceRequest.Remove(assistanceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssistanceRequestExists(int id)
        {
            return _context.AssistanceRequest.Any(e => e.Id == id);
        }
    }
}
