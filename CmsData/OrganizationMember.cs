﻿/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;
using System.Web;
using System.Data.SqlClient;

namespace CmsData
{
	public partial class OrganizationMember
	{
		private const string STR_MeetingsToUpdate = "MeetingsToUpdate";

		public EnrollmentTransaction Drop(CMSDataContext Db, bool addToHistory)
		{
			Db.SubmitChanges();
			int ntries = 2;
			while (true)
			{
				try
				{
					var q = from o in Db.Organizations
							where o.OrganizationId == OrganizationId
							let count = Db.Attends.Count(a => a.PeopleId == PeopleId
															  && a.OrganizationId == OrganizationId
															  && (a.MeetingDate < DateTime.Today || a.AttendanceFlag == true))
							select new { count, Organization.DaysToIgnoreHistory };
					var i = q.Single();
					if (!EnrollmentDate.HasValue)
						EnrollmentDate = CreatedDate;
					var droptrans = new EnrollmentTransaction
									{
										OrganizationId = OrganizationId,
										PeopleId = PeopleId,
										MemberTypeId = MemberTypeId,
										OrganizationName = Organization.OrganizationName,
										TransactionDate = Util.Now,
										TransactionTypeId = 5,
										// drop
										CreatedBy = Util.UserId1,
										CreatedDate = Util.Now,
										Pending = Pending,
										AttendancePercentage = AttendPct,
									};
					Db.EnrollmentTransactions.InsertOnSubmit(droptrans);
					Db.OrgMemMemTags.DeleteAllOnSubmit(this.OrgMemMemTags);
					Db.OrganizationMembers.DeleteOnSubmit(this);
					Db.ExecuteCommand("DELETE FROM dbo.SubRequest WHERE EXISTS(SELECT NULL FROM Attend a WHERE a.AttendId = AttendId AND a.OrganizationId = {0} AND a.MeetingDate > {1} AND a.PeopleId = {2})", OrganizationId, Util.Now, PeopleId);
					Db.ExecuteCommand("DELETE dbo.Attend WHERE OrganizationId = {0} AND MeetingDate > {1} AND PeopleId = {2} AND ISNULL(Commitment, 1) = 1", OrganizationId, Util.Now, PeopleId);
					return droptrans;
				}
				catch (SqlException ex)
				{
					if (ex.Number == 1205)
						if (--ntries > 0)
						{
							Db.Dispose();
							System.Threading.Thread.Sleep(500);
							continue;
						}
					throw;
				}
			}
		}

		public static void UpdateMeetingsToUpdate()
		{
			UpdateMeetingsToUpdate(DbUtil.Db);
		}

		public static void UpdateMeetingsToUpdate(CMSDataContext Db)
		{
			var mids = HttpContext.Current.Items[STR_MeetingsToUpdate] as List<int>;
			if (mids != null)
				foreach (var mid in mids)
					Db.UpdateMeetingCounters(mid);
		}

		public static OrganizationMember Load(CMSDataContext Db, int PeopleId, string OrgName)
		{
			var q = from om in Db.OrganizationMembers
					where om.PeopleId == PeopleId
					where om.Organization.OrganizationName == OrgName
					select om;
			return q.SingleOrDefault();
		}

		public bool ToggleGroup(CMSDataContext Db, int groupid)
		{
			var group = OrgMemMemTags.SingleOrDefault(g =>
													  g.OrgId == OrganizationId && g.PeopleId == PeopleId &&
													  g.MemberTagId == groupid);
			if (group == null)
			{
				OrgMemMemTags.Add(new OrgMemMemTag { MemberTagId = groupid });
				return true;
			}
			OrgMemMemTags.Remove(group);
			Db.OrgMemMemTags.DeleteOnSubmit(group);
			return false;
		}
		public bool IsInGroup(string name)
		{
			var mt = Organization.MemberTags.SingleOrDefault(t => t.Name == name);
			if (mt == null)
				return false;
			var omt = OrgMemMemTags.SingleOrDefault(t => t.MemberTagId == mt.Id);
			return omt != null;
		}

		public void AddToGroup(CMSDataContext Db, string name)
		{
			int? n = null;
			AddToGroup(Db, name, n);
		}

		public void AddToGroup(CMSDataContext Db, string name, int? n)
		{
			if (!name.HasValue())
				return;
			var mt = Db.MemberTags.SingleOrDefault(t => t.Name == name.Trim() && t.OrgId == OrganizationId);
			if (mt == null)
			{
				mt = new MemberTag { Name = name.Trim(), OrgId = OrganizationId };
				Db.MemberTags.InsertOnSubmit(mt);
				Db.SubmitChanges();
			}
			var omt = Db.OrgMemMemTags.SingleOrDefault(t =>
													   t.PeopleId == PeopleId
													   && t.MemberTagId == mt.Id
													   && t.OrgId == OrganizationId);
			if (omt == null)
				mt.OrgMemMemTags.Add(new OrgMemMemTag
									 {
										 PeopleId = PeopleId,
										 OrgId = OrganizationId,
										 Number = n
									 });
			Db.SubmitChanges();
		}

