using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWSData.Model
{
    /// <summary>
    /// 实体树信息类
    /// </summary>
    public class Dnl_EntityTree
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public ObjectId UsrId { get; set; }
        public ObjectId ParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 实体名称，此时属性名为null
        /// </summary>
        public EntityAndVariant Entity { get; set; }
        /// <summary>
        /// 实体相关属性
        /// </summary>
        public List<AttrAndVariant> Attributes { get; set; }
        public bool IsDel { get; set; }
        /// <summary>
        /// 图片位置
        /// </summary>
        public string PicUrl { get; set; }
    }

    /// <summary>
    /// 实体信息及其变种
    /// </summary>
    public class EntityAndVariant
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 其可能变种
        /// </summary>
        public List<string> Varients { get; set; }
    }

    /// <summary>
    /// 属性信息及其变种
    /// </summary>
    public class AttrAndVariant
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 属性值可能变种
        /// </summary>
        public List<string> Varients { get; set; }
    }

    /// <summary>
    /// 实体树类
    /// </summary>
    public class Dnl_EntityTreeDto
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 实体名称，此时属性名为null
        /// </summary>
        public EntityAndVariant Entity { get; set; }
        /// <summary>
        /// 实体相关属性
        /// </summary>
        public List<AttrAndVariant> Attributes { get; set; }
        /// <summary>
        /// 子树
        /// </summary>
        public List<Dnl_EntityTreeDto> Children { get; set; }
        /// <summary>
        /// 图片位置
        /// </summary>
        public string PicUrl { get; set; }
    }
}
