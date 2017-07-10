<%@ Control Language="C#" AutoEventWireup="true" CodeFile="widget.ascx.cs" Inherits="widgets_Silverlight_Tag_Cloud_widget" %>
<div id="silverlightControlHost">
    <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
        width="250" height="250">
        <param name="source" value="widgets/silverlight tag cloud/Slvn.TagCloud.xap" />
        <param name="background" value="white" />
        <param name="minRuntimeVersion" value="4.0.50826.0" />
        <param name="autoUpgrade" value="true" />
        <param name="initParams" value="<%= GetTagsString() %>" />
        <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50826.0" style="text-decoration: none">
            <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight"
                style="border-style: none" />
        </a>
    </object>
    <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px;
        border: 0px"></iframe>
</div>
