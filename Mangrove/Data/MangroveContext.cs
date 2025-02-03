using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Mangrove.Data;

public partial class MangroveContext : DbContext {
    public MangroveContext() {
    }

    public MangroveContext(DbContextOptions<MangroveContext> options)
        : base(options) {
    }

    public virtual DbSet<TblHome> TblHomes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<TblHome>(entity => {
            entity
                .HasNoKey()
                .ToTable("tblHome");

            entity.Property(e => e.ItemsSlider).HasColumnName("itemsSlider");
            entity.Property(e => e.TimeWorkClose)
                .HasColumnType("datetime")
                .HasColumnName("timeWork_close");
            entity.Property(e => e.TimeWorkOpen)
                .HasColumnType("datetime")
                .HasColumnName("timeWork_open");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
