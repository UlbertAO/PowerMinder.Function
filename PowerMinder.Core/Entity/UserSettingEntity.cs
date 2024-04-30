using System.ComponentModel.DataAnnotations.Schema;

namespace PowerMinder.Core.Entity
{
    [Table("UserSetting")]
    public class UserSettingEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string AppId { get; set; }

        public string Setting { get; set; }
    }
}
