$(document).ready(function () {
    // Значение отступа, выше которого блок не поднимается,
    // когда ползунок прокрутки на самом верху
    var MIN_TOP = 200;
    // Отступ от левого края блока контента,
    // может быть положительным или отрицательным
    var POSITIONLEFT = - 25;
    // Центрирование окна
    //var width_minus = Math.floor(parseInt($('#message_box').css("width")) / 2);

    var POSITIONTOP = 0;

    $('#page').css("left", +$('#dersa').width() + 75 + "px");
    $('#page').css("top", "200px");
    $('#page').hide();
    $('#infoboard').css("left", +$('#dersa').width() + 75 + "px");
    $('#infoboard').css("top", "200px");
    $('#infoboard').show();


    $(window).scroll(function () {
        var pos = parseInt($(window).scrollTop());
        if (pos < 200) pos = 200;
        $('#tool_box').animate({ top: pos + "px" }, { queue: false, duration: 200 });
        //console.log(POSITIONTOP);
        //console.log(pos);
        if ($('#infotable').height() + POSITIONTOP - pos + 200 < $(window).height() || POSITIONTOP > pos + 100) {
            $('#infoboard').animate({ top: pos + "px" }, { queue: false, duration: 200 });
            $('#page').animate({ top: pos + "px" }, { queue: false, duration: 200 });
            POSITIONTOP = pos;
        }
        //if (POSITIONTOP > pos + 100)
        //    console.log('need move');
        //console.log('left block height:' + $('#dersa').height() + '; right block height: ' + $('#infotable').height() + '; window height: ' + $(window).height() + '; scroll:' + $(window).scrollTop());
    });

    $(window).resize(function () {
        //    $('#message_box').css("left", $('#mycontent').offset().left + POSITIONLEFT + "px");
        $('#infoboard').animate({ left: +$('#dersa').width() + 75 + "px" }, { queue: false, duration: 200 });
    });

});


$.jstree.defaults.dnd = {
        copy: false,//function (data) { console.log(data.event.target.id); return data.event.target.id === "Entity_anchor"; },//true,//{ return nodes[0].id == "Attribute" || nodes[0].id == "Entity"; },
        always_copy: false,
        open_timeout: 500,
        is_draggable: function (nodes) { return nodes[0].id != "ENTITIES" && nodes[0].id != "STEREOTYPES";/*nodes[0].id === "Package" || nodes[0].id === "Entity" || nodes[0].id === "Attribute";*/ },
        can_dnd: function (ndsrc, nddst, data) {
            return CanDnD(ndsrc, nddst, data);
        }
}

var lastDnDsource;
var lastDnDtarget;
var lastDnDresult;
var selectedForCut;

function InsertNode(data) {
    var inst = $.jstree.reference(data.reference);
    obj = inst.get_node(data.reference);
    var result = DnD(data.item.label, obj.id, 0);
    var newObjArr = JSON.parse(result);
    var newId = newObjArr[0].id;
    var newIcon = newObjArr[0].icon;
    var newText = newObjArr[0].text;
    var new_node = inst.create_node(obj, newId, 'last');
    inst.set_id(new_node, newId);
    inst.set_icon(newId, newIcon);
    inst.set_text(newId, newText);
    if (obj.state.loaded) {
        inst.deselect_all();
        inst.select_node(newId);
        EditNode(newId, newText);
    }
    else
        RefreshNode(newId);  
}

function AddSubmenu(items, dest) {
    dest.submenu = {};
    for (var i = 0; i < items.length; i++) {
        dest.submenu[i] = {
            "label": items[i].label,
            "icon": items[i].icon,
            "action": function (data) {
                InsertNode(data);
            }
        };
        if (items[i].children && items[i].children.length > 0) {
            dest.submenu[i].action = null;
            AddSubmenu(items[i].children, dest.submenu[i]);
        }

    }
}

