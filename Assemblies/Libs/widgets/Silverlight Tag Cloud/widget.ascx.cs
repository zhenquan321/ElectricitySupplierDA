#region Project Description
/* 
 *                                    PROJECT DESCRIPTION
 * -------------------------------------------------------------------------------------
 * Class		: widgets_Silverlight_Tag_Cloud_widget
 * Developer	: Silverlight VN
 * 
 */
#endregion

#region Record of Change
//             							CHANGE HISTORY
// -------------------------------------------------------------------------------------
// |   DATE    | DEVELOPER  | DESCRIPTION                                              |
// -------------------------------------------------------------------------------------
// | 25-Nov-10 | Slvn       | First creation.                                          |
// -------------------------------------------------------------------------------------
//
#endregion

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using BlogEngine.Core;
#endregion

public partial class widgets_Silverlight_Tag_Cloud_widget : WidgetBase
{
    #region Fields

    private static object mSyncRoot = new object();
    private static IList<Tag> mTags = null;
    private const string Link = "{0}?tag=/{1}";

    #endregion

    /// <summary>
    /// This method works as a substitute for Page_Load. You should use this method for
    /// data binding etc. instead of Page_Load.
    /// </summary>
    public override void LoadWidget()
    {
        // Nothing to load
    }

    static widgets_Silverlight_Tag_Cloud_widget()
    {
        Post.Saved += delegate { mTags = null; };
    }

    /// <summary>
    /// Gets the name. It must be exactly the same as the folder that contains the widget.
    /// </summary>
    /// <value></value>
    public override string Name
    {
        get { return "Silverlight Tag Cloud"; }
    }

    /// <summary>
    /// Gets wether or not the widget can be edited.
    /// <remarks>
    /// The only way a widget can be editable is by adding a edit.ascx file to the widget folder.
    /// </remarks>
    /// </summary>
    /// <value></value>
    public override bool IsEditable
    {
        get { return false; }
    }

    /// <summary>
    /// Gets a value indicating if the header is visible. This only takes effect if the widgets isn't editable.
    /// </summary>
    /// <value><c>true</c> if [display header]; otherwise, <c>false</c>.</value>
    public override bool DisplayHeader
    {
        get { return true; }
    }

    protected string GetTagsString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("tags=<tags>");
        foreach (Tag tag in GetTags())
        {
            sb.Append(string.Format("<tag name='{0}' link='{1}' count='{2}' />", tag.Name, tag.Link, tag.Count));
        }
        sb.Append("</tags>");
        return sb.ToString();
    }

    private IList<Tag> GetTags()
    {
        if (mTags == null)
        {
            lock (mSyncRoot)
            {
                if (mTags == null)
                {
                    List<Tag> tagList = new List<Tag>();

                    Tag tagInfo;

                    SortedDictionary<string, int> dic = CreateRawList();

                    foreach (string tag in dic.Keys)
                    {
                        tagInfo = new Tag();
                        tagInfo.Link = string.Format(Link, Utils.AbsoluteWebRoot, HttpUtility.UrlEncode(tag));
                        tagInfo.Name = tag;
                        tagInfo.Count = dic[tag];
                        tagList.Add(tagInfo);
                    }

                    mTags = tagList;
                }
            }
        }

        return mTags;
    }

    /// <summary>
    /// Builds a raw list of all tags and the number of times
    /// they have been added to a post.
    /// </summary>
    private static SortedDictionary<string, int> CreateRawList()
    {
        SortedDictionary<string, int> dic = new SortedDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
        foreach (Post post in Post.Posts)
        {
            if (post.IsVisibleToPublic)
            {
                foreach (string tag in post.Tags)
                {
                    if (dic.ContainsKey(tag))
                        dic[tag]++;
                    else
                        dic[tag] = 1;
                }
            }
        }
        return dic;
    }

    private class Tag
    {
        public string Name;
        public string Link;
        public int Count;
    }
}
