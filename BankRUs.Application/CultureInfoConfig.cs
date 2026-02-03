using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankRUs.Application;

public class CultureInfoConfig
{

}
public partial class ApiConfig
{
    [Required]
    public string SystemCulture { get; set; } = string.Empty;
}
