using System;
using System.Linq;
using Dikubot.Database.Models.Course;
using Discord.WebSocket;

namespace Dikubot.Permissions
{
    public partial class PermissionsService
    {

        public void CreateMissingDiscordRolesAndChannels(CourseModel courseModel)
        {
            SocketTextChannel mainChannel = guild.GetTextChannel(Convert.ToUInt64(courseModel.MainTextChannelModel.DiscordId)) 
                                            ?? guild.TextChannels.First(channel => channel.Name == courseModel.Name);

            SocketRole memberRole = guild.GetRole(Convert.ToUInt64(_roleServices.Get(courseModel.MemberRoleModelId).DiscordId));

            if (memberRole == null)
            {
                
            }
            
            if (mainChannel == null)
            {
                SocketCategoryChannel categoryChannel = guild.CategoryChannels.First(category => category.Name == "Kursus");
                if (categoryChannel == null)
                {
                    throw new MissingCourseChannel();
                }

            }
        }
        
    }

    public class MissingCourseChannel : Exception
    {
        
    }
}