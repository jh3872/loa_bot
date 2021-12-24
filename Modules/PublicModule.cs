using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using loa_bot.Services;

namespace loa_bot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public PictureService PictureService { get; set; }

        [Command("auction")]
        public async Task AuctionAsync()
        {
            double money = 0;
            string rString;
            try
            {
                money = double.Parse(Context.Message.Content);
                double commission = money * 0.05;

                double 분배금4인 = (money * 0.95 / 4);
                double 분배금8인 = (money * 0.95 / 8);

                double 분기점4인 = 분배금4인 * 3;
                double 분기점8인 = 분배금8인 * 7;

                double 추천선입찰4인 = 분기점4인 * 100 / 110;
                double 추천선입찰8인 = 분기점8인 * 100 / 110;

                double 선입찰이득4인 = (money * 0.95) - 추천선입찰4인;
                double 선입찰이득8인 = (money * 0.95) - 추천선입찰8인;

                rString =
                    "```cs\r\n" +
                    "경매가 : " + money.ToString() + "    수수료 : " + commission.ToString() + "\r\n" +

                    "4인 : 최대 입찰 가격 (" + 추천선입찰4인.ToString("#") + ") => 이득 (" + (추천선입찰4인 * 1 / 3).ToString("#") + " + " + (선입찰이득4인 - (추천선입찰4인 * 1 / 3)).ToString("#") + ") > 나머지 3인(" + (추천선입찰4인 * 1 / 3).ToString("#") + ")\r\n" +
                    "4인(1/4) : 손익 분기점 (" + 분기점4인.ToString("#") + ") => 분배금 (" + 분배금4인.ToString("#") + ")\r\n" +
                    "8인 : 최대 입찰 가격 (" + 추천선입찰8인.ToString("#") + ") => 이득 (" + (추천선입찰8인 * 1 / 7).ToString("#") + " + " + (선입찰이득8인 - (추천선입찰8인 * 1 / 7)).ToString("#") + ") > 나머지 7인(" + (추천선입찰8인 * 1 / 7).ToString("#") + ")\r\n" +
                    "8인(1/8) : 손익 분기점 (" + 분기점8인.ToString("#") + ") => 분배금 (" + 분배금8인.ToString("#") + ")\r\n" +
                    "```\r\n";


            }
            catch
            {
                rString =
                    "```cs\r\n" +
                    "#입력값 오류 : " + Context.Message.Content + "\r\n" +
                    "```\r\n";
            }
            await ReplyAsync(rString);
        }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("cat")]
        public async Task CatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await PictureService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= Context.User;

            await ReplyAsync(user.ToString());
        }

        // Ban a user
        [Command("ban")]
        [RequireContext(ContextType.Guild)]
        // make sure the user invoking the command can ban
        [RequireUserPermission(GuildPermission.BanMembers)]
        // make sure the bot itself can ban
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            await user.Guild.AddBanAsync(user, reason: reason);
            await ReplyAsync("ok!");
        }

        // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            // Insert a ZWSP before the text to prevent triggering other bots!
            => ReplyAsync('\u200B' + text);

        // 'params' will parse space-separated elements into a list
        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync("You listed: " + string.Join("; ", objects));

        // Setting a custom ErrorMessage property will help clarify the precondition error
        [Command("guild_only")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public Task GuildOnlyCommand()
            => ReplyAsync("Nothing to see here!");
    }
}
