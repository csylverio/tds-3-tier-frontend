using System.ComponentModel.DataAnnotations;

namespace MyFinance.Models;

public class AccountViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome da conta.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    [Display(Name = "Nome")]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999999.99", ErrorMessage = "O saldo deve ser maior ou igual a zero.", ParseLimitsInInvariantCulture = true)]
    [DataType(DataType.Currency)]
    [Display(Name = "Saldo")]
    public decimal Balance { get; set; }

    [Display(Name = "Data de execução")]
    public DateTimeOffset DataExecucao { get; set; }
}
