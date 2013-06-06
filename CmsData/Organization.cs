using System;
using System.Collections.Generic;
using System.Linq;
using CmsData.Codes;
using UtilityExtensions;
using System.Text;
using System.Text.RegularExpressions;

namespace CmsData
{
	public partial class Organization
	{
		public static string FormatOrgName(string name, string leader, string loc)
		{
			if (loc.HasValue())
				loc = ", " + loc;
			if (leader.HasValue())
				leader = ":" + leader;
			return "{0}{1}{2}".Fmt(name, leader, loc);
		}

		public string FullName
		{
			get { return FormatOrgName(OrganizationName, LeaderName, Location); }
		}
		public string FullName2
		{
			get { return DivisionName + ", " + FormatOrgName(OrganizationName, LeaderName, Location); }
		}

		private string _TagString;
		public string TagString()
		{
			if (_TagString == null)
			{
				var sb = new StringBuilder();
				var q = from d in DivOrgs
						orderby d.Division.Name
						select d.Division.Name;
				foreach (var name in q)
					sb.Append(name + ",");
				if (sb.Length > 0)
					sb.Remove(sb.Length - 1, 1);
				_TagString = sb.ToString();
			}
			return _TagString;
		}
		public void SetTagString(CMSDataContext Db, string value)
		{
			if (!value.HasValue())
			{
				Db.DivOrgs.DeleteAllOnSubmit(DivOrgs);
				return;
			}
			var a = value.Split(',');
			var qdelete = from d in DivOrgs
						  where !a.Contains(d.Division.Name)
						  select d;
			Db.DivOrgs.DeleteAllOnSubmit(qdelete);

			var q = from s in a
					join d2 in DivOrgs on s equals d2.Division.Name into g
					from d in g.DefaultIfEmpty()
					where d == null
					select s;

			foreach (var s in q)
			{
				var div = Db.Divisions.FirstOrDefault(d => d.Name == s);
				if (div == null)
				{
					div = new Division { Name = s };
					string misctags = Db.Setting("MiscTagsString", "Misc Tags");
					var prog = Db.Programs.SingleOrDefault(p => p.Name == misctags);
					if (prog == null)
					{
						prog = new Program { Name = misctags };
						Db.Programs.InsertOnSubmit(prog);
					}
					div.Program = prog;
				}
				DivOrgs.Add(new DivOrg { Division = div });
			}
			_TagString = value;
		}
		public bool ToggleTag(CMSDataContext Db, int divid)
		{
			var divorg = DivOrgs.SingleOrDefault(d => d.DivId == divid);
			if (divorg == null)
			{
				DivOrgs.Add(new DivOrg { DivId = divid });
				return true;
			}
			if (DivOrgs.Count == 1)
				return true;
			DivOrgs.Remove(divorg);
			Db.DivOrgs.DeleteOnSubmit(divorg);
			return false;
		}
		public void AddToDiv(CMSDataContext Db, int divid)
		{
			var divorg = DivOrgs.SingleOrDefault(d => d.DivId == divid);
			if (divorg == null)
				DivOrgs.Add(new DivOrg { DivId = divid });
		}

