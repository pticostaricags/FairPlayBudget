using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.Models.MonthlyBudgetInfo
{
    public class ImportCreditCardMonthlyBudgetInfoModel
    {
        //Ref,Fecha,Tipo,Establecimiento,Monto,Moneda
        public string? Ref { get; set; }
        public string? Fecha { get; set; }
        public string? Tipo { get; set; }
        public string? Establecimiento { get; set; }
        public decimal? Monto { get; set; }
        public string? Moneda { get; set; }
    }
}
