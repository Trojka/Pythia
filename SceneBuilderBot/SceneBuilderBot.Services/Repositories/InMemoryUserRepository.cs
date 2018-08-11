using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SceneBuilderBot.Services.Model;

namespace SceneBuilderBot.Services.Repositories
{
    public class InMemoryUserRepository : IUserRepository {
        List<ChannelUser> _channelUserTable = new List<ChannelUser>();
        List<BotBuilderUser> _botBuilderUserTable = new List<BotBuilderUser>();

        static InMemoryUserRepository _me;

        private InMemoryUserRepository() { }

        public static IUserRepository Get() {
            if(_me == null)
                _me = new InMemoryUserRepository();

            return _me;
        }

        public bool ChannelUserExists(string channelUserId) {
            var exists = _channelUserTable.Where(u => u.ChannelUserId == channelUserId).Count() > 0;
            return exists;
        }

        public string RegisterChannelUser(string channelUserId, string name) {
            var bbUser = CreateBotBuilderUser(name);
            var cUser = CreateChannelUser(channelUserId, bbUser.Id);

            _botBuilderUserTable.Add(bbUser);
            _channelUserTable.Add(cUser);

            return bbUser.Id;
        }

        private BotBuilderUser CreateBotBuilderUser(string name) {
            var bbUser = new BotBuilderUser() { Id = Guid.NewGuid().ToString(), Name = name };
            return bbUser;
        }

        private ChannelUser CreateChannelUser(string channelUserId, string botBuilderUserId) {
            var cUser = new ChannelUser() { ChannelUserId = channelUserId, BotBuilderUserId = botBuilderUserId };
            return cUser;
        }
    }
}
