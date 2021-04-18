using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeneralAPI.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required ,StringLength(20)]
        public string FirstName { get; set; }
        [Required, StringLength(20)]
        public string LastName { get; set; }
        public string Fullname
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}