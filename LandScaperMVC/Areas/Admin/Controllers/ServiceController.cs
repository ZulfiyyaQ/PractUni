using LandScaperMVC.Areas.Admin.ViewModels;
using LandScaperMVC.DAL;
using LandScaperMVC.Models;
using LandScaperMVC.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LandScaperMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="admin")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ServiceController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Service> services = await _context.Services.ToListAsync();
            return View(services);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var result = _context.Services.Any(s => s.Name.ToLower().Trim() == vm.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bele service var");
                return View(vm);
            }
            if (!vm.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "Tipi uygun deyil");
                return View(vm);
            }
            if (!vm.Photo.ValidateSize(2))
            {
                ModelState.AddModelError("Photo", "Olcusu uygun deyil");
                return View(vm);
            }
            string filename = await vm.Photo.CreateFile(_env.WebRootPath, "img", "services");
            Service service = new Service
            {
                Name = vm.Name,
                Description = vm.Description,
                Image = filename
            };
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Service existed = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (existed == null) return NotFound();
            UpdateServiceVM vm = new UpdateServiceVM
            {
                Name = existed.Name,
                Description = existed.Description,
                Image = existed.Image

            };
            await _context.SaveChangesAsync();
            return View(vm);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateServiceVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Service existed = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (existed == null) return NotFound();
            var result = _context.Services.Any(s => s.Name.ToLower().Trim() == vm.Name.ToLower().Trim()&&s.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele service var");
                return View(vm);
            }
            if (vm.Photo is not null)
            {
                if (!vm.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "Tipi uygun deyil");
                    return View(vm);
                }
                if (!vm.Photo.ValidateSize(2))
                {
                    ModelState.AddModelError("Photo", "Olcusu uygun deyil");
                    return View(vm);
                }
            string newimage= await vm.Photo.CreateFile(_env.WebRootPath, "img", "services");
            existed.Image.DeleteFile(_env.WebRootPath, "img", "services");
            existed.Image = newimage;
            }
            existed.Name= vm.Name;
            existed.Description= vm.Description;
            await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) BadRequest();
            Service service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (service == null) return NotFound();
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) BadRequest();
            Service service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (service == null) return NotFound();
            
            await _context.SaveChangesAsync();
            return View(service);

        }

    }
}