function CanDnD(src, dstNode, data)
{
    var graphControl = document.getElementById("graph");
    if (chEdit.checked/*graph != null*/ && data.event.target != null) {
        if (graphControl.innerHTML.trim() == data.event.target.outerHTML) {
            var instance = data.data.origin;
            var src_node = instance.get_node(src);
            AddVertex(src, src_node.text, data.event.offsetX, data.event.offsetY);
            return;
        }
    }
    if (dstNode) {
        var result = lastDnDresult;
        if (dstNode.id) {
            if (src != lastDnDsource || dstNode.id != lastDnDtarget) {
                var xhr = new XMLHttpRequest();
                args = "src=" + src + "&dst=" + dstNode.id;
                xhr.open('GET', "node/CanDnD?" + args, false);
                xhr.send();
                result = xhr.responseText > 0;
                lastDnDresult = result;
                lastDnDsource = src;
                lastDnDtarget = dstNode.id;
            }
        }
        else
            result = true;
        return result;
    }
    else if (data.event.target.ondrop) {
        data.event.data = { id: src, name: data.element.text };
        return data.event.target.ondrop(data.event);
    }
    return false;
}

$(document)
.on("dnd_stop.vakata", function (e, data) {
    var instance = $('#dersa').jstree(true);

    var node = data.data.nodes[0];
    var target = instance.get_node(data.event.target);
    var canDnD = CanDnD(node, target, data);
    if (!canDnD)
        return;
    var options = 0;
    if (data.event.ctrlKey)
        options += 1;
    if (data.event.shiftKey)
        options += 2;
    if (data.event.altKey)
        options += 4;
    var handlerEcho = DnD(node, target.id, options);
    instance.delete_node(node);
    var newObjArr = JSON.parse(handlerEcho);
    var newId = newObjArr[0].id;
    var newIcon = newObjArr[0].icon;
    var newText = newObjArr[0].text;
    var new_node = instance.create_node('#', node, 'last');
    instance.set_id(new_node, newId);
    instance.set_icon(newId, newIcon);
    instance.set_text(newId, newText);
    data.data.nodes = [newId];
})

$.jstree.defaults.contextmenu = {
    select_node: true,
    show_at_node: false,
    items: function (o, cb) { // Could be an object directly
        return {
            "refresh": {
                "separator_before": false,
                "separator_after": true,
                "_disabled": o.id < 0 || o.id[0] == "D",//this.check("create_node", data.reference, {}, "last"),
                "label": "Refresh",
                "action": function (data) {
                    var inst = $.jstree.reference(data.reference),
                        obj = inst.get_node(data.reference);
                    RefreshNode();
                }
            },
            "rename": {
                "separator_before": false,
                "separator_after": false,
                "_disabled": o.text == 'Entities' || o.text == 'Stereotypes',//false, //(this.check("rename_node", data.reference, this.get_parent(data.reference), "")),
                "label": "Rename",
                "action": function (data) {
                    var inst = $.jstree.reference(data.reference),
                        obj = inst.get_node(data.reference);
                    obj_text = inst.get_text(data.reference);
                    EditNode(obj, obj_text);
                    //inst.edit(obj, obj_text, function (obj, tmp, nv, cancel) { RenameNode(obj.id, inst.get_text(data.reference)); });
                }
            },
            "edit":
            {
                "separator_before": true,
                "icon": false,
                "separator_after": true,
                "label": "Edit node",
                "action": false,
                "sumbenu": {},
            },
            "find": {
                "separator_before": false,
                "separator_after": true,
                "_disabled": false,// +o.id.isNaN || +o.id > 0, //(this.check("create_node", data.reference, {}, "last")),
                "label": "Find",
                "action": false,
                "submenu": {},
            },
            "methods": {
                "separator_before": false,
                "separator_after": false,
                "_disabled": false,
                "label": "Methods",
                "action": function (data) {
                    var inst = $.jstree.reference(data.reference),
                    obj = inst.get_node(data.reference);
                    formCaption = obj.text.substring(0, 20) + ' methods';
                    var form = new mxForm(formCaption);
                    //// Adds a readonly field for the entity id
                    //var id = form.addText('ID', objectid/*cell.getId()*/);
                    //id.setAttribute('readonly', 'true');

                    var xhr = new XMLHttpRequest();
                    args = "id=" + obj.id;
                    xhr.open('GET', "node/MethodsForm?" + args, false);
                    xhr.send();
                    var attrs = JSON.parse(xhr.responseText);

                    var Props = CreateProperties(form, attrs, "");
                    //alert(Props.innerHTML);
                    var wnd = new mxWindow(formCaption,
                        Props, 100, +$(window).scrollTop() + 100, 250, 400, false, true);
                    form.window = wnd;
                    wnd.setVisible(true);
                }
            },
            "properties": {
                "separator_before": false,
                "separator_after": true,
                "_disabled": false,
                "label": "Properties",
                "action": function (data) {
                    var inst = $.jstree.reference(data.reference),
                    obj = inst.get_node(data.reference);
                    var form = new mxForm('properties');
                    //// Adds a readonly field for the entity id
                    //var id = form.addText('ID', objectid/*cell.getId()*/);
                    //id.setAttribute('readonly', 'true');

                    var xhr = new XMLHttpRequest();
                    args = "id=" + obj.id;
                    xhr.open('GET', "node/PropertiesForm?" + args, false);
                    xhr.send();
                    var attrs = JSON.parse(xhr.responseText);
                    var Props = CreateProperties(form, attrs, "Node/SetProperties");
                    //alert(Props.innerHTML);
                    var wnd = new mxWindow('properties',
                        Props, 100, +$(window).scrollTop() + 100, 300, (attrs.length * 22) + 60, false, true);
                    form.window = wnd;
                    wnd.setVisible(true);
                }
            },
            "insertEntity": {
                        "separator_before": true,
                        "icon": false,
                        "separator_after": false,
                        "label": "Insert",
                        "action": false,
                        "sumbenu": {}
                    }
        }
    }
}


    $('#goto_button').on("click", function () {
        var instance = $('#dersa').jstree(true);
        instance.deselect_all();
        var found = document.getElementById("found");
        var nextId = found.value;
        if (instance.get_node(nextId)) {
            instance.select_node(nextId);
            var documentId = '#' + nextId + '_anchor';
            var scrollTop = $(documentId).offset().top;
            $(document).scrollTop(scrollTop);
        }
});

