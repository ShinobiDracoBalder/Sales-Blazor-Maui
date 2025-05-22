using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SBM.Shared.Entities
{

    public class Country
    {
        public int Id { get; set; }

        [Display(Name = "País")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caractéres")]
        public string Name { get; set; } = null!;
        [Display(Name = "Iso2")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caractéres")]
        public string? Iso2 { get; set; } = null!;

        [Display(Name = "PhoneCode")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(15, ErrorMessage = "El campo {0} no puede tener más de {1} caractéres")]
        public string? PhoneCode { get; set; } = null!;

        public ICollection<State>? States { get; set; }
        public int StatesNumber => States == null ? 0 : States.Count;
    }
}
