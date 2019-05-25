<%@ Page Language="VB" AutoEventWireup="false" CodeFile="demo.aspx.vb" Inherits="demo" %>
<!DOCTYPE HTML>
<html lang="en-US">
<head>
	<meta charset="UTF-8">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link href="bootstrap/css/bootstrap.min.css" rel="stylesheet" media="screen">
	<link rel="stylesheet" href="jqueryui/themes/base/jquery-ui.css" />
<link href="src/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    
	<title>
		Demo 2
	</title>

	<style>

		
		.hoverDroppable {
			background-color: lightgreen;
		}

		.draggableField {
			/* float: left; */
			padding-left:5px;
            position:relative;
            /*border:2px solid black;*/
		}

		.draggableField > input,select, button, .checkboxgroup, .selectmultiple, .radiogroup {
			margin-top: 10px;

			margin-right: 10px;
			margin-bottom: 10px;
		}

		.draggableField:hover{
			background-color: #ccffcc;
		}

.overlay{
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 10%;
  z-index: 10;
  
}

.overlay2 {
  position: absolute;
  top: -12em;
  left: 0;
  z-index: 5;
  
}


	    .ConfigurationButton {
	        width: 30px;
	        margin: 5px;
	    }
	</style>

	<style id="content-styles">
		/* Styles that are also copied for Preview */
		body {
			margin: 10px 0 0 10px;
		}

		.control-label {
			display: inline-block !important;
			padding-top: 5px;
			text-align: right;
			vertical-align: baseline;
			padding-right: 10px;
		}

		.droppedField {
			padding-left:5px;
		}

		.droppedField > input,select, button, .checkboxgroup, .selectmultiple, .radiogroup {
			margin-top: 10px;

			margin-right: 10px;
			margin-bottom: 10px;
		}

		.action-bar .droppedField {
			float: left;
			padding-left:5px;
		}

	</style>
<script>

    function makeDraggable() {
        $(".selectorField").draggable({ helper: "clone", stack: "div", cursor: "move", cancel: null });
    }

    var _ctrl_index = 1001;
    function docReady() {
        console.log("document ready");
        compileTemplates();

        makeDraggable();
        
        $(".droppedFields").droppable({
            activeClass: "activeDroppable",
            hoverClass: "hoverDroppable",
            accept: ":not(.ui-sortable-helper)",
            drop: function (event, ui) {
                //console.log(event, ui);
                var draggable = ui.draggable;
                draggable = draggable.clone();
                draggable.removeClass("selectorField");
                draggable.addClass("droppedField");
                draggable[0].id = "CTRL-DIV-" + (_ctrl_index++); // Attach an ID to the rendered control
                draggable.appendTo(this);


                /* Once dropped, attach the customization handler to the control */
                //draggable.click(CustomizeControl);

                makeDraggable();
            }
        });

        /* Make the droppedFields sortable and connected with other droppedFields containers*/
        $(".droppedFields").sortable({
            //cancel: null, // Cancel the default events on the controls
            connectWith: ".droppedFields"
        });
        //.disableSelection();

        $(".selectorField").click(function () {
            
            //var draggable = $(this).html().clone();
            var controlTypeContent = $(this).clone();
            console.log('id - ' + $(this).attr('id'));
            controlTypeContent.removeClass("selectorField");
            controlTypeContent.addClass("droppedField");
            controlTypeContent[0].id = "CTRL-DIV-" + (_ctrl_index++); // Attach an ID to the rendered control
            
            $(controlTypeContent).hover(ShowConfigurationOptions);
            $(controlTypeContent).blur(ShowConfigurationOptions);
            $(controlTypeContent).find("[id$=Button]").click(ShowConfigurationPanel);
            $(controlTypeContent).appendTo($("#selected-action-column"));
            makeDraggable();
        });
    }


    function ShowConfigurationOptions() {
        $(this).find('#ConfigurationButtons').toggle();
        $(this).find('#RequiredSelection').toggle();
        console.log($(this).find("#ConfigurationButtons").is(":visible"));
       
        if (!$(this).find("#ConfigurationButtons").is(":visible")) {
            $(this).find("#Required").is(":checked") ? $(this).find("#RequiredText").show() : $(this).find("#RequiredText").hide();
            $(this).find("#RequiredCheckbox").hide();
        }
        else {
            $(this).find("#RequiredText").hide();
            $(this).find("#RequiredCheckbox").show();
        }

    }

    function ShowConfigurationPanel() {
        console.log(this.id);
        console.log($(this).parent().attr('id'));
        console.log($(this).parent().parent().attr('id'));
        //$(this).parent().siblings().find("#ConfigurationPanel").show();
        var configurationPanel = $(this).parent().siblings().find("#ConfigurationPanel");
        
        $(configurationPanel).tabs();
        var index = $(configurationPanel).find('a[href="#' + this.id.replace("Button", "Tab") + '"]').parent().index();
        $(configurationPanel).tabs('option','active', index);
        $(configurationPanel).show();
    }

    function CloseConfigurationPanel(item) {
        $(item).parent().parent().hide();
    }


    /* Compile the templates for use */
    function compileTemplates() {
        window.templates = {};
        window.templates.common = Handlebars.compile($("#control-customize-template").html());

        /* HTML Templates required for specific implementations mentioned below */

        // Mostly we donot need so many templates

        window.templates.textbox = Handlebars.compile($("#textbox-template").html());
        window.templates.firstnametextbox = Handlebars.compile($("#firstname-textbox-template").html());
        window.templates.lastnametextbox = Handlebars.compile($("#firstname-textbox-template").html());
        window.templates.passwordbox = Handlebars.compile($("#textbox-template").html());
        window.templates.combobox = Handlebars.compile($("#combobox-template").html());
        window.templates.selectmultiplelist = Handlebars.compile($("#combobox-template").html());
        window.templates.radiogroup = Handlebars.compile($("#combobox-template").html());
        window.templates.checkboxgroup = Handlebars.compile($("#combobox-template").html());

    }

    
    

    
