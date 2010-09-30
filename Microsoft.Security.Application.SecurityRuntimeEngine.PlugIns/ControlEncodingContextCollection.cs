// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlEncodingContextCollection.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
//
// </copyright>
// <summary>
//   A collection of ControlEncodingContexts sourced from a configuration file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;

    /// <summary>
    /// A collection of <see cref="ControlEncodingContext"/>s sourced from a configuration file.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", 
        "CA1812:AvoidUninstantiatedInternalClasses", 
        Justification = "It is instantiated, by System.Configuration during casting.")]
    [ConfigurationCollection(typeof(ControlEncodingContext))]
    internal sealed class ControlEncodingContextCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlEncodingContextCollection"/> class.
        /// </summary>
        public ControlEncodingContextCollection()
        {
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.HtmlControls.HtmlAnchor", "HRef", EncodingContext.HtmlAttribute));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.HtmlControls.HtmlHead", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.HtmlControls.HtmlImage", "Src", EncodingContext.HtmlAttribute));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.HtmlControls.HtmlInputImage", "Src", EncodingContext.HtmlAttribute));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.HtmlControls.HtmlInputRadioButton", "Value", EncodingContext.HtmlAttribute));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.BaseDataList", "Caption", EncodingContext.HtmlAttribute));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Calendar", "Caption", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Calendar", "NextMonthText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Calendar", "PrevMonthText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Calendar", "SelectMonthText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Calendar", "SelectWeekText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "CancelDestinationPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "ChangePasswordFailureText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "ChangePasswordTitleText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "ConfirmNewPasswordLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "ContinueDestinationPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "CreateUserText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "EditProfileText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "HelpPageText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "InstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "NewPasswordLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "PasswordHintText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "PasswordLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "PasswordRecoveryText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "SuccessPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "SuccessText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "SuccessTitleText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ChangePassword", "UserNameLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CheckBox", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CompareValidator", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "AnswerLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "CompleteSuccessText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "ConfirmPasswordLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "ContinueDestinationPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "DuplicateEmailErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "DuplicateUserNameErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "EditProfileText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "EmailLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "UnknownErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "HelpPageText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "InstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "InvalidAnswerErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "InvalidEmailErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "InvalidPasswordErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "InvalidQuestionErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "PasswordHintText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "PasswordLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "QuestionLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "UserNameLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "CancelDestinationPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "FinishDestinationPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CreateUserWizard", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.CustomValidator", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.DataControlFieldCell", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.DataControlFieldHeaderCell", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.DataGrid", "Caption", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.DataList", "Caption", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.DetailsView", "Caption", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.DetailsView", "EmptyDataText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.DetailsView", "FooterText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.DetailsView", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.FormView", "Caption", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.FormView", "EmptyDataText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.FormView", "FooterText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.FormView", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.GridView", "Caption", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.GridView", "EmptyDataText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.HyperLink", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Label", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.LinkButton", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ListBox", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.ListControl", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Literal", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "CreateUserText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "DestinationPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "HelpPageText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "InstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "FailureText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "LoginButtonText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "PasswordLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "PasswordRecoveryText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "RememberMeText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "TitleText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Login", "UserNameLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.LoginStatus", "LoginText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.LoginStatus", "LogoutPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.LoginStatus", "LogoutText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Panel", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "AnswerLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "GeneralFailureText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "HelpPageText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "QuestionFailureText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "QuestionInstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "QuestionLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "QuestionTitleText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "SubmitButtonText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "SuccessPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "SuccessText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "UserNameFailureText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "UserNameInstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "UserNameLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.PasswordRecovery", "UserNameTitleText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.RadioButton", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.RadioButtonList", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.RangeValidator", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.RegularExpressionValidator", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.RequiredFieldValidator", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Table", "Caption", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.TableCell", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.TableHeaderCell", "Text", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.AppearanceEditorPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.BehaviorEditorPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.BehaviorEditorPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogZone", "EmptyZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogZone", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogZone", "InstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogZone", "SelectTargetZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogZoneBase", "EmptyZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogZoneBase", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogZoneBase", "InstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.CatalogZoneBase", "SelectTargetZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ConfigureConnectionTitle", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ConnectToConsumerInstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ConnectToConsumerText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ConnectToConsumerTitle", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ConnectToProviderInstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ConnectToProviderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ConnectToProviderTitle", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ConsumersTitle", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ConsumersInstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "EmptyZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ExistingConnectionErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "GetText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "GetFromText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "InstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "InstructionTitle", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "NewConnectionErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "NoExistingConnectionInstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "NoExistingConnectionTitle", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ProvidersTitle", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "ProvidersInstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "SendText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ConnectionsZone", "SendToText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.DeclarativeCatalogPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.DeclarativeCatalogPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorZone", "EmptyZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorZone", "ErrorText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorZone", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorZone", "InstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorZoneBase", "EmptyZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorZoneBase", "ErrorText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorZoneBase", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.EditorZoneBase", "InstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ErrorWebPart", "AuthorizationFilter", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ErrorWebPart", "ImportErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ErrorWebPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ErrorWebPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.GenericWebPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.GenericWebPart", "AuthorizationFilter", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.GenericWebPart", "ImportErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.GenericWebPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ImportCatalogPart", "BrowseHelpText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ImportCatalogPart", "ImportedPartLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ImportCatalogPart", "PartImportErrorLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ImportCatalogPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ImportCatalogPart", "UploadHelpText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ImportCatalogPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.LayoutEditorPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.LayoutEditorPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.PageCatalogPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.PageCatalogPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.Part", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.Part", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.PropertyGridEditorPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.PropertyGridEditorPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ProxyWebPart", "AuthorizationFilter", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ProxyWebPart", "ImportErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ProxyWebPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ProxyWebPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ToolZone", "InstructionText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ToolZone", "EmptyZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.ToolZone", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.UnauthorizedWebPart", "AuthorizationFilter", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.UnauthorizedWebPart", "ImportErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.UnauthorizedWebPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.UnauthorizedWebPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPart", "AuthorizationFilter", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPart", "ImportErrorMessage", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPart", "Title", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPart", "GroupingText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPartZone", "EmptyZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPartZone", "MenuLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPartZone", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPartZoneBase", "EmptyZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPartZoneBase", "MenuLabelText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebPartZoneBase", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebZone", "EmptyZoneText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.WebParts.WebZone", "HeaderText", EncodingContext.Html));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Wizard", "CancelDestinationPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Wizard", "FinishDestinationPageUrl", EncodingContext.Url));
            this.BaseAdd(new ControlEncodingContext("System.Web.UI.WebControls.Wizard", "HeaderText", EncodingContext.Html));
        }

        /// <summary>
        /// Returns a <see cref="ControlEncodingContext" /> at the specified index.
        /// </summary>
        /// <param name="index">A zero based position in the collection.</param>
        /// <returns>The ControlEncodingContext at the specified index.</returns>
        public ControlEncodingContext this[int index]
        {
            get
            {
                return BaseGet(index) as ControlEncodingContext;
            }
        }

        /// <summary>
        /// Gets the <see cref="ControlEncodingContext"/> with the specified key.
        /// </summary>
        /// <param name="typeParameterIdentifier">The type and parameter combination to search for.</param>
        /// <returns>The <see cref="ControlEncodingContext"/> with the specified type.</returns>
        public new ControlEncodingContext this[string typeParameterIdentifier]
        {
            get
            {
                return (ControlEncodingContext)BaseGet(typeParameterIdentifier);
            }
        }

        /// <summary>
        /// Gets the zero-based index for the specified <see cref="ControlEncodingContext"/>.
        /// </summary>
        /// <param name="controlEncodingContext">The control encoding context.</param>
        /// <returns>The zero-based index for the specified <see cref="ControlEncodingContext"/>.</returns>
        public int IndexOf(ControlEncodingContext controlEncodingContext)
        {
            return BaseIndexOf(controlEncodingContext);
        }

        /// <summary>
        /// Adds the specified control encoding context to the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="controlEncodingContext">The control encoding context.</param>
        public void Add(ControlEncodingContext controlEncodingContext)
        {
            this.BaseAdd(controlEncodingContext);
        }

        /// <summary>
        /// Removes the specified <see cref="ControlEncodingContext"/>.
        /// </summary>
        /// <param name="controlEncodingContext">The control encoding context.</param>
        public void Remove(ControlEncodingContext controlEncodingContext)
        {
            if (BaseIndexOf(controlEncodingContext) >= 0)
            {
                BaseRemove(controlEncodingContext.Id);
            }
        }

        /// <summary>
        /// Removes the <see cref="ControlEncodingContext"/> at the specified index location.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Removes a <see cref="ControlEncodingContext"/> from the collection.
        /// </summary>
        /// <param name="name">The key of the <see cref="ControlEncodingContext"/> to remove.</param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        /// <summary>
        /// Removes all configuration elements from the collection.
        /// </summary>
        public void Clear()
        {
            this.BaseClear();
        }

        /// <summary>
        /// Returns true if the specified type of control exists in 
        /// the configuration. 
        /// </summary>
        /// <param name="typeName">Full type name of the control.</param>
        /// <returns>Returns true if the configuration exists.</returns>
        internal bool Contains(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName");
            }

            int listCount = this.Count;
            for (int index = 0; index < listCount; index++)
            {
                ControlEncodingContext controlEncodingContext = this[index];
                if (controlEncodingContext.FullClassName == typeName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns all the ControlEncodingContext objects for a specified control type.
        /// </summary>
        /// <param name="fullName">Full type name of the control.</param>
        /// <returns>Returns a list of ControlEncodingContext</returns>
        internal Collection<ControlEncodingContext> GetEncodingTypes(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentNullException("fullName");
            }

            Collection<ControlEncodingContext> propertiesAndEncodingForType = new Collection<ControlEncodingContext>();
            int listCount = this.Count;
            for (int index = 0; index < listCount; index++)
            {
                ControlEncodingContext encodingContext = this[index];
                if (encodingContext.FullClassName == fullName)
                {
                    propertiesAndEncodingForType.Add(encodingContext);
                }
            }

            return propertiesAndEncodingForType;
        }

        /// <summary>
        /// Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to add.</param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            this.BaseAdd(element, false);
        }

        /// <summary>
        /// Creates a new <see cref="ControlEncodingContext"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="ControlEncodingContext"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ControlEncodingContext();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ControlEncodingContext)element).Id.ToUpperInvariant();
        }
    }
}
