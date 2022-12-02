using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Data;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.Session;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Logic.WebDiscordBridge;
using Dikubot.Discord;
using Dikubot.Webapp.Authentication.Discord.OAuth2;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;


namespace Dikubot.Webapp.Authentication
{
    public class Authenticator : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccssor;
        private SocketGuild _guild;
        private readonly UserGlobalMongoService _userGlobalMongoService;

        public Authenticator(IHttpContextAccessor httpContextAccssor,
            UserGlobalMongoService userGlobalMongoService)
        {
            _httpContextAccssor = httpContextAccssor;
            _userGlobalMongoService = userGlobalMongoService;
        }
        
        /// <summary>
        /// Get the current UserGlobalModel
        /// </summary>
        /// <returns>Returns a nullable UserGlobalModel based on the current session</returns>
        public async Task<UserGlobalModel> GetUserGlobal()
        {
            
            var authState = await GetAuthenticationStateAsync();
            UserIdentity user = (UserIdentity) authState.User.Identity!;
            return user?.UserGlobalModel;
        }
        
        /// <summary>
        /// Gets the user's discord oauth2 access token
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<string> GetTokenAsync()
        {
            if (!_httpContextAccssor.HttpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var tk = await _httpContextAccssor.HttpContext.GetTokenAsync("Discord", "access_token");
            return tk;
        }
        
        /// <summary>
        /// Get SocketGuild of the current session
        /// </summary>
        /// <returns></returns>
        public async Task<SocketGuild> GetSocketGuild()
        {
            return await GetSocketGuild(await GetUserGlobal());
        }
        
        private async Task<SocketGuild> GetSocketGuild(UserGlobalModel user)
        {
            return user == null ? null : DiscordBot.ClientStatic.GetGuild(user.SelectedGuild);
        }
        

        private DiscordUserClaim GetDiscordUserClaim()
        {
            if (_httpContextAccssor.HttpContext == null)
            {
                return null;
            }
            return GetInfo(_httpContextAccssor.HttpContext);
        }
        
        
        public DiscordUserClaim GetInfo(HttpContext httpContext)
        {
            if (!httpContext.User.Identity?.IsAuthenticated ?? false)
            {
                return null;
            }

            var claims = httpContext.User.Claims;
            bool? verified;
            if (bool.TryParse(claims.FirstOrDefault(x => x.Type == "urn:discord:verified")?.Value, out var _verified))
            {
                verified = _verified;
            }
            else
            {
                verified = null;
            }
            
            var userClaim = new DiscordUserClaim
            {
                UserId = ulong.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value),
                Name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value,
                Discriminator = claims.FirstOrDefault(x => x.Type == "urn:discord:discriminator")?.Value,
                Avatar = claims.FirstOrDefault(x => x.Type == "urn:discord:avatar")?.Value,
                Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                Verified = verified
            };

            return userClaim;
        }

        /// <summary>
        /// GetAuthenticationStateAsync is used to set the AuthenticationState based on a UserIdentity.
        /// </summary>
        /// <returns>Task</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            
            DiscordUserClaim discordUserClaim = GetDiscordUserClaim();
            if (discordUserClaim == null)
            {
                //Returns an empty UserIdentity, meaning the user isn't authorized to do anything
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity())));
            }
            
            //Here we return a UserIdentity with our sessionModel. The system will then check the user connected to the session,
            //to see what authorisation step the user is at.
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(discordUserClaim, _userGlobalMongoService))));
        }

        /// <summary>
        /// Updates the session in localStorage as well as tells the AuthenticationState that the session has been updated
        /// </summary>
        /// <param name="sessionModel">The current session will be set to this model</param>
        /// <returns>Task</returns>
        public async Task UpdateSession()
        {
            DiscordUserClaim discordUserClaim = GetDiscordUserClaim();
            UserIdentity userIdentity =
                discordUserClaim == null ? new UserIdentity() : new UserIdentity(discordUserClaim, _userGlobalMongoService);
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(new ClaimsPrincipal(userIdentity))));
        }
    }
}