using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 公有微信链接类
    /// </summary>
    public class WeiXinLinkCommentMongo
    {
        /// <summary>
        /// mongodb唯一标识Id
        /// </summary>
        public ObjectId _id { get; set; }
        /// <summary>
        /// 数据创建时间
        /// </summary>
        public DateTime CreateAt { get; set; }
        /// <summary>
        /// 链接Id
        /// </summary>
        public ObjectId LinkId { get; set; }
        /// <summary>
        /// 评论Id
        /// </summary>
        public int CommentId { get; set; }
        /// <summary>
        /// 评论正文
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 评论者呢称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 评论者头像链接
        /// </summary>
        public string LogoUrl { get; set; }
        /// <summary>
        /// 评论创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeNum { get; set; }
        /// <summary>
        /// 评论获取时间
        /// </summary>
        public DateTime GetTime { get; set; }
        /// <summary>
        /// 作者回复列表
        /// </summary>
        public List<AuthorReply> AuReplys { get; set; }
    }

    /// <summary>
    /// 作者回复
    /// </summary>
    public class AuthorReply
    {
        /// <summary>
        /// 回复正文
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 说明文档中未说明，猜测为本条作者回复Id
        /// </summary>
        public int ReplyId { get; set; }
    }
}
