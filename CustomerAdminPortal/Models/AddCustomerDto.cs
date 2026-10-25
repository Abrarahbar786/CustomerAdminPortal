namespace CustomerAdminPortal.Models
{
    public class AddCustomerDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required decimal Salary { get; set; }
    }
}
