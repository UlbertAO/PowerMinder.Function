using EntityFrameworkCore.EncryptColumn.Attribute;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerMinder.Core.Entity
{
    [Table("User")]
    public class UserEntity
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; } = string.Empty;

        [EncryptColumn]
        public string Email { get; set; }

        [EncryptColumn]
        public string? Password { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string AuthProvider { get; set; } = string.Empty;

        public string? TimeZone { get; set; } = string.Empty;

        public bool IsActivated { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedOn { get; set; }

        public string RefreshToken { get; set; } = string.Empty;
    }
}
