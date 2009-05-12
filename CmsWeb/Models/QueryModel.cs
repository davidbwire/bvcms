﻿/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;
using System.Data.Linq.SqlClient;
using System.Web.UI.WebControls;

namespace CMSWeb.Models
{
    interface IQBUpdateable
    {
        int? SelectedId { get; set; }
        string ConditionName { get; set; }
        int? Program { get; set; }
        int? Division { get; set; }
        int? Organization { get; set; }
        int? Schedule { get; set; }
        string Days { get; set; }
        string Week { get; set; }
        string Quarters { get; set; }
        string StartDate { get; set; }
        string EndDate { get; set; }
        string Comparison { get; set; }
        string[] Tags { get; set; }
        string CodeValue { get; set; }
        string[] CodeValues { get; set; }
        string TextValue { get; set; }
        string DateValue { get; set; }
        string NumberValue { get; set; }
        string IntegerValue { get; set; }
        string SavedQueryDesc { get; set; }
        string Sort { get; set; }
        string Direction { get; set; }
        int? Page { get; set; }
        int? PageSize { get; set; }
    }
    public class QueryModel : IQBUpdateable
    {
        private CMSDataContext Db;
        private QueryBuilderClause Qb;
        private int TagTypeId { get; set; }
        private string TagName { get; set; }
        private int? TagOwner { get; set; }

        public QueryModel()
        {
            Db = new CMSDataContext(Util.ConnectionString);
            ConditionName = "Group";
            Direction = "asc";
            TagTypeId = DbUtil.TagTypeId_Personal;
            TagName = Util.CurrentTagName;
            TagOwner = Util.CurrentTagOwnerId;
            Errors = new Dictionary<string, string>();
        }
        public string Description { get; set; }
        public int? QueryId { get; set; }

        public void LoadScratchPad()
        {
            Qb = Db.QueryBuilderScratchPad();
            if (QueryId.HasValue && QueryId.Value != Qb.QueryId)
            {
                var existing = Db.LoadQueryById(QueryId.Value);
                if (existing != null)
                {
                    Qb.CopyFromAll(existing);
                    Description = Qb.Description;
                    Qb.Description = Util.ScratchPad;
                    Db.SubmitChanges();
                    SelectedId = Qb.QueryId;
                    EditCondition();
                }
            }
            QueryId = Qb.QueryId;
            if (SelectedId == null)
                SelectedId = Qb.QueryId;
        }

        //private int? SessionQueryId
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session["QueryId"].IsNotNull())
        //            return (int)HttpContext.Current.Session["QueryId"];
        //        return null;
        //    }
        //    set
        //    {
        //        if (value.HasValue)
        //            HttpContext.Current.Session["QueryId"] = value.Value;
        //        else
        //            HttpContext.Current.Session.Remove("QueryId");
        //    }
        //}

        public int? SelectedId { get; set; }

        public bool RightPanelVisible { get; set; }
        public bool TextVisible { get; set; }
        public bool NumberVisible { get; set; }
        public bool IntegerVisible { get; set; }
        public bool CodeVisible { get; set; }
        public bool CodesVisible { get; set; }
        public bool DateVisible { get; set; }
        public bool ProgramVisible { get; set; }
        public bool DivisionVisible { get; set; }
        public bool EndDateVisible { get; set; }
        public bool StartDateVisible { get; set; }
        public bool OrganizationVisible { get; set; }
        public bool ScheduleVisible { get; set; }
        public bool DaysVisible { get; set; }
        public bool WeekVisible { get; set; }
        public bool SavedQueryVisible { get; set; }
        public bool QuartersVisible { get; set; }
        public bool TagsVisible { get; set; }

