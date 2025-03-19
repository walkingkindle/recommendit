using System.ComponentModel.DataAnnotations;

namespace Recommendit.Infrastructure
{
    public class ShowInfo
    {
        public int Id { get; set; }

        [MaxLength(8500)]
        public required string VectorDouble { get; set; }
        
        public int ShowId { get; set; }
        
        public Show Show { get; set; }
    }
}
