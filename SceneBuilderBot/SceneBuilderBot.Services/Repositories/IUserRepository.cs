using System;
using System.Collections.Generic;
using System.Text;

namespace SceneBuilderBot.Services.Repositories
{
    public interface IUserRepository {
        bool ChannelUserExists(string id);
        string RegisterChannelUser(string channelUserId, string name);
    }
}
