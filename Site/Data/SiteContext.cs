using Microsoft.EntityFrameworkCore;

namespace Site.Data
{
    public partial class SiteContext : DbContext
    {
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Companies> Companies { get; set; }
        public virtual DbSet<CompanyUsers> CompanyUsers { get; set; }
        public virtual DbSet<Cultures> Cultures { get; set; }
        public virtual DbSet<DataItemFiles> DataItemFiles { get; set; }
        public virtual DbSet<DataItemResources> DataItemResources { get; set; }
        public virtual DbSet<DataItems> DataItems { get; set; }
        public virtual DbSet<DataTemplateFields> DataTemplateFields { get; set; }
        public virtual DbSet<DataTemplates> DataTemplates { get; set; }
        public virtual DbSet<DataTemplateSections> DataTemplateSections { get; set; }
        public virtual DbSet<DataTemplateUploads> DataTemplateUploads { get; set; }
        public virtual DbSet<LanguageTranslate> LanguageTranslate { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<NavigationItems> NavigationItems { get; set; }
        public virtual DbSet<Navigations> Navigations { get; set; }
        public virtual DbSet<OAuthTokens> OAuthTokens { get; set; }
        public virtual DbSet<PageResources> PageResources { get; set; }
        public virtual DbSet<PageFiles> PageFiles { get; set; }
        public virtual DbSet<Pages> Pages { get; set; }
        public virtual DbSet<PageTemplateFields> PageTemplateFields { get; set; }
        public virtual DbSet<PageTemplates> PageTemplates { get; set; }
        public virtual DbSet<PageTemplateSections> PageTemplateSections { get; set; }
        public virtual DbSet<PageTemplateUploads> PageTemplateUploads { get; set; }
        public virtual DbSet<ReviewResources> ReviewResources { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }
        public virtual DbSet<ReviewTemplateFields> ReviewTemplateFields { get; set; }
        public virtual DbSet<ReviewTemplates> ReviewTemplates { get; set; }
        public virtual DbSet<WebsiteFields> WebsiteFields { get; set; }
        public virtual DbSet<WebsiteFiles> WebsiteFiles { get; set; }
        public virtual DbSet<WebsiteLanguages> WebsiteLanguages { get; set; }
        public virtual DbSet<WebsiteResources> WebsiteResources { get; set; }
        public virtual DbSet<Websites> Websites { get; set; }
        public virtual DbSet<WebsiteUploads> WebsiteUploads { get; set; }

        public SiteContext(DbContextOptions<SiteContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId)
                    .HasName("IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex");

                entity.Property(e => e.Id).HasMaxLength(450);

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                    .HasName("PK_AspNetUserLogins");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(450);

                entity.Property(e => e.ProviderKey).HasMaxLength(450);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK_AspNetUserRoles");

                entity.HasIndex(e => e.RoleId)
                    .HasName("IX_AspNetUserRoles_RoleId");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_AspNetUserRoles_UserId");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.Property(e => e.RoleId).HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                    .HasName("PK_AspNetUserTokens");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.Property(e => e.LoginProvider).HasMaxLength(450);

                entity.Property(e => e.Name).HasMaxLength(450);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).HasMaxLength(450);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Companies>(entity =>
            {
                entity.Property(e => e.Coc)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Company)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Vat)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<CompanyUsers>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<Cultures>(entity =>
            {
                entity.Property(e => e.CultureCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.CultureName)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LanguageCode)
                    .IsRequired()
                    .HasColumnType("varchar(10)");
            });

            modelBuilder.Entity<DataItemFiles>(entity =>
            {
                entity.Property(e => e.OriginalPath)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CompressedPath)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Alt)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<DataItemResources>(entity =>
            {
                entity.Property(e => e.Text).IsRequired();
            });

            modelBuilder.Entity<DataItems>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Subtitle).IsRequired();

                entity.Property(e => e.Text).IsRequired();

                entity.Property(e => e.HtmlEditor).IsRequired();

                entity.Property(e => e.PageDescription)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.PageKeywords)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.PageTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PageUrl)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.AlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<DataTemplateFields>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.DefaultValue)
                    .IsRequired();
            });

            modelBuilder.Entity<DataTemplates>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Description)
                    .IsRequired();

                entity.Property(e => e.PageAlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.TitleHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.SubtitleHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TextHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.HtmlEditorHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PublishDateHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.FromDateHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ToDateHeading)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.MenuType)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasColumnType("varchar(40)");
            });

            modelBuilder.Entity<DataTemplateSections>(entity =>
            {
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Section)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<DataTemplateUploads>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MimeTypes)
                    .IsRequired()
                    .HasColumnType("varchar(120)");

                entity.Property(e => e.FileExtensions)
                    .IsRequired()
                    .HasColumnType("varchar(70)");
            });

            modelBuilder.Entity<Languages>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Language)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Culture)
                    .IsRequired()
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.TimeZoneId)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<LanguageTranslate>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Translate)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<NavigationItems>(entity =>
            {
                entity.Property(e => e.LinkedToType)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.LinkedToAlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.FilterAlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Target)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.CustomUrl)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Navigations>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<OAuthTokens>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.AccessToken).IsRequired();

                entity.Property(e => e.RefreshToken).IsRequired();
            });

            modelBuilder.Entity<PageFiles>(entity =>
            {
                entity.Property(e => e.OriginalPath)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CompressedPath)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Alt)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<PageResources>(entity =>
            {
                entity.Property(e => e.Text).IsRequired();
            });

            modelBuilder.Entity<Pages>(entity =>
            {
                entity.Property(e => e.AlternateGuid)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Keywords)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<PageTemplateFields>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.DefaultValue)
                    .IsRequired();
            });

            modelBuilder.Entity<PageTemplates>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(35);

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasColumnType("varchar(40)");
            });

            modelBuilder.Entity<PageTemplateSections>(entity =>
            {
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Section)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<PageTemplateUploads>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MimeTypes)
                    .IsRequired()
                    .HasColumnType("varchar(120)");

                entity.Property(e => e.FileExtensions)
                    .IsRequired()
                    .HasColumnType("varchar(70)");
            });

            modelBuilder.Entity<ReviewResources>(entity =>
            {
                entity.Property(e => e.Text).IsRequired();
            });

            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Text)
                    .IsRequired();
            });

            modelBuilder.Entity<ReviewTemplateFields>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.DefaultValue);
            });

            modelBuilder.Entity<ReviewTemplates>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(40)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LinkedToType)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<WebsiteFields>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.DefaultValue)
                    .IsRequired();
            });

            modelBuilder.Entity<WebsiteFiles>(entity =>
            {
                entity.Property(e => e.OriginalPath)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CompressedPath)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Alt)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<WebsiteResources>(entity =>
            {
                entity.Property(e => e.Text).IsRequired();
            });

            modelBuilder.Entity<Websites>(entity =>
            {
                entity.Property(e => e.Domain)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Extension)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Folder)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Subdomain)
                    .HasMaxLength(200);

                entity.Property(e => e.TypeClient)
                    .HasMaxLength(20);

                entity.Property(e => e.RootPageAlternateGuid)
                    .HasMaxLength(450);

                entity.Property(e => e.Subtitle)
                    .HasMaxLength(60);
            });

            modelBuilder.Entity<WebsiteUploads>(entity =>
            {
                entity.Property(e => e.CallName)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MimeTypes)
                    .IsRequired()
                    .HasColumnType("varchar(120)");

                entity.Property(e => e.FileExtensions)
                    .IsRequired()
                    .HasColumnType("varchar(70)");
            });
        }
    }
}