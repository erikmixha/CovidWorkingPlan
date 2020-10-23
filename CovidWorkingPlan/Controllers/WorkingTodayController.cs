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
    public class WorkingTodayController : Controller
    {
        private readonly WorkPlanDBContext _context;

        public WorkingTodayController(WorkPlanDBContext context)
        {
            _context = context;
        }

        // GET: WorkingTodays
        public async Task<IActionResult> Index()
        {
            var workPlanDBContext = _context.WorkingToday.Include(w => w.IdemployeeNavigation);
            return View(await workPlanDBContext.ToListAsync());
        }

        // GET: WorkingTodays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingToday = await _context.WorkingToday
                .Include(w => w.IdemployeeNavigation)
                .FirstOrDefaultAsync(m => m.IdworkToday == id);
            if (workingToday == null)
            {
                return NotFound();
            }

            return View(workingToday);
        }

        // GET: WorkingTodays/Create
        public IActionResult Create()
        {
            ViewData["Idemployee"] = new SelectList(_context.EmployeeInfo, "Idemployee", "EmpNameSurname");
            return View();
        }

        // POST: WorkingTodays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkDayViewModel workVM)
        {
            if (ModelState.IsValid)
            {
                ViewData["ErrorMsg"] = "";
                List<int> SeatTakenList = new List<int>(); //will contain list of nr of seats taken 
                SeatTakenList.Clear();

                int seatNrGenerated =0;//will be used to add to db the seat nr
                //check if user code entered exists in db 
                bool CodeExists = _context.EmployeeInfo.Any(r => r.EmpCode == workVM.EmpCode);

                if(CodeExists==false)
                {
                    ViewData["ErrorMsg"] = "The entered Code was not found!";

                    return View();
                }
                else
                {
                    //check if entered date is >= today 
                    var today = DateTime.Now;
                    if (workVM.WorkDate.Date < today.Date)
                    {
                        ViewData["ErrorMsg"] = "The entered Date must be today or a later date!";
                        return View();
                    }
                    else
                    {
                        //get id of employee by his code
                        var idEmployee =  _context.EmployeeInfo.FirstOrDefault(e =>e.EmpCode == workVM.EmpCode).Idemployee;

                        //check if user has choosen a seat for this date 
                        var AlreadyRegistered =  _context.WorkingToday.Where(r => r.Idemployee == idEmployee && r.WorkDate.Date == workVM.WorkDate.Date).ToList();
                        if (AlreadyRegistered.Count()>0)
                        {
                            var GetSeatNr = _context.WorkingToday.FirstOrDefault(r => r.Idemployee == idEmployee && r.WorkDate.Date == workVM.WorkDate.Date).SeatNr;

                            ViewData["ErrorMsg"] = "This employee has already been registered for this date, the seating nr is " + GetSeatNr;
                            return View();

                        }
                        else
                        {
                            //count how many seats have been taken for this given date 
                            var NrOfSeatsTaken =  _context.WorkingToday.Where(r => r.WorkDate.Date == workVM.WorkDate.Date).ToList();

                            //get total nr of employees in the company
                            var countEmp =  _context.EmployeeInfo.ToList().Count();

                            //only 30% of employees must be at work
                            var NrOfEmployees = countEmp * 30/100 ;

                            //check if nr of seats taken is bigger than 30%
                            if (NrOfSeatsTaken.Count() > Convert.ToInt32(NrOfEmployees))
                            {
                                ViewData["ErrorMsg"] = "There are no more seats for this date available!";
                                return View();
                            }
                            else
                            {
                                //get total seats in the company 
                                var TotalSeats =  _context.CompanyInfoSize.FirstOrDefault().TotalSeatingNumbers;
                                
                                int count = 0;

                                if (NrOfSeatsTaken.Count()>0)
                                {
                                    //add all seats taken to a list 

                                    foreach (var item in NrOfSeatsTaken)
                                    {
                                        SeatTakenList.Add(item.SeatNr);
                                    }

                                    //we will random for 100 times(for example) if needed until it finds a nr distinct from the other seats taken
                                    while (count < 100)
                                    {
                                       //generate a random nr for the user 
                                        seatNrGenerated = new Random().Next(1, TotalSeats);
                                        //check if this seat generated is already taken

                                        if (!SeatTakenList.Contains(seatNrGenerated))
                                        {
                                            //We found the distinct nr no need to continue further
                                            break;
                                        }
                                        else
                                        {
                                            count++;
                                        }
                                        //in case we iterate all 100 times and still we dont find nr, we show msg so user can retry(pretty unusual)
                                        if(count==100)
                                        {
                                            ViewData["ErrorMsg"] = "An error happened in generating the seat number, please try again!";
                                            return View();

                                        }

                                    }


                                }
                                else
                                {
                                    //generate a random nr for the user 
                                    seatNrGenerated = new Random().Next(1, TotalSeats);
                                }

                                //assign values to add them to db
                                workVM.SeatNr = seatNrGenerated;
                                workVM.Idemployee = idEmployee;


                            }

                        }
                    }

                    var workday = new WorkingToday
                    {
                        Idemployee = workVM.Idemployee,
                        SeatNr = workVM.SeatNr,
                        WorkDate = workVM.WorkDate
                    };
                    _context.Add(workday);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
              
            }
            return View(workVM);
        }

        // GET: WorkingTodays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingToday = await _context.WorkingToday.FindAsync(id);
            var code =  _context.EmployeeInfo.FirstOrDefault(c => c.Idemployee == workingToday.Idemployee).EmpCode;
            workingToday.IdemployeeNavigation.EmpCode = code;
            if (workingToday == null)
            {
                return NotFound();
            }
            return View(workingToday);
        }

        // POST: WorkingTodays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( WorkingToday workVM)
        {
            if (ModelState.IsValid)
            {
                ViewData["ErrorMsg"] = "";
                List<int> SeatTakenList = new List<int>(); //will contain list of nr of seats taken 
                SeatTakenList.Clear();

                int seatNrGenerated = 0;//will be used to add to db the seat nr
                //check if user code entered exists in db 
                bool CodeExists = _context.EmployeeInfo.Any(r => r.EmpCode == workVM.IdemployeeNavigation.EmpCode);

                if (CodeExists == false)
                {
                    ViewData["ErrorMsg"] = "The entered Code was not found!";

                    return View();
                }
                else
                {
                    //check if entered date is >= today 
                    var today = DateTime.Now;
                    if (workVM.WorkDate.Date < today.Date)
                    {
                        ViewData["ErrorMsg"] = "The entered Date must be today or a later date!";
                        return View();
                    }
                    else
                    {
                        //get id of employee by his code
                        var idEmployee = _context.EmployeeInfo.FirstOrDefault(e => e.EmpCode == workVM.IdemployeeNavigation.EmpCode);

                        //check if user has choosen a seat for this date 
                        var AlreadyRegistered = _context.WorkingToday.Where(r => r.Idemployee == idEmployee.Idemployee && r.WorkDate.Date == workVM.WorkDate.Date).ToList();
                        if (AlreadyRegistered.Count() > 0)
                        {
                            var GetSeatNr = _context.WorkingToday.FirstOrDefault(r => r.Idemployee == idEmployee.Idemployee && r.WorkDate.Date == workVM.WorkDate.Date).SeatNr;

                            ViewData["ErrorMsg"] = "This employee has already been registered for this date, the seating nr is " + GetSeatNr;
                            return View();

                        }
                        else
                        {
                            //count how many seats have been taken for this given date 
                            var NrOfSeatsTaken = _context.WorkingToday.Where(r => r.WorkDate.Date == workVM.WorkDate.Date).ToList();

                            //get total nr of employees in the company
                            var countEmp = _context.EmployeeInfo.ToList().Count();

                            //only 30% of employees must be at work
                            var NrOfEmployees = countEmp * 30 / 100;

                            //check if nr of seats taken is bigger than 30%
                            if (NrOfSeatsTaken.Count() > Convert.ToInt32(NrOfEmployees))
                            {
                                ViewData["ErrorMsg"] = "There are no more seats for this date available!";
                                return View();
                            }
                            else
                            {
                                //get total seats in the company 
                                var TotalSeats = _context.CompanyInfoSize.FirstOrDefault().TotalSeatingNumbers;

                                int count = 0;

                                if (NrOfSeatsTaken.Count() > 0)
                                {
                                    //add all seats taken to a list 

                                    foreach (var item in NrOfSeatsTaken)
                                    {
                                        SeatTakenList.Add(item.SeatNr);
                                    }

                                    //we will random for 100 times(for example) if needed until it finds a nr distinct from the other seats taken
                                    while (count < 100)
                                    {
                                        //generate a random nr for the user 
                                        seatNrGenerated = new Random().Next(1, TotalSeats);
                                        //check if this seat generated is already taken

                                        if (!SeatTakenList.Contains(seatNrGenerated))
                                        {
                                            //We found the distinct nr no need to continue further
                                            break;
                                        }
                                        else
                                        {
                                            count++;
                                        }
                                        //in case we iterate all 100 times and still we dont find nr, we show msg so user can retry(pretty unusual)
                                        if (count == 100)
                                        {
                                            ViewData["ErrorMsg"] = "An error happened in generating the seat number, please try again!";
                                            return View();

                                        }

                                    }


                                }
                                else
                                {
                                    //generate a random nr for the user 
                                    seatNrGenerated = new Random().Next(1, TotalSeats);
                                }

                                //assign values to add them to db
                                workVM.SeatNr = seatNrGenerated;
                                workVM.Idemployee = idEmployee.Idemployee;
                                workVM.IdemployeeNavigation.EmpNameSurname = idEmployee.EmpNameSurname;
                            }

                        }
                    }

                    _context.Update(workVM);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
            return View(workVM);
        }

        // GET: WorkingTodays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingToday = await _context.WorkingToday
                .Include(w => w.IdemployeeNavigation)
                .FirstOrDefaultAsync(m => m.IdworkToday == id);
            if (workingToday == null)
            {
                return NotFound();
            }

            return View(workingToday);
        }

        // POST: WorkingTodays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workingToday = await _context.WorkingToday.FindAsync(id);
            _context.WorkingToday.Remove(workingToday);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkingTodayExists(int id)
        {
            return _context.WorkingToday.Any(e => e.IdworkToday == id);
        }
    }
}
