﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using UtilityExtensions;

namespace CMSWeb.Models
{
    public class VolunteerModel
    {
        private int _OpportunityId;
        public int OpportunityId
        {
            get
            {
                return _OpportunityId;
            }
            set
            {
                _OpportunityId = value;
                Opportunity = DbUtil.Db.VolOpportunities.Single(o => o.Id == value);
            }
        }
        public VolOpportunity Opportunity;
        public string first {get; set;}
        public string lastname {get; set;}
        public string dob {get; set;}
        private DateTime _dob;
        public DateTime DOB { get { return _dob;} }
        public string zip {get; set;}
        public string phone {get; set;}
        public string email { get; set; }
        public Person person { get; set; }
        public string[] interests { get; set; }

        public int FindMember()
        {
            var fone = Util.GetDigits(phone);
            var q = from p in DbUtil.Db.People
                    where (p.LastName.StartsWith(lastname) || p.MaidenName.StartsWith(lastname))
                            && (p.FirstName.StartsWith(first)
                            || p.NickName.StartsWith(first)
                            || p.MiddleName.StartsWith(first))
                    where p.CellPhone.Contains(phone)
                            || p.WorkPhone.Contains(phone)
                            || p.Family.HomePhone.Contains(phone)
                    where p.BirthDay == DOB.Day && p.BirthMonth == DOB.Month && p.BirthYear == DOB.Year
                    select p;
            var count = q.Count();
            if (count == 1)
                person = q.Single();
            return count;
        }

        public void ValidateModel(ModelStateDictionary ModelState)
        {
            if (!first.HasValue())
                ModelState.AddModelError("first", "first name required");
            if (!lastname.HasValue())
                ModelState.AddModelError("lastname", "last name required");
            if (!DateTime.TryParse(dob, out _dob))
                ModelState.AddModelError("dob", "valid birth date required");
            var d = phone.GetDigits().Length;
            if (d != 7 && d != 10)
                ModelState.AddModelError("phone", "7 or 10 digits");
            if (!email.HasValue() || !Util.ValidEmail(email))
                ModelState.AddModelError("email", "Please specify a valid email address.");
        }
    }
}