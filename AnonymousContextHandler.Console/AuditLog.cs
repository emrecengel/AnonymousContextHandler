using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Console
{
    [Table("AuditLog")]
    public class AuditLog
    {
        [Required]
        public string LogInfo { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [MaxLength(50), Required]
        public string CreatedBy { get; set; }
        [MaxLength(50)]
        public string ModifiedBy { get; set; }
        [Column("AuditLogID")]
        public int Id { get; set; }
        [Column("CustomerID"), Required]
        public int CustomerId { get; set; }
    }
}