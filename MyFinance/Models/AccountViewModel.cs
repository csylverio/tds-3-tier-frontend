using System;

namespace MyFinance.Models;

public class AccountViewModel
{
        public int Id { get; set; }
        public required string Name { get; set; }
        public double Balance { get; set; }
        public DateTime  DataExecucao { get; set; }
}
