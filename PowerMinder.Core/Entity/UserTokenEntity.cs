using System.ComponentModel.DataAnnotations.Schema;

namespace PowerMinder.Core.Entity
{
    [Table("UserToken")]
    public class UserTokenEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string IpAddress { get; set; }

        public string DeviceName { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
