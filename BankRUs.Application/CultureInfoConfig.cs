using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankRUs.Application
{
    public class CultureInfoConfig
    {
        [Required]
        public readonly string SystemCulture = string.Empty;
    }
}
