﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/bvorg.Master" Inherits="System.Web.Mvc.ViewPage<CMSWeb.Models.RecRegModel>" %>

<asp:Content ID="registerHead" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Recreation Registration</title>
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
<% var IsAdult = Model.IsAdult(); %>
    <h2>Register for <%=Model.division.Name %></h2>
    <% using (Html.BeginForm()) 
    { %>
        <div>
            <fieldset>
                <table style="empty-cells:show">
                <col style="width: 13em; text-align:right" />
                <col />
                <col />
                <tr>
                    <td><label for="shirtsize">ShirtSize</label></td>
                    <td><%= Html.DropDownList("shirtsize", CMSWeb.Models.RecRegModel.ShirtSizes())%></td>
                    <td><%= Html.ValidationMessage("shirtsize")%></td>
                </tr>
                <tr>
                    <td><label for="request">Request Teammate</label></td>
                    <td><%= Html.TextBox("request") %></td>
                    <td><%= Html.ValidationMessage("request") %></td>
                </tr>
                <tr>
                    <td><label for="emcontact">Emergency Friend</label></td>
                    <td><%= Html.TextBox("emcontact") %></td>
                    <td><%= Html.ValidationMessage("emcontact")%></td>
                </tr>
                <tr>
                    <td><label for="emphone">Emergency Phone</label></td>
                    <td><%= Html.TextBox("emphone")%></td>
                    <td><%= Html.ValidationMessage("emphone")%></td>
                </tr>

                <tr>
                    <td><label for="insurance">Health Insurance Carrier</label></td>
                    <td><%= Html.TextBox("insurance")%></td>
                    <td><%= Html.ValidationMessage("insurance")%></td>
                </tr>
                <tr>
                    <td><label for="policy">Policy #</label></td>
                    <td><%= Html.TextBox("policy")%></td>
                    <td><%= Html.ValidationMessage("policy")%></td>
                </tr>
                <tr>
                    <td><label for="doctor">Family Physician Name</label></td>
                    <td><%= Html.TextBox("doctor")%></td>
                    <td><%= Html.ValidationMessage("doctor")%></td>
                </tr>
                <tr>
                    <td><label for="docphone">Family Physician Phone</label></td>
                    <td><%= Html.TextBox("docphone")%></td>
                    <td><%= Html.ValidationMessage("docphone")%></td>
                </tr>

<% if (!IsAdult)
   { %>
                <tr>
                    <td><label for="medical">Allergies or<br />
                           Medical Problems</label></td>
                    <td><%= Html.TextArea("medical")%></td>
                    <td><%= Html.ValidationMessage("medical")%>Leave blank if none</td>
                </tr>
                <tr>
                    <td><label for="mname">Mother's Name (first last)</label></td>
                    <td><%= Html.TextBox("mname")%></td>
                    <td><%= Html.ValidationMessage("mname")%></td>
                </tr>
                <tr>
                    <td><label for="fname">Father's Name (first last)</label></td>
                    <td><%= Html.TextBox("fname")%></td>
                    <td><%= Html.ValidationMessage("fname")%></td>
                </tr>
<% } %>
                 <tr>
                    <td><label for="coaching">Interested in Coaching?</label></td>
                    <td><%= Html.RadioButton("coaching", 1) %> Yes
                    <%= Html.RadioButton("coaching", 0) %> No</td>
                    <td><%= Html.ValidationMessage("coaching2") %></td>
                </tr>
                <tr>
                    <td><label for="church"><%= !IsAdult ? "Parent's Church" : "Church" %></label></td>
                    <td><%= Html.CheckBox("member") %> Member of Bellevue<br />
                    <%= Html.CheckBox("otherchurch") %> Active in another Local Church</td>
                    <td><%= Html.ValidationMessage("member")%></td>
                </tr>
                <tr>
                    <td>&nbsp;</td><td><input type="submit" value="Next Page" /></td>
                </tr>
                </table>
            </fieldset>
        </div>       
        <%= Html.Hidden("peopleid", Model.peopleid) %>
        <%= Html.Hidden("divid", Model.divid) %>
        <%= Html.Hidden("orgid", Model.orgid) %>
        <%= Html.Hidden("first", Model.first) %> 
        <%= Html.Hidden("last", Model.last) %> 
        <%= Html.Hidden("dob", Model.dob) %> 
        <%= Html.Hidden("phone", Model.phone) %> 
        <%= Html.Hidden("homecell", Model.homecell) %> 
        <%= Html.Hidden("gender", Model.gender) %> 
        <%= Html.Hidden("email", Model.email) %> 
        <%= Html.Hidden("shownew", Model.shownew) %> 
        <%= Html.Hidden("addr", Model.addr) %> 
        <%= Html.Hidden("zip", Model.zip) %> 
        <%= Html.Hidden("city", Model.city) %> 
        <%= Html.Hidden("state", Model.state) %> 
    <% } %>

</asp:Content>