$('#listBox').on("click", function () {
    var listBox = document.getElementById("listBox");
    Index = listBox.selectedIndex;
    if (Index >= 0) {
        var found = document.getElementById("found");
        found.value = listBox.options[Index].value;
        var foundpath = document.getElementById("foundpath");
        foundpath.value = listBox.options[Index].text;
    }
});

$('#reset_button').on("click", function () {
    var listBox = document.getElementById("listBox");
    while (listBox.options.length > 0) {
        listBox.options[0] = null;
    }
});

$('#find_button').on("click", function () {
    var searchtext = document.getElementById("searchtext");
    if (searchtext.value.length < 4)
        return;
    var instance = $('#dersa').jstree(true);
    var found = document.getElementById("found");
    var foundpath = document.getElementById("foundpath");
    var nextId = found.value;
    nextId = FindNext(nextId, searchtext.value);
    found.value = nextId;
    foundpath.value = GetPath(nextId);
    addValue();
});

function EditNode(obj, obj_text) {
    var instance = $('#dersa').jstree(true);
    instance.edit(obj, obj_text, function (obj, tmp, nv, cancel) { RenameNode(obj.id, obj.text); });
}

function DoPaste(selectedForCut, dst) {
    while (selectedForCut.length) {
        DnD(selectedForCut[0], dst, 0);
        selectedForCut.shift();
    }
}

function FindInChildren(nodeId) {
    var fheader = 'find (parent = ' + nodeId + ')';
    var form = new mxForm(fheader);
    var xhr = new XMLHttpRequest();
    var srchVal = prompt('Введите строку поиска');
    if (!srchVal)
        return;
    args = 'srchval=' + srchVal + '&root_entity=' + nodeId;
    xhr.open('GET', 'Entity/Find?' + args, false); xhr.send();
    var attrs = JSON.parse(xhr.responseText);
    var Props = CreateProperties(form, attrs, '');
    var wnd = new mxWindow(fheader, Props, 100, 100, 800, 400, false, true);
    form.window = wnd;
    wnd.setVisible(true); 
}


function RefreshNode(selectId) {
    var instance = $('#dersa').jstree(true);
    var selected_node = instance.get_selected();
    instance.refresh_node(selected_node);
    if (selectId)
    {
        instance = $('#dersa').jstree(true);
        var selectedNode = instance.get_node(selectId);
        if (selectedNode) {
            instance.select_node(selectId);
        }
        else {
            var jsonPath = GetPath(selectId, 1);
            npath = JSON.parse(jsonPath);
            LoadPath();
        }
    }
}

