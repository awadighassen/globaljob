using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace globaljob.Models
{
    public class Offre
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime DatePublication { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime DateFin { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Description { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public string Titre { get; set; }

        public List<Candidature> Candidature { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
