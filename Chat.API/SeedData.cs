using Chat.API.Database;
using Microsoft.EntityFrameworkCore;

namespace Chat.API
{
    public class SeedData
    {
        public static void EnsureSeedData(WebApplication app)
        {
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ChatContext>();
                if (context == null) return;
                context.Database.Migrate();

                if (context.ChatRoom.Any()) return;
                context.ChatRoom.Add(new ChatRoom
                {
                    Id = 1,
                    Name = "Default Room",
                    Description = "Default Room Chat"
                });
                context.ChatRoom.Add(new ChatRoom
                {
                    Id = 2,
                    Name = "Second Room",
                    Description = "Second Room Chat"
                });
                context.SaveChanges();
            }
        }
    }
}
