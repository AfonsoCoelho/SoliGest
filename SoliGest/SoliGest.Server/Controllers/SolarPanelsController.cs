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
    public class SolarPanelsController : Controller
    {
        private readonly SoliGestServerContext _context;

        public SolarPanelsController(SoliGestServerContext context)
        {
            _context = context;
        }

        // GET: SolarPanels
        public async Task<IActionResult> Index()
        {
            return View(await _context.SolarPanel.ToListAsync());
        }

        // GET: SolarPanels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solarPanel = await _context.SolarPanel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (solarPanel == null)
            {
                return NotFound();
            }

            return View(solarPanel);
        }

        // GET: SolarPanels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SolarPanels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PhoneNumber,Email")] SolarPanel solarPanel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solarPanel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(solarPanel);
        }

        // GET: SolarPanels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solarPanel = await _context.SolarPanel.FindAsync(id);
            if (solarPanel == null)
            {
                return NotFound();
            }
            return View(solarPanel);
        }

        // POST: SolarPanels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PhoneNumber,Email")] SolarPanel solarPanel)
        {
            if (id != solarPanel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solarPanel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolarPanelExists(solarPanel.Id))
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
            return View(solarPanel);
        }

        // GET: SolarPanels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solarPanel = await _context.SolarPanel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (solarPanel == null)
            {
                return NotFound();
            }

            return View(solarPanel);
        }

        // POST: SolarPanels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var solarPanel = await _context.SolarPanel.FindAsync(id);
            if (solarPanel != null)
            {
                _context.SolarPanel.Remove(solarPanel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SolarPanelExists(int id)
        {
            return _context.SolarPanel.Any(e => e.Id == id);
        }
    }
}
