﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using UtilityExtensions;

namespace CMSRegCustom.Models
{
    public class SalesModel
    {
        public SalesModel(int? itemid)
        {
            this.itemid = itemid;
            quantity = saleitem.DefaultItems ?? 1;
        }
        public SalesModel()
        {
            
        }
        public int? itemid { get; set; }
        public decimal amount
        {
            get { return saleitem.Price * quantity; }
        }
        public int quantity { get; set; }
        public int? peopleid { get; set; }
        public string phone { get; set; }
        public string homecell { get; set; }
        public string email { get; set; }
        public bool preferredEmail { get; set; }
        public string TransactionId { get; set; }
        public string ServiceUOrgID
        {
            get
            {
                if ((string)HttpContext.Current.Session["test"] == "1")
                    return DbUtil.Settings("ServiceUOrgIDTest", "0");
                return DbUtil.Settings("ServiceUOrgID", "0");
            }
        }
        public string ServiceUOrgAccountID
        {
            get
            {
                if ((string)HttpContext.Current.Session["test"] == "1")
                    return DbUtil.Settings("ServiceUOrgAccountIDTest", "0");
                return DbUtil.Settings("ServiceUOrgAccountID", "0");
            }
        }

        private SaleItem _saleitem;
        public SaleItem saleitem
        {
            get
            {
                if (_saleitem == null)
                    _saleitem = DbUtil.Db.SaleItems.Single(s => s.Id == itemid);
                return _saleitem;
            }
        }
        private bool? _available;
        private bool available
        {
            get
            {
                if (!_available.HasValue && saleitem != null)
                    _available = saleitem.Available == true;
                return _available ?? false;
            }
        }
        public string disabled
        {
            get
            {
                return available ? "" : "disabled = \"disabled\"";
            }
        }
        public string Description
        {
            get
            {
                if (available)
                    return saleitem.Description;
                return saleitem.Description + " is no longer available, Sorry";
            }
        }
        internal SaleTransaction transaction;
        public int? tranid
        {
            get
            {
                return transaction.Id;
            }
            set
            {
                transaction = DbUtil.Db.SaleTransactions.Single(d => d.Id == value);
                peopleid = transaction.PeopleId;
                itemid = transaction.ItemId;
                quantity = transaction.Quantity;
                email = transaction.EmailAddress;
            }
        }

        private Person _person;
        public Person person
        {
            get
            {
                if (_person == null)
                    _person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == peopleid);
                return _person;
            }
        }
        public string first { get; set; }
        public string last { get; set; }
        public string dob { get; set; }
        public int? gender { get; set; }
        public DateTime birthday;

        public bool shownew { get; set; }
        public string addr { get; set; }
        public string zip { get; set; }
        public string city { get; set; }
        public string state { get; set; }

        public int FindMember()
        {
            first = first.Trim();
            last = last.Trim();
            var fone = Util.GetDigits(phone);
            var q = from p in DbUtil.Db.People
                    where (p.FirstName == first || p.NickName == first || p.MiddleName == first)
                    where (p.LastName == last || p.MaidenName == last)
                    where p.BirthDay == birthday.Day && p.BirthMonth == birthday.Month && p.BirthYear == birthday.Year
                    select p;
            var count = q.Count();
            if (count > 1)
                q = from p in q
                    where p.CellPhone.Contains(fone)
                            || p.WorkPhone.Contains(fone)
                            || p.Family.HomePhone.Contains(fone)
                    select p;
            count = q.Count();

            peopleid = null;
            if (count == 1)
                peopleid = q.Select(p => p.PeopleId).Single();
            return count;
        }

        public void ValidateModel(ModelStateDictionary modelState)
        {
            if (!first.HasValue())
                modelState.AddModelError("first", "first name required");
            if (!last.HasValue())
                modelState.AddModelError("last", "last name required");
            if (!Util.DateValid(dob, out birthday))
                modelState.AddModelError("dob", "valid birth date required");

            var d = phone.GetDigits().Length;
            if (d != 7 && d != 10)
                modelState.AddModelError("phone", "7 or 10 digits");
            if (!email.HasValue() || !Util.ValidEmail(email))
                modelState.AddModelError("email", "Please specify a valid email address.");
            if (shownew)
            {
                if (!gender.HasValue)
                    modelState.AddModelError("gender", "gender required");
                if (!addr.HasValue())
                    modelState.AddModelError("addr", "need address");
                if (zip.GetDigits().Length != 5)
                    modelState.AddModelError("zip", "need 5 digit zip");
                if (!city.HasValue())
                    modelState.AddModelError("city", "need city");
                if (!state.HasValue())
                    modelState.AddModelError("state", "need state");
            }
        }
        internal void AddPerson()
        {
            var f = new Family
            {
                AddressLineOne = addr,
                CityName = city,
                StateCode = state,
                ZipCode = zip,
            };
            var p = Person.Add(f, 30,
                null, first.Trim(), null, last.Trim(), dob, false, 1,
                    DbUtil.Settings("SaleOrigin", "0").ToInt(),
                    DbUtil.Settings("SaleEntry", "0").ToInt());
            p.MaritalStatusId = (int)Person.MaritalStatusCode.Unknown;
            p.EmailAddress = email;
            p.CampusId = DbUtil.Settings("DefaultCampusId", "").ToInt2();
            if (p.Age >= 18)
                p.PositionInFamilyId = (int)Family.PositionInFamily.PrimaryAdult;
            switch (homecell)
            {
                case "h":
                    f.HomePhone = phone.GetDigits();
                    break;
                case "c":
                    p.CellPhone = phone.GetDigits();
                    break;
            }
            DbUtil.Db.SubmitChanges();
            peopleid = p.PeopleId;
        }
        public IEnumerable<SaleTransaction> Transactions()
        {
            var q = from a in DbUtil.Db.SaleTransactions
                    orderby a.SaleDate descending
                    select a;
            return q;
        }
        public IEnumerable<SaleItem> SaleItems()
        {
            var q = from a in DbUtil.Db.SaleItems
                    orderby a.Id descending
                    select a;
            return q;
        }
    }
}