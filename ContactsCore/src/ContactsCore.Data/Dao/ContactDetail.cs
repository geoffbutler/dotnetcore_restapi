using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ContactsCore.Common.Interfaces;
using ContactsCore.Common.Enums;

namespace ContactsCore.Data.Dao
{
    [Table("contactdetails")]
    public class ContactDetail : Base, IContactDetail
    {           
        [Column("description", Order = MaxBaseColumnOrder + 1)]
        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Column("value", Order = MaxBaseColumnOrder + 2)]
        [Required]
        [StringLength(50)]
        public string Value { get; set; }

        [Column("type", Order = MaxBaseColumnOrder + 3)]
        [Required]
        public ContactDetailType Type { get; set; }


        [Column("contactid", Order = MaxBaseColumnOrder + 4)]
        [Required]
        public virtual long ContactId { get; set; }

        [Required]
        public virtual Contact Contact { get; set; }


        public override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ContactDetail>()
                .HasOne(p => p.Contact)
                .WithMany(p => p.Details)
                .HasForeignKey(p => p.ContactId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
