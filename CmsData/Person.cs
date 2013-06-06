﻿/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Data.SqlClient;
using System.Linq;
using UtilityExtensions;
using System.Text;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Transactions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.Linq.SqlClient;
using System.Web;
using CmsData.Codes;

namespace CmsData
{

    public partial class Person
    {
        public static int[] DiscClassStatusCompletedCodes = new int[]
        { 
            NewMemberClassStatusCode.AdminApproval, 
            NewMemberClassStatusCode.Attended, 
            NewMemberClassStatusCode.ExemptedChild 
        };
        public static int[] DropCodesThatDrop = new int[] 
        { 
            DropTypeCode.Administrative,
            DropTypeCode.AnotherDenomination,
            DropTypeCode.LetteredOut,
            DropTypeCode.Requested,
            DropTypeCode.Other,
        };
        public DateTime Now()
        {
            return Util.Now;
        }
        /* Origins
        10		Visit						Worship or BFClass Visit
        30		Referral					see Request
        40		Request						Task, use this for Referral too
        50		Deacon Telephone			Contact, type = phoned in
        60		Survey (EE)					Contact, EE
        70		Enrollment					Member of org
        80		Membership Decision			Contact, Type=Worship Visit
        90		Contribution				-1 peopleid in Excel with Name?
        98		Other						Task, use task description
        */
        public string CityStateZip
        {
            get { return Util.FormatCSZ4(PrimaryCity, PrimaryState, PrimaryZip); }
        }
        public string CityStateZip5
        {
            get { return Util.FormatCSZ(PrimaryCity, PrimaryState, PrimaryZip); }
        }
        public string AddrCityStateZip
        {
            get { return PrimaryAddress + " " + CityStateZip; }
        }
        public string Addr2CityStateZip
        {
            get { return PrimaryAddress2 + " " + CityStateZip; }
        }
        public string FullAddress
        {
            get
            {
                var sb = new StringBuilder(PrimaryAddress + "\n");
                if (PrimaryAddress2.HasValue())
                    sb.AppendLine(PrimaryAddress2);
                sb.Append(CityStateZip);
                return sb.ToString();
            }
        }
        public string SpouseName(CMSDataContext Db)
        {
            if (SpouseId.HasValue)
            {
                var q = from p in Db.People
                        where p.PeopleId == SpouseId
                        select p.Name;
                return q.SingleOrDefault();
            }
            return "";
        }
        public DateTime? BirthDate
        {
            get
            {
                DateTime dt;
                if (DateTime.TryParse(DOB, out dt))
                    return dt;
                return null;
            }
        }
        public string DOB
        {
            get
            { return Util.FormatBirthday(BirthYear, BirthMonth, BirthDay); }
            set
            {
                // reset all values before replacing b/c replacement may be partial
                BirthDay = null;
                BirthMonth = null;
                BirthYear = null;
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    BirthDay = dt.Day;
                    BirthMonth = dt.Month;
                    if (Regex.IsMatch(value, @"\d+/\d+/\d+"))
                        BirthYear = dt.Year;
                }
                else
                {
                    int n;
                    if (int.TryParse(value, out n))
                        if (n >= 1 && n <= 12)
                            BirthMonth = n;
                        else
                            BirthYear = n;
                }
            }
        }
        public DateTime? GetBirthdate()
        {
            DateTime dt;
            if (DateTime.TryParse(DOB, out dt))
                return dt;
            return null;
        }
        public int GetAge()
        {
            int years;
            var dt0 = GetBirthdate();
            if (!dt0.HasValue)
                return -1;
            var dt = dt0.Value;
            years = Util.Now.Year - dt.Year;
            if (Util.Now.Month < dt.Month || (Util.Now.Month == dt.Month && Util.Now.Day < dt.Day))
                years--;
            return years;
        }
        public void MovePersonStuff(CMSDataContext db, int targetid)
        {
            var toperson = db.People.Single(p => p.PeopleId == targetid);
            foreach (var om in this.OrganizationMembers)
            {
                var om2 = OrganizationMember.InsertOrgMembers(db, om.OrganizationId, targetid, om.MemberTypeId, om.EnrollmentDate.Value, om.InactiveDate, om.Pending ?? false);
                db.UpdateMainFellowship(om.OrganizationId);
                om2.CreatedBy = om.CreatedBy;
                om2.CreatedDate = om.CreatedDate;
                om2.AttendPct = om.AttendPct;
                om2.AttendStr = om.AttendStr;
                om2.LastAttended = om.LastAttended;
                om2.Request = om.Request;
                om2.Grade = om.Grade;
                om2.Amount = om.Amount;
                om2.RegisterEmail = om.RegisterEmail;
                om2.ShirtSize = om.ShirtSize;
                om2.Tickets = om.Tickets;
                om2.UserData = om.UserData;
                db.SubmitChanges();
                foreach (var m in om.OrgMemMemTags)
                    if (!om2.OrgMemMemTags.Any(mm => mm.MemberTagId == m.MemberTagId))
                        om2.OrgMemMemTags.Add(new OrgMemMemTag { MemberTagId = m.MemberTagId });
                db.SubmitChanges();
                db.OrgMemMemTags.DeleteAllOnSubmit(om.OrgMemMemTags);
                db.SubmitChanges();
                TrySubmit(db, "Organizations (orgid:{0})".Fmt(om.OrganizationId));
            }
            db.OrganizationMembers.DeleteAllOnSubmit(this.OrganizationMembers);
            TrySubmit(db, "DeletingMemberships");

            foreach (var et in this.EnrollmentTransactions)
                et.PeopleId = targetid;
            TrySubmit(db, "EnrollmentTransactions");

            var q = from a in db.Attends
                    where a.PeopleId == this.PeopleId
                    let oa = db.Attends.SingleOrDefault(a2 => a2.MeetingId == a.MeetingId && a2.PeopleId == targetid)
                    where oa == null
                    select a;
            var list = q.ToList();
            foreach (var a in list)
            {
                a.PeopleId = targetid;
                db.SubmitChanges();
                TrySubmit(db, "Attends (meetingid:{0})".Fmt(a.MeetingId));
            }

            foreach (var c in this.Contributions)
                c.PeopleId = targetid;
            TrySubmit(db, "Contributions");

            foreach (var u in this.Users)
                u.PeopleId = targetid;
            TrySubmit(db, "Users");
            
            if (this.Volunteers.Any() && !toperson.Volunteers.Any())
                foreach (var v in this.Volunteers)
                {
                    var vv = new Volunteer
                    {
                        PeopleId = targetid,
                        Children = v.Children,
                        Comments = v.Comments,
                        Leader = v.Leader,
                        ProcessedDate = v.ProcessedDate,
                        Standard = v.Standard,
                        StatusId = v.StatusId,
                    };
                    db.Volunteers.InsertOnSubmit(vv);
                }
            TrySubmit(db, "Volunteers");

            foreach (var v in this.VolunteerForms)
                v.PeopleId = targetid;
            TrySubmit(db, "VolunteerForms");

            foreach (var c in this.contactsMade)
            {
                var cp = db.Contactors.SingleOrDefault(c2 => c2.PeopleId == targetid && c.ContactId == c2.ContactId);
                if (cp == null)
                    c.contact.contactsMakers.Add(new Contactor { PeopleId = targetid });
                db.Contactors.DeleteOnSubmit(c);
            }
            TrySubmit(db, "ContactsMade");

            foreach (var c in this.contactsHad)
            {
                var cp = db.Contactees.SingleOrDefault(c2 => c2.PeopleId == targetid && c.ContactId == c2.ContactId);
                if (cp == null)
                    c.contact.contactees.Add(new Contactee { PeopleId = targetid });
                db.Contactees.DeleteOnSubmit(c);
            }
            TrySubmit(db, "ContactsHad");

            foreach (var e in this.PeopleExtras)
            {
                var field = e.Field;
            FindExisting:
                var cp = db.PeopleExtras.FirstOrDefault(c2 => c2.PeopleId == targetid && c2.Field == field);
                if (cp != null)
                {
                    field = field + "_mv";
                    goto FindExisting;
                }
                var e2 = new PeopleExtra
                             {
                                 PeopleId = targetid,
                                 Field = field,
                                 Data = e.Data,
                                 StrValue = e.StrValue,
                                 DateValue = e.DateValue,
                                 IntValue = e.IntValue,
                                 IntValue2 = e.IntValue2,
                                 TransactionTime = e.TransactionTime
                             };
                db.PeopleExtras.InsertOnSubmit(e2);
                TrySubmit(db, "ExtraValues (pid={0},field={1})".Fmt(e2.PeopleId, e2.Field));
            }
            db.PeopleExtras.DeleteAllOnSubmit(PeopleExtras);
            TrySubmit(db, "Delete ExtraValues");

            var torecreg = toperson.RecRegs.SingleOrDefault();
            var frrecreg = RecRegs.SingleOrDefault();
            if (torecreg == null && frrecreg != null)
                frrecreg.PeopleId = targetid;
            if (torecreg != null && frrecreg != null)
            {
                torecreg.Comments = frrecreg.Comments + "\n" + torecreg.Comments;
                if (frrecreg.ShirtSize.HasValue())
                    torecreg.ShirtSize = frrecreg.ShirtSize;
                if (frrecreg.MedicalDescription.HasValue())
                    torecreg.MedicalDescription = frrecreg.MedicalDescription;
                if (frrecreg.Doctor.HasValue())
                    torecreg.Doctor = frrecreg.Doctor;
                if (frrecreg.Docphone.HasValue())
                    torecreg.Docphone = frrecreg.Docphone;
                if (frrecreg.MedAllergy.HasValue)
                    torecreg.MedAllergy = frrecreg.MedAllergy;
                if (frrecreg.Tylenol.HasValue)
                    torecreg.Tylenol = frrecreg.Tylenol;
                if (frrecreg.Robitussin.HasValue)
                    torecreg.Robitussin = frrecreg.Robitussin;
                if (frrecreg.Advil.HasValue)
                    torecreg.Advil = frrecreg.Advil;
                if (frrecreg.Maalox.HasValue)
                    torecreg.Maalox = frrecreg.Maalox;
                if (frrecreg.Insurance.HasValue())
                    torecreg.Insurance = frrecreg.Insurance;
                if (frrecreg.Policy.HasValue())
                    torecreg.Policy = frrecreg.Policy;
                if (frrecreg.Mname.HasValue())
                    torecreg.Mname = frrecreg.Mname;
                if (frrecreg.Fname.HasValue())
                    torecreg.Fname = frrecreg.Fname;
                if (frrecreg.Emcontact.HasValue())
                    torecreg.Emcontact = frrecreg.Emcontact;
                if (frrecreg.Emphone.HasValue())
                    torecreg.Emphone = frrecreg.Emphone;
                if (frrecreg.ActiveInAnotherChurch.HasValue)
                    torecreg.ActiveInAnotherChurch = frrecreg.ActiveInAnotherChurch;
            }
            TrySubmit(db, "RegReg");

            var mg = db.ManagedGivings.FirstOrDefault(mm => mm.PeopleId == targetid);
            if (mg == null)
            {
                var v = this.ManagedGivings.FirstOrDefault();
                if (v!=null)
                {
                    db.ManagedGivings.InsertOnSubmit(new ManagedGiving()
                                {
                                    Day1 = v.Day1,
                                    Day2 = v.Day2,
                                    EveryN = v.EveryN,
                                    NextDate = v.NextDate,
                                    PeopleId = targetid,
                                    Period = v.Period,
                                    SemiEvery = v.SemiEvery,
                                    StartWhen = v.StartWhen,
                                    StopAfter = v.StopAfter,
                                    StopWhen = v.StopWhen,
                                    Type = v.Type,
                                });
                    var qq = from ra in db.RecurringAmounts
                             where ra.PeopleId == PeopleId
                             select ra;
                    foreach (var ra in qq)
                        db.RecurringAmounts.InsertOnSubmit(
                            new RecurringAmount()
                                {
                                    PeopleId = targetid,
                                    Amt = ra.Amt,
                                    FundId = ra.FundId,
                                });
                }
                TrySubmit(db, "ManagedGivings");
            }

            var pi = db.PaymentInfos.FirstOrDefault(mm => mm.PeopleId == targetid);
            if (pi == null) // the target has none
                foreach (var i in PaymentInfos)
                    DbUtil.Db.PaymentInfos.InsertOnSubmit(
                        new PaymentInfo()
                            {
                                Address = i.Address,
                                AuNetCustId = i.AuNetCustId,
                                Ccv = i.Ccv,
                                AuNetCustPayId = i.AuNetCustPayId,
                                City = i.City,
                                Expires = i.Expires,
                                FirstName = i.FirstName,
                                LastName = i.LastName,
                                MaskedAccount = i.MaskedAccount,
                                MaskedCard = i.MaskedCard,
                                MiddleInitial = i.MiddleInitial,
                                PeopleId = targetid,
                                Phone = i.Phone,
                                PreferredGivingType = i.PreferredGivingType,
                                PreferredPaymentType = i.PreferredPaymentType,
                                Routing = i.Routing,
                                SageBankGuid = i.SageBankGuid,
                                SageCardGuid = i.SageCardGuid,
                                State = i.State,
                                Suffix = i.Suffix,
                                Testing = i.Testing,
                                Zip = i.Zip,
                            });
            TrySubmit(db, "PaymentInfos");

            foreach (var c in this.CheckInTimes)
                c.PeopleId = targetid;
            TrySubmit(db, "CheckinTimes");
        }

