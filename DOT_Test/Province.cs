using System.ComponentModel.DataAnnotations;

namespace DOT_Test
{
    public class Province
    {
        public int ProvinceID { get; set; }

        [Required]
        [Display(Name = "Provinsi")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Display(Name = "Kota")]
        public ICollection<City> Cities { get; set; }
    }
}