		public bool PurgeOrg(CMSDataContext Db)
		{
			try
			{
				Db.PurgeOrganization(OrganizationId);
			}
			catch
			{
				return false;
			}
			return true;
		}
		public void CopySettings(CMSDataContext Db, int fromid)
		{
			var frorg = Db.LoadOrganizationById(fromid);

			//only Copy settings
			NotifyIds = frorg.NotifyIds;
			FirstMeetingDate = frorg.FirstMeetingDate;
			LastDayBeforeExtra = frorg.LastDayBeforeExtra;
			LastMeetingDate = frorg.LastMeetingDate;
			Limit = frorg.Limit;
			RegistrationTypeId = frorg.RegistrationTypeId;

			RegSetting = frorg.RegSetting;

			CopySettings2(frorg, this);
			Db.SubmitChanges();
		}
		public static void CopySettings2(Organization frorg, Organization toorg)
		{
			toorg.AllowNonCampusCheckIn = frorg.AllowNonCampusCheckIn;
			toorg.AllowAttendOverlap = frorg.AllowAttendOverlap;
			toorg.CanSelfCheckin = frorg.CanSelfCheckin;
			toorg.NumWorkerCheckInLabels = frorg.NumWorkerCheckInLabels;
			toorg.NoSecurityLabel = frorg.NoSecurityLabel;
			toorg.NumCheckInLabels = frorg.NumCheckInLabels;
			toorg.PhoneNumber = frorg.PhoneNumber;
			toorg.EntryPointId = frorg.EntryPointId;
			toorg.RollSheetVisitorWks = frorg.RollSheetVisitorWks;
			toorg.GradeAgeStart = frorg.GradeAgeStart;
			toorg.DivisionId = frorg.DivisionId;
		}
		public Organization CloneOrg(CMSDataContext Db, int? DivisionId)
		{
			var neworg = new Organization
			{
				SecurityTypeId = SecurityTypeId,
				CreatedDate = Util.Now,
				CreatedBy = Util.UserId1,
				LeaderMemberTypeId = LeaderMemberTypeId,
				OrganizationName = OrganizationName + " (copy)",
				OrganizationStatusId = OrganizationStatusId,
				CampusId = CampusId,
				IsBibleFellowshipOrg = IsBibleFellowshipOrg,
			};
			Db.Organizations.InsertOnSubmit(neworg);
			foreach (var div in DivOrgs)
				neworg.DivOrgs.Add(new DivOrg { Organization = neworg, DivId = div.DivId });
			foreach (var sc in OrgSchedules)
				neworg.OrgSchedules.Add(new OrgSchedule
				{
					OrganizationId = OrganizationId,
					AttendCreditId = sc.AttendCreditId,
					SchedDay = sc.SchedDay,
					SchedTime = sc.SchedTime,
					Id = sc.Id
				});

			CopySettings2(this, neworg);
			Db.SubmitChanges();
			return neworg;
		}
		public Organization CloneOrg(CMSDataContext Db)
		{
			return CloneOrg(Db, DivisionId);
		}
		public static DateTime? GetDateFromScheduleId(int id)
		{
			int dw = id / 10000 - 1;
			id %= 10000;
			if (dw == 10) // any day
				dw = DateTime.Today.DayOfWeek.ToInt();
			if (dw == 0)
				dw = 7;
			int hour = id / 100;
			int min = id % 100;
			if (hour > 0)
				return new DateTime(1900, 1, dw, hour, min, 0);
			return null;
		}
		public string DivisionName
		{
			get
			{
				return Division != null ?
					(Division.Program != null ? Division.Program.Name : "no program")
						+ ":" + Division.Name :
					   "<span style='color:red'>need a main division</span>";
			}
		}
        public static OrgSchedule ParseSchedule(string s)
        {
            var m = Regex.Match(s, @"\A(?<dow>.*)\s(?<time>\d{1,2}:\d{2}\s(A|P)M)", RegexOptions.IgnoreCase);
            var d = new Dictionary<string, int>
            {
                { "Sun", 0 },
                { "Mon", 1 },
                { "Tue", 2 },
                { "Wed", 3 },
                { "Thu", 4 },
                { "Fri", 5 },
                { "Sat", 6 },
            };
            var dow = m.Groups["dow"].Value;
            var time = m.Groups["time"].Value;
            var sc = new OrgSchedule
            {
                SchedDay = d[dow],
                SchedTime = DateTime.Parse(time),
                AttendCreditId = 1
            };
            return sc;
        }
		public static OrganizationType FetchOrCreateType(CMSDataContext Db, string type)
		{
			var t = Db.OrganizationTypes.SingleOrDefault(pp => pp.Description == type);
			if (t == null)
			{
				var max = 10;
				if(Db.OrganizationTypes.Any())
					max = Db.OrganizationTypes.Max(mm => mm.Id) + 10;
				t = new OrganizationType { Description = type, Code = type.Substring(0, 3), Id = max };
				Db.OrganizationTypes.InsertOnSubmit(t);
				Db.SubmitChanges();
			}
			return t;
		}
		public static Program FetchOrCreateProgram(CMSDataContext Db, string program)
		{
			var p = Db.Programs.SingleOrDefault(pp => pp.Name == program);
			if (p == null)
			{
				p = new Program { Name = program };
				Db.Programs.InsertOnSubmit(p);
				Db.SubmitChanges();
			}
			return p;
		}
		public static Division FetchOrCreateDivision(CMSDataContext Db, Program program, string division)
		{
			var d = Db.Divisions.SingleOrDefault(pp => pp.Name == division);
			if (d == null)
			{
				d = new Division { Name = division, Program = program };
				var progdiv = new ProgDiv { Division = d, Program = program };
				Db.ProgDivs.InsertOnSubmit(progdiv);
				Db.SubmitChanges();
			}
			return d;
		}
		public static MemberType FetchOrCreateMemberType(CMSDataContext Db, string type)
		{
			var mt = Db.MemberTypes.SingleOrDefault(pp => pp.Description == type);
			if (mt == null)
			{
				var max = Db.MemberTypes.Max(mm => mm.Id) + 10;
				if (max < 1000)
					max = 1010;
				mt = new MemberType { Id = max, Description = type, Code = type.Truncate(20), AttendanceTypeId = AttendTypeCode.Member };
				Db.MemberTypes.InsertOnSubmit(mt);
				Db.SubmitChanges();
			}
			return mt;
		}
		public static Organization FetchOrCreateOrganization(CMSDataContext Db, Division division, string organization)
		{
			var o = Db.LoadOrganizationByName(organization);
			if (o == null)
				return CreateOrganization(Db, division, organization);
			return o;
		}
		public static Organization FetchOrCreateOrganization(CMSDataContext Db, Division division, string organization, string description)
		{
		    var o = Db.Organizations.SingleOrDefault(oo => oo.Description == description);
		    if (o == null)
		    {
		        o = CreateOrganization(Db, division, organization);
                o.Description = description;
		    }
		    return o;
		}
		public static Organization CreateOrganization(CMSDataContext Db, Division division, string organization)
		{
			var o = new Organization
			{
				OrganizationName = organization,
				SecurityTypeId = 0,
				CreatedDate = Util.Now,
				CreatedBy = Util.UserId1,
				OrganizationStatusId = 30,
			};
			division.Organizations.Add(o);
			Db.DivOrgs.InsertOnSubmit(new DivOrg { Division = division, Organization = o });
			Db.SubmitChanges();
			return o;
		}
		public static Organization CreateOrganization(CMSDataContext Db, int divid, string organization)
		{
			var o = new Organization
			{
				OrganizationName = organization,
				SecurityTypeId = 0,
				CreatedDate = Util.Now,
				CreatedBy = Util.UserId1,
				OrganizationStatusId = 30,
                DivisionId = divid,
			};
			Db.DivOrgs.InsertOnSubmit(new DivOrg { DivId = divid, Organization = o });
			Db.SubmitChanges();
			return o;
		}
		public OrganizationExtra GetExtraValue(string field)
		{
			var ev = OrganizationExtras.AsEnumerable().FirstOrDefault(ee => string.Compare(ee.Field, field, ignoreCase: true) == 0);
			if (ev == null)
			{
				ev = new OrganizationExtra()
				{
					OrganizationId = OrganizationId,
					Field = field,
					
				};
				OrganizationExtras.Add(ev);
			}
			return ev;
		}

		public void AddEditExtra(CMSDataContext Db, string field, string value, bool multiline = false)
		{
			var oev = Db.OrganizationExtras.SingleOrDefault(oe => oe.OrganizationId == OrganizationId && oe.Field == field);
			if (oev == null)
			{
				oev = new OrganizationExtra
					  {
						  OrganizationId = OrganizationId,
						  Field = field,
					  };
				Db.OrganizationExtras.InsertOnSubmit(oev);
			}
			oev.Data = value;
			oev.DataType = multiline ? "text" : null;
		}
		public void AddToExtraData(string field, string value)
		{
			if (!value.HasValue())
				return;
			var ev = GetExtraValue(field);
			ev.DataType = "text";
			if (ev.Data.HasValue())
				ev.Data = value + "\n" + ev.Data;
			else
				ev.Data = value;
		}

		public string GetExtra(string field)
		{
			var oev = OrganizationExtras.SingleOrDefault(oe => oe.OrganizationId == OrganizationId && oe.Field == field);
			if (oev == null)
				return null;
			return oev.Data;
		}
	}
}