</script>

    

</head>
<body>

	<!--[if lt IE 9]>
	<b class="text-error">All components may not work correctly on IE 8 or below </b><br/><br/>
	<![endif]-->
  <legend>Simple form builder demo (Part 2)</legend>

    <form id="Form1" runat="server">

               <asp:ScriptManager ID="smMain" runat="server" EnablePageMethods="true"></asp:ScriptManager>
     
            
     

  <div class="tabbable"> 
	<!-- List of controls rendered into Bootstrap Tabs -->
	<ul class="nav nav-tabs">
		<li class="active">
			<a href="#simple" data-toggle="tab">Simple input</a>
		</li>
		<li>
			<a href="#multiple" data-toggle="tab">Radio/Checkbox/List</a>
		</li>
		<li>
			<a href="#btns" data-toggle="tab" >Buttons</a>
		</li>		
		<li>
			<a href="#disqus_thread">Comments</a>
		</li>
	</ul>
	<div class="row-fluid">
	<div id="listOfFields" class="span3 well tab-content">
	  <div class="tab-pane active" id="simple">
		<div class='selectorField draggableField' id="ctrl-textbox">
			<label class="control-label">Text Input</label>
            <br />
			<input type="text"  placeholder="Text here..." class="ctrl-textbox"></input>
		</div>
               <asp:Label ID="lblControlTypes" runat="server"></asp:Label>
        
        <div class='selectorField draggableField'>
			<label class="control-label"><strong>First Name</strong> <span class="required">(required)</span></label>
              <br />
			<input type="text"  class="SlText ctrl-firstname-textbox"></input>
              
              <div style="visibility:hidden">
              <label class="control-name">First Name</label>
                  </div>
		</div>

          <div class='selectorField draggableField'>
          
<strong>Address</strong> <span class="required">(required)</span><br />
<input type="text" class="SlText" />

                            <div style="visibility:hidden">
              <label class="control-name">Address</label>
                  </div>

              </div>

