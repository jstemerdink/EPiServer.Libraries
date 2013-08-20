<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentProviderAdministration.aspx.cs"
    Inherits="EPiServer.Libraries.ContentProviders.Plugins.ContentProviderAdministration" %>

<%@ Register TagPrefix="ux" Namespace="EPiServer.UI.WebControls" Assembly="EPiServer.UI" %>
<%@ Register TagPrefix="ux" Namespace="EPiServer.Web.WebControls" Assembly="EPiServer" %>
<asp:content contentplaceholderid="MainRegion" runat="server">
   
   <div class="epi-formArea epi-paddingVertical-small" ID="formArea" runat="server">
        <asp:ScriptManager runat="server" EnablePartialRendering="True" ScriptMode="Release"/>
        <asp:UpdatePanel runat="server" ChildrenAsTriggers="True" UpdateMode="Always">
            <ContentTemplate>
                <asp:Panel class="EP-validationSummary" style="color: Black;" ID="errorContainer" runat="server" Visible="False">
                    <ul><li><asp:Literal ID="errorLabel" runat="server" Visible="false" /></li></ul>
                </asp:Panel>
                <asp:ListView runat="server" OnItemDataBound="RowDataBound" ID="listView" OnItemDeleting="DeletingItem" >
                    <LayoutTemplate>
                        <div class="epi-formArea">
                            <div class="epi-size25">
                                <table class="epi-default">
                                    <tr>
                                        <thead>
                                            <th></th>
                                            <th>Name</th>
                                            <th>Root</th>
                                            <th>Entrypoint</th>
                                        </thead>
                                    </tr>
                                        <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
                                </table> 
                            </div>
                        </div>
                        <div class="epi-buttonContainer epi-paddingVertical-small">
                            <div style="float:left;display:inline-block;margin-right:15px;"><asp:Label runat="server" AssociatedControlID="rootPage" Text="Root" style="float:left;display:inline-block;margin-right:15px;" /><ux:InputPageReference runat="server" id="rootPage" style="float:left;display:inline-block;" /></div>
                            <div style="float:left;"><asp:Label runat="server" AssociatedControlID="entryPoint" Text="Entrypoint" style="float:left;display:inline-block;margin-right:15px;" /><ux:InputPageReference runat="server" id="entryPoint"  style="float:left;display:inline-block;" /></div>
                            <ux:ToolButton  DisablePageLeaveCheck="true" OnClick="AddProvider" runat="server" SkinID="Save" text="Add" ToolTip="Add provider" />
                        </div> 
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><span class="epi-cmsButton" runat="server" ID="cmsButtonDelete"><asp:Button ID="buttonDelete" runat="server" CommandName="Delete" CssClass="epi-cmsButton-tools epi-cmsButton-Delete" Text="" ToolTip="Remove providers" /></span></td>
                            <td><asp:Label runat="server" ID="labelName" CssClass="episize30" /></td>
                            <td><asp:Label runat="server" ID="labelRoot" /></td>
                            <td><asp:Label runat="server" ID="labelEntryPoint" /></td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
                
            </ContentTemplate>
                
        </asp:UpdatePanel>
            
    </div>	

</asp:content>
