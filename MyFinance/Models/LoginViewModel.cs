using System.ComponentModel.DataAnnotations;

namespace MyFinance.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Informe o usuário.")]
    [StringLength(100, ErrorMessage = "O usuário deve ter no máximo 100 caracteres.")]
    [Display(Name = "Usuário")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Informe a senha.")]
    [StringLength(200, ErrorMessage = "A senha deve ter no máximo 200 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string? Password { get; set; }

    public string? ReturnUrl { get; set; }
}
