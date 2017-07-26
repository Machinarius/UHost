using System.ComponentModel.DataAnnotations;

namespace UHost.Models.AccountViewModels {
  public class ForgotPasswordViewModel {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
