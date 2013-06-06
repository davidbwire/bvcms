using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class Contact
    {
        public static int AddContact(int qid)
        {
            var q = DbUtil.Db.PeopleQuery(qid);
            if (q.Count() > 100)
                return -1;
            if (q.Count() == 0)
                return 0;
            var c = new Contact 
			{ 
				ContactDate = DateTime.Now.Date, 
				CreatedBy = Util.UserId1,
	            CreatedDate = DateTime.Now,
	            ContactTypeId = ContactTypeCode.Other,
	            ContactReasonId = ContactReasonCode.Other,
			};
            foreach (var p in q)
                c.contactees.Add(new Contactee { PeopleId = p.PeopleId });
            DbUtil.Db.Contacts.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();
            return c.ContactId;
        }
        public static Contact AddContact(CMSDataContext Db, int pid, DateTime? date, string comments)
        {
            var c = new Contact 
			{ 
				ContactDate = date ?? DateTime.Parse("1/1/1900"), 
				CreatedBy = Util.UserPeopleId ?? Util.UserId1,
	            CreatedDate = DateTime.Now,
	            ContactTypeId = ContactTypeCode.Other,
	            ContactReasonId = ContactReasonCode.Other,
				Comments = comments
			};
            c.contactees.Add(new Contactee { PeopleId = pid });
            Db.Contacts.InsertOnSubmit(c);
            Db.SubmitChanges();
			return c;
        }
		public static ContactType FetchOrCreateContactType(CMSDataContext Db, string type)
		{
			var ct = Db.ContactTypes.SingleOrDefault(pp => pp.Description == type);
			if (ct == null)
			{
				var max = Db.ContactTypes.Max(mm => mm.Id) + 10;
				if (max < 1000)
					max = 1010;
				ct = new ContactType { Id = max, Description = type, Code = type.Truncate(20) };
				Db.ContactTypes.InsertOnSubmit(ct);
				Db.SubmitChanges();
			}
			return ct;
		}
		public static Ministry FetchOrCreateMinistry(CMSDataContext Db, string name)
		{
			var m = Db.Ministries.SingleOrDefault(pp => pp.MinistryName == name);
			if (m == null)
			{
				m = new Ministry { MinistryName = name };
				Db.Ministries.InsertOnSubmit(m);
				Db.SubmitChanges();
			}
			return m;
		}


        public static ContactReason FetchOrCreateContactReason(CMSDataContext db, string reason)
        {
            var r = db.ContactReasons.SingleOrDefault(pp => pp.Description == reason);
            if (r == null)
            {
                var max = db.ContactReasons.Max(mm => mm.Id) + 10;
                if (max < 1000)
                    max = 1010;
                r = new ContactReason { Id = max, Description = reason, Code = reason.Truncate(20) };
                db.ContactReasons.InsertOnSubmit(r);
                db.SubmitChanges();
            }
            return r;
        }
    }
}
