using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SceneBuilderBot.Services.Repositories
{
    public class InMemoryUserRepository : IUserRepository {
        static List<string> _userTable = new List<string>();

        private InMemoryUserRepository() { }

        public static IUserRepository Create() {
            return new InMemoryUserRepository();
        }

        public bool UserExists(string id) {
            var exists = _userTable.Where(u => u == id).Count() > 0;
            if(!exists) {
                _userTable.Add(id);
            }
            return exists;
        }
    }
}