        private void TrySubmit(CMSDataContext db, string message)
        {
            try
            {
                db.SubmitChanges();
            }
            catch (SqlException ex)
            {
                throw new Exception("Merge Error: " + message + " \nFrom SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Merge Error: " + message + " \n" + ex.Message);
            }
        }

        public bool Deceased
        {
            get { return DeceasedDate.HasValue; }
        }
        public string FromEmail
        {
            get { return Util.FullEmail(EmailAddress, Name); }
        }
        public string FromEmail2
        {
            get { return Util.FullEmail(EmailAddress2, Name); }
        }
        private static void NameSplit(string name, out string First, out string Last)
        {
            First = "";
            Last = "";
            if (!name.HasValue())
                return;
            var a = name.Trim().Split(' ');
            if (a.Length > 1)
            {
                First = a[0];
                Last = a[1];
            }
            else
                Last = a[0];

        }
        public static Person Add(Family fam, int position, Tag tag, string name, string dob, bool Married, int gender, int originId, int? EntryPointId)
        {
            string First, Last;
            NameSplit(name, out First, out Last);
            if (!First.HasValue() || Married)
                switch (gender)
                {
                    case 0: First = "A"; break;
                    case 1: if (!First.HasValue()) First = "Husbander"; break;
                    case 2: First = "Wifey"; break;
                }
            return Add(fam, position, tag, First, null, Last, dob, Married, gender, originId, EntryPointId);
        }
        public static Person Add(Family fam,
            int position,
            Tag tag,
            string firstname,
            string nickname,
            string lastname,
            string dob,
            int MarriedCode,
            int gender,
            int originId,
            int? EntryPointId)
        {
            return Person.Add(DbUtil.Db, true, fam, position, tag, firstname, nickname, lastname, dob, MarriedCode, gender, originId, EntryPointId);
        }
        public static Person Add(CMSDataContext Db, Family fam, string firstname, string nickname, string lastname, DateTime? dob)
        {
            return Person.Add(Db, false, fam, 20, null, firstname, nickname, lastname, dob.FormatDate(), 0, 0, 0, 0);
        }

