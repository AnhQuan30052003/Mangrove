using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Mangrove.Data;

public partial class MangroveContext : DbContext
{
    public MangroveContext()
    {
    }

    public MangroveContext(DbContextOptions<MangroveContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAdmin> TblAdmins { get; set; }

    public virtual DbSet<TblDistributiton> TblDistributitons { get; set; }

    public virtual DbSet<TblHome> TblHomes { get; set; }

    public virtual DbSet<TblIndividual> TblIndividuals { get; set; }

    public virtual DbSet<TblMangrove> TblMangroves { get; set; }

    public virtual DbSet<TblPhoto> TblPhotos { get; set; }

    public virtual DbSet<TblSetting> TblSettings { get; set; }

    public virtual DbSet<TblStage> TblStages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAdmin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblAdmin__DED88B1C4EAC5E3C");

            entity.ToTable("tblAdmin");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.CodeSendEmail)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("_codeSendEmail");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("_email");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("_password");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("_username");
        });

        modelBuilder.Entity<TblDistributiton>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblDistr__DED88B1C426E7C3D");

            entity.ToTable("tblDistributiton");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.ImageMap)
                .HasMaxLength(256)
                .HasColumnName("_imageMap");
            entity.Property(e => e.MapNameEn)
                .HasMaxLength(256)
                .HasColumnName("_mapNameEN");
            entity.Property(e => e.MapNameVi)
                .HasMaxLength(256)
                .HasColumnName("_mapNameVI");
            entity.Property(e => e.UpdateLast)
                .HasColumnType("datetime")
                .HasColumnName("_updateLast");
        });

        modelBuilder.Entity<TblHome>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblHome__DED88B1C969FC8A1");

            entity.ToTable("tblHome");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.BannerImg)
                .HasMaxLength(50)
                .HasColumnName("_bannerImg");
            entity.Property(e => e.BannerTitleEn)
                .HasMaxLength(256)
                .HasColumnName("_bannerTitleEN");
            entity.Property(e => e.BannerTitleVi)
                .HasMaxLength(256)
                .HasColumnName("_bannerTitleVI");
            entity.Property(e => e.ItemRecent).HasColumnName("_itemRecent");
            entity.Property(e => e.PurposeEn).HasColumnName("_purposeEN");
            entity.Property(e => e.PurposeVi).HasColumnName("_purposeVI");
            entity.Property(e => e.TitleListItemEn)
                .HasMaxLength(256)
                .HasColumnName("_titleListItemEN");
            entity.Property(e => e.TitleListItemVi)
                .HasMaxLength(256)
                .HasColumnName("_titleListItemVI");
            entity.Property(e => e.TitlePurposeEn)
                .HasMaxLength(256)
                .HasColumnName("_titlePurposeEN");
            entity.Property(e => e.TitlePurposeVi)
                .HasMaxLength(256)
                .HasColumnName("_titlePurposeVI");
        });

        modelBuilder.Entity<TblIndividual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblIndiv__DED88B1C83F6D367");

            entity.ToTable("tblIndividual");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.IdMangrove)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_idMangrove");
            entity.Property(e => e.Latitude)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("_latitude");
            entity.Property(e => e.Longitude)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("_longitude");
            entity.Property(e => e.PositionEn)
                .HasMaxLength(256)
                .HasColumnName("_positionEN");
            entity.Property(e => e.PositionVi)
                .HasMaxLength(256)
                .HasColumnName("_positionVI");
            entity.Property(e => e.QrName)
                .HasMaxLength(256)
                .HasColumnName("_qrName");
            entity.Property(e => e.UpdateLast)
                .HasColumnType("datetime")
                .HasColumnName("_updateLast");
            entity.Property(e => e.View).HasColumnName("_view");

            entity.HasOne(d => d.IdMangroveNavigation).WithMany(p => p.TblIndividuals)
                .HasForeignKey(d => d.IdMangrove)
                .HasConstraintName("FK__tblIndivi___idMa__6166761E");
        });

        modelBuilder.Entity<TblMangrove>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblMangr__DED88B1C689D66DB");

            entity.ToTable("tblMangrove");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.CommonNameEn)
                .HasMaxLength(256)
                .HasColumnName("_commonNameEN");
            entity.Property(e => e.CommonNameVi)
                .HasMaxLength(256)
                .HasColumnName("_commonNameVI");
            entity.Property(e => e.ConservationStatusEn)
                .HasMaxLength(256)
                .HasColumnName("_conservationStatusEN");
            entity.Property(e => e.ConservationStatusVi)
                .HasMaxLength(256)
                .HasColumnName("_conservationStatusVI");
            entity.Property(e => e.DistributionEn)
                .HasMaxLength(256)
                .HasColumnName("_distributionEN");
            entity.Property(e => e.DistributionVi)
                .HasMaxLength(256)
                .HasColumnName("_distributionVI");
            entity.Property(e => e.EcologyEn).HasColumnName("_ecologyEN");
            entity.Property(e => e.EcologyVi).HasColumnName("_ecologyVI");
            entity.Property(e => e.Familia)
                .HasMaxLength(256)
                .HasColumnName("_familia");
            entity.Property(e => e.MainImage).HasColumnName("_mainImage");
            entity.Property(e => e.MorphologyEn).HasColumnName("_morphologyEN");
            entity.Property(e => e.MorphologyVi).HasColumnName("_morphologyVI");
            entity.Property(e => e.NameEn)
                .HasMaxLength(256)
                .HasColumnName("_nameEN");
            entity.Property(e => e.NameVi)
                .HasMaxLength(256)
                .HasColumnName("_nameVI");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(256)
                .HasColumnName("_scientificName");
            entity.Property(e => e.TitleDistributionEn)
                .HasMaxLength(256)
                .HasColumnName("_titleDistributionEN");
            entity.Property(e => e.TitleDistributionVi)
                .HasMaxLength(256)
                .HasColumnName("_titleDistributionVI");
            entity.Property(e => e.UpdateLast)
                .HasColumnType("datetime")
                .HasColumnName("_updateLast");
            entity.Property(e => e.UseEn).HasColumnName("_useEN");
            entity.Property(e => e.UseVi).HasColumnName("_useVI");
            entity.Property(e => e.View).HasColumnName("_view");
        });

        modelBuilder.Entity<TblPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblPhoto__DED88B1C40B912C6");

            entity.ToTable("tblPhotos");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.IdObj)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_idObj");
            entity.Property(e => e.ImageName)
                .HasMaxLength(256)
                .HasColumnName("_imageName");
            entity.Property(e => e.NoteImgEn)
                .HasMaxLength(256)
                .HasColumnName("_noteImgEN");
            entity.Property(e => e.NoteImgVi)
                .HasMaxLength(256)
                .HasColumnName("_noteImgVI");
            entity.Property(e => e.NumberOrder).HasColumnName("_numberOrder");
        });

        modelBuilder.Entity<TblSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSetti__DED88B1C1F3365DD");

            entity.ToTable("tblSetting");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.AddressEn)
                .HasMaxLength(256)
                .HasColumnName("_addressEN");
            entity.Property(e => e.AddressVi)
                .HasMaxLength(256)
                .HasColumnName("_addressVI");
            entity.Property(e => e.AuthImg)
                .HasMaxLength(50)
                .HasColumnName("_authImg");
            entity.Property(e => e.DescriptionWebsiteEn).HasColumnName("_descriptionWebsiteEN");
            entity.Property(e => e.DescriptionWebsiteVi).HasColumnName("_descriptionWebsiteVI");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("_email");
            entity.Property(e => e.FacultyEn)
                .HasMaxLength(256)
                .HasColumnName("_facultyEN");
            entity.Property(e => e.FacultyVi)
                .HasMaxLength(256)
                .HasColumnName("_facultyVI");
            entity.Property(e => e.FooterBgImg)
                .HasMaxLength(50)
                .HasColumnName("_footerBgImg");
            entity.Property(e => e.FooterDark).HasColumnName("_footerDark");
            entity.Property(e => e.LogoImg)
                .HasMaxLength(50)
                .HasColumnName("_logoImg");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("_phone");
            entity.Property(e => e.SchoolNameEn)
                .HasMaxLength(256)
                .HasColumnName("_schoolNameEN");
            entity.Property(e => e.SchoolNameVi)
                .HasMaxLength(256)
                .HasColumnName("_schoolNameVI");
        });

        modelBuilder.Entity<TblStage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblStage__DED88B1C3A34F8BF");

            entity.ToTable("tblStage");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_id");
            entity.Property(e => e.Height)
                .HasMaxLength(100)
                .HasColumnName("_height");
            entity.Property(e => e.IdIndividual)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("_idIndividual");
            entity.Property(e => e.MainImage)
                .HasMaxLength(256)
                .HasColumnName("_mainImage");
            entity.Property(e => e.NameEn)
                .HasMaxLength(256)
                .HasColumnName("_nameEN");
            entity.Property(e => e.NameVi)
                .HasMaxLength(256)
                .HasColumnName("_nameVI");
            entity.Property(e => e.NumberOrder).HasColumnName("_numberOrder");
            entity.Property(e => e.Perimeter)
                .HasMaxLength(100)
                .HasColumnName("_perimeter");
            entity.Property(e => e.SurveyDay)
                .HasColumnType("datetime")
                .HasColumnName("_surveyDay");
            entity.Property(e => e.WeatherEn)
                .HasMaxLength(256)
                .HasColumnName("_weatherEN");
            entity.Property(e => e.WeatherVi)
                .HasMaxLength(256)
                .HasColumnName("_weatherVI");

            entity.HasOne(d => d.IdIndividualNavigation).WithMany(p => p.TblStages)
                .HasForeignKey(d => d.IdIndividual)
                .HasConstraintName("FK__tblStage___idInd__6442E2C9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
