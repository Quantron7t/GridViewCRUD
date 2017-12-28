<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CarList.aspx.cs" Inherits="GridViewOnSteroids.CarList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href="css/bootstrap.css" />
    <script src="js/jquery-3.2.1.js"></script>
    <script src="js/bootstrap.bundle.js"></script>
    <script src="js/bootstrap.js"></script>
    <title>List of Cars</title>
    <script type="text/javascript">
        function openSelectedCar() {
            $('#selectCarModal').modal('show');
        }
        function editCar() {
            $('#editCarModal').modal('show');
        }
        function addCar() {
            $('#addCarModal').modal('show');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            
            <div class="page-header">
                <h1>CAR LIST</h1>
            </div>
            <asp:GridView CssClass="table table-striped" runat="server" HeaderStyle-BackColor="#0066cc" ID="CarGridView" OnSelectedIndexChanging="CarGridView_SelectedIndexChanging" DataKeyNames="id" OnRowDataBound="CarGridView_RowDataBound" OnRowDeleting="CarGridView_RowDeleting" OnRowCommand="CarGridView_RowCommand" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="id" />               
                    <asp:TemplateField HeaderText="Available">
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="CheckBox" Checked='<%# Eval("availability") %>' AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged"/>                           <as
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="name" HeaderText="Car"/>
                    <asp:TemplateField ShowHeader="False" HeaderStyle-Width="85" HeaderText="Actions">
                        <ItemTemplate> 
                            <asp:LinkButton runat="server" CommandName="Select" ToolTip="View More Info" CausesValidation="False" ID="LinkButton1">
                                <asp:Image runat="server" ImageUrl="~/images/select.png" Height="17" Width="17"/>
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="EditRow" CommandArgument="<%# Container.DataItemIndex %>" ToolTip="Edit Row" ID="LinkButton2">
                                <asp:Image runat="server" ImageUrl="~/images/edit.png" Height="17" Width="17"/>
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Delete" ToolTip="Delete Row" OnClientClick="return confirm('Do you want to delete the record?');" CausesValidation="False" ID="LinkButton3">
                                <asp:Image runat="server" ImageUrl="~/images/delete.png" Height="17" Width="17"/>
                            </asp:LinkButton>                           
                        </ItemTemplate>                        
                    </asp:TemplateField>                    
                </Columns>
            </asp:GridView>      
            
            <br />
            <asp:LinkButton runat="server" ID="AddWindowPopupButton" OnClick="AddWindowPopupButton_Click">
                <asp:Image runat="server" ImageUrl="~/images/add.png" Width="25" Height="25"/>
            </asp:LinkButton>

        </div>
        <!-- Select Modal START -->
        <div id="selectCarModal" class="modal fade" role="dialog">
          <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
              <div class="modal-header">
                <h2><small><asp:Label ID="carNameLabel" runat="server"></asp:Label></small></h2>
                <button type="button" class="close" data-dismiss="modal">&times;</button>               
              </div>
              <div class="modal-body">
                <dl>
                    <dt>Class</dt>
                    <dd>- <asp:Label runat="server" ID="carTypeLabel"></asp:Label></dd>
                    <dt>Cost</dt>
                    <dd>- <asp:Label ID="carPriceLabel" runat="server"></asp:Label></dd>
                    <dt>Country of Origin</dt>
                    <dd>- <asp:Label runat="server" ID="carCountryLabel"></asp:Label></dd>
                </dl>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
              </div>
            </div>
          </div>
        </div>
        <!-- Select Modal END -->

        <!-- Edit Modal START-->
        <div id="editCarModal" class="modal fade" role="dialog">
          <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
              <div class="modal-header">
                <h4 class="modal-title"><small>Edit Window</small></h4>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
              </div>
              <div class="modal-body">
                <p class="bg-info"><em>&nbsp;You are editing : <b><asp:Label runat="server" ID="updateCarNameLabel"></asp:Label></b></em></p>
                <div class="input-group">
                    <span class="input-group-addon" style="width:108px"><b>Availablility</b></span>  
                    <asp:Checkbox runat="server" ID="updateAvailableCheckBox" CssClass="form-control"></asp:Checkbox>
                </div>
                <div class="input-group">
                    <span class="input-group-addon" style="width:108px"><b>Type</b></span>  
                    <asp:TextBox runat="server" ID="updateTypeTextBox" CssClass="form-control" AutoComplete="false" placeholder="Class of Car"></asp:TextBox>
                </div>
                 <div class="input-group">
                    <span class="input-group-addon" style="width:108px"><b>Cost</b></span>                    
                    <asp:TextBox runat="server" ID="updatePriceTextBox"  CssClass="form-control" AutoComplete="false" placeholder="Cost of Car" min="1" MaxLength="10000000" TextMode="Number"></asp:TextBox> 
                  </div>
                  <div class="input-group">
                    <span class="input-group-addon" style="width:108px"><b>Country</b></span>                    
                    <asp:TextBox runat="server" ID="updateCountryTextBox"  CssClass="form-control" AutoComplete="false" placeholder="Country of Origin"></asp:TextBox>
                  </div>
              </div>
              <div class="modal-footer">
                <asp:Button runat="server" Text="Update" CssClass="btn btn-primary" OnClick="UpdateButton_Click" id="UpdateButton" OnClientClick="return confirm('Are you sure about updating?');"/>
              </div>
            </div>

          </div>
        </div>
        <!-- Edit Modal END-->

        <!-- Add Modal Start-->
        <div id="addCarModal" class="modal fade" role="dialog">
          <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
              <div class="modal-header">
                 <h4 class="modal-title"><small>Add a new car</small></h4>
                <button type="button" class="close" data-dismiss="modal">&times;</button>                
              </div>
              <div class="modal-body">

               
                    <div class="form-group">
                        <label><b>Car:</b></label>
                        <asp:TextBox runat="server" CssClass="form-control" ID="addCarNameTextBox"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label><b>Class:</b></label>
                        <asp:TextBox CssClass="form-control" runat="server" ID="addCarTypeTextBox"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label><b>Price:</b></label>
                        <asp:TextBox CssClass="form-control" runat="server" ID="addCarPriceTextBox"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label><b>Country of Origin</b></label>
                        <asp:TextBox CssClass="form-control" runat="server" ID="addCarCountryTextBox"></asp:TextBox>
                    </div>
                    </div>
                

              <div class="modal-footer">
                  <asp:Button runat="server" Text="Add Car" CssClass="btn btn-primary" OnClick="AddButton_Click" ID="AddButton" />
              </div>
            </div>

          </div>
        </div>
        <!-- Add Modal END-->

    </form>
</body>
</html>
