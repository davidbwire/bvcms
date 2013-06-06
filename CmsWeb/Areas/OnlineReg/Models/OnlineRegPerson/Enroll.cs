using System;
using System.Linq;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;
using System.Text;
using CmsData.Codes;

namespace CmsWeb.Models
{
	public partial class OnlineRegPersonModel
	{
		public OrganizationMember Enroll(Transaction ti, string paylink, bool? testing, string others)
		{
			var om = OrganizationMember.InsertOrgMembers(DbUtil.Db,
				org.OrganizationId, person.PeopleId,
				MemberTypeCode.Member, DateTime.Now, null, false);
			om.Amount = TotalAmount();
		    om.AmountPaid = ti.Amt;

			var reg = person.RecRegs.SingleOrDefault();

			if (reg == null)
			{
				reg = new RecReg();
				person.RecRegs.Add(reg);
			}
			foreach (var ask in setting.AskItems)
			{
				switch (ask.Type)
				{
					case "AskSize":
						om.ShirtSize = shirtsize;
						reg.ShirtSize = shirtsize;
						break;
					case "AskChurch":
						reg.ActiveInAnotherChurch = otherchurch;
						reg.Member = memberus;
						break;
					case "AskAllergies":
						reg.MedAllergy = medical.HasValue();
						reg.MedicalDescription = medical;
						break;
					case "AskParents":
						reg.Mname = mname;
						reg.Fname = fname;
						break;
					case "AskEmContact":
						reg.Emcontact = emcontact;
						reg.Emphone = emphone;
						break;
					case "AskTylenolEtc":
						reg.Tylenol = tylenol;
						reg.Advil = advil;
						reg.Robitussin = robitussin;
						reg.Maalox = maalox;
						break;
					case "AskDoctor":
						reg.Docphone = docphone;
						reg.Doctor = doctor;
						break;
					case "AskCoaching":
						reg.Coaching = coaching;
						break;
					case "AskInsurance":
						reg.Insurance = insurance;
						reg.Policy = policy;
						break;
					case "AskTickets":
						om.Tickets = ntickets;
						break;
					case "AskYesNoQuestions":
						if (setting.TargetExtraValues == false)
						{
							foreach (var yn in ((AskYesNoQuestions)ask).list)
							{
								om.RemoveFromGroup(DbUtil.Db, "Yes:" + yn.SmallGroup);
								om.RemoveFromGroup(DbUtil.Db, "No:" + yn.SmallGroup);
							}
							foreach (var g in YesNoQuestion)
								om.AddToGroup(DbUtil.Db, (g.Value == true ? "Yes:" : "No:") + g.Key);
						}
						else
							foreach (var g in YesNoQuestion)
								person.AddEditExtraValue(g.Key, g.Value == true ? "Yes" : "No");
						break;
					case "AskCheckboxes":
						if (setting.TargetExtraValues)
						{
							foreach (var ck in ((AskCheckboxes)ask).list)
								person.RemoveExtraValue(DbUtil.Db, ck.SmallGroup);
							foreach (var g in ((AskCheckboxes)ask).CheckboxItemsChosen(Checkbox))
								person.AddEditExtraValue(g.SmallGroup, "true");
						}
						else
						{
							foreach (var ck in ((AskCheckboxes)ask).list)
								ck.RemoveFromSmallGroup(DbUtil.Db, om);
							foreach (var i in ((AskCheckboxes)ask).CheckboxItemsChosen(Checkbox))
								i.AddToSmallGroup(DbUtil.Db, om, PythonEvents);
						}
						break;
					case "AskExtraQuestions":
						foreach (var g in ExtraQuestion[ask.UniqueId])
							if (g.Value.HasValue())
        						if (setting.TargetExtraValues)
									person.AddEditExtraData(g.Key, g.Value);
        						else
									om.AddToMemberData("{0}: {1}".Fmt(g.Key, g.Value));
						break;
					case "AskMenu":
						foreach (var i in MenuItem)
							om.AddToGroup(DbUtil.Db, i.Key, i.Value);
						{
							var menulabel = "Menu Items";
							foreach (var i in ((AskMenu)ask).MenuItemsChosen(MenuItem))
							{
								om.AddToMemberData(menulabel);
								string desc;
								if (i.amt > 0)
									desc = "{0} {1} (at {2:N2})".Fmt(i.number, i.desc, i.amt);
								else
									desc = "{0} {1}".Fmt(i.number, i.desc);
								om.AddToMemberData(desc);
								menulabel = string.Empty;
							}
						}
						break;
					case "AskDropdown":
						if (setting.TargetExtraValues)
						{
							foreach (var op in ((AskDropdown)ask).list)
								person.RemoveExtraValue(DbUtil.Db, op.SmallGroup);
							person.AddEditExtraValue(((AskDropdown)ask).SmallGroupChoice(option).SmallGroup, "true");
						}
						else
						{
							foreach (var op in ((AskDropdown)ask).list)
								op.RemoveFromSmallGroup(DbUtil.Db, om);
							((AskDropdown)ask).SmallGroupChoice(option).AddToSmallGroup(DbUtil.Db, om, PythonEvents);
						}
						break;
					case "AskGradeOptions":
						if (setting.TargetExtraValues)
						    person.Grade = gradeoption.ToInt();
						else
						{
							om.Grade = gradeoption.ToInt();
							person.Grade = gradeoption.ToInt();
						}
						break;
				}
			}
			if (setting.TargetExtraValues)
			{
				foreach (var ag in setting.AgeGroups)
					person.RemoveExtraValue(DbUtil.Db, ag.SmallGroup);
				if (setting.AgeGroups.Count > 0)
					person.AddEditExtraValue(AgeGroup(), "true");
			}
			else
			{
				foreach (var ag in setting.AgeGroups)
					om.RemoveFromGroup(DbUtil.Db, ag.SmallGroup);
				if (setting.AgeGroups.Count > 0)
					om.AddToGroup(DbUtil.Db, AgeGroup());
			}

			if (setting.LinkGroupsFromOrgs.Count > 0)
			{
				var q = from omt in DbUtil.Db.OrgMemMemTags
						where setting.LinkGroupsFromOrgs.Contains(omt.OrgId)
						where omt.PeopleId == om.PeopleId
						select omt.MemberTag.Name;
				foreach (var name in q)
					om.AddToGroup(DbUtil.Db, name);
			}

		    var sb = new StringBuilder();

		    sb.AppendFormat("{0:g} ----------------\n", DateTime.Now);
			if (om.AmountPaid > 0)
			{
				sb.AppendFormat("{0:c} ({1} id) transaction amount\n", ti.Amt, ti.Id);
				sb.AppendFormat("{0:c} total due all registrants\n", ti.Amtdue);
				if (others.HasValue())
					sb.AppendFormat("Others: {0}\n", others);
			}
            om.AddToMemberData(sb.ToString());


		    var sbreg = new StringBuilder();
			sbreg.AppendFormat("{0}\n".Fmt(org.OrganizationName));
		    sbreg.AppendFormat("{0:g} ----------------\n", DateTime.Now);
			if (om.AmountPaid > 0)
			{
				sbreg.AppendFormat("{0:c} ({1} id) transaction amount\n", ti.Amt, ti.Id);
				sbreg.AppendFormat("{0:c} total due all registrants\n", ti.Amtdue);
				sbreg.AppendFormat("{0:c} total fee this registrant\n", TotalAmount());
			}
			if (paylink.HasValue())
			{
				sbreg.AppendLine(paylink);
				om.PayLink = paylink;
			}
			if (request.HasValue())
			{
				sbreg.AppendFormat("Request: {0}\n", request);
				om.Request = request;
			}
			sbreg.AppendFormat("{0}\n", email);

            reg.AddToComments(sbreg.ToString());

			DbUtil.Db.SubmitChanges();
			return om;
		}

