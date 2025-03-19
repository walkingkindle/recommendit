using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recommendit.Infrastructure    {
    public class Show
    {
        public int Id { get; set; }

        [StringLength(500)]
        public required string Name { get; set; }

        [StringLength(5000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ImageUrl { get; set; }
        public DateTime? FinalEpisodeAired { get; set; }
        public int? Score { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }

        [StringLength(100)]
        public string? OriginalCountry { get; set; }

        [StringLength(100)]
        public string? OriginalLanguage { get; set; }
        public ShowInfo? ShowInfo { get; set; }
        public int? ReleaseYear { get; set; }
       
     

    }
}
