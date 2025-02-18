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

    public virtual DbSet<TblIndividual> TblIndividuals { get; set; }

    public virtual DbSet<TblMangrove> TblMangroves { get; set; }

    public virtual DbSet<TblPhoto> TblPhotos { get; set; }

    public virtual DbSet<TblStage> TblStages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<TblHome>(entity => {
            entity
                .HasNoKey()
                .ToTable("tblHome");

            entity.Property(e => e.FooterImg)
                .HasMaxLength(256)
                .HasColumnName("_footerImg");
            entity.Property(e => e.TimeWorkClose).HasColumnName("_timeWork_close");
            entity.Property(e => e.TimeWorkOpen).HasColumnName("_timeWork_open");
            entity.Property(e => e.YearEnd).HasColumnName("_yearEnd");
            entity.Property(e => e.YearStart).HasColumnName("_yearStart");
        });

        modelBuilder.Entity<TblIndividual>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__tblIndiv__DED88B1C970B8777");

            entity.ToTable("tblIndividual");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.IdMangrove)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_idMangrove");
            entity.Property(e => e.Number).HasColumnName("_number");
            entity.Property(e => e.SurveyDay)
                .HasColumnType("datetime")
                .HasColumnName("_surveyDay");

            entity.HasOne(d => d.IdMangroveNavigation).WithMany(p => p.TblIndividuals)
                .HasForeignKey(d => d.IdMangrove)
                .HasConstraintName("FK__tblIndivi___idMa__4CA06362");
        });

        modelBuilder.Entity<TblMangrove>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__tblMangr__DED88B1C5B93AFD4");

            entity.ToTable("tblMangrove");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.ConservationStatus)
                .HasMaxLength(256)
                .HasColumnName("_conservationStatus");
            entity.Property(e => e.Distribution)
                .HasMaxLength(256)
                .HasColumnName("_distribution");
            entity.Property(e => e.Ecology).HasColumnName("_ecology");
            entity.Property(e => e.MainImage).HasColumnName("_mainImage");
            entity.Property(e => e.Morphology).HasColumnName("_morphology");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("_name");
            entity.Property(e => e.OtherName)
                .HasMaxLength(50)
                .HasColumnName("_otherName");
            entity.Property(e => e.Quantity).HasColumnName("_quantity");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("_scientificName");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("_surname");
            entity.Property(e => e.UpdateLast)
                .HasColumnType("datetime")
                .HasColumnName("_updateLast");
            entity.Property(e => e.Use).HasColumnName("_use");
            entity.Property(e => e.View).HasColumnName("_view");
        });

        modelBuilder.Entity<TblPhoto>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__tblPhoto__DED88B1C42ECDA73");

            entity.ToTable("tblPhotos");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.IdObj)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_idObj");
            entity.Property(e => e.ImageNameId).HasColumnName("_imageNameId");
            entity.Property(e => e.NoteImg)
                .HasMaxLength(256)
                .HasColumnName("_noteImg");
        });

        modelBuilder.Entity<TblStage>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__tblStage__DED88B1CB13A4F71");

            entity.ToTable("tblStage");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.IdIndividual)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_idIndividual");

            entity.HasOne(d => d.IdIndividualNavigation).WithMany(p => p.TblStages)
                .HasForeignKey(d => d.IdIndividual)
                .HasConstraintName("FK__tblStage___idInd__4F7CD00D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
