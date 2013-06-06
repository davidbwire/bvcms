using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData;
using System.Data.Linq;
using System.ComponentModel;
using UtilityExtensions;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq.Dynamic;
using System.Data.SqlClient;

namespace CmsWeb.Models
{
    public class ExportInvolvements
    {
        public class ActivityInfo
        {
            public string Name { get; set; }
            public decimal? Pct { get; set; }
            public string Leader { get; set; }
        }
        public static IEnumerable<InvovementInfo> InvolvementList(int queryid)
        {
            var Db = DbUtil.Db;
            var q = Db.PeopleQuery(queryid);
            var q2 = from p in q
                     orderby p.LastName, p.FirstName
                     let spouse = Db.People.SingleOrDefault(w => p.SpouseId == w.PeopleId)
                     let om = p.OrganizationMembers.SingleOrDefault(m => m.OrganizationId == p.BibleFellowshipClassId)
                     select new InvovementInfo
                     {
                         PeopleId = p.PeopleId,
                         Addr = p.PrimaryAddress,
                         Addr2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         Name = p.Name,
                         HomePhone = p.HomePhone,
                         WorkPhone = p.WorkPhone,
                         CellPhone = p.CellPhone,
                         Email = p.EmailAddress,
                         DivName = om.Organization.Division.Name,
                         OrgName = om.Organization.OrganizationName,
                         Teacher = p.BFClass.LeaderName,
                         MemberType = om.MemberType.Description,
                         AttendPct = om.AttendPct,

                         Age = p.Age,
                         Spouse = spouse != null ? spouse.FirstName : "",

                         activities = from m in p.OrganizationMembers
                                      where m.Organization.SecurityTypeId != 3 || !(Util2.OrgMembersOnly && Util2.OrgLeadersOnly)
                                      select new ActivityInfo
                                      {
                                          Name = m.Organization.OrganizationName,
                                          Pct = m.AttendPct,
                                          Leader = m.Organization.LeaderName
                                      },

                         JoinInfo = p.JoinType.Description + " , " + p.JoinDate.ToString().Substring(0, 11),
                         Notes = "",
                         OfficeUseOnly = "",
                         LastName = p.LastName,
                         FirstName = p.PreferredName,
                         Campus = p.Campu.Description,
                         CampusDate = Db.LastChanged(p.PeopleId, "CampusId").FormatDate(),
                     };
            return q2;
        }
        public static IEnumerable ChildrenList(int queryid, int maximumRows)
        {
            var q = DbUtil.Db.PeopleQuery(queryid);
            var q2 = from p in q
                     select new
                     {
                         PeopleId = p.PeopleId,
                         Title = p.TitleCode,
                         FirstName = p.PreferredName,
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                         JoinDate = p.JoinDate.FormatDate(),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         FellowshipLeader = p.BFClass.LeaderName,
                         Age = p.Age.ToString(),
                         School = p.SchoolOther,
                         Grade = p.Grade.ToString(),
                     };
            return q2.Take(maximumRows);
        }
        public static IEnumerable ChurchList(int queryid, int maximumRows)
        {
            var q = DbUtil.Db.PeopleQuery(queryid);
            var q2 = from p in q
                     let rescode = DbUtil.Db.ResidentCodes.SingleOrDefault(r => r.Id == p.PrimaryResCode).Description
                     select new
                     {
                         PeopleId = p.PeopleId,
                         Title = p.TitleCode,
                         FirstName = p.PreferredName,
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         FellowshipLeader = p.BFClass.LeaderName,
                         Age = p.Age.ToString(),
                         MemberStatus = p.MemberStatus.Description,
                         DropType = p.DropType.Description,
                         DropDate = p.DropDate.FormatDate(),
                         NewChurch = p.OtherNewChurch,
                         JoinType = p.JoinType.Description,
                         JoinDate = p.JoinDate.FormatDate(),
                         BaptismDate = p.BaptismDate.FormatDate(),
                         PrevChurch = p.OtherPreviousChurch,
                         Resident = rescode,
                     };
            return q2.Take(maximumRows);
        }
        public static IEnumerable AttendList(int queryid, int maximumRows)
        {
            var q = DbUtil.Db.PeopleQuery(queryid);
            var q2 = from p in q
                     let bfm = DbUtil.Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId && om.PeopleId == p.PeopleId)
                     select new
                     {
                         PeopleId = p.PeopleId,
                         Title = p.TitleCode,
                         FirstName = p.PreferredName,
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                         JoinDate = p.JoinDate.FormatDate(),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         FellowshipLeader = p.BFClass.LeaderName,
                         Age = p.Age.ToString(),
                         School = p.SchoolOther,
                         Grade = p.Grade.ToString(),
                         LastAttend = bfm.LastAttended.ToString(),
                         AttendPct = bfm.AttendPct.ToString(),
                         AttendStr = bfm.AttendStr,
                     };
            return q2.Take(maximumRows);
        }
        public static IEnumerable OrgMemberList2(int qid)
        {
            var q = DbUtil.Db.PeopleQuery(qid);
            var q2 = q.Select(p => new
            {
                om = DbUtil.Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == Util2.CurrentOrgId && om.PeopleId == p.PeopleId),
                rr = p.RecRegs.FirstOrDefault(),
                p = p,
                test = p.PeopleExtras.SingleOrDefault(vv => vv.Field == "test")
            });
            var q3 = q2.Select("new(p.PreferredName,p.LastName,om.AttendStr,om.AmountPaid)");
            return q3;
        }
        public static IEnumerable OrgMemberListGroups()
        {
            var Db = DbUtil.Db;
            var gids = string.Join(",", Util2.CurrentGroups);
            var cmd = new SqlCommand(
                "dbo.OrgMembers {0}, '{1}'".Fmt(Util2.CurrentOrgId, gids));
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            return cmd.ExecuteReader();
        }

        public class MemberInfoClass
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public string Grade { get; set; }
            public string ShirtSize { get; set; }
            public string Request { get; set; }
            public decimal Amount { get; set; }
            public decimal AmountPaid { get; set; }
            public bool HasBalance { get; set; }
            public string Email { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public string WorkPhone { get; set; }
            public string Age { get; set; }
            public string BirthDate { get; set; }
            public string JoinDate { get; set; }
            public string MemberStatus { get; set; }
            public string School { get; set; }
            public string LastAttend { get; set; }
            public string AttendPct { get; set; }
            public string AttendStr { get; set; }
            public string MemberType { get; set; }

            public string MemberInfo
            {
                get
                {
                    var sb = new StringBuilder();
                    if (_memberInfoRaw.HasValue())
                        sb.Append(_memberInfoRaw.Replace("\n", "<br/>"));
                    if (_fname.HasValue())
                        sb.AppendFormat("Father: {0}<br/>", _fname);
                    if (_mname.HasValue())
                        sb.AppendFormat("Mother: {0}<br/>", _mname);
                    return sb.ToString();
                }
            }

            private string _memberInfoRaw;

            public string MemberInfoRaw
            {
                set { _memberInfoRaw = value; }
            }

            public string InactiveDate { get; set; }
            public string Medical { get; set; }
            public int PeopleId { get; set; }
            public string EnrollDate { get; set; }
            public string Groups { get; set; }
            private string _fname;

            public string Fname
            {
                set { _fname = value; }
            }

            private string _mname;

            public string Mname
            {
                set { _mname = value; }
            }

            public string Tickets { get; set; }
        }

        public static IEnumerable<MemberInfoClass> OrgMemberList(int queryid)
        {
            var Db = DbUtil.Db;
            var q = Db.PeopleQuery(queryid);
            var q2 = from p in q
                     let om = Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == Util2.CurrentOrgId && om.PeopleId == p.PeopleId)
                     let recreg = p.RecRegs.FirstOrDefault()
                     let bal = (from t in Db.Transactions
                                where t.OriginalTransaction.TransactionPeople.Any(pp => pp.PeopleId == p.PeopleId)
                                where t.OriginalTransaction.OrgId == Util2.CurrentOrgId
                                orderby t.Id descending
                                select t.Amtdue).FirstOrDefault()
                     select new MemberInfoClass
                     {
                         FirstName = p.PreferredName,
                         LastName = p.LastName,
                         Gender = p.Gender.Code,
                         Grade = om.Grade.ToString(),
                         ShirtSize = om.ShirtSize,
                         Request = om.Request,
                         Amount = om.Amount ?? 0,
                         AmountPaid = om.AmountPaid ?? 0,
                         HasBalance = bal != null && bal > 0,
                         Groups = string.Join(",", om.OrgMemMemTags.Select(mt => mt.MemberTag.Name)),
                         Email = p.EmailAddress,
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         Age = p.Age.ToString(),
                         BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                         JoinDate = p.JoinDate.FormatDate(),
                         MemberStatus = p.MemberStatus.Description,
                         School = p.SchoolOther,
                         LastAttend = om.LastAttended.ToString(),
                         AttendPct = om.AttendPct.ToString(),
                         AttendStr = om.AttendStr,
                         MemberType = om.MemberType.Description,
                         MemberInfoRaw = om.UserData,
                         InactiveDate = om.InactiveDate.FormatDate(),
                         Medical = recreg.MedicalDescription,
                         PeopleId = p.PeopleId,
                         EnrollDate = om.EnrollmentDate.FormatDate(),
                         Tickets = om.Tickets.ToString(),
                     };
            return q2;
        }
        public static IEnumerable PromoList(int queryid, int maximumRows)
        {
            var Db = DbUtil.Db;
            var q = Db.PeopleQuery(queryid);
            var q2 = from p in q
                     let bfm = Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == Util2.CurrentOrgId && om.PeopleId == p.PeopleId)
                     let sc = bfm.Organization.OrgSchedules.FirstOrDefault() // SCHED
                     let tm = sc.SchedTime.Value
                     select new
                     {
                         PeopleId = p.PeopleId,
                         Title = p.TitleCode,
                         FirstName = p.PreferredName,
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         MemberType = bfm.MemberType.Description,
                         Location = bfm.Organization.Location,
                         PendingLoc = bfm.Organization.PendingLoc,
                         Leader = bfm.Organization.LeaderName,
                         OrgName = bfm.Organization.OrganizationName,
                         Schedule = tm.Hour + ":" + tm.Minute.ToString().PadLeft(2, '0'),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone1 = p.Family.HeadOfHousehold.CellPhone.FmtFone(),
                         CellPhone2 = p.Family.HeadOfHouseholdSpouse.CellPhone.FmtFone(),
                     };
            return q2.Take(maximumRows);
        }
    }
}
