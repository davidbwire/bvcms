<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<table id="toolbar">
<tr>
    <td class="dropdown">
        <div><a href='/Email/Index/<%=ViewData["queryid"]%>''><img src="/images/Mail.png" /> 
        Email</a></div>
        <div class="sublinks">
            <a href='/Email/Index/<%=ViewData["queryid"]%>'><img src="/images/Mail.png" /> 
                Individuals</a>
            <a href='/Email/Index/<%=ViewData["queryid"]%>?parents=true'><img src="/images/Mail.png" /> 
                Parents</a>
        </div>
    </td>
    <td class="dropdown">
        <div><a href='#'><img src="/images/BulkMailing.png" /> 
        Export</a></div>
        <div class="sublinks">
            <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"]%>' class="ChooseLabelType" 
                title="For mail merge"><img src="/images/Excel.png" /> 
                Excel</a>
            <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"]%>&format=AllFamily'
                title="For mail merge"><img src="/images/Excel.png" /> 
                Excel Family</a>
            <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"]%>&format=IndividualPicture' 
                title="For picture directory word merge"><img src="/images/Excel.png" /> 
                Excel Pictures</a>
            <a href='/Export/UpdatePeople/<%=ViewData["queryid"]%>' 
                title="For Doing a Mass Update"><img src="/images/Excel.png" /> 
                Excel Update</a>
            <a href='/bulkmail.aspx?id=<%=ViewData["queryid"]%>' class="ChooseLabelType" 
                title="Comma separated values text file, opens in excel, for bulk mailings"><img src="/images/Excel.png" /> 
                Bulk (csv)</a>
            <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"]%>&format=Library'
                title="For Atrium Library Import"><img src="/images/Excel.png" /> 
                Excel Library</a>
<% if ((bool?)ViewData["OrganizationContext"] ?? false)
   { %>
            
                <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"] %>&format=Organization' title="Includes Org Member info"><img src="/images/Excel.png" /> 
                    Member Export</a>
    <% if ((bool?)ViewData["OrgMemberContext"] ?? false)
       { %>
            
                <a href='/ExportExcel.aspx?format=Groups' title="Includes Org Member and Small Group columns"><img src="/images/Excel.png" /> 
                    Groups Export</a>
    <% } %>
            
                <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"] %>&format=Promotion' title="Just for Promotion Mail Merge"><img src="/images/Excel.png" />
                    Promotion Export</a>
<% } %>
        </div>
    </td>
    <td class="dropdown">
        <div><a href='#'><img src="/images/Report.png" /> 
        Reports</a></div>
        <div class="sublinks">
            <a href='/Reports/Prospect/<%=ViewData["queryid"]%>'
                target="_blank"><img src="/images/Report.png" /> 
                Inreach/Outreach</a>
            <a href='/Reports/Prospect/<%=ViewData["queryid"]%>?Form=true' 
                target="_blank"><img src="/images/Report.png" /> 
                Inreach/Outreach with Form</a>
            <a href='/Reports/Contacts/<%=ViewData["queryid"]%>' 
                target="_blank" title="Report for Robo-calling Contacts"><img src="/images/Report.png" />
                Contact Report</a>
            <a href='/Reports/WeeklyAttendance/<%=ViewData["queryid"]%>' 
                target="_blank" title="General Attendance Stats"><img src="/images/Report.png" />
                Weekly Attend</a>
            <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"]%>&format=Involvement' 
                target="_blank" title="Personal, Contact and Enrollment Info"><img src="/images/Excel.png" />
                Involvement</a>
            <a href='/Reports/Family/<%=ViewData["queryid"]%>' 
                target="_blank"><img src="/images/Report.png" /> 
                Family Report</a>
            <a href='/Volunteers/Index/<%=ViewData["queryid"]%>' 
                target="_blank"><img src="/images/Report.png" /> 
                Volunteer Report</a>
<% if ((bool?)ViewData["OrganizationContext"] ?? false)
   { %>
            <a href='/Reports/Registration/<%=ViewData["queryid"]%>?oid=<%=Util2.CurrentOrgId %>' 
                target="_blank"><img src="/images/Report.png" /> 
                Registration Report</a>
<% }
   else
   { %>
            <a href='/Reports/Registration/<%=ViewData["queryid"]%>' 
                target="_blank"><img src="/images/Report.png" /> 
                Registration Rpt</a>
<% } %>   
            <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"]%>&format=Attend' 
                target="_blank" title="Contains attendance information for their class"><img src="/images/Excel.png" />
                BF Attendance</a>
            <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"]%>&format=Children' 
                target="_blank" title="Contains emergency contact, who brought child info"><img src="/images/Excel.png" />
                Children</a>
            <a href='/ExportExcel.aspx?id=<%=ViewData["queryid"]%>&format=Church' 
                target="_blank" title="Contains other Church Info"><img src="/images/Excel.png" />
                Other Churches</a>
        </div>
    </td>
    <td class="dropdown">
        <div><a href='#'><img src="/images/BulkMailing.png" /> 
        Labels</a></div>
        <div class="sublinks">
            <a href='/Reports/RollLabels/<%=ViewData["queryid"]%>' class="ChooseLabelType" 
                title="Labels (pdf for Datamax label printer)" target="_blank"><img src="/images/tags.png" /> 
                Roll Labels</a>
<% if ((bool?)ViewData["OrganizationContext"] ?? false)
   { %>                
            
            <a id="RollsheetLink" href='#' title="Rollsheet Report"><img src="/images/tags.png" />
                Rollsheet Report</a>
<% } %>
            <a href='/Reports/BarCodeLabels/<%=ViewData["queryid"]%>' 
                target="_blank" title="Labels for Choir Attendance"><img src="/images/tags.png" />
                Barcode Labels</a>
            <a href='/Reports/Avery/<%=ViewData["queryid"] %>' 
                title="Avery Name Labels" target="_blank">
                <img src="/images/tags.png" />
                Avery Labels</a>
            <a href='/Reports/Avery3/<%=ViewData["queryid"] %>' 
                title="Avery 3 Across Labels (person per row)" target="_blank">
                <img src="/images/tags.png" />
                Avery Labels 3</a>
            <a href='/Reports/AveryAddress/<%=ViewData["queryid"]%>' class="ChooseLabelType" 
                title="Address Labels"><img src="/images/tags.png" /> 
                Avery Address</a>
        </div>
    </td>
    <td class="dropdown">
        <div><a href='#'><img src="/images/Tag.png" />
        Tag</a></div>
        <div class="sublinks">
            <a id="TagAll" href='<%=ViewData["TagAction"] %>'><img src="/images/Tag.png" />
                Add All</a>
            <a id="UnTagAll" href='<%=ViewData["UnTagAction"] %>'><img src="/images/Tag.png" />
                Remove All</a>
        </div>
    </td>
    <td class="dropdown">
        <div><a href='#'><img src="/images/Tag.png" />
        Other</a></div>
        <div class="sublinks">
            <a id="AddContact" href='<%=ViewData["AddContact"] %>'>
                Add Contact</a>
            <a id="AddTasks" href='<%=ViewData["AddTasks"] %>'>
                Add Tasks</a>
            <a href='/Task/NotesExcel/<%=ViewData["queryid"] %>'>
                Export Task Notes</a>
            <a href='/Person/TagDuplicates/<%=ViewData["queryid"] %>'>
                Tag Duplicates</a>
        </div>
    </td>
</tr>
</table>