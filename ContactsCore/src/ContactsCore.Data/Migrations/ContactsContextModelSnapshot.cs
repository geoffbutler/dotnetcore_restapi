using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContactsCore.Data.Migrations
{
    [DbContext(typeof(ContactsContext))]
    partial class ContactsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("ContactsCore.Data.Dao.Contact", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created")
                        .HasAnnotation("Npgsql:DefaultValueSql", "(now() at time zone 'utc')");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnName("firstname")
                        .HasMaxLength(50);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("isdeleted")
                        .HasAnnotation("Npgsql:DefaultValue", false);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnName("lastname")
                        .HasMaxLength(50);

                    b.Property<DateTime>("Modified")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("modified")
                        .HasAnnotation("Npgsql:DefaultValueSql", "(now() at time zone 'utc')");

                    b.Property<Guid>("Uid")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("uid")
                        .HasAnnotation("Npgsql:DefaultValueSql", "uuid_generate_v4()");

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.ToTable("contacts");
                });

            modelBuilder.Entity("ContactsCore.Data.Dao.ContactDetail", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("ContactId")
                        .HasColumnName("contactid");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created")
                        .HasAnnotation("Npgsql:DefaultValueSql", "(now() at time zone 'utc')");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("description")
                        .HasMaxLength(50);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("isdeleted")
                        .HasAnnotation("Npgsql:DefaultValue", false);

                    b.Property<DateTime>("Modified")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("modified")
                        .HasAnnotation("Npgsql:DefaultValueSql", "(now() at time zone 'utc')");

                    b.Property<int>("Type")
                        .HasColumnName("type");

                    b.Property<Guid>("Uid")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("uid")
                        .HasAnnotation("Npgsql:DefaultValueSql", "uuid_generate_v4()");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnName("value")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasAlternateKey("Uid");

                    b.HasIndex("ContactId");

                    b.ToTable("contactdetails");
                });

            modelBuilder.Entity("ContactsCore.Data.Dao.ContactDetail", b =>
                {
                    b.HasOne("ContactsCore.Data.Dao.Contact", "Contact")
                        .WithMany("Details")
                        .HasForeignKey("ContactId");
                });
        }
    }
}
