namespace Accounting.Models.UserViewModels
{
    public class UserDetailsViewModel
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CreatedById { get; set; }
        public DateTime Created { get; set; }
    }
}