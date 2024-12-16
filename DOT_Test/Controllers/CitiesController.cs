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

namespace DOT_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCity()
        {
            var data = from c in _context.City join p in _context.Province on c.ProvinceID equals p.ProvinceID
                       select new
                       {
                           c.CityID,
                           CityName = c.Name,
                           c.ProvinceID,
                           ProvinceName = p.Name

                       };
            return  Ok(data);
        }

        // GET: api/Cities/5
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var data = from c in _context.City
                       join p in _context.Province on c.ProvinceID equals p.ProvinceID
                       where c.CityID == id
                       select new
                       {
                           c.CityID,
                           CityName = c.Name,
                           c.ProvinceID,
                           ProvinceName = p.Name

                       };

            if (data.Count() == 0)
            {
                return Ok(new { Status = "Failed", Result = "Id tidak ditemukan " });
            }

            return Ok(data);
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, ApiModel.ApiCity City)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return Ok(new { Status = "Failed", Result = messages });
            }

            if (id != City.CityID)
            {
                return Ok(new { Status = "Failed", Result = "Id tidak ditemukan " });
            }

            var newCity = new City
            {
                CityID = City.CityID,
                ProvinceID = City.ProvinceID,
                Name = City.Name
            };

            _context.Entry(newCity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(newCity);
        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{CityName}, {ProvinceId}")]
        public async Task<ActionResult<City>> PostCity([FromRoute] string CityName, int ProvinceId)
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

                if (CityName == "")
                {
                    return Ok(new { Status = "Failed", Result = "Nama Provinsi harus diisi " });
                }

                City cit = new City();

                cit.Name = CityName;
                cit.ProvinceID = ProvinceId;

                //Check Existing Data, prevent redundancy
                var cek = _context.City.Count(e => e.Name == CityName);
                if (cek == 0)
                {
                    _context.City.Add(cit);
                    await _context.SaveChangesAsync();

                    return Ok(cit);
                }
                else
                {
                    return Ok(new { Status = "Failed", Result = "Sudah ada Provinsi dengan nama " + CityName });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Failed", Result = ex.Message });
            }

        }

        // DELETE: api/Cities/5
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return Ok(new { Status = "Failed", Result = messages });
            }

            var City = await _context.City.FindAsync(id);
            if (City == null)
            {
                return Ok(new { Status = "Failed", Result = "Id tidak ditemukan " });
            }

            try
            {
                _context.City.Remove(City);
                await _context.SaveChangesAsync();
                return Ok(City);
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "Failed", Result = ex.Message });
            }
        }

        private bool CityExists(int id)
        {
            return _context.City.Any(e => e.CityID == id);
        }
    }
}
