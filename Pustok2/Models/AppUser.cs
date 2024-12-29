using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class AppUser:IdentityUser
    {
        //PhoneNumberConfirmed-telefon nomresi tesdiqlenipmi
        //PasswordHash sifrenin bazada aciq sekildesaxlanilmasinin qarsisini alir
        public string FullName { get; set; }

        public bool IsAdmin { get; set; }//adminlerin sayi  coxalir admin
        //super admin,admin,book admin,edit admin ve s
        //IsAdmin==false member ve ,member managa/account/login ola bilmez
        //Adminde account/login ola bilmez
        //panellerin sayi artarsa userpanel(member p),admin panel,campani panel isleri paylasan,
        //enumda saxlaya biler
        [StringLength(maximumLength:40)]
        public string Country { get; set; }
        [StringLength(maximumLength: 40)]
        public string City { get; set; }
        [StringLength(maximumLength: 40)]
        public string State { get; set; }
        [StringLength(maximumLength: 250)]
        public string Address { get; set; }
        public string ConnectionId { get; set; }//Elave olaraq IsOnline,LastOnlineDate-ni tuta bilerik
        //Meqsed,15 nomreli order alan user onlinedirse ona toastr cixarmaq
        public List<BasketItem> BasketItems { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Order> Orders { get; set; }

    }
}
 