function OnMenuItemClick() {
    // Удаляем предыдущие вызванное контекстное меню:
    $('.context-menu').remove();
    var instance = $('#dersa').jstree(true);
    var selected_node = instance.get_selected();
    if (selected_node[0][0] == "D") {
        EditDiagram();
    }
    else if (selected_node < 0)
        GoToB(selected_node);
    else
        RefreshNode(selected_node);
}

function MenuAction()
{
    var instance = $('#dersa').jstree(true);
    var selected_node = instance.get_selected();
    if (selected_node[0][0] == "D") {
        return "editDiagram";
    }
    else if (selected_node < 0)
        return "goto_b";
    else
        return "refresh";
}
function RefreshNode() {
    var instance = $('#dersa').jstree(true);
    var selected_node = instance.get_selected();
    instance.refresh_node(selected_node);
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

function GoToB() {
//    var instance = $('#dersa').jstree(true);
    var relId = instance.get_selected();
//    instance.deselect_all();
    nodeId = FindNext(relId, '');
//    if (instance.get_node(nodeId))
//        instance.select_node(nodeId);
    GoToNode(nodeId);
}
function EditDiagram() {
    chEdit.attributes.removeNamedItem("hidden");
    chEdit.checked = true;
    laEdit.innerHTML = "Diagram edit mode";
    infoControl.innerHTML = "";
    dID = document.getElementById("DiagramId");
    diagramXml = GetText(dID.value, "DiagramXml");
    drawGraph(infoControl, diagramXml);
}
function CancelEditDiagram() {
    if (chEdit.attributes.getNamedItem("hidden") == null)
        chEdit.attributes.setNamedItem(document.createAttribute("hidden"));
    laEdit.innerHTML = "";
}

function drawGraph(container, xml) {
    // Checks if the browser is supported
    if (!mxClient.isBrowserSupported()) {
        // Displays an error message if the browser is not supported.
        mxUtils.error('Browser is not supported!', 200, false);
    }
    else {
        // Disables the built-in context menu
        mxEvent.disableContextMenu(container);

        // Creates the graph inside the given container
        var graph = new mxGraph(container);

        // Enables rubberband selection
        new mxRubberband(graph);

        // Gets the default parent for inserting new cells. This
        // is normally the first child of the root (ie. layer 0).
        var parent = graph.getDefaultParent();

        // Adds cells to the model in a single step
        graph.getModel().beginUpdate();
        try {
            //var v1 = graph.insertVertex(parent, null, 'Hello,', 20, 20, 80, 30);
            //var v2 = graph.insertVertex(parent, null, 'World!', 200, 150, 80, 30);
            //var e1 = graph.insertEdge(parent, null, '', v1, v2);
            //var graphxml = document.getElementById("graphxml");
            //alert(graphxml.innerText);
            var doc = mxUtils.parseXml(xml);
            var codec = new mxCodec(doc);
            codec.decode(doc.documentElement, graph.getModel());
        }
        finally {
            // Updates the display
            graph.getModel().endUpdate();
        }
    }
}

//function GetProperties(id) {
//    var xhr = new XMLHttpRequest();
//    args = "id=" + id;
//    xhr.open('GET', "/node/properties?" + args, false);
//    xhr.send();
//    var response = xhr.responseText;
//    return response;
//}

function DnD(src, dst)
{
    var xhr = new XMLHttpRequest();
    args = "src=" + src + "&dst=" + dst;
    xhr.open('GET', "DnDHandler.ashx?" + args, false);
    xhr.send();
    var response = xhr.responseText;
    return response;
}

function RenameNode(id, name) {
    var xhr = new XMLHttpRequest();
    xhr.open('POST', "/node/rename/" + id + "?name=" + name, false);
    xhr.send();
    var response = xhr.responseText;
    return response;
}

function GetText(id, attr_name) {
            var xhr = new XMLHttpRequest();
            //if (attr_name == "Diagram")
            //{
            //    xhr.open('GET', "Hello.aspx", false);
            //}
            //else
            //{
                args = "id=" + id;
                if (attr_name != "")
                    args = args + "&attr_name=" + attr_name;
                xhr.open('GET', "/node/description?" + args, false);
            //}
            xhr.send();
            var response = xhr.responseText;
            return response;
        }

        function FindNext(id, searchtext) {
            var xhr = new XMLHttpRequest();
            xhr.open('GET', "SearchHandler.ashx?id=" + id + "&searchtext=" + searchtext, false);
            xhr.send();
            var response = xhr.responseText;
            return response;
        }

        function GetPath(id) {
            var xhr = new XMLHttpRequest();
            xhr.open('GET', "FoundInfoHandler.ashx?id=" + id, false);
            xhr.send();
            var response = xhr.responseText;
            return response;
        }

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
