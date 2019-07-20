using Microsoft.EntityFrameworkCore;
using System;

namespace ContactsCore.Data
{
    public class ContactsContext : DbContext
    {
        //private readonly IConfigurationRoot _config;

        //public ContactsContext(IConfigurationRoot config)
        //{
        //    _config = config;
        //}

        public ContactsContext() { } // for unit testing

        public ContactsContext(DbContextOptions<ContactsContext> options)
            : base(options)
        {
        }

        // TODO: ef migrations used to bootstap using Api Startup - so previously this 
        //  override was not required - also means we can't inject IConfigurationRoot :(
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           if (optionsBuilder.IsConfigured)
                return; // already configured by API startup (not running migrations)

           //string contactsConnString = _config["ConnectionStrings:Contacts"];
           string contactsConnString = "Host=localhost;Database=contacts;Username=postgres;Password=sql";
           optionsBuilder.UseNpgsql(contactsConnString, options =>
           {
               options.MigrationsAssembly("ContactsCore.Data");
           });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var type = (Dao.Base)Activator.CreateInstance(entityType.ClrType);
                type.OnModelCreating(modelBuilder);
            }
        }

        //private void UpdateModifiedDateTime()
        //{            
        //    var modifiedEntries = ChangeTracker
        //       .Entries()
        //       .Where(x => x.State == EntityState.Modified)
        //       .Select(x => x.Entity)
        //       .Cast<Dao.Base>()
        //       .ToList();
        //    foreach (var entry in modifiedEntries)
        //    {
        //        // update modified datetime
        //        entry.Modified = DateTime.UtcNow;
        //    }
        //}

        //public override int SaveChanges()
        //{
        //    UpdateModifiedDateTime();

        //    return base.SaveChanges();
        //}

        //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    UpdateModifiedDateTime();

        //    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //}

        public DbSet<Dao.Contact> Contacts { get; set; }
        public DbSet<Dao.ContactDetail> ContactDetails { get; set; }
    }
}