function CanShow(event) {
    laEdit = document.getElementById("laEdit");
    if (laEdit != null && laEdit.innerHTML != "")
        return false;
    var displaytext = document.getElementById("displaytext");
    displaytext.value = event.target.id;
    return displaytext.value == GetSelectedNode();
}

function GetSelectedNode() {
    var instance = $('#dersa').jstree(true);
    sel = instance.get_selected();
    if (sel == -1 || sel == null || sel.length == 0) {
        return "пыщьпыщь";//потому что в этом случае не надо показывать меню
    }
    res = sel + "_anchor";
    return res;
}

var instance;
function GoToB() {
    if(!instance)
        instance = $('#dersa').jstree(true);
    var relId = instance.get_selected();
    instance.deselect_all();
    nodeId = FindNext(relId, '');
    if (instance.get_node(nodeId)) {
        instance.select_node(nodeId);
        var documentId = '#' + nodeId + '_anchor';
        var scrollTop = $(documentId).offset().top;
        $(document).scrollTop(scrollTop);

    }
    else {
        var jsonPath = GetPath(nodeId, 1);
        npath = JSON.parse(jsonPath);
        LoadPath();
    }
}

function GoToNode(N)
{
    jP = GetPath(N, 1);
    npath = JSON.parse(jP);
    LoadPath();
}

var navigationArray = new Array();;
var navigationArrayIndex = -1;
var preventSaveNode = false;

function SaveCurrentNode(nodeId) {
    if (preventSaveNode) {
        preventSaveNode = false;
        return;
    }
    navigationArrayIndex++;
    navigationArray[navigationArrayIndex] = nodeId;
}

function moveForward() {
    if (navigationArrayIndex >= navigationArray.length - 1) {
        navigationArrayIndex = navigationArray.length - 1;
        return null;
    }
    preventSaveNode = true;
    return navigationArray[++navigationArrayIndex];
}
function moveBack() {
    if (navigationArrayIndex <= 0) {
        navigationArrayIndex = 0;
        return null;
    }
    preventSaveNode = true;
    return navigationArray[--navigationArrayIndex];
}

var npath;
function LoadPath() {
    if (!instance)
        instance = $('#dersa').jstree(true);
    if (npath.length == 1) {
        //console.log(instance.get_node(npath[0]).state);
        if (!instance.get_node(npath[0]) || !instance.get_node(npath[0]).state.loaded)
            instance.load_node(npath[0]);
        instance.deselect_node(instance.get_selected());
        instance.select_node(npath[0]);
        var documentId = '#' + npath[0] + '_anchor';
        var scrollTop = $(documentId).offset().top;
        $(document).scrollTop(scrollTop);
    }
    else {
        var lnode = npath[0];
        npath.shift();
        var nodeObject = instance.get_node(lnode);
        if (!nodeObject || !nodeObject.state.loaded) {
            instance.load_node(lnode, LoadPath);
        }
        else {
            LoadPath();
        }
    }
}

function GetPath(id, for_system) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', "entity/GetPath?id=" + id + "&for_system=" + for_system, false);
    xhr.send();
    var response = xhr.responseText;
    return response;
}


var DiagramIsEdit = false;

function NewDiagram(id) {
    //alert(id);
    args = "parent=" + id;
    xhr = new XMLHttpRequest();
    xhr.open('POST', "diagram/Create", false);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.send(args);
}

