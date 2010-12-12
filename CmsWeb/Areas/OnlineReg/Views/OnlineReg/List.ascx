﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CmsWeb.Models.OnlineRegModel>" %>
<%
    Html.RenderPartial("ModelHidden", Model);
    if (Model.DisplayLogin())
    // nobody registered yet
        Html.RenderPartial("Login", Model);
    else
    { %>
<table cellpadding="0" width="100%">
<%      if (!Model.OnlyOneAllowed() && Model.List.Count > 1)
        // label is necessary if more than one
        { %>
    <tr><td><div class="instruct">Registrants</div></td></tr>
<%      }
        for (var i = 0; i < Model.List.Count; i++)
        // show each registrant, including the one being worked on
        {
            Model.current = Model.List[i];
            Model.current.index = i;
            Model.current.LastItem = (i == (Model.List.Count - 1));
            Html.RenderPartial("Registrant", Model);
        } %>
</table>
<% 
    Html.RenderPartial("Buttons", Model);
    } %>