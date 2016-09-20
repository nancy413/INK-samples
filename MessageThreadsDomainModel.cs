using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Web.Domain
{
    public class MessageThreads
    {
        public int Id { get; set; }

        public string ThreadTitle { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastMessageDate { get; set; }

        public int CommentCount { get; set; }

        public int ViewCount { get; set; }

        public UserMini UserInfo { get; set; }

        public Groups GroupInfo { get; set; }

    }
}