function EditDiagram() {
    if (Editor != null)
        Editor.execute("clearDiagram");
    chEdit.attributes.removeNamedItem("hidden");
    chEdit.checked = true;
    laEdit.innerHTML = "Diagram edit mode";
    infoControl.innerHTML = "";
    dID = document.getElementById("DiagramId");
    diagramXml = GetText(dID.value, "DiagramXml");
    //diagramXmlAdd = GetText(dID.value, "DiagramXmlAdditional");
    
    //diagXmlMain = document.getElementById("diag_xml_main");
    //diagXmlAdd = document.getElementById("diag_xml_add");
    //diagXmlMain.value = diagramXml;
    //diagXmlAdd.value = diagramXmlAdd;
    //chAddXml = document.getElementById("add_xml");
    //console.log(diagXmlMain.value);
    //if (chAddXml.checked)
    //    drawGraph(/*infoControl,*/ diagXmlAdd.value);//'<mxGraphModel> <root> <mxCell id="0"/> <mxCell id="1" parent="0"/>  <Shape label="Shape" href="" id="208"> <mxCell style="ellipse" parent="1" vertex="1"> <mxGeometry x="310" y="210" width="60" height="60" as="geometry"/> </mxCell>  </Shape>  <mxCell id="602257" parent="1" vertex="1">  <Object entity="602256" label="SYSTEM_X " as="value"/>  <mxGeometry x="270" y="86" width="120" height="25" as="geometry"/> </mxCell> </root></mxGraphModel>');
    //else 
    drawGraph(diagramXml);
    DiagramIsEdit = true;
}

function CancelEditDiagram() {
    console.log('CancelEditDiagram was commented');
    return;
    codec = new mxCodec();
    encXml = codec.encode(Editor.graph.getModel())
    Editor.execute("clearDiagram");
    if (DiagramIsEdit)//(chEdit.checked)//(graph != null)
    {
        if (confirm('Сохранить диаграмму ?'))
        {
            var xml = mxUtils.getXml(encXml);
            safeXml = xml.replace(new RegExp("<", 'g'), "{lt;").replace(new RegExp(">", 'g'), "{gt;");
            dID = document.getElementById("DiagramId");
            args = "id=" + dID.value + "&xml=" + safeXml;
            xhr = new XMLHttpRequest();
            //formData = new FormData();
            //formData.append('id', 0);
            xhr.open('POST', "diagram/Save", false);
            xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            xhr.send(args);
            //xhr.open('POST', "diagram/Save?" + args, false);
            //xhr.setRequestHeader("Content-Type", "text/xml");
            //xhr.send();
            response = xhr.responseText;
            //console.log(response);
            if (response == 0)
                //graph = null;
                DiagramIsEdit = false;
        }
        else
        {
            //graph = null;
            DiagramIsEdit = false;
        }
    }
    if (chEdit.attributes.getNamedItem("hidden") == null)
        chEdit.attributes.setNamedItem(document.createAttribute("hidden"));
    laEdit.innerHTML = "";
}

function AddVertex(id, text, x, y)
{
    Editor.execute('insertNode', id, text, x, y);
}

function drawGraph(xml) {
    Editor.execute('initG', xml);
}

function DnD(src, dst, options) {
    var xhr = new XMLHttpRequest();
    args = "src=" + src + "&dst=" + dst + "&options=" + options;
    xhr.open('GET', "node/DnD?" + args, false);
    xhr.send();
    return xhr.responseText;
}

function RenameNode(id, name) {
    var xhr = new XMLHttpRequest();
    xhr.open('POST', "node/rename/" + id + "?name=" + name, false);
    xhr.send();
    return xhr.responseText;
}

function RestoreNode(id) {
    var xhr = new XMLHttpRequest();
    args = '?id=' + id;
    xhr.open('POST', "node/restore" + args, false);
    xhr.send();
    return xhr.responseText;
}

function RemoveNode(id, options) {
    var xhr = new XMLHttpRequest();
    diagram_id = id[0] == 'D' ? id : '';
    args = diagram_id == '' ? id : '0&diagram_id=' + diagram_id;
    args = '?id=' + args + "&options=" + options;
    xhr.open('POST', "node/remove" + args, false);
    xhr.send();
    return xhr.responseText;
}

function GetText(id, attr_name) {
    var xhr = new XMLHttpRequest();
    args = "id=" + id;
    if (attr_name != "")
        args = args + "&attr_name=" + attr_name;
    xhr.open('GET', "node/description?" + args, false);
    xhr.send();
    return xhr.responseText;
}

function FindNext(id, searchtext) {
    var xhr = new XMLHttpRequest();
    //xhr.open('GET', "SearchHandler.ashx?id=" + id + "&searchtext=" + searchtext, false);
    xhr.open('GET', "Entity/Find?entity=" + id + "&srchval=" + searchtext, false);
    xhr.send();
    return xhr.responseText;
}

