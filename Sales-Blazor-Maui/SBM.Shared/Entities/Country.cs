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
        public string? Iso2 { get; set; }

        [Display(Name = "PhoneCode")]
        public string? PhoneCode { get; set; }

        public ICollection<State>? States { get; set; }
        public int StatesNumber => States == null ? 0 : States.Count;
    }
}
