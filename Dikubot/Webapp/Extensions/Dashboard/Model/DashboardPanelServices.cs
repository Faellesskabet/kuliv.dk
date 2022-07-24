using System.Linq;
using Dikubot.Discord;
using Dikubot.Extensions.Models.Equipment;
using Dikubot.Webapp.Extensions.Dashboard.Model;
using Discord.WebSocket;
using Syncfusion.Blazor.Layouts;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Equipment
{
    public class DashboardPanelServices : GuildServices<DashboardPanelModel.DashboardPanel>
    {
        public DashboardPanelServices(SocketGuild guild) : base("DashboardPanel", guild)
        {
        }
        public DashboardPanelServices(string guidId) : base("DashboardPanel", DiscordBot.Client.Guilds?.FirstOrDefault(g => g.Id.ToString().Equals(guidId)))
        {
        }
        
        public void SavePanel(PanelModel panelModel)
        {
            DashboardPanelServices panelServices = this;
            DashboardPanelModel.DashboardPanel dashboardPanel  = panelServices.Get().First(model => model.Id.ToString() == panelModel.Id);
            
            dashboardPanel.Row = panelModel.Row;
            dashboardPanel.Col = panelModel.Column;
            dashboardPanel.SizeX = panelModel.SizeX;
            dashboardPanel.SizeY = panelModel.SizeY;

            panelServices.Upsert(dashboardPanel);

        }

    }
}