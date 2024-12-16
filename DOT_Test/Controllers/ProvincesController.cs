using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOT_Test;
using DOT_Test.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;

namespace DOT_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvincesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProvincesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Provinces
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Province>>> GetProvince()
        {
            try
            {
                //CONTOH EAGER LOAD
                var data = _context.Province.Include(c => c.Cities).Select(e=>
                           new
                           {
                               e.ProvinceID,
                               ProvinceName = e.Name,
                               CityCount = e.Cities.Count(),
                               Cities = e.Cities.Select(d => new { d.CityID, d.Name })
                           });

                return Ok(await data.ToListAsync());
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Failed", Result = ex.Message });
            }
            
        }

        // GET: api/Provinces/5
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Province>> GetProvince(int id)
        {
            //CONTOH LAZY LOAD
            var data = from p in _context.Province
                       where p.ProvinceID == id
                       let citydata = (from c in _context.City
                                       where c.ProvinceID == p.ProvinceID
                                       select new
                                       {
                                           c.CityID,
                                           CityName = c.Name
                                       }
                                       ).ToList()
                       select new
                       {
                           p.ProvinceID,
                           ProvinceName = p.Name,
                           CityCount = citydata.Count(),
                           Cities = citydata
                       };

            if (data.Count() == 0)
            {
                return Ok(new { Status = "Failed", Result = "Id tidak ditemukan " });
            }

            return Ok(await data.ToListAsync());
        }

        // PUT: api/Provinces/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProvince(int id, ApiModel.ApiProvince province)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return Ok(new { Status = "Failed", Result = messages });
            }

            if (id != province.ProvinceID)
            {
                return Ok(new { Status = "Failed", Result = "Id tidak ditemukan " });
            }

            var newPro = new Province
            {
                ProvinceID = province.ProvinceID,
                Name = province.Name
            };

            _context.Entry(newPro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProvinceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(newPro);
        }

        // POST: api/Provinces
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{ProvinceName}")]
        public async Task<ActionResult<Province>> PostProvince([FromRoute] string ProvinceName)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string messages = string.Join("; ", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
                    return Ok(new { Status = "Failed", Result = messages });
                }

                if (ProvinceName == "")
                {
                    return Ok(new { Status = "Failed", Result = "Nama Provinsi harus diisi " });
                }

                Province pro = new Province();

                pro.Name = ProvinceName;

                //Check Existing Data, prevent redundancy
                var cek = _context.Province.Count(e => e.Name == ProvinceName);
                if (cek == 0)
                {
                    _context.Province.Add(pro);
                    await _context.SaveChangesAsync();

                    return Ok(pro);
                }
                else
                {
                    return Ok(new { Status = "Failed", Result = "Sudah ada Provinsi dengan nama " + ProvinceName });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Failed", Result = ex.Message });
            }
            
        }

        // DELETE: api/Provinces/5
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvince(int id)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return Ok(new { Status = "Failed", Result = messages });
            }

            var province = await _context.Province.FindAsync(id);
            if (province == null)
            {
                return Ok(new { Status = "Failed", Result = "Id tidak ditemukan " });
            }

            try
            {
                _context.Province.Remove(province);
                await _context.SaveChangesAsync();
                return Ok(province);
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Failed", Result = ex.Message });
            }
        }

        private bool ProvinceExists(int id)
        {
            return _context.Province.Any(e => e.ProvinceID == id);
        }

        
    }
}
