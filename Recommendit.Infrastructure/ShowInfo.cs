using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recommendit.Infrastructure
{
    public class ShowInfo
    {
        public int Id { get; set; }

        [MaxLength(8200)]
        [Column(TypeName ="varchar(8200)")]
        public required string VectorDouble { get; set; }
        
        public int ShowId { get; set; }
        
        public Show Show { get; set; }
    }
}
