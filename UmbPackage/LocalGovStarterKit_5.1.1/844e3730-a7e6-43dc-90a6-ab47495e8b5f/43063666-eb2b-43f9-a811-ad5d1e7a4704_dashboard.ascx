<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="dashboard.ascx.cs" Inherits="Jumoo.LocalGovStarterKit.dashboard" %>
<script type="text/javascript">
    $(document.forms[0]).submit(function () {
        document.getElementById("navprogress").innerHTML
            = "Importing Stuff... <small>can take a little while</small><div class='umb-loader'></div>";
        $('.btnNav').hide();
    });
</script>
<div style="max-width: 1200px">
    <div class="row">
        <div class="span12">
            <h2 class="page-header">Localgov Starter Kit<sup>5</sup></h2>
            <p>
                Yeah! you have successfully installed the LocalGov Starter Kit, and it's packed with all sorts of cool stuff.
                Why not go and have a look around, you can see how it all fits together?
            </p>
            <h3 class="page-header"><span class="color-green"><i class="icon-page-add"></i></span> Just Add Content
            </h3>
            <p>
                The Starter Kit doesn't install content by default, that's because we want to give you a blank slate if you
                want to build your site from here, but we have loads of options if you do want content ...
            </p>
       </div>
    </div>
        <div class="row">

        <div class="span6">
            <h4 class="page-header"><i class="icon-paper-plane"></i> Example Content</h4>
            <p>
                If you think you need some content to see how it works, you can install the sample content, which has
                bits of everything in it. 
            </p>
            <p>
                The help content tells you how it all fits together, if this is the first time you are 
                installing the starter kit, We would recommend you install the help.
            </p>
            <div class="btn-group">
                <asp:Button ID="btnAddExampleContent" runat="server" Text="Add Example & Help Content" CssClass="btn btn-large btn-danger" OnClick="btnAddExampleContent_Click" />
            </div>
        </div>

        <div class="span6">
            <h4 class="page-header"><i class="icon-library"></i> LocalGov Content</h4>
            <p>
                Why not give you're content editors a leg up, install an existing localgov strcuture - then all you need
                do is fill in the blanks
            </p>
            <p>
                <div class="btn-group">
                    <asp:Button ID="btnImportLGNL" runat="server" Text="Import LocalGov Navigation List (LGNL)" CssClass="btn btn-large btn-default" OnClick="btnImportLGNL_Click"/>
                </div>
            </p>
            <p>
                <div class="btn-group">
                    <asp:Button ID="btnImportSNL" runat="server" Text="Import Scottish Navigation List (SNL)" CssClass="btn btn-large btn-default" OnClick="btnImportSNL_Click"/>
                </div>
            </p>
            <small class="muted">LGNL and SNL are licened under an open government Licence and are a product of the esd toolkit progreamme </small>
        </div>
    </div>
    <div class="row">
        <div class="span12">
            <div id="navprogress">
                <asp:Label ID="importMessage" runat="server" />
            </div>
        </div>
    </div> 
</div>

<asp:Button ID="btnCreateExport" runat="server" Text="create export" CssClass="btn btn-lg btn-danger" OnClick="btnCreateExport_Click" />
<!-- <div class="umb-loader-wrapper"><div class="umb-loader"></div></div> -->
