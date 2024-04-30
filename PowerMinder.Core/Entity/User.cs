using PowerMinder.Core.Entity;

namespace PowerMinder.Core.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string? Provider { get; set; }
        public bool? IsConfirmed { get; set; }
        public string TimeZone { get; set; }
    }


    public class TestTimeZone: BaseEntity
    {
      public  DateTime UtcTime { get; set; }
    }


}
