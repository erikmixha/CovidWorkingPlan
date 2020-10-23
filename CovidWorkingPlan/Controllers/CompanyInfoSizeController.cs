using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CovidWorkingPlan.Models;

namespace CovidWorkingPlan.Controllers
{
    public class CompanyInfoSizeController : Controller
    {
        private readonly WorkPlanDBContext _context;

        public CompanyInfoSizeController(WorkPlanDBContext context)
        {
            _context = context;
        }

        // GET: CompanyInfoSize
        public async Task<IActionResult> Index()
        {
            return View(await _context.CompanyInfoSize.ToListAsync());
        }

        // GET: CompanyInfoSize/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyInfoSize = await _context.CompanyInfoSize
                .FirstOrDefaultAsync(m => m.IdcompanyInfo == id);
            if (companyInfoSize == null)
            {
                return NotFound();
            }

            return View(companyInfoSize);
        }

        // GET: CompanyInfoSize/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CompanyInfoSize/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdcompanyInfo,TotalEmployees,TotalSeatingNumbers")] CompanyInfoSize companyInfoSize)
        {
            if (ModelState.IsValid)
            {
                _context.Add(companyInfoSize);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(companyInfoSize);
        }

        // GET: CompanyInfoSize/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyInfoSize = await _context.CompanyInfoSize.FindAsync(id);
            if (companyInfoSize == null)
            {
                return NotFound();
            }
            return View(companyInfoSize);
        }

        // POST: CompanyInfoSize/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdcompanyInfo,TotalEmployees,TotalSeatingNumbers")] CompanyInfoSize companyInfoSize)
        {
            if (id != companyInfoSize.IdcompanyInfo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyInfoSize);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyInfoSizeExists(companyInfoSize.IdcompanyInfo))
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
            return View(companyInfoSize);
        }

        // GET: CompanyInfoSize/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyInfoSize = await _context.CompanyInfoSize
                .FirstOrDefaultAsync(m => m.IdcompanyInfo == id);
            if (companyInfoSize == null)
            {
                return NotFound();
            }

            return View(companyInfoSize);
        }

        // POST: CompanyInfoSize/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var companyInfoSize = await _context.CompanyInfoSize.FindAsync(id);
            _context.CompanyInfoSize.Remove(companyInfoSize);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyInfoSizeExists(int id)
        {
            return _context.CompanyInfoSize.Any(e => e.IdcompanyInfo == id);
        }
    }
}
