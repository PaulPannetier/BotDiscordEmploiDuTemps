using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BGLeopold
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")] //on devra utilisé !ping pour appelé la commande
        public async Task PingAsync()
        {
            await ReplyAsync("Pong");
        }

        [Command("avatar")] //on devra utilisé !avatar pour appelé la commande
        public async Task AvatarAsync(ushort size = 512)
        {
            await ReplyAsync(CDN.GetUserAvatarUrl(Context.User.Id, Context.User.AvatarId, size, ImageFormat.Auto));
        }

        [Command("react")]
        public async Task ReactAsync(string pMessage, string pEmoji)
        {
            var message = await Context.Channel.SendMessageAsync(pMessage);
            var emoji = new Emoji(pEmoji);
            await message.AddReactionAsync(emoji);
        }
    }
}