<div class='selectorField draggableField'>
    <label class="control-label InputHeader">Middle Name</label>
    <br />
    <input type="text" maxlength="50" style="max-width:300px;" placeholder="Text here..."  />
    
    <div style="visibility:hidden">
        <label class="control-name">Middle Name</label>
    </div>
 </div>




          <div class='selectorField draggableField'>
			<label class="control-label">Last Name</label>
              <br />
			<input type="text" placeholder="Text here..." class="ctrl-fastname-textbox"></input>
              
              <div style="visibility:hidden">
              <label class="control-name">Last Name</label>
                  </div>
		</div>
		<div class='selectorField draggableField'>
			<label class="control-label">Password</label>
            <br />
			<input type="password" placeholder="Password..." class="ctrl-passwordbox"></input>
		</div>
		<div class='selectorField draggableField'>
			<label class="control-label">Combobox</label>
            <br />
			<select class="ctrl-combobox">
				<option value="option1">Option 1</option>
				<option value="option2">Option 2</option>
				<option value="option3">Option 3</option>
			</select>
		</div>
	  </div>

	  <div class="tab-pane" id="multiple">
		<div class='selectorField draggableField radiogroup'>
			<label class="control-label" style="vertical-align:top">Radio buttons</label>
            <br />
			<div style="display:inline-block;" class="ctrl-radiogroup">
				<label class="radio"><input type="radio" name="radioField" value="option1">Option 1</input></label>
				<label class="radio"><input type="radio" name="radioField" value="option2">Option 2</input></label>
				<label class="radio"><input type="radio" name="radioField" value="option3">Option 3</input></label>
			</div>
		</div>
		<div class='selectorField draggableField checkboxgroup' >
			<label class="control-label" style="vertical-align:top">Checkboxes</label>
            <br />
			<div style="display:inline-block;" class="ctrl-checkboxgroup">
				<label class="checkbox"><input type="checkbox" name="checkboxField" value="option1">Option 1</input></label>
				<label class="checkbox"><input type="checkbox" name="checkboxField" value="option2">Option 2</input></label>
				<label class="checkbox"><input type="checkbox" name="checkboxField" value="option3">Option 3</input></label>
			</div>
		</div>
		<div class='selectorField draggableField selectmultiple'>
			<label class="control-label" style="vertical-align:top">Select multiple</label>
            <br />
			<div style="display:inline-block;">
				<select multiple="multiple" style="width:150px" class="ctrl-selectmultiplelist">
					<option value="option1">Option 1</option>
					<option value="option2">Option 2</option>
					<option value="option3">Option 3</option>
				</select>
			</div>
		</div>
	  </div>
	  <div class="tab-pane" id="btns">
		<div class='selectorField draggableField'>
			<button class="btn ctrl-btn">Simple Button</button>
		</div>
		<div class='selectorField draggableField'>
			<button class="btn btn-primary ctrl-btn">Primary Button</button>
		</div>
		<div class='selectorField draggableField'>
			<button class="btn btn-success ctrl-btn"><i class="icon-ok-sign icon-white"></i> Save Button</button>
		</div>
		<div class='selectorField draggableField'>
			<button class="btn btn-danger ctrl-btn"><i class="icon-trash icon-white"></i> Delete Button</button>
		</div>
	  </div>
    </div>
	<!-- End of list of controls -->

	<!-- 
		Below we have the columns to drop controls
			-- Removed the TABLE based implementations from earlier code
			-- Grid system used for rendering columns 
			-- Columns can be simply added by defining a div with droppedFields class
	-->
             



	<div class="span8" id="selected-content">
		<!--[if lt IE 9]>
		<div class="row-fluid" id="FormTitle-div">
			<label>Type form title here...</label>
		</div>
		<![endif]-->
        <!-- #include virtual="FrontendTemplate1.htm" -->
        	  <div class="row-fluid" id="FormTitle-div">
                  

                  <textarea name="FormTitle" rows="3"  cols="20" id="FormTitle" class="input-large span10" placeholder="Type form title here" style="font-size:1.75em;line-height:1.25em;font-family:inherit;display:none"></textarea>

                  
<%--<asp:TextBox ID="FormTitle" runat="server" style="font-size:1.75em;line-height:1.25em;font-family:inherit"  CssClass="input-large span10" placeholder="Type form title here" TextMode="MultiLine" Rows="3"></asp:TextBox>--%>
		<%--<input type="text" class="input-large span10" placeholder="Type form title here" id="FormTitle" />--%>
	  </div>
        <div class="row-fluid">
		<div id="selected-action-column" class="span10 well droppedFields" style="min-height:80px;">

		</div>
          
	  </div>
       <!-- #include virtual="FrontendTemplate2.htm" -->

	  <!-- Action bar - Suited for buttons on form -->
	  
	</div>
	</div>

	<!-- Preview button -->
	<div class="row-fluid">	
		<div class="span12">
			<input type="button" class="btn btn-primary" value="Preview" onclick="preview();"></input>
		</div>
	</div>
  </div>

<script type="text/javascript" src="bootstrap/js/jquery.js"></script>	
    
<script type="text/javascript" src="bootstrap/js/bootstrap.min.js"></script>


<script type="text/javascript" src="jqueryui/ui/minified/jquery-ui.min.js"></script>
     <script src="src/jquery.ui.position.js" type="text/javascript"></script>
    <script src="src/jquery.contextMenu.js" type="text/javascript"></script>
    
    