		public string PrepareSummaryText(Transaction ti)
		{
			var om = GetOrgMember();
			var sb = new StringBuilder();
			sb.Append("<table>");
			sb.AppendFormat("<tr><td>Org:</td><td>{0}</td></tr>\n", org.OrganizationName);
			sb.AppendFormat("<tr><td>First:</td><td>{0}</td></tr>\n", person.PreferredName);
			sb.AppendFormat("<tr><td>Last:</td><td>{0}</td></tr>\n", person.LastName);

			var rr = person.RecRegs.Single();

			foreach (var ask in setting.AskItems)
			{
				switch (ask.Type)
				{
					case "AskTickets":
						sb.AppendFormat("<tr><td>Tickets:</td><td>{0}</td></tr>\n", om.Tickets);
						break;
					case "AskSize":
						sb.AppendFormat("<tr><td>Shirt:</td><td>{0}</td></tr>\n", om.ShirtSize);
						break;
					case "AskEmContact":
						sb.AppendFormat("<tr><td>Emerg Contact:</td><td>{0}</td></tr>\n", rr.Emcontact);
						sb.AppendFormat("<tr><td>Emerg Phone:</td><td>{0}</td></tr>\n", rr.Emphone);
						break;
					case "AskDoctor":
						sb.AppendFormat("<tr><td>Physician Name:</td><td>{0}</td></tr>\n", rr.Doctor);
						sb.AppendFormat("<tr><td>Physician Phone:</td><td>{0}</td></tr>\n", rr.Docphone);
						break;
					case "AskInsurance":
						sb.AppendFormat("<tr><td>Insurance Carrier:</td><td>{0}</td></tr>\n", rr.Insurance);
						sb.AppendFormat("<tr><td>Insurance Policy:</td><td>{0}</td></tr>\n", rr.Policy);
						break;
					case "AskRequest":
						sb.AppendFormat("<tr><td>{1}:</td><td>{0}</td></tr>\n", om.Request, ((AskRequest)ask).Label);
						break;
					case "AskHeader":
				        sb.AppendFormat("<tr><td colspan='2'><h4>{0}</h4></td></tr>\n", ((AskHeader)ask).Label);
						break;
					case "AskInstruction":
				        break;
					case "AskAllergies":
						sb.AppendFormat("<tr><td>Medical:</td><td>{0}</td></tr>\n", rr.MedicalDescription);
						break;
					case "AskTylenolEtc":
						sb.AppendFormat("<tr><td>Tylenol?: {0},", tylenol == true ? "Yes" : tylenol == false ? "No" : "");
						sb.AppendFormat(" Advil?: {0},", advil == true ? "Yes" : advil == false ? "No" : "");
						sb.AppendFormat(" Robitussin?: {0},", robitussin == true ? "Yes" : robitussin == false ? "No" : "");
						sb.AppendFormat(" Maalox?: {0}</td></tr>\n", maalox == true ? "Yes" : maalox == false ? "No" : "");
						break;
					case "AskChurch":
						sb.AppendFormat("<tr><td>Member:</td><td>{0}</td></tr>\n", rr.Member);
						sb.AppendFormat("<tr><td>OtherChurch:</td><td>{0}</td></tr>\n", rr.ActiveInAnotherChurch);
						break;
					case "AskParents":
						sb.AppendFormat("<tr><td>Mother's name:</td><td>{0}</td></tr>\n", rr.Mname);
						sb.AppendFormat("<tr><td>Father's name:</td><td>{0}</td></tr>\n", rr.Fname);
						break;
					case "AskCoaching":
						sb.AppendFormat("<tr><td>Coaching:</td><td>{0}</td></tr>\n", rr.Coaching);
						break;
					case "AskDropdown":
						sb.AppendFormat("<tr><td>{1}:</td><td>{0}</td></tr>\n", ((AskDropdown)ask).SmallGroupChoice(option).Description,
										Util.PickFirst(((AskDropdown)ask).Label, "Options"));
						break;
					case "AskMenu":
						{
							var menulabel = "Menu Items";
							foreach (var i in ((AskMenu)ask).MenuItemsChosen(MenuItem))
							{
								string row;
								if (i.amt > 0)
									row = "<tr><td>{0}</td><td>{1} {2} (at {3:N2})</td></tr>\n".Fmt(menulabel, i.number, i.desc, i.amt);
								else
									row = "<tr><td>{0}</td><td>{1} {2}</td></tr>\n".Fmt(menulabel, i.number, i.desc);
								sb.AppendFormat(row);
								menulabel = string.Empty;
							}
						}
						break;
					case "AskCheckboxes":
				        {
				            var askcb = (AskCheckboxes) ask;
				            var menulabel = askcb.Label;
							foreach (var i in askcb.CheckboxItemsChosen(Checkbox))
							{
								string row;
                                if (menulabel.HasValue())
									sb.Append("<tr><td colspan='2'><br>{0}</td></tr>\n".Fmt(menulabel));
								if (i.Fee > 0)
									row = "<tr><td></td><td>{0} (${1:N2})<br>({2})</td></tr>\n".Fmt(i.Description, i.Fee, i.SmallGroup);
								else
									row = "<tr><td></td><td>{0}<br>({1})</td></tr>\n".Fmt(i.Description, i.SmallGroup);
								sb.Append(row);
								menulabel = string.Empty;
							}
						}
						break;
					case "AskYesNoQuestions":
						foreach (var a in ((AskYesNoQuestions)ask).list)
                            if(YesNoQuestion.ContainsKey(a.SmallGroup))
    							sb.AppendFormat("<tr><td>{0}:</td><td>{1}</td></tr>\n".Fmt(a.Question,
    													   YesNoQuestion[a.SmallGroup] == true ? "Yes" : "No"));
						break;
					case "AskExtraQuestions":
						foreach (var a in ExtraQuestion[ask.UniqueId])
							if (a.Value.HasValue())
								sb.AppendFormat("<tr><td>{0}:</td><td>{1}</td></tr>\n".Fmt(a.Key, a.Value));
						break;
					case "AskGradeOptions":
						sb.AppendFormat("<tr><td>GradeOption:</td><td>{0}</td></tr>\n",
										GradeOptions(ask).SingleOrDefault(s => s.Value == (gradeoption ?? "00")).Text);
						break;

				}
			}
			if (setting.AgeGroups.Count > 0)
				sb.AppendFormat("<tr><td>AgeGroup:</td><td>{0}</td></tr>\n", AgeGroup());

			sb.Append("</table>");

			return sb.ToString();
		}
		private string AgeGroup()
		{
			foreach (var i in setting.AgeGroups)
				if (person.Age >= i.StartAge && person.Age <= i.EndAge)
					return i.SmallGroup;
			return string.Empty;
		}
	}
}