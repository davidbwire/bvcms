﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CmsData.Person.Duplicate>" %>
    <tr>
        <td valign="top"><a href="/Person/Index/<%=Model.PeopleId%>"><%=Model.First%></a><br />
            <%=Model.Nick %><br />
            <%=Model.Middle %></td>
        <td valign="top"><%=Model.Last %><br />
            <%=Model.Maiden %></td>
        <td valign="top"><%=Util.FormatBirthday(Model.BYear, Model.BMon, Model.BDay) %></td>
        <td valign="top"><%=Model.Email%><br /><br />
            <%=Model.Member %></td>
        <td valign="top"><%=Model.FamAddr %><br />
            <%=Model.PerAddr %></td>
    </tr>