<!-- using handlebars for templating, but DustJS might be better for the current purpose -->
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/handlebars.js/1.0.0-rc.3/handlebars.min.js"></script>

<!-- 
	Starting templates declaration
	DEV-NOTE: Keeping the templates and code simple here for demo  -- use some better template inheritance for multiple controls 
---> 

    
<script id="control-customize-template" type="text/x-handlebars-template">
	<div class="modal-header">
		<h3>{{header}}</h3>
	</div>
	<div class="modal-body">
		<form id="theForm" class="form-horizontal">
			<input type="hidden" value="{{type}}" name="type"></input>
			<input type="hidden" value="{{forCtrl}}" name="forCtrl"></input>
			<p><label class="control-label">Label</label> <input type="text" name="label" value=""></input></p>
			<p><label class="control-label">Name</label> <input type="text" value="" name="name"></input></p>
			{{{content}}}
		</form>
	</div>
	<div class="modal-footer">
		<button class="btn btn-primary" data-dismiss="modal" onclick='save_customize_changes()'>Save changes</button>
		<button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
		<button class="btn btn-danger" data-dismiss="modal" aria-hidden="true" onclick='delete_ctrl()'>Delete</button>
	</div>
</script>

<script id="textbox-template" type="text/x-handlebars-template">
	<p><label class="control-label">Placeholder</label> <input type="text" name="placeholder" value=""></input></p>
</script>

<script id="firstname-textbox-template" type="text/x-handlebars-template">
	<p><label class="control-label">Placeholder</label> <input type="text" name="placeholder" value=""></input></p>
</script>

    <script id="lastname-textbox-template" type="text/x-handlebars-template">
	<p><label class="control-label">Placeholder</label> <input type="text" name="placeholder" value=""></input></p>
</script>


<script id="combobox-template" type="text/x-handlebars-template">
	<p><label class="control-label">Options</label> <textarea name="options" rows="5"></textarea></p>
</script>

<!-- End of templates -->


<script>
    $(document).ready(docReady);


    /* GA tracking */
    var _gaq = _gaq || [];
    _gaq.push(['_setAccount', 'UA-665946-1']);
    _gaq.push(['_setDomainName', 'anupshinde.com']);
    _gaq.push(['_trackPageview']);

    (function () {
        var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
    })();

</script>

<hr/>

	
<hr/>
<h6>
Usage:
  <ul>
	  <li>Drag drop fields from left</li>
	  <li>Click on the fields to customize</li>
	  <li>Click Preview to see the rendered form and code</li>
  </ul>
</h6>
<h6><a href="http://www.anupshinde.com/form-builder-part-2" target="_blank">Learn more about this code</a></h6>


