using System;
using System.Collections.Generic;
using System.Text;

namespace SceneBuilderBot.Services.Repositories
{
    public interface IUserRepository {
        bool UserExists(string id);
    }
}
