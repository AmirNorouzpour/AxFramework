using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Symbol
    {
        public Symbol()
        {
        }

        public Symbol(string title)
        {
            Title = title;
        }

        public Symbol(string title, int decimals, int qd)
        {
            Title = title;
            Decimals = decimals;
            Qd = qd;
        }

        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Decimals { get; set; }
        public int Qd { get; set; }
        public DateTime LastUpdate { get; set; }
        public decimal? Price { get; set; }
        public bool IsActive { get; set; }
    }
}
