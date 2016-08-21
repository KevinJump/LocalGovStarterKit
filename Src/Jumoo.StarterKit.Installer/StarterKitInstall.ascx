<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StarterKitInstall.ascx.cs" Inherits="Jumoo.StarterKit.Installer.StarterKitInstall" %>
<script type="text/javascript">
    $(document.forms[0]).submit(function () {
        document.getElementById("updater").style.display = "block";
        $(".content-import-options").hide();
    });
</script>
<div class="lgsk-dashboard">
    <div class="row">
        <div class="span12">
            <h2>Lgsk Starterkit<sup>6</sup></h2>
            <p>
                Yeah! you have successfully installed the localgov starterkit, now your
                Umbraco site is packages with all sorts of helpful things, you should
                go have a look around. 
            </p>
        </div>
    </div>
    <div class="row">
        <div class="span12">
            <h3>
                <span class="colour-green"><i class="icon-page-add"></i></span>
                Just add content
            </h3>
            <p>
                The starterkit installs everything you need to go off and build a site, but
                it doesn't add any content by default. If you want to see how the content 
                fits together then install the sample content.
            </p>
        </div>
    </div>
    <div id="contentWarning" runat="server" visible="false">
        <div class="row">
            <div class="span12">
                <div class="alert alert-warning">
                    <h3>Beware of content moshing!</h3>
                    <p>
                        If you install the example content or structure content over an existing
                        set of content you might loose things, you should only really import 
                        the content into a blank site. 
                    </p>
                </div>
            </div>
        </div>
    </div>
    <div class="content-import-options" runat="server" id="contentOptions">
        <div class="row">
            <div class="span6">
                <h4><i class="icon-paper-plane"></i> Example Content</h4>
                <p>
                    The example content gives you a feel for all the features in the starter kit.
                </p>
                <p>
                    Once you have installed the example content, you will get pages for each 
                    document type in the kit, and you can see how it all fits together.
                </p>
                <p>
                    &nbsp;
                </p>
                <p class="text-center">
                    <asp:Button CssClass="btn btn-large btn-primary btn-importer" runat="server" id="btnExampleImport" Text="Add Example Content" OnClick="btnExampleImport_Click" />
                </p>
            </div>
            <div class="span6">
                <h4><i class="icon-library"></i> Structures </h4>
                <p>
                    if you need to kickstart your site, you can install one of the 
                    pre-defined structures.
                </p>
                <p>
                    these structures give you all the categories usally included in a
                    local government site, you just need to fill in the blanks.
                </p>
                <p>
                    <asp:Button CssClass="btn btn-warning btn-importer" runat="server" id="btnLgnlImport" Text="Local Government Navigation List (LGNL)" OnClick="btnLgnlImport_Click" />
                </p>
                <p>
                    <asp:Button CssClass="btn btn-warning btn-importer" runat="server" id="btnSnlImport" Text="Scotish Navigation List (SNL)" OnClick="btnSnlImport_Click" />
                </p>
                <small class="muted">LGNL and SNL are licened under an open government Licence and are a product of the esd toolkit progreamme </small>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="span12">
            <hr />
            <div id="divResults" runat="server" visible="false">
                <h4><asp:Label ID="lbResults" runat="server"></asp:Label></h4>
                <a href="/umbraco#/content" target="_top">view content</a>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="span12">
            <div id="updater" style="display:none;text-align:center;">
                <h4>Importing .... <small>(it can take a while)</small></h4>
                <ul class="animated -half-second" style="list-style: none;position:relative;left:45%;">
                    <li class="umb-load-indicator__bubble"></li>
                    <li class="umb-load-indicator__bubble"></li>
                    <li class="umb-load-indicator__bubble"></li>
                </ul>
            </div>
        </div>
        <asp:Label ID="lbDone" Visible="false" runat="server"></asp:Label>
    </div>
</div>
<div id="googletag" runat="server" visible="false">
<!-- Google Tag Manager -->
<noscript><iframe src="//www.googletagmanager.com/ns.html?id=GTM-THGKJW"
height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>
<script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
'//www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
})(window, document, 'script', 'dataLayer', 'GTM-THGKJW');</script>
<!-- End Google Tag Manager -->
</div>