<hr/>

    <div id="disqus_thread"></div>
    <script type="text/javascript">
        /* * * CONFIGURATION VARIABLES: EDIT BEFORE PASTING INTO YOUR WEBPAGE * * */
        var disqus_shortname = 'demosanupshinde'; // required: replace example with your forum shortname

        /* * * DON'T EDIT BELOW THIS LINE * * */
        (function () {
            var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
            dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
            (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
        })();
    </script>
    <noscript>Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript">comments powered by Disqus.</a></noscript>
    <a href="http://disqus.com" class="dsq-brlink">comments powered by <span class="logo-disqus">Disqus</span></a>


    <script id="entry-template" type="text/x-handlebars-template"><div class="entry">
  <h1>{{title}}</h1>
  <div class="body">
    {{body}}
  </div>
        {{other}}
</div>
</script>

    <div id="HandlebarsTest">

    </div>
        </form>


   <script type="text/javascript">
       var source = $("#entry-template").html();
       var template = Handlebars.compile(source);

       var context = { title: "My New Post", body: "This is my first post!", other: "Some other stuff" }
       var html = template(context);

       $(html).appendTo("#HandlebarsTest");
   </script> 


    
     
    
         <menu id="html5menu" type="context"><%-- style="display:none">--%>
  <command label="rotate" onclick="alert('rotate')">
  <%--<command label="resize" onclick="alert('resize')">
  <menu label="share">
    <command label="twitter" onclick="alert('twitter')">
    <hr>
    <command label="facebook" onclick="alert('facebook')">--%>
  </menu>

    <div id="testmenu"></div>

    <script type="text/javascript">
        var WebRADControls;

        PageMethods.TestMethod(onFinished);

        function onFinished(result) {
            WebRADControls = result;

            for (i = 0; i < WebRADControls.length; i++) {
                commandText = '<command label="' + WebRADControls[i].key + '">'; //onclick="alert(''rotate'')">';
                //alert(commandText);
                $(commandText).appendTo($("#html5menu"));

            }

            $("asdfsdf").appendTo($("#testmenu"));
            //$(WebRADControl).each(function (index) {
            //    alert(index + ": " + $(this).text());
            //});
        }
        </script> 

    

  <script type="text/javascript">
      $(function () {
          $.contextMenu({
              selector: '#FormTitle',
              items: $.contextMenu.fromMenu($('#html5menu'))
          });
      });
  </script> 
    

    <script type="text/javascript">
        $('#FormTitle').blur(ToggleFormTitle);

        function ToggleFormTitle() {
            if ($('#FormTitle').is(":visible")) {
                $('#FormTitleText').html($('#FormTitle').val());
                $('#FormTitleText').show();
                $('#FormTitle').hide();
                
            }
            else
            {
                $('#FormTitle').val($('#FormTitleText').html());
                $('#FormTitle').show();
                $('#FormTitleText').hide();
                
                
            }

            
            $('#FormTitle').focus();
        }
                  </script>

    <asp:Panel ID="pnlHide" runat="server" Visible="false">
        
 <br /><br />
 <span class="MainHeaders">Project Details</span>
 <br /><br />
 <br />
	    <strong>Page Title</strong> 
		<br />
		<asp:label ID="lblPageTitle" runat="server" ></asp:label>
		
		<br />
		<br />
	    <strong>Department Name</strong> 
		<br />
		<asp:Label ID="lblDepartmentName" runat="server"></asp:Label>
		
		<br />
		<br />
	    <strong>Is this an e-commerce form?</strong>
		<br />
        <asp:Label ID="lblEcommerce" runat="server"></asp:Label>
		
		<asp:Panel ID="pnlEcommerce" runat="server" Visible="false">
		<br />
		<br />
	    <strong>E-Commerce Product</strong>
		<br />
		<asp:Label ID="lblEcommerceProduct" runat="server"></asp:Label>
		
		</asp:Panel>
		
            	<br />
		<br />
		<strong>SQL Server</strong>
		<br />
		<asp:Label ID="lblSQLServer" runat="server"></asp:Label>
			
		<br />
		<br />
		<strong>SQL Database</strong>
		<br />
		<asp:Label ID="lblSQLDatabase" runat="server"></asp:Label>
			
		<br />
		<br />
		<strong>SQL Main Table Name</strong>
		<br />
		<asp:label ID="lblSQLMainTableName" runat="server"></asp:label>
	
	    <br />
		<br />
		<strong>SQL Insert Stored Procedure Name</strong>
		<br />
		usp_Insert<asp:label ID="lblSQLInsertStoredProcedureName" runat="server"></asp:label>
		
		<br />
		<br />
		<strong>SQL Update Stored Procedure Name</strong>
		<br />
		usp_Update<asp:label ID="lblSQLUpdateStoredProcedureName" runat="server"></asp:label>

        <br /><br />
        <strong>Control List</strong>
        <br />
        <asp:Label ID="lblControlList" runat="server"></asp:Label>

        <br /><br />
        <strong>Notes</strong>
        <br />
        <asp:TextBox ID="txtNotes" runat="server" CssClass="MlText" TextMode="MultiLine" Rows="5" Columns="100"></asp:TextBox>
        
        <br />
        <br />
            <asp:CheckBox ID="chkCreateFrontend" runat="server" Checked="true" Text="Create frontend now?" />
        
            <div style="padding-left:30px">
                <asp:CheckBoxList ID="cblPages" runat="server"></asp:CheckBoxList>
            </div>

        <br />
        <br />
            <asp:CheckBox ID="chkCreateBackend" runat="server" Checked="true" Text="Create backend now?" />
        <br /><br />
        <asp:RadioButtonList ID="rblProjectType" runat="server" RepeatDirection="horizontal">
            <asp:ListItem Value="Live" Selected="true">Create Live Project</asp:ListItem>
            <asp:ListItem Value="Test">Create Test Project</asp:ListItem>
        </asp:RadioButtonList>
        
        <br /><br /><br />
        To finalize the creation of this project, please click the Submit button.  Once all project files and data are created, you will be linked to the locations of your new project pages.

		<div style="text-align:center">
		<asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="Button" />
		</div>
		<div style="text-align:center">
		    <asp:ValidationSummary ID="vsWebRAD" runat="server" />
		</div>
    </asp:Panel>

</body>
</html>