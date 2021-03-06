using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using UtilityExtensions;
using System.Web.UI.HtmlControls;

namespace CustomControls
{
    public class DisplayOrEditDropCheck : DropCheck, IDisplayOrEdit
    {
        public enum DisplayModes
        {
            Code,
            Text
        }
        public DisplayOrEditDropCheck() : base()
        {
            DisplayMode = DisplayModes.Text;
        }

        [Bindable(true), Category("Appearance"), DefaultValue(DisplayModes.Text)]
        public DisplayModes DisplayMode { get; set; }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.RegisterEditButton(Page);
        }
        public void DataBindList()
        {
            base.DataBind();
        }
        public override void DataBind()
        {
            if (EditUpdateButton.Updating)
                EditUpdateButton.UpdateBinding(this);
            //base.DataBind();
            string v = EditUpdateButton.DataBindValue(this);
            Text = v;
            if (AssociatedRowId.HasValue())
            {
                var tr = this.Parent.Parent as HtmlTableRow;
                if (tr != null && tr.ID == AssociatedRowId)
                    tr.Visible = Text.HasValue() || Editing;
            }
        }
        private bool ShowEditMode
        {
            get
            {
                if (EditUpdateButton.Editing && BindingModes.TwoWay == BindingMode)
                    if (!EditRole.HasValue() || HttpContext.Current.User.IsInRole(EditRole))
                        return true;
                return false;
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Title, BindingMember);
            if (DesignMode || ShowEditMode)
                base.Render(writer);
            else
            {
                const string STR_style = "style";
                if (Attributes[STR_style].HasValue())
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, Attributes[STR_style]);
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                writer.Write(Text);
// removed 3/23/2208 KL
//                    writer.Write(Text);
                writer.RenderEndTag();
            }
        }
        #region IDisplayOrEdit Members

        private EditUpdateButton EditUpdateButton;
        public void SetEditUpdateButton(EditUpdateButton bt)
        {
            EditUpdateButton = bt;
        }
        private string _BindingMember;
        public string BindingMember
        {
            get
            {
                if (!_BindingMember.HasValue())
                    return this.ID;
                return _BindingMember;
            }
            set { _BindingMember = value; }
        }
        public string BindingSource { get; set; }
        public BindingModes BindingMode { get; set; }
        public string GetTextValue()
        {
            return Text;
        }
        public bool GetChangedStatus()
        {
            return ChangedStatus;
        }

        public bool Editing
        {
            get { return EditUpdateButton.Editing; }
        }
        public string AssociatedRowId { get; set; }

        public string EditGroup { get; set; }
        public string EditRole { get; set; }
        #endregion

    }
}