        public static Person Add(CMSDataContext Db, bool SendNotices, Family fam, int position, Tag tag, string firstname, string nickname, string lastname, string dob, int MarriedCode, int gender, int originId, int? EntryPointId, bool testing = false)
        {
            var p = new Person();
            p.CreatedDate = Util.Now;
            p.CreatedBy = Util.UserId;
            Db.People.InsertOnSubmit(p);
            p.PositionInFamilyId = position;
            p.AddressTypeId = 10;

            if (firstname.HasValue())
                p.FirstName = firstname.Trim().ToProper().Truncate(25);
            else
                p.FirstName = "";

            if (nickname.HasValue())
                p.NickName = nickname.Trim().ToProper().Truncate(15);

            if (lastname.HasValue())
                p.LastName = lastname.Trim().ToProper().Truncate(30);
            else
                p.LastName = "?";

            p.GenderId = gender;
            if (p.GenderId == 99)
                p.GenderId = 0;
            p.MaritalStatusId = MarriedCode;

            DateTime dt;
            if (Util.DateValid(dob, out dt))
            {
                while (dt.Year < 1900)
                    dt = dt.AddYears(100);
                if (dt > Util.Now)
                    dt = dt.AddYears(-100);
                p.BirthDay = dt.Day;
                p.BirthMonth = dt.Month;
                p.BirthYear = dt.Year;
                if (p.GetAge() < 18 && MarriedCode == 0)
                    p.MaritalStatusId = MaritalStatusCode.Single;
            }
            else if (DateTime.TryParse(dob, out dt))
            {
                p.BirthDay = dt.Day;
                p.BirthMonth = dt.Month;
                if (Regex.IsMatch(dob, @"\d+[-/]\d+[-/]\d+"))
                {
                    p.BirthYear = dt.Year;
                    while (p.BirthYear < 1900)
                        p.BirthYear += 100;
                    if (p.GetAge() < 18 && MarriedCode == 0)
                        p.MaritalStatusId = MaritalStatusCode.Single;
                }
            }

            p.MemberStatusId = MemberStatusCode.JustAdded;
            if (fam == null)
            {
                fam = new Family();
                Db.Families.InsertOnSubmit(fam);
                p.Family = fam;
            }
            else
                fam.People.Add(p);

            var PrimaryCount = fam.People.Where(c => c.PositionInFamilyId == PositionInFamily.PrimaryAdult).Count();
            if (PrimaryCount > 2 && p.PositionInFamilyId == PositionInFamily.PrimaryAdult)
                p.PositionInFamilyId = PositionInFamily.SecondaryAdult;

            if (tag != null)
                tag.PersonTags.Add(new TagPerson { Person = p });

            p.OriginId = originId;
            p.EntryPointId = EntryPointId;
            p.FixTitle();
            if (!testing)
                Db.SubmitChanges();
            if (SendNotices)
            {
                if (Util.UserPeopleId.HasValue
                        && Util.UserPeopleId.Value != Db.NewPeopleManagerId
                        && HttpContext.Current.User.IsInRole("Access")
                        && !HttpContext.Current.User.IsInRole("OrgMembersOnly") 
                        && !HttpContext.Current.User.IsInRole("OrgLeadersOnly"))
                    Task.AddNewPerson(p.PeopleId);
                else
                    Db.Email(Util.SysFromEmail, Db.GetNewPeopleManagers(),
                            "Just Added Person on " + Db.Host, "{0} ({1})".Fmt(p.Name, p.PeopleId));
            }
            return p;
        }
        public static Person Add(Family fam, int position, Tag tag, string firstname, string nickname, string lastname, string dob, bool Married, int gender, int originId, int? EntryPointId)
        {
            return Add(fam, position, tag, firstname, nickname, lastname, dob, Married ? 20 : 10, gender, originId, EntryPointId);
        }
        public List<Duplicate> PossibleDuplicates()
        {
            var fone = Util.GetDigits(Util.PickFirst(CellPhone, HomePhone));
            using (var ctx = new CMSDataContext(Util.ConnectionString))
            {
                ctx.SetNoLock();
                string street = GetStreet(ctx) ?? "--";
                var nick = NickName ?? "--";
                var maid = MaidenName ?? "--";
                var em = EmailAddress ?? "--";
                if (!em.HasValue())
                    em = "--";
                var bd = BirthDay ?? -1;
                var bm = BirthMonth ?? -1;
                var byr = BirthYear ?? -1;
                var q = from p in ctx.People
                        let firstmatch = p.FirstName == FirstName || (p.NickName ?? "") == FirstName || (p.MiddleName ?? "") == FirstName
                                    || p.FirstName == nick || (p.NickName ?? "") == nick || (p.MiddleName ?? "") == nick
                        let lastmatch = p.LastName == LastName || (p.MaidenName ?? "") == LastName
                                    || (p.MaidenName ?? "") == maid || p.LastName == maid
                        let nobday = (p.BirthMonth == null && p.BirthYear == null && p.BirthDay == null)
                                    || (BirthMonth == null && BirthYear == null && BirthDay == null)
                        let bdmatch = (p.BirthDay ?? -2) == bd && (p.BirthMonth ?? -2) == bm && (p.BirthYear ?? -2) == byr
                        let bdmatchpart = (p.BirthDay ?? -2) == bd && (p.BirthMonth ?? -2) == bm
                        let emailmatch = p.EmailAddress != null && p.EmailAddress == em
                        let addrmatch = (p.AddressLineOne ?? "").Contains(street) || (p.Family.AddressLineOne ?? "").Contains(street)
                        let phonematch = (p.CellPhoneLU == CellPhoneLU
                                            || p.CellPhoneLU == Family.HomePhoneLU
                                            || p.CellPhone == WorkPhoneLU
                                            || p.Family.HomePhoneLU == CellPhoneLU
                                            || p.Family.HomePhoneLU == Family.HomePhoneLU
                                            || p.Family.HomePhoneLU == WorkPhoneLU
                                            || p.WorkPhoneLU == CellPhoneLU
                                            || p.WorkPhoneLU == Family.HomePhoneLU
                                            || p.WorkPhoneLU == WorkPhoneLU)
                        let samefamily = p.FamilyId == FamilyId
                        let nmatches = samefamily ? 0 :
                                        (firstmatch ? 1 : 0)
                                        + (bdmatch ? 1 : 0)
                                        + (emailmatch ? 1 : 0)
                                        + (phonematch ? 1 : 0)
                                        + (addrmatch ? 1 : 0)
                        where (lastmatch && nmatches >= 3)
                                || ((firstmatch && lastmatch && bdmatchpart) && p.PeopleId != PeopleId)
                        select new Duplicate
                                                {
                                                    PeopleId = p.PeopleId,
                                                    First = p.FirstName,
                                                    Last = p.LastName,
                                                    Nick = p.NickName,
                                                    Middle = p.MiddleName,
                                                    BMon = p.BirthMonth,
                                                    BDay = p.BirthDay,
                                                    BYear = p.BirthYear,
                                                    Email = p.EmailAddress,
                                                    FamAddr = p.Family.AddressLineOne,
                                                    PerAddr = p.AddressLineOne,
                                                    Member = p.MemberStatus.Description
                                                };
                var list = q.ToList();
                return list;
            }
        }
        public class Duplicate
        {
            public bool s0 { get; set; }
            public bool s1 { get; set; }
            public bool s2 { get; set; }
            public bool s3 { get; set; }
            public bool s4 { get; set; }
            public bool s5 { get; set; }
            public bool s6 { get; set; }
            public int PeopleId { get; set; }
            public string First { get; set; }
            public string Last { get; set; }
            public string Nick { get; set; }
            public string Middle { get; set; }
            public string Maiden { get; set; }
            public int? BMon { get; set; }
            public int? BDay { get; set; }
            public int? BYear { get; set; }
            public string Email { get; set; }
            public string FamAddr { get; set; }
            public string PerAddr { get; set; }
            public string Member { get; set; }
        }
        public List<Duplicate> PossibleDuplicates2()
        {
            using (var ctx = new CMSDataContext(Util.ConnectionString))
            {
                ctx.SetNoLock();
                string street = GetStreet(ctx) ?? "--";
                var nick = NickName ?? "--";
                var maid = MaidenName ?? "--";
                var em = EmailAddress ?? "--";
                if (!em.HasValue())
                    em = "--";
                var bd = BirthDay ?? -1;
                var bm = BirthMonth ?? -1;
                var byr = BirthYear ?? -1;
                var q = from p in ctx.People
                        let firstmatch = p.FirstName == FirstName || (p.NickName ?? "") == FirstName || (p.MiddleName ?? "") == FirstName
                                    || p.FirstName == nick || (p.NickName ?? "") == nick || (p.MiddleName ?? "") == nick
                        let lastmatch = p.LastName == LastName || (p.MaidenName ?? "") == LastName
                                    || (p.MaidenName ?? "") == maid || p.LastName == maid
                        let nobday = (p.BirthMonth == null && p.BirthYear == null && p.BirthDay == null)
                                    || (BirthMonth == null && BirthYear == null && BirthDay == null)
                        let bdmatch = (p.BirthDay ?? -2) == bd && (p.BirthMonth ?? -2) == bm && (p.BirthYear ?? -2) == byr
                        let bdmatchpart = (p.BirthDay ?? -2) == bd && (p.BirthMonth ?? -2) == bm
                        let emailmatch = p.EmailAddress != null && p.EmailAddress == em
                        let addrmatch = (p.AddressLineOne ?? "").Contains(street) || (p.Family.AddressLineOne ?? "").Contains(street)
                        let s1 = firstmatch && bdmatchpart
                        let s2 = firstmatch && bdmatch
                        let s3 = firstmatch && lastmatch && nobday
                        let s4 = firstmatch && addrmatch
                        let s5 = firstmatch && emailmatch
                        let s6 = lastmatch && bdmatch
                        where p.FamilyId != FamilyId && (s1 || s2 || s3 || s4 || s5 || s6)
                        select new Duplicate
                        {
                            s1 = s1,
                            s2 = s2,
                            s3 = s3,
                            s4 = s4,
                            s5 = s5,
                            s6 = s6,
                            PeopleId = p.PeopleId,
                            First = p.FirstName,
                            Last = p.LastName,
                            Nick = p.NickName,
                            Middle = p.MiddleName,
                            BMon = p.BirthMonth,
                            BDay = p.BirthDay,
                            BYear = p.BirthYear,
                            Email = p.EmailAddress,
                            FamAddr = p.Family.AddressLineOne,
                            PerAddr = p.AddressLineOne,
                            Member = p.MemberStatus.Description
                        };
                try
                {
                    var list = q.ToList();
                    var t = new Duplicate
                    {
                        s0 = true,
                        PeopleId = PeopleId,
                        First = FirstName,
                        Last = LastName,
                        Nick = NickName,
                        Middle = MiddleName,
                        BMon = BirthMonth,
                        BDay = BirthDay,
                        BYear = BirthYear,
                        Email = EmailAddress,
                        FamAddr = Family.AddressLineOne,
                        PerAddr = AddressLineOne,
                        Member = MemberStatus.Description
                    };
                    list.Insert(0, t);

                    return list;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        private string GetStreet(CMSDataContext db)
        {
            if (!PrimaryAddress.HasValue())
                return null;
            try
            {
                var s = PrimaryAddress.Replace(".", "");
                var a = s.SplitStr(" ");
                var la = a.ToList();
                if (la[0].AllDigits())
                    la.RemoveAt(0);
                var quadrants = new string[] { "N", "NORTH", "S", "SOUTH", "E", "EAST", "W", "WEST", "NE", "NORTHEAST", "NW", "NORTHWEST", "SE", "SOUTHEAST", "SW", "SOUTHWEST" };
                if (quadrants.Contains(a[0].ToUpper()))
                    la.RemoveAt(0);
                la.Reverse();
                if (la[0].AllDigits())
                    la.RemoveAt(0);
                if (la[0].StartsWith("#"))
                    la.RemoveAt(0);
                var apt = new string[] { "APARTMENT", "APT", "BUILDING", "BLDG", "DEPARTMENT", "DEPT", "FLOOR", "FL", "HANGAR", "HNGR", "LOT", "LOT", "PIER", "PIER", "ROOM", "RM", "SLIP", "SLIP", "SPACE", "SPC", "STOP", "STOP", "SUITE", "STE", "TRAILER", "TRLR", "UNIT", "UNIT", "UPPER", "UPPR",
        	                    "BASEMENT","BSMT", "FRONT","FRNT", "LOBBY","LBBY", "LOWER","LOWR", "OFFICE","OFC", "PENTHOUSE","PH", "REAR", "SIDE" };
                if (apt.Contains(la[0].ToUpper()))
                    la.RemoveAt(0);
                if (db.StreetTypes.Any(t => t.Type == la[0]))
                    la.RemoveAt(0);
                la.Reverse();
                var street = string.Join(" ", la);
                return street;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void FixTitle()
        {
            if (GenderId == 1)
                TitleCode = "Mr.";
            else if (GenderId == 2)
                if (MaritalStatusId == 20 || MaritalStatusId == 50)
                    TitleCode = "Mrs.";
                else
                    TitleCode = "Ms.";
        }
        public string OptOutKey(string FromEmail)
        {
            return Util.EncryptForUrl("{0}|{1}".Fmt(PeopleId, FromEmail));
        }

        public static bool ToggleTag(int PeopleId, string TagName, int? OwnerId, int TagTypeId)
        {
            var Db = DbUtil.Db;
            var tag = Db.FetchOrCreateTag(TagName, OwnerId, TagTypeId);
            if (tag == null)
                throw new Exception("ToggleTag, tag '{0}' not found");
            var tp = Db.TagPeople.SingleOrDefault(t => t.Id == tag.Id && t.PeopleId == PeopleId);
            if (tp == null)
            {
                tag.PersonTags.Add(new TagPerson { PeopleId = PeopleId });
                return true;
            }
            Db.TagPeople.DeleteOnSubmit(tp);
            return false;
        }
        public static void Tag(CMSDataContext Db, int PeopleId, string TagName, int? OwnerId, int TagTypeId)
        {
            var tag = Db.FetchOrCreateTag(TagName, OwnerId, TagTypeId);
            var tp = Db.TagPeople.SingleOrDefault(t => t.Id == tag.Id && t.PeopleId == PeopleId);
            var isperson = Db.People.Count(p => p.PeopleId == PeopleId) > 0;
            if (tp == null && isperson)
                tag.PersonTags.Add(new TagPerson { PeopleId = PeopleId });
        }
        public static void UnTag(int PeopleId, string TagName, int? OwnerId, int TagTypeId)
        {
            var Db = DbUtil.Db;
            var tag = Db.FetchOrCreateTag(TagName, OwnerId, TagTypeId);
            var tp = Db.TagPeople.SingleOrDefault(t => t.Id == tag.Id && t.PeopleId == PeopleId);
            if (tp != null)
                Db.TagPeople.DeleteOnSubmit(tp);
        }
        partial void OnNickNameChanged()
        {
            if (NickName != null && NickName.Trim() == String.Empty)
                NickName = null;
        }
        private bool _DecisionTypeIdChanged;
        public bool DecisionTypeIdChanged
        {
            get { return _DecisionTypeIdChanged; }
        }
        partial void OnDecisionTypeIdChanged()
        {
            _DecisionTypeIdChanged = true;
        }
        private bool _NewMemberClassStatusIdChanged;
        public bool NewMemberClassStatusIdChanged
        {
            get { return _NewMemberClassStatusIdChanged; }
        }
        partial void OnNewMemberClassStatusIdChanged()
        {
            _NewMemberClassStatusIdChanged = true;
        }
        private bool _BaptismStatusIdChanged;
        public bool BaptismStatusIdChanged
        {
            get { return _BaptismStatusIdChanged; }
        }
        partial void OnBaptismStatusIdChanged()
        {
            _BaptismStatusIdChanged = true;
        }
        private bool _DeceasedDateChanged;
        public bool DeceasedDateChanged
        {
            get { return _DeceasedDateChanged; }
        }
        partial void OnDeceasedDateChanged()
        {
            _DeceasedDateChanged = true;
        }
        private bool _DropCodeIdChanged;
        public bool DropCodeIdChanged
        {
            get { return _DropCodeIdChanged; }
        }
        partial void OnDropCodeIdChanged()
        {
            _DropCodeIdChanged = true;
        }
        //internal static int FindResCode(string zipcode)
        //{
        //    if (zipcode.HasValue() && zipcode.Length >= 5)
        //    {
        //        var z5 = zipcode.Substring(0, 5);
        //        var z = DbUtil.Db.Zips.SingleOrDefault(zip => z5 == zip.ZipCode);
        //        if (z == null)
        //            return 30;
        //        return z.MetroMarginalCode ?? 30;
        //    }
        //    return 30;
        //}
        private bool? _CanUserEditAll;
        public bool CanUserEditAll
        {
            get
            {
                if (!_CanUserEditAll.HasValue)
                    _CanUserEditAll = HttpContext.Current.User.IsInRole("Edit");
                return _CanUserEditAll.Value;
            }
        }
        private bool? _CanUserEditFamilyAddress;
        public bool CanUserEditFamilyAddress
        {
            get
            {
                if (!_CanUserEditFamilyAddress.HasValue)
                    _CanUserEditFamilyAddress = CanUserEditAll
                        || Util.UserPeopleId == Family.HeadOfHouseholdId
                        || Util.UserPeopleId == Family.HeadOfHouseholdSpouseId;
                return _CanUserEditFamilyAddress.Value;
            }
        }
        private bool? _CanUserEditBasic;
        public bool CanUserEditBasic
        {
            get
            {
                if (!_CanUserEditBasic.HasValue)
                    _CanUserEditBasic = CanUserEditFamilyAddress
                        || Util.UserPeopleId == PeopleId;
                return _CanUserEditBasic.Value;
            }
        }
        private bool? _CanUserSee;
        public bool CanUserSee
        {
            get
            {
                if (!_CanUserSee.HasValue)
                    _CanUserSee = CanUserEditBasic
                        || Family.People.Any(m => m.PeopleId == Util.UserPeopleId);
                return _CanUserSee.Value;
            }
        }

        //partial void OnZipCodeChanged()
        //{
        //    ResCodeId = FindResCode(Db, ZipCode);
        //}
        //partial void OnAltZipCodeChanged()
        //{
        //    AltResCodeId = FindResCode(Db, AltZipCode);
        //}
        public RecReg GetRecReg()
        {
            var rr = RecRegs.SingleOrDefault();
            if (rr == null)
                return new RecReg();
            return rr;
        }
        public RecReg SetRecReg()
        {
            var rr = RecRegs.SingleOrDefault();
            if (rr == null)
            {
                rr = new RecReg();
                RecRegs.Add(rr);
            }
            return rr;
        }
        public Contact AddContactReceived(CMSDataContext Db, DateTime dt, int type, int reason, string notes)
        {
            var c = new Contact
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                ContactDate = dt,
                ContactTypeId = type,
                ContactReasonId = reason,
                Comments = notes
            };
            Db.Contacts.InsertOnSubmit(c);
            c.contactees.Add(new Contactee { PeopleId = PeopleId });
            Db.SubmitChanges();
            return c;
        }

        private StringBuilder psbDefault;
        public void UpdateValue(string field, object value)
        {
            if (psbDefault == null)
                psbDefault = new StringBuilder();
            UpdateValue(psbDefault, field, value);
        }
        public void UpdateValue(StringBuilder psb, string field, object value)
        {
            if (value is string)
                value = ((string)value).TrimEnd();
            var o = Util.GetProperty(this, field);
            if (o is string)
                o = ((string)o).TrimEnd();
            if (o == null && value == null)
                return;
            if (o != null && o.Equals(value))
                return;
            if (o == null && value is string && !((string)value).HasValue())
                return;
            if (value == null && o is string && !((string)o).HasValue())
                return;
            psb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", field, o, value ?? "(null)");
            Util.SetProperty(this, field, value);
        }
        public void UpdateValueFromText(StringBuilder psb, string field, string value)
        {
            value = value.TrimEnd();
            var o = Util.GetProperty(this, field);
            if (o is string)
                o = ((string)o).TrimEnd();
            if (o == null && value == null)
                return;
            if (o != null && o.Equals(value))
                return;
            if (o == null && value is string && !((string)value).HasValue())
                return;
            if (value == null && o is string && !((string)o).HasValue())
                return;
            psb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", field, o, value ?? "(null)");
            Util.SetPropertyFromText(this, field, value);
        }
        public void LogChanges(CMSDataContext Db, int UserPeopleId)
        {
            if (psbDefault != null)
                LogChanges(Db, psbDefault, UserPeopleId);
        }
        public void LogChanges(CMSDataContext Db, StringBuilder psb, int UserPeopleId)
        {
            if (psb.Length > 0)
            {
                var c = new ChangeLog
                {
                    UserPeopleId = UserPeopleId,
                    PeopleId = PeopleId,
                    Field = "Basic Info",
                    Data = "<table>\n" + psb.ToString() + "</table>",
                    Created = Util.Now
                };
                Db.ChangeLogs.InsertOnSubmit(c);
            }
        }
        public void LogPictureUpload(CMSDataContext Db, int UserPeopleId)
        {
            var c = new ChangeLog
            {
                UserPeopleId = UserPeopleId,
                PeopleId = PeopleId,
                Field = "Basic Info",
                Data = "<table>\n<tr><td>Picture</td><td></td><td>(new upload)</td></tr>\n</table>",
                Created = Util.Now
            };
            Db.ChangeLogs.InsertOnSubmit(c);
        }
        public override string ToString()
        {
            return Name + "(" + PeopleId + ")";
        }
        public void SetExtra(string field, string value)
        {
            var e = PeopleExtras.FirstOrDefault(ee => ee.Field == field);
            if (e == null)
            {
                e = new PeopleExtra { Field = field, PeopleId = PeopleId, TransactionTime = DateTime.Now };
                this.PeopleExtras.Add(e);
            }
            e.StrValue = value;
        }
        public string GetExtra(string field)
        {
            var e = PeopleExtras.SingleOrDefault(ee => ee.Field == field);
            if (e == null)
                return "";
            if (e.StrValue.HasValue())
                return e.StrValue;
            if (e.Data.HasValue())
                return e.Data;
            if (e.DateValue.HasValue)
                return e.DateValue.FormatDate();
            if (e.IntValue.HasValue)
                return e.IntValue.ToString();
            return e.BitValue.ToString();
        }
        public PeopleExtra GetExtraValue(string field)
        {
            if (!field.HasValue())
                field = "blank";
            field = field.Replace(",", "_");
            var ev = PeopleExtras.AsEnumerable().FirstOrDefault(ee => string.Compare(ee.Field, field, ignoreCase: true) == 0);
            if (ev == null)
            {
                ev = new PeopleExtra
                {
                    Field = field,
                    TransactionTime = DateTime.Now
                };
                PeopleExtras.Add(ev);
            }
            return ev;
        }
        public void RemoveExtraValue(CMSDataContext Db, string field)
        {
            var ev = PeopleExtras.AsEnumerable().FirstOrDefault(ee => string.Compare(ee.Field, field, ignoreCase: true) == 0);
            if (ev != null)
                Db.PeopleExtras.DeleteOnSubmit(ev);
        }
        public void AddEditExtraValue(string field, string value)
        {
            if (!field.HasValue())
                return;
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.StrValue = value;
        }
        public void AddEditExtraDate(string field, DateTime? value)
        {
            if (!value.HasValue)
                return;
            var ev = GetExtraValue(field);
            ev.DateValue = value;
        }
        public void AddEditExtraData(string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.Data = value;
        }
        public void AddToExtraData(string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(field);
            if (ev.Data.HasValue())
                ev.Data = value + "\n" + ev.Data;
            else
                ev.Data = value;
        }
        public void AddEditExtraInt(string field, int value)
        {
            var ev = GetExtraValue(field);
            ev.IntValue = value;
        }
        public void AddEditExtraBool(string field, bool tf)
        {
            if (!field.HasValue())
                return;
            var ev = GetExtraValue(field);
            ev.BitValue = tf;
        }
        public void AddEditExtraInts(string field, int value, int value2)
        {
            var ev = GetExtraValue(field);
            ev.IntValue = value;
            ev.IntValue2 = value2;
        }
        public ManagedGiving ManagedGiving()
        {
            var mg = ManagedGivings.SingleOrDefault();
            return mg;
        }
        public PaymentInfo PaymentInfo()
        {
            var pi = PaymentInfos.SingleOrDefault();
            return pi;
        }
        public Contribution PostUnattendedContribution(CMSDataContext Db, decimal Amt, int? Fund, string Description, bool pledge = false)
        {
            var typecode = BundleTypeCode.Online;
            if (pledge)
                typecode = BundleTypeCode.OnlinePledge;

            var now = DateTime.Now;
            var d = Util.Now.Date;
            var q = from b in Db.BundleHeaders
                    where b.BundleHeaderTypeId == typecode
                    where b.BundleStatusId == BundleStatusCode.Open
                    where b.ContributionDate >= d
                    where b.ContributionDate < now
                    where b.ContributionDate.Year == now.Year
                    where b.ContributionDate.Month == now.Month
                    orderby b.ContributionDate descending
                    select b;
            var bundle = q.FirstOrDefault();
            if (bundle == null)
            {
                bundle = new BundleHeader
                {
                    BundleHeaderTypeId = typecode,
                    BundleStatusId = BundleStatusCode.Open,
                    CreatedBy = Util.UserId1,
                    ContributionDate = d,
                    CreatedDate = now,
                    FundId = Db.Setting("DefaultFundId", "1").ToInt(),
                    RecordStatus = false,
                    TotalCash = 0,
                    TotalChecks = 0,
                    TotalEnvelopes = 0,
                    BundleTotal = 0
                };
                Db.BundleHeaders.InsertOnSubmit(bundle);
            }
            if (!Fund.HasValue)
                Fund = (from f in Db.ContributionFunds
                        where f.FundStatusId == 1
                        orderby f.FundId
                        select f.FundId).First();

            var FinanceManagerId = Db.Setting("FinanceManagerId", "").ToInt2();
            if (!FinanceManagerId.HasValue)
            {
                var qu = from u in Db.Users
                         where u.UserRoles.Any(ur => ur.Role.RoleName == "Finance")
                         orderby u.Person.LastName
                         select u.UserId;
                FinanceManagerId = qu.FirstOrDefault();
                if (!FinanceManagerId.HasValue)
                    FinanceManagerId = 1;
            }
            var bd = new CmsData.BundleDetail
            {
                BundleHeaderId = bundle.BundleHeaderId,
                CreatedBy = FinanceManagerId.Value,
                CreatedDate = now,
            };
            var typid = ContributionTypeCode.CheckCash;
            if (pledge)
                typid = ContributionTypeCode.Pledge;
            bd.Contribution = new Contribution
            {
                CreatedBy = FinanceManagerId.Value,
                CreatedDate = bd.CreatedDate,
                FundId = Fund.Value,
                PeopleId = PeopleId,
                ContributionDate = bd.CreatedDate,
                ContributionAmount = Amt,
                ContributionStatusId = 0,
                ContributionTypeId = typid,
                ContributionDesc = Description,
            };
            bundle.BundleDetails.Add(bd);
            Db.SubmitChanges();
            return bd.Contribution;
        }
        public static int FetchOrCreateMemberStatus(CMSDataContext Db, string type)
        {
            var ms = Db.MemberStatuses.SingleOrDefault(m => m.Description == type);
            if (ms == null)
            {
                var max = Db.MemberStatuses.Max(mm => mm.Id) + 1;
                ms = new MemberStatus() { Id = max, Code = "M" + max, Description = type };
                Db.MemberStatuses.InsertOnSubmit(ms);
                Db.SubmitChanges();
            }
            return ms.Id;
        }
        public static Campu FetchOrCreateCampus(CMSDataContext Db, string campus)
        {
            if (!campus.HasValue())
                return null;
            var cam = Db.Campus.SingleOrDefault(pp => pp.Description == campus);
            if (cam == null)
            {
                int max = 10;
                if (Db.Campus.Any())
                    max = Db.Campus.Max(mm => mm.Id) + 10;
                cam = new Campu() { Id = max, Description = campus, Code = campus.Truncate(20) };
                Db.Campus.InsertOnSubmit(cam);
                Db.SubmitChanges();
            }
            else if (!cam.Code.HasValue())
            {
                cam.Code = campus.Truncate(20);
                Db.SubmitChanges();
            }
            return cam;
        }
        public Task AddTaskAbout(CMSDataContext Db, int AssignTo, string description)
        {
            var t = new Task
            {
                OwnerId = AssignTo,
                Description = description,
                ForceCompleteWContact = true,
                ListId = Task.GetRequiredTaskList("InBox", AssignTo).Id,
                StatusId = TaskStatusCode.Active,
            };
            TasksAboutPerson.Add(t);
            return t;
        }
    }
}