using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactsCore.Data.Dao
{
    public abstract class Base
    {
        public const int MaxBaseColumnOrder = 5;

        [Column("id", Order = 1)] // NOTE: Order is currently ignored by ef core :(
        [Key]
        public long Id { get; set; }

        [Column("uid", Order = 2)]
        [Required]
        public Guid Uid { get; set; }

        [Column("created", Order = 3)]
        [Required]
        public DateTime Created { get; set; }
        
        [Column("modified", Order = 4)]
        [Required]
        public DateTime Modified { get; set; }

        [Column("isdeleted", Order = MaxBaseColumnOrder)]
        [Required]
        public bool IsDeleted { get; set; }


        public virtual void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity(GetType()) // Uid: DEFAULT
                .Property("Uid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .ValueGeneratedOnAdd();

            builder.Entity(GetType()) // Uid: UNIQUE
                .HasAlternateKey("Uid");


            builder.Entity(GetType()) // Created: DEFAULT
                .Property("Created")
                .HasDefaultValueSql("(now() at time zone 'utc')")
                .ValueGeneratedOnAdd();


            builder.Entity(GetType()) // Created: DEFAULT
                .Property("Modified")
                .HasDefaultValueSql("(now() at time zone 'utc')")
                .ValueGeneratedOnAddOrUpdate()
                .Metadata.AfterSaveBehavior = Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Save; // allow updating value in SaveChanges()


            builder.Entity(GetType()) // Deleted: DEFAULT
                .Property("IsDeleted")
                .HasDefaultValue(false)
                .ValueGeneratedOnAdd();
        }
    }
}