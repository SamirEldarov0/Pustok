using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pustok2.Models;
using System;
using System.Threading.Tasks;

namespace Pustok2
{
    public class PustokHub:Hub
    {
        private readonly UserManager<AppUser> _userManager;

        public PustokHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser appUser = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                appUser.ConnectionId = Context.ConnectionId;
                var result = _userManager.UpdateAsync(appUser).Result;
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser appUser = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                appUser.ConnectionId = null;
                var result = _userManager.UpdateAsync(appUser).Result;
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
