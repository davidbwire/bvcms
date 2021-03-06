﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportExcel.aspx.cs" Inherits="CmsWeb.ExportExcel1" EnableEventValidation="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns = "false" Font-Names = "Arial" >
            <Columns>
                <asp:TemplateField HeaderText="Picture" ItemStyle-Height = "203" ItemStyle-Width = "160">
                    <ItemTemplate>
                        <img src='<%# Eval("Image") %>' runat="server" width="160" height="200" />
                    </ItemTemplate> 
                </asp:TemplateField>
                <asp:BoundField DataField = "PeopleId" HeaderText = "PeopleId" />
                <asp:BoundField DataField = "Title" HeaderText = "Title"/>
                <asp:BoundField DataField = "FirstName" HeaderText = "First" />
                <asp:BoundField DataField = "LastName" HeaderText = "Last" />
                <asp:BoundField DataField = "Address" HeaderText = "Address1" />
                <asp:BoundField DataField = "Address2" HeaderText = "Address2" />
                <asp:BoundField DataField = "City" HeaderText = "City" />
                <asp:BoundField DataField = "State" HeaderText = "State" />
                <asp:BoundField DataField = "Zip" HeaderText = "Zip">
                    <ItemStyle CssClass="Text" />
                </asp:BoundField>
                <asp:BoundField DataField = "Email" HeaderText = "Email" />
                <asp:BoundField DataField = "BirthDate" HeaderText = "BirthDate" />
                <asp:BoundField DataField = "BirthDay" HeaderText = "BirthDay" />
                <asp:BoundField DataField = "Anniversary" HeaderText = "Anniversary" />
                <asp:BoundField DataField = "JoinDate" HeaderText = "JoinDate" />
                <asp:BoundField DataField = "JoinType" HeaderText = "JoinType" />
                <asp:BoundField DataField = "HomePhone" HeaderText = "HomePhone" />
                <asp:BoundField DataField = "CellPhone" HeaderText = "CellPhone" />
                <asp:BoundField DataField = "WorkPhone" HeaderText = "WorkPhone" />
                <asp:BoundField DataField = "MemberStatus" HeaderText = "Church" />
                <asp:BoundField DataField = "FellowshipLeader" HeaderText = "Teacher" />
                <asp:BoundField DataField = "Spouse" HeaderText = "Spouse" />
                <asp:BoundField DataField = "Age" HeaderText = "Age" />
                <asp:BoundField DataField = "School" HeaderText = "School" />
                <asp:BoundField DataField = "Grade" HeaderText = "Grade" />
                <asp:BoundField DataField = "AttendPctBF" HeaderText = "AttPct" />
                <asp:BoundField DataField = "Married" HeaderText = "Marital" />
                <asp:BoundField DataField = "FamilyId" HeaderText = "FamilyId" />
                <asp:BoundField DataField = "Image" HeaderText = "ImageUrl" />
            </Columns> 
        </asp:GridView>
        <asp:GridView ID="FamilyMembers" runat="server" AutoGenerateColumns = "false" Font-Names = "Arial" >
            <Columns>
                <asp:BoundField DataField = "PeopleId" HeaderText = "PeopleId" />
                <asp:BoundField DataField = "Title" HeaderText = "Title"/>
                <asp:BoundField DataField = "FirstName" HeaderText = "FirstName" />
                <asp:BoundField DataField = "LastName" HeaderText = "LastName" />
                <asp:BoundField DataField = "Address" HeaderText = "Address" />
                <asp:BoundField DataField = "Address2" HeaderText = "Address2" />
                <asp:BoundField DataField = "City" HeaderText = "City" />
                <asp:BoundField DataField = "State" HeaderText = "State" />
                <asp:BoundField DataField = "Zip" HeaderText = "Zip">
                    <ItemStyle CssClass="Text" />
                </asp:BoundField>
                <asp:BoundField DataField = "Email" HeaderText = "Email" />
                <asp:BoundField DataField = "BirthDate" HeaderText = "BirthDate" />
                <asp:BoundField DataField = "BirthDay" HeaderText = "BirthDay" />
                <asp:BoundField DataField = "JoinDate" HeaderText = "JoinDate" />
                <asp:BoundField DataField = "HomePhone" HeaderText = "HomePhone" />
                <asp:BoundField DataField = "CellPhone" HeaderText = "CellPhone" />
                <asp:BoundField DataField = "WorkPhone" HeaderText = "WorkPhone" />
                <asp:BoundField DataField = "MemberStatus" HeaderText = "Church" />
                <asp:BoundField DataField = "Age" HeaderText = "Age" />
                <asp:BoundField DataField = "School" HeaderText = "School" />
                <asp:BoundField DataField = "Married" HeaderText = "Marital" />
                <asp:BoundField DataField = "FamilyName" HeaderText = "FamilyName" />
                <asp:BoundField DataField = "FamilyId" HeaderText = "FamilyId" />
                <asp:BoundField DataField = "FamilyPosition" HeaderText = "FamilyPositionId" />
                <asp:BoundField DataField = "Grade" HeaderText = "Grade" />
                <asp:BoundField DataField = "FellowshipLeader" HeaderText = "Teacher" />
                <asp:BoundField DataField = "AttendPctBF" HeaderText = "AttendPct" />
                <asp:BoundField DataField = "FellowshipClass" HeaderText = "FellowshipClass" />
                <asp:BoundField DataField = "AltName" HeaderText = "AltName" />
            </Columns> 
        </asp:GridView>
    </div>
    </form>
</body>
</html>