		public void RemoveFromGroup(CMSDataContext Db, string name)
		{
			var mt = Db.MemberTags.SingleOrDefault(t => t.Name == name && t.OrgId == OrganizationId);
			if (mt == null)
				return;
			var omt =
				Db.OrgMemMemTags.SingleOrDefault(t => t.PeopleId == PeopleId && t.MemberTagId == mt.Id && t.OrgId == OrganizationId);
			if (omt != null)
			{
				OrgMemMemTags.Remove(omt);
				Db.OrgMemMemTags.DeleteOnSubmit(omt);
				Db.SubmitChanges();
			}
		}

		public void AddToMemberData(string s)
		{
			if (UserData.HasValue())
				UserData += "\n";
			UserData += s;
		}

		public static OrganizationMember InsertOrgMembers
			(CMSDataContext Db,
			 int OrganizationId,
			 int PeopleId,
			 int MemberTypeId,
			 DateTime EnrollmentDate,
			 DateTime? InactiveDate, bool pending
			)
		{
			Db.SubmitChanges();
			int ntries = 2;
			while (true)
			{
				try
				{
					var m = Db.OrganizationMembers.SingleOrDefault(m2 => m2.PeopleId == PeopleId && m2.OrganizationId == OrganizationId);
					if (m != null)
						return m;
					var org = Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == OrganizationId);
					if (org == null)
						return null;
					var name = org.OrganizationName;

					var om = new OrganizationMember
							 {
								 OrganizationId = OrganizationId,
								 PeopleId = PeopleId,
								 MemberTypeId = MemberTypeId,
								 EnrollmentDate = EnrollmentDate,
								 InactiveDate = InactiveDate,
								 CreatedDate = Util.Now,
								 Pending = pending,
							 };

					var et = new EnrollmentTransaction
							 {
								 OrganizationId = om.OrganizationId,
								 PeopleId = om.PeopleId,
								 MemberTypeId = om.MemberTypeId,
								 OrganizationName = name,
								 TransactionDate = EnrollmentDate,
								 EnrollmentDate = EnrollmentDate,
								 TransactionTypeId = 1,
								 // join
								 CreatedBy = Util.UserId1,
								 CreatedDate = Util.Now,
								 Pending = pending,
								 AttendancePercentage = om.AttendPct
							 };
					Db.OrganizationMembers.InsertOnSubmit(om);
					Db.EnrollmentTransactions.InsertOnSubmit(et);

					Db.SubmitChanges();
					return om;
				}
				catch (SqlException ex)
				{
					if (ex.Number == 1205)
						if (--ntries > 0)
						{
							System.Threading.Thread.Sleep(500);
							continue;
						}
					throw;
				}
			}
		}

		public Transaction AddTransaction(CMSDataContext Db, decimal amt, string reason)
		{
			var qq = from t in Db.Transactions
					 where t.OriginalTransaction.TransactionPeople.Any(pp => pp.PeopleId == PeopleId)
					 where t.OriginalTransaction.OrgId == OrganizationId
					 orderby t.Id descending
					 select t;
			var ti = qq.FirstOrDefault();
			if (ti == null)
				return null; 
			var ti2 = new Transaction
				{
					Amt = amt,
					Amtdue = ti.Amtdue - amt,
					Name = ti.Name,
                    First = ti.First,
                    MiddleInitial = ti.MiddleInitial,
                    Last = ti.Last,
                    Suffix = ti.Suffix,
					TransactionId = "{0} ({1})".Fmt(reason, Util.UserPeopleId),
					Address = ti.Address,
					City = ti.City,
					State = ti.State,
					Zip = ti.Zip,
					Testing = ti.Testing,
					Description = ti.Description,
					Url = ti.Url,
					Emails = ti.Emails,
					TransactionDate = DateTime.Now,
					OrgId = ti.OrgId,
					Financeonly = ti.Financeonly,
					OriginalId = ti.OriginalId ?? ti.Id // links all the transactions together
				};
			return ti2;
		}
		public Decimal? AmountDue(CMSDataContext Db)
		{
			var qq = from t in Db.Transactions
					 where t.OriginalTransaction.TransactionPeople.Any(pp => pp.PeopleId == PeopleId)
					 where t.OriginalTransaction.OrgId == OrganizationId
					 orderby t.Id descending
					 select t;
			var tt = qq.FirstOrDefault();
			if (tt == null)
				return null;
			return tt.Amtdue;
		}
	}
}
