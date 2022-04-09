using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace globaljob.Models
{
    public class Candidature
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le champ nom est obligatoire")]
        [Display(Name = "Nom Candidat:")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Le champ email est obligatoire")]
        [Display(Name = "Email:")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le champ numéro téléphone est obligatoire")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Numéro Téléphone:")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Le champ cv est obligatoire")]
        [DataType(DataType.Upload)]
        [Display(Name = "Votre CV:")]
        [NotMapped]
        public IFormFile Resume { get; set; }
        public string ResumeURL { get; set; }

        public int? OffreId { get; set; }

        public Offre Offre { get; set; }
    }
}
