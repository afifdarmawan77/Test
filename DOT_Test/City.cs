using System.ComponentModel.DataAnnotations;
using DOT_Test.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace DOT_Test
{
    public class City
    {
        public int CityID { get; set; }

        [Required]
        public int ProvinceID { get; set; }

        [Required]
        [Display(Name = "Kota")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Display(Name = "Provinsi")]
        public Province Province { get; set; }
    }
}
