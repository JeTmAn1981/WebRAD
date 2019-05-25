

window.onload = function () {
    LoadTinyMCE();
};


function LoadTinyMCE() {
    var WebRADControls;

    WebRAD.WebService.GetProjectControls(getParameterByName("ID"), onFinished);

    function onFinished(result) {
        WebRADControls = result;
        //console.log(result);
        //return;
        var str = "<script type='text/javascript'>tinymce.init({";

        str += "selector: '.RichText',";
        //str += "height: '30px',";
        str += "relative_urls: true,";
        str += "remove_script_host: false,";
        str += "valid_elements : '*[*]',";
        str += "theme: 'modern',";
        str += "plugins: [";
        str += "'advlist autolink lists link image charmap print preview hr anchor pagebreak',";
        str += "'searchreplace wordcount visualblocks visualchars code fullscreen',";
        str += "'insertdatetime media nonbreaking save table contextmenu directionality',";
        str += "'emoticons template paste textcolor colorpicker textpattern'";
        str += "],";
        str += "toolbar1: 'insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image',";
        str += "toolbar2: 'print preview media | forecolor backcolor emoticons',";
        str += "image_advtab: true,";
        str += "templates: [";
        str += "{ title: 'Test template 1', content: 'Test 1' },";
        str += "{ title: 'Test template 2', content: 'Test 2' }";
        str += "],";
        str += " setup: function (ed) {";
        str += "ed.addMenuItem('example', {";
        str += "text: 'Insert Control Value',";
        str += "context: 'insert',";
        str += "menu: [";

        var items = "";

        for (i = 0; i < WebRADControls.length; i++) {
            if (items)
                items += ",";

            items += "{ text: '" + WebRADControls[i].item.Name + "', onclick: function () { ed.insertContent('{{" + WebRADControls[i].key + "}}'); }}";

        }

        str += items;
        str += " ]});";
        str += " }";
        str += "});";
        str += "<\/script>"

        console.log(str);
        $('body').append(str);

    }
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}



//         window.WebRADControls = [];

//function GetInsertControls(ed) {
//    var controls = [];


//    PageMethods.TestMethod(onFinished);



//    alert(window.WebRADControls.length);

//    for (i = 0; i < WebRADControls.length; i++) {
//        currentKey = WebRADControls[i].key;

//        controls.push({ text: currentKey, onclick: function () { ed.insertContent(GetCurrentKey(i)); } });

//    }


//    controls.push({ text: "test1", onclick: function () { ed.insertContent('test1'); }});
//    controls.push({ text: "test2", onclick: function () { ed.insertContent('test2'); }});
//    return controls;

//}

//function onFinished(result) {
//    window.WebRADControls = result;


//}


//function GetCurrentKey(i) {
//    return WebRADControls[i].key;
//}