        public List<SelectListItem> TagData { get; set; }
        public List<SelectListItem> CodeData { get; set; }
        public List<SelectListItem> CompareData { get; set; }
        public List<SelectListItem> ProgramData { get; set; }
        public List<SelectListItem> DivisionData { get; set; }
        public List<SelectListItem> OrganizationData { get; set; }
        public int? Program { get; set; }
        public int? Division { get; set; }
        public int? Organization { get; set; }
        public int? Schedule { get; set; }
        public string Days { get; set; }
        public string Week { get; set; }
        public string Quarters { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Comparison { get; set; }
        public string[] Tags { get; set; }
        public string SavedQueryDesc { get; set; }
        public bool IsPublic { get; set; }

        public string CodeValue { get; set; }
        public string[] CodeValues { get; set; }
        public string TextValue { get; set; }
        public string DateValue { get; set; }
        public string NumberValue { get; set; }
        public string IntegerValue { get; set; }

        public bool UpdateEnabled { get; set; }
        public bool AddToGroupEnabled { get; set; }
        public bool AddEnabled { get; set; }
        public bool RemoveEnabled { get; set; }
        public bool SelectMultiple { get; set; }

        private FieldClass fieldMap;
        private string _ConditionName;
        public string ConditionName
        {
            get { return _ConditionName; }
            set
            {
                _ConditionName = value;
                fieldMap = FieldClass.Fields[value];
            }
        }
        public string ConditionText { get { return fieldMap.Title; } }

        public void SetVisibility()
        {
            //TextValue = "";
            //IntegerValue = "";
            //NumberValue = "";
            //DateValue = "";
            //Program = 0;
            //CodeValue = null;

            RightPanelVisible = true;
            TextVisible = false;
            NumberVisible = false;
            CodeVisible = false;
            CodesVisible = false;
            DateVisible = false;
            CodeData = null;
            ConditionName = ConditionName;
            CompareData = Comparisons().ToList();
            DivisionVisible = fieldMap.HasParam("SubDivOrg");
            ProgramVisible = fieldMap.HasParam("DivOrg");
            OrganizationVisible = fieldMap.HasParam("Organization");
            ScheduleVisible = fieldMap.HasParam("Schedule");
            DaysVisible = fieldMap.HasParam("Days");
            WeekVisible = fieldMap.HasParam("Week");
            SavedQueryVisible = fieldMap.HasParam("SavedQueryIdDesc");
            QuartersVisible = fieldMap.HasParam("Quarters");
            TagsVisible = fieldMap.HasParam("Tags");
            if (TagsVisible)
            {
                var cv = new CMSPresenter.CodeValueController();
                TagData = ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
            }
            StartDateVisible = fieldMap.HasParam("StartDate");
            EndDateVisible = fieldMap.HasParam("EndDate");

            var cvctl = new CMSPresenter.CodeValueController();
            switch (fieldMap.Type)
            {
                case FieldType.Bit:
                case FieldType.NullBit:
                    CodeVisible = true;
                    CodeData = ConvertToSelect(cvctl.BitCodes(), fieldMap.DataValueField);
                    break;
                case FieldType.String:
                case FieldType.DateString:
                    TextVisible = true;
                    break;
                case FieldType.NullNumber:
                case FieldType.Number:
                    NumberVisible = true;
                    break;
                case FieldType.NullInteger:
                case FieldType.Integer:
                    IntegerVisible = true;
                    break;
                case FieldType.Code:
                case FieldType.CodeStr:
                    CodeVisible = true;
                    CodeData = ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
                    break;
                case FieldType.Date:
                    DateVisible = true;
                    break;
                case FieldType.DateField:
                    CodeVisible = true;
                    CodeData = ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
                    break;
            }
            var cc = Db.LoadQueryById(SelectedId);
            if (cc == null)
                return;

            if (fieldMap.Type == FieldType.Group)
            {
                CompareData = Comparisons().ToList();
                RightPanelVisible = false;
                UpdateEnabled = cc.IsGroup;
                return;
            }
            UpdateEnabled = !cc.IsGroup && !cc.IsFirst;
            AddToGroupEnabled = cc.IsGroup;
            AddEnabled = !cc.IsFirst;
            RemoveEnabled = !cc.IsFirst;
        }
        private List<SelectListItem> ConvertToSelect(object items, string valuefield)
        {
            var list = items as IEnumerable<CMSPresenter.CodeValueItem>;
            switch (valuefield)
            {
                case "IdCode":
                    return list.Select(c => new SelectListItem { Text = c.Value, Value = c.IdCode }).ToList();
                case "Code":
                    return list.Select(c => new SelectListItem { Text = c.Value, Value = c.Code }).ToList();
                default:
                    return list.Select(c => new SelectListItem { Text = c.Value, Value = c.Value }).ToList();
            }
        }
        DateTime? DateParse(string s)
        {
            DateTime dt;
            if (DateTime.TryParse(s, out dt))
                return dt;
            return null;
        }
        int? IntParse(string s)
        {
            int i;
            if (int.TryParse(s, out i))
                return i;
            return null;
        }
        string DateString(DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToShortDateString();
            return "";
        }
        private void UpdateCondition(QueryBuilderClause c)
        {
            c.Field = ConditionName;
            c.Comparison = Comparison;
            switch (c.FieldInfo.Type)
            {
                case FieldType.String:
                case FieldType.DateString:
                    c.TextValue = TextValue;
                    break;
                case FieldType.Integer:
                case FieldType.NullInteger:
                    c.TextValue = IntegerValue;
                    break;
                case FieldType.Number:
                case FieldType.NullNumber:
                    c.TextValue = NumberValue;
                    break;
                case FieldType.Date:
                    c.DateValue = DateParse(DateValue);
                    break;
                case FieldType.Code:
                case FieldType.CodeStr:
                case FieldType.DateField:
                case FieldType.Bit:
                case FieldType.NullBit:
                    if (c.HasMultipleCodes)
                        c.CodeIdValue = string.Join(";", CodeValues);
                    else
                        c.CodeIdValue = CodeValue;
                    break;
            }
            c.DivOrg = Program ?? 0;
            c.SubDivOrg = Division ?? 0;
            c.Organization = Organization ?? 0;
            c.Schedule = Schedule ?? 0;
            c.StartDate = DateParse(StartDate);
            c.EndDate = DateParse(EndDate);
            c.Days = Days.ToInt();
            c.Quarters = Quarters;
            if (Tags != null)
                c.Tags = string.Join(";", Tags);
            c.SavedQueryIdDesc = SavedQueryDesc;
            Db.SubmitChanges();
        }
        public void EditCondition()
        {
            var c = Db.LoadQueryById(SelectedId);
            if (c == null)
                return;
            ConditionName = c.FieldInfo.Name;
            SetVisibility();
            Comparison = c.Comparison;
            switch (c.FieldInfo.Type)
            {
                case FieldType.String:
                case FieldType.DateString:
                    TextValue = c.TextValue;
                    break;
                case FieldType.Integer:
                case FieldType.NullInteger:
                    IntegerValue = c.TextValue;
                    break;
                case FieldType.Number:
                case FieldType.NullNumber:
                    NumberValue = c.TextValue;
                    break;
                case FieldType.Date:
                    DateValue = DateString(c.DateValue);
                    break;
                case FieldType.Code:
                case FieldType.CodeStr:
                case FieldType.DateField:
                case FieldType.Bit:
                case FieldType.NullBit:
                    CodeValue = c.CodeIdValue;
                    if (c.HasMultipleCodes && CodeValue.HasValue())
                    {
                        CodeValues = c.CodeIdValue.Split(';');
                        foreach (var i in CodeData)
                            i.Selected = CodeValues.Contains(i.Value);
                    }
                    break;
            }
            Program = c.DivOrg;
            DivisionData = Divisions(Program).ToList();
            Division = c.SubDivOrg;
            OrganizationData = Organizations(Division).ToList();
            Organization = c.Organization;
            Schedule = c.Schedule;
            StartDate = DateString(c.StartDate);
            EndDate = DateString(c.EndDate);
            SelectMultiple = c.HasMultipleCodes;
            AddToGroupEnabled = c.IsGroup;
            AddEnabled = !c.IsFirst;
            RemoveEnabled = !c.IsFirst;
            Days = c.Days.ToString();
            Quarters = c.Quarters;
            if (TagsVisible)
            {
                if(c.Tags != null)
                    Tags = c.Tags.Split(';');
                var cv = new CMSPresenter.CodeValueController();
                TagData = ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Code");
                foreach (var i in TagData)
                    i.Selected = Tags.Contains(i.Value);
            }
            SavedQueryDesc = c.SavedQueryIdDesc;
        }
        public void SetCodes()
        {
            SetVisibility();
            SelectMultiple = Comparison.EndsWith("OneOf");
        }
        private void NewCondition(QueryBuilderClause gc, int order)
        {
            var c = new QueryBuilderClause();
            c.ClauseOrder = order;
            gc.Clauses.Add(c);
            gc.ReorderClauses();
            UpdateCondition(c);
        }
        public void SaveQuery()
        {
            var saveto = Db.QueryBuilderClauses.FirstOrDefault(c =>
                c.SavedBy == Util.UserName && c.Description == Description);
            if (saveto == null)
            {
                saveto = new QueryBuilderClause();
                Db.QueryBuilderClauses.InsertOnSubmit(saveto);
            }
            saveto.CopyFromAll(Qb); // save Qb on top of existing
            saveto.SavedBy = Util.UserName;
            saveto.Description = SavedQueryDesc;
            saveto.IsPublic = IsPublic;
            Db.SubmitChanges();
            Description = SavedQueryDesc;
        }
        public void AddConditionToGroup()
        {
            var c = Db.LoadQueryById(SelectedId);
            NewCondition(c, c.MaxClauseOrder() + 1);
        }
        public void AddConditionAfterCurrent()
        {
            var c = Db.LoadQueryById(SelectedId);
            NewCondition(c.Parent, c.ClauseOrder + 1);
        }
        public void DeleteCondition()
        {
            var c = Db.LoadQueryById(SelectedId);
            SelectedId = c.Parent.QueryId;
            Db.DeleteQueryBuilderClauseOnSubmit(c);
            Db.SubmitChanges();
            EditCondition();
        }
        public void UpdateCondition()
        {
            var c = Db.LoadQueryById(SelectedId);
            UpdateCondition(c);
        }
        public void CopyAsNew()
        {
            var Qb = Db.LoadQueryById(SelectedId).Clone();
            if (!Qb.IsGroup)
            {
                var g = new QueryBuilderClause();
                g.SetQueryType(QueryType.Group);
                g.SetComparisonType(CompareType.AllTrue);
                Qb.Parent = g;
                Qb = g;
            }
            Db.SubmitChanges();
            QueryId = Qb.QueryId;
        }
        public void InsertGroupAbove()
        {
            var cc = Db.LoadQueryById(SelectedId);
            var g = new QueryBuilderClause();
            g.SetQueryType(QueryType.Group);
            g.SetComparisonType(CompareType.AllTrue);
            g.ClauseOrder = cc.ClauseOrder;
            if (cc.IsFirst)
                cc.Parent = g;
            else
            {
                var currParent = cc.Parent;
                // find all clauses from cc down at same level
                var q = from c in cc.Parent.Clauses
                        orderby c.ClauseOrder
                        where c.ClauseOrder >= cc.ClauseOrder
                        select c;
                foreach (var c in q)
                    c.Parent = g;   // change to new parent
                g.Parent = currParent;
            }
            if (cc.SavedBy.HasValue())
            {
                g.SavedBy = Util.UserName;
                g.Description = cc.Description;
                g.CreatedOn = cc.CreatedOn;
                cc.IsPublic = false;
                cc.Description = null;
                cc.SavedBy = null;
            }
            Db.SubmitChanges();
            if (g.IsFirst)
            {
                Qb = g;
                QueryId = g.QueryId;
            }
        }
        public IEnumerable<SelectListItem> Comparisons()
        {
            return from c in CompareClass.Comparisons
                   where c.FieldType == fieldMap.Type
                   select new SelectListItem { Text = c.CompType.ToString(), Value = c.CompType.ToString() };
        }
        public IEnumerable<SelectListItem> Schedules()
        {
            var q = from t in Db.WeeklySchedules
                    orderby t.MeetingTime
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Programs()
        {
            var q = from t in Db.Programs
                    orderby t.Name
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Divisions(int? progid)
        {
            var q = from div in Db.Divisions
                    where div.ProgId == progid
                    orderby div.Name
                    select new SelectListItem
                    {
                        Value = div.Id.ToString(),
                        Text = div.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Organizations(int? divid)
        {
            var q = from ot in Db.DivOrgs
                    where ot.DivId == divid
                    && (SqlMethods.DateDiffMonth(ot.Organization.OrganizationClosedDate, Util.Now) < 14
                        || ot.Organization.OrganizationStatusId == 30)
                    where Util.OrgMembersOnly == false || (ot.Organization.SecurityTypeId != 3)
                    orderby ot.Organization.OrganizationStatusId, ot.Organization.OrganizationName
                    select new SelectListItem
                    {
                        Value = ot.OrgId.ToString(),
                        Text = CmsData.Organization.FormatOrgName(ot.Organization.OrganizationName,
                           ot.Organization.LeaderName, ot.Organization.Location)
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        private int level;
        public List<QueryClauseDisplay> ConditionList()
        {
            if (Qb == null)
                LoadScratchPad();
            level = 0;
            return ClauseAndSubs(new List<QueryClauseDisplay>(), Qb);
        }
        private List<QueryClauseDisplay> ClauseAndSubs(List<QueryClauseDisplay> list, QueryBuilderClause qc)
        {
            list.Add(new QueryClauseDisplay { Level = (level * 25), Clause = qc });
            level++;
            var q = qc.Clauses.OrderBy(c => c.ClauseOrder);
            foreach (var c in q)
                list = ClauseAndSubs(list, c);
            level--;
            return list;
        }
        public IEnumerable<CategoryClass> FieldCategories()
        {
            var q = from c in CategoryClass.Categories
                    where c.Title != "Grouping"
                    select c;
            return q;
        }
        public List<SelectListItem> SavedQueries()
        {
            var cv = new CMSPresenter.CodeValueController();
            return ConvertToSelect(cv.UserQueries(), null);
        }

        private IQueryable<Person> query;
        public int FetchCount()
        {
            query = PersonQuery();
            count = query.Count();
            return count ?? 0;
        }
        public IEnumerable<PeopleInfo> FetchPeopleList()
        {
            query = ApplySort(query);
            query = query.Skip(StartRow).Take(PageSize.Value);
            return FetchPeopleList(query);
        }
        private IQueryable<Person> PersonQuery()
        {
            Db.SetNoLock();
            if (Qb == null)
                LoadScratchPad();
            var q = Db.People.Where(Qb.Predicate());
            return q;
        }
        public void TagAll()
        {
            if (Qb == null)
                LoadScratchPad();
            var q = Db.People.Where(Qb.Predicate());
            Db.TagAll(q);
        }
        public void UnTagAll()
        {
            if (Qb == null)
                LoadScratchPad();
            var q = Db.People.Where(Qb.Predicate());
            Db.UnTagAll(q);
        }
        private IEnumerable<PeopleInfo> FetchPeopleList(IQueryable<Person> query)
        {
            var q = from p in query
                    select new PeopleInfo
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                        Address = p.PrimaryAddress,
                        Address2 = p.PrimaryAddress2,
                        CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                        HomePhone = p.HomePhone,
                        CellPhone = p.CellPhone,
                        WorkPhone = p.WorkPhone,
                        PhonePref = p.PhonePrefId,
                        MemberStatus = p.MemberStatus.Description,
                        Email = p.EmailAddress,
                        BFTeacher = p.BibleFellowshipTeacher,
                        BFTeacherId = p.BibleFellowshipTeacherId,
                        Age = p.Age.ToString(),
                        HasTag = p.Tags.Any(t => t.Tag.Name == TagName && t.Tag.PeopleId == TagOwner && t.Tag.TypeId == TagTypeId),
                    };
            return q;
        }
        private IQueryable<Person> ApplySort(IQueryable<Person> q)
        {
            if (Sort == null)
                Sort = "Name";
            if (Direction != "desc")
                switch (Sort)
                {
                    case "Name":
                        q = from p in q
                            orderby p.LastName,
                            p.FirstName,
                            p.PeopleId
                            select p;
                        break;
                    case "Status":
                        q = from p in q
                            orderby p.MemberStatus.Code,
                            p.LastName,
                            p.FirstName,
                            p.PeopleId
                            select p;
                        break;
                    case "Address":
                        q = from p in q
                            orderby p.PrimaryState,
                            p.PrimaryCity,
                            p.PrimaryAddress,
                            p.PeopleId
                            select p;
                        break;
                    case "Teacher":
                        q = from p in q
                            orderby p.BibleFellowshipTeacher,
                            p.LastName,
                            p.FirstName,
                            p.PeopleId
                            select p;
                        break;
                    case "DOB":
                        q = from p in q
                            orderby p.BirthMonth, p.BirthDay,
                            p.LastName, p.FirstName
                            select p;
                        break;
                }
            else
                switch (Sort)
                {
                    case "Status":
                        q = from p in q
                            orderby p.MemberStatus.Code descending,
                            p.LastName descending,
                            p.FirstName descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Address":
                        q = from p in q
                            orderby p.PrimaryState descending,
                            p.PrimaryCity descending,
                            p.PrimaryAddress descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Name":
                        q = from p in q
                            orderby p.LastName descending,
                            p.LastName descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Teacher":
                        q = from p in q
                            orderby p.BibleFellowshipTeacher descending,
                            p.LastName descending,
                            p.FirstName descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "DOB":
                        q = from p in q
                            orderby p.BirthMonth descending, p.BirthDay descending,
                            p.LastName descending, p.FirstName descending
                            select p;
                        break;
                }
            return q;
        }
        public Dictionary<string, string> Errors;

        #region Paging
        public bool ShowResults { get; set; }
        public string Sort { get; set; }
        public string Direction { get; set; }

        private int? _Page;
        public int? Page
        {
            get { return _Page ?? 1; }
            set { _Page = value; }
        }
        private int StartRow
        {
            get { return (Page.Value - 1) * PageSize.Value; }
        }
        public int? PageSize
        {
            get { return DbUtil.Db.UserPreference("PageSize", "10").ToInt(); }
            set
            {
                if (value.HasValue)
                    DbUtil.Db.SetUserPreference("PageSize", value);
            }
        }
        private int? count;
        public int Count
        {
            get
            {
                return count ?? 0;
            }
        }
        public PagerModel pagerModel()
        {
            return new PagerModel
            {
                Page = Page.Value,
                PageSize = PageSize.Value,
                Action = "List",
                Controller = "Task",
                Count = Count,
            };
        }
        #endregion
    }
    public class QueryClauseDisplay
    {
        public Unit Level { get; set; }
        public QueryBuilderClause Clause;
    }
}