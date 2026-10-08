using System.ComponentModel.DataAnnotations;

namespace FormReportManagement.Models
{
    public class FormSubmission
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        public string Phone { get; set; } = "";

}
}
