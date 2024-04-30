using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EntityFrameworkCore.EncryptColumn.Attribute;

namespace PowerMinder.Core.Entity
{
    public class AccountContext : DbContext, IDataProtectionKeyContext
    {
        private IEncryptionProvider encryptionProvider = new GenerateEncryptionProvider("TyzenRAccountKey");

        public AccountContext(DbContextOptions<AccountContext> options) : base(options)
        {

        }

        // Shared Cookie encryption key stored at DB  level   
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<UserTokenEntity> UserTokens { get; set; }

        public DbSet<UserSettingEntity> UserSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.UseEncryption(this.encryptionProvider);
            EncryptionConverter valueConverter = new EncryptionConverter(encryptionProvider);
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (IMutableProperty property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && !IsDiscriminator(property) && property.PropertyInfo.GetCustomAttributes(typeof(EncryptColumnAttribute), inherit: false).Any())
                    {
                        property.SetValueConverter(valueConverter);
                    }
                }
            }
            base.OnModelCreating(modelBuilder);

        }
        private static bool IsDiscriminator(IMutableProperty property)
        {
            if (!(property.Name == "Discriminator"))
            {
                return property.PropertyInfo == null;
            }

            return true;
        }

    }

}
