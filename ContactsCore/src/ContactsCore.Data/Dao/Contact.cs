using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ContactsCore.Common.Interfaces;

namespace ContactsCore.Data.Dao
{
    [Table("contacts")]
    public class Contact : Base, IContact
    {        
        [Column("firstname", Order = MaxBaseColumnOrder + 1)]
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Column("lastname", Order = MaxBaseColumnOrder + 2)]
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }


        public virtual List<ContactDetail> Details { get; set; }


        //public override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //}
    }
}
