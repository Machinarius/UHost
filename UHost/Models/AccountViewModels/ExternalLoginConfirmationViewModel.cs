using System.ComponentModel.DataAnnotations;

namespace UHost.Models.AccountViewModels {
  public class ExternalLoginConfirmationViewModel {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