//function GetPath(id) {
//    var xhr = new XMLHttpRequest();
//    xhr.open('GET', "FoundInfoHandler.ashx?id=" + id, false);
//    xhr.send();
//    return xhr.responseText;
//}

function addValue() {
    var foundpath = document.getElementById("foundpath");
    var text = foundpath.value;
    var found = document.getElementById("found");
    var v = found.value;
    if (v > 0) {
        var listBox = document.getElementById("listBox");
        count = listBox.options.length;
        exists = false;
        for (i = 0; i < count; i++) {
            if (listBox.options[i].value == v)
                exists = true;
        }
        if (!exists) {
            AddOpt = new Option(text, v);
            listBox.options[count] = AddOpt;
        }
    }
    return true;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
$('#dersa')
    .jstree({
        "core": {
            "animation": 0,
            "check_callback": true,
            'force_text': true,
            "themes": { "stripes": true },
            "data": {
                'multiple': false,
                "url": "/node/list",
                "dataType": "json",
                "data": function (node) {
                    return { "id": node.id };
                }
            }
        },
        "contextmenu": {
            'items': function (o, cb) {
                var tmp = $.jstree.defaults.contextmenu.items(o, cb);
                tmp.find.submenu = {
                    "goto_b": {
                        "separator_before": false,
                        "separator_after": true,
                        "_disabled": +o.id.isNaN || o.id.split('_').length > 1 || o.id > 0, //(this.check("create_node", data.reference, {}, "last")),
                        "label": "Go to B",
                        "action": function (data) {
                            var inst = $.jstree.reference(data.reference),
                                obj = inst.get_node(data.reference);
                            GoToB();
                        }
                    },
                    "goto_a": {
                        "separator_before": false,
                        "separator_after": true,
                        "_disabled": o.id.split('_').length < 2,
                        "label": "Go to A",
                        "action": function (data) {
                            var inst = $.jstree.reference(data.reference);
                            var strId = ' ' + inst.get_selected()
                            GoToNode(strId.split('_')[1]);
                        }
                    },
                    "find_in_children": {
                        "separator_before": false,
                        "separator_after": true,
                        "_disabled": +o.id.isNaN || o.id.split('_').length > 1 || o.id < 0,
                        "label": "Find lower",
                        "action": function (data) {
                            var inst = $.jstree.reference(data.reference);
                            var strId = ' ' + inst.get_selected()
                            FindInChildren(o.id);
                        }
                    }

                };
                tmp.edit.submenu = {
                    "restore": {
                        "separator_before": false,
                        "icon": false,
                        "separator_after": true,
                        "_disabled": o.icon != 'Recycle',
                        "label": "Restore",
                        "action": function (data) {
                            var inst = $.jstree.reference(data.reference),
                                obj = inst.get_node(data.reference);
                            if (inst.is_selected(obj)) {
                                selectedNodes = inst.get_selected();
                                restoreMessage = selectedNodes.length > 1 ? "узлы" : obj.id;
                                if (confirm('Восстановить ' + restoreMessage + '?')) {
                                    while (selectedNodes.length > 0) {
                                        RestoreNode(selectedNodes[0]);
                                        selectedNodes.shift();
                                    }
                                }
                            }
                        }
                    },
                    "remove": {
                        "separator_before": false,
                        "icon": false,
                        "separator_after": true,
                        "_disabled": o.text == 'Entities' || o.text == 'Stereotypes',//false, //(this.check("delete_node", data.reference, this.get_parent(data.reference), "")),
                        "label": "Delete",
                        "action": function (data) {
                            var currentOptionsControl = document.getElementById("tbOptions");
                            var addInfo = "";
                            if (currentOptionsControl.value == 4)
                                addInfo = "без возможности восстановления "
                            var inst = $.jstree.reference(data.reference),
                                obj = inst.get_node(data.reference);
                            if (inst.is_selected(obj)) {
                                selectedNodes = inst.get_selected();
                                deleteMessage = selectedNodes.length > 1 ? "узлы" : obj.id;
                                if (confirm('Удалить ' + addInfo + deleteMessage + '?')) {
                                    inst.delete_node(selectedNodes);
                                    while (selectedNodes.length > 0) {
                                        RemoveNode(selectedNodes[0], currentOptionsControl.value);
                                        selectedNodes.shift();
                                    }
                                }
                            }
                            else {
                                if (confirm('Удалить ' + addInfo + obj.id + '?')) {
                                    RemoveNode(obj.id, currentOptionsControl.value);
                                    inst.delete_node(obj);
                                }
                            }
                            currentOptionsControl.value = 0;
                        }
                    },
                    "cut":
                    {
                        "separator_before": true,
                        "separator_after": false,
                        "_disabled": false,
                        "label": "Cut",
                        "action": function (data) {
                            var inst = $.jstree.reference(data.reference),
                                obj = inst.get_node(data.reference);
                            selectedForCut = inst.get_selected(false);
                        }
                    },
                    "copy":
                    {
                        "separator_before": false,
                        "separator_after": false,
                        "_disabled": true,
                        "label": "Copy",
                        "action": false
                    },
                    "paste":
                    {
                        "separator_before": false,
                        "separator_after": true,
                        "_disabled": !selectedForCut || selectedForCut.length < 1,
                        "label": "Paste",
                        "action": function (data) {
                            var inst = $.jstree.reference(data.reference),
                                obj = inst.get_node(data.reference);
                            DoPaste(selectedForCut, obj.id);
                            inst.refresh_node(obj);
                        }
                    },
                    "editDiagram": {
                        "separator_before": false,
                        "separator_after": false,
                        "_disabled": o.id[0] != "D", //(this.check("create_node", data.reference, {}, "last")),
                        "label": "Edit diagram",
                        "action": function (data) {
                            var inst = $.jstree.reference(data.reference),
                                obj = inst.get_node(data.reference);
                            EditDiagram();
                        }
                    },
                    "newDiagram": {
                        "separator_before": false,
                        "separator_after": true,
                        "_disabled": o.text == 'Entities' || o.text == 'Stereotypes' || o.id < 0 || o.id[0] == "D", //(this.check("create_node", data.reference, {}, "last")),
                        "label": "New diagram",
                        "action": function (data) {
                            var inst = $.jstree.reference(data.reference),
                                obj = inst.get_node(data.reference);
                            NewDiagram(obj.id);
                        }
                    }
                };
                tmp.insertEntity.submenu = {};
                if (o.id[0] == "D" || o.text == 'Entities' || o.text == 'Stereotypes' || o.id.split('_').length > 1) {
                    delete tmp.insertEntity;
                    return tmp;
                }

                var xhr = new XMLHttpRequest();
                args = "id=" + o.id;
                xhr.open('GET', "node/GetInsertSubmenu?" + args, false);
                xhr.send();
                var childStereotypes = JSON.parse(xhr.responseText);
                AddSubmenu(childStereotypes, tmp.insertEntity);
                return tmp;
            }
        },

        "plugins": ["contextmenu", "dnd", "state"]
    })
    .on("changed.jstree", function (e, data) {
        if (data.selected.length) {
            selected_id = data.instance.get_node(data.selected[0]).id;
            SaveCurrentNode(selected_id);
            var currentOptionsControl = document.getElementById("tbOptions");
            currentOptionsControl.value = 0;
            if (data.event) {
                var options = 0;
                if (data.event.ctrlKey)
                    options += 1;
                if (data.event.shiftKey)
                    options += 2;
                if (data.event.altKey)
                    options += 4;
                currentOptionsControl.value = options;
            }
            var infoControl = document.getElementById("infoboard");
            var diagControl = document.getElementById("page");
            var graphControl = document.getElementById("graph");
            bnEdit = document.getElementById("bnEdit");
            if (bnEdit != null && bnEdit.checked) {
                return;
            }

            CancelEditDiagram();
            if (selected_id.length == 0 || selected_id[0] != "D") {
                infoControl.style.display = "";
                diagControl.style.display = "none";
                infoControl.innerHTML = GetText(selected_id, "") + GetText(selected_id, "Text");
            }
            else {
                infoControl.style.display = "none";
                diagControl.style.display = "";
                dID = document.getElementById("DiagramId");
                dID.value = selected_id;
                var diaghtml = GetText(selected_id, "Diagram");
                //console.log(diaghtml);
                Editor.execute('showSavedDiagram', diaghtml);
                //infoControl.innerHTML = GetText(selected_id, "Diagram");
            }
        }
    });
