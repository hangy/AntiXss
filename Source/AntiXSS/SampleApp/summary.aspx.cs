using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Security.Application;
using Microsoft.Security.Application.SecurityRuntimeEngine;

//===============================================================================
// Microsoft Connected Info Security Group samples
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

namespace Feedback
{
    /// <summary>
    /// Feedback summary page provides the list of comments provided by the earlier users.
    /// Additionally allows the user to enter feedback.
    /// </summary>
    public partial class summary : System.Web.UI.Page
    {

        public DataSet dsComments = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Checking whether feedback already exists in the application variable
            if (Application["FeedbackDS"] != null)
                dsComments = (DataSet)Application["FeedbackDS"];
            else
                dsComments = this.BuildDataset();

            //Checking if a session name is passed in the QueryString.
            if (Request.QueryString["sname"] != null)
            {
                //Label.Text will place the data inside <span> tag 
                //thus we are using AntiXss.HtmlEncode to encode the data.
                lblSessionName.Text = AntiXss.HtmlEncode(Request.QueryString["sname"]);
            }
            lblUser.Text = Context.User.Identity.Name;
            BindRepeater();
        }

        /// <summary>
        /// Binds the dataset to the grid.
        /// </summary>
        private void BindRepeater()
        {
            repFeedback.DataSource = dsComments;
            repFeedback.DataBind();
        }

        /// <summary>
        /// Creates a new dataset and adds some default data.
        /// </summary>
        /// <returns>Returns a new dataset with some default data.</returns>
        private DataSet BuildDataset()
        {
            DataSet dsNew = new DataSet("Feedback");
            DataTable dt = new DataTable("Feedback");
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Email", typeof(string)));
            dt.Columns.Add(new DataColumn("Comments", typeof(string)));
            dsNew.Tables.Add(dt);

            DataRow dr = dsNew.Tables["Feedback"].NewRow();
            dr["Name"] = "Mark";
            dr["Email"] = "mark.curphey@microsoft.com";
            dr["Comments"] = "Excellent session, learnt how to fix a cross site scripting vulnerability.";
            dsNew.Tables["Feedback"].Rows.Add(dr);

            dr = dsNew.Tables["Feedback"].NewRow();
            dr["Name"] = "Anil";
            dr["Email"] = "anil.revuru@microsoft.com";
            dr["Comments"] = "Very good session. AntiXss Library is free.";
            dsNew.Tables["Feedback"].Rows.Add(dr);
            return dsNew;
        }

        
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //Creating a new row
            DataRow dr = dsComments.Tables["Feedback"].NewRow();
            //Storing the user input in appropriate columns
            dr["Name"] = lblUser.Text;
            dr["Email"] = txtEmail.Text;
            dr["Comments"] = txtComments.Text;
            //Adding the row to the table
            dsComments.Tables["Feedback"].Rows.Add(dr);
            Application["FeedbackDS"] = dsComments;
            //Binding the dataset
            BindRepeater();
            lblFeedback.Text = "Thank you for providing your Feedback!";
            txtEmail.Text = "";
            txtComments.Text = ""; 
        }

        protected void repFeedback_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            //Checking to see if an item is being bound to the grid
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRow dr = ((DataRowView)e.Item.DataItem).Row;
                //Filtering and encoding the user input and passing it to the appropriate label controls
                ((Label)e.Item.FindControl("CommentsLabel")).Text = AntiXss.HtmlEncode(dr["Comments"].ToString());
                ((Label)e.Item.FindControl("NameLabel")).Text = AntiXss.HtmlEncode(dr["Name"].ToString());
                ((Label)e.Item.FindControl("EmailLabel")).Text = AntiXss.HtmlEncode(dr["Email"].ToString());
                
            }
        }    



        
    }
}
