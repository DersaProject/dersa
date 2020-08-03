$(document).ready(function () {
    $('#editor').keydown(function (e) {
        if (e.keyCode == 13 && !e.shiftKey && !e.ctrlKey) {
            $('#ok').click();
        }
        else
            e.stopPropagation();
    });

    var getEditorValue = function (row, cellvalue, editor) {
        return $('#editor').jqxTextArea('val');
    };
    var initEditor = function (row, cellvalue, editor, celltext, pressedChar) {
        $('#editor').jqxTextArea('val', cellvalue);
        $('#window').jqxWindow('open');
    };
    var createEditor = function (row, cellvalue, editor) {
        $('#window').jqxWindow({
            autoOpen: false, width: 500, position: 'bottom, center', height: 400, maxWidth: 800,
            resizable: false, isModal: true,
            okButton: $('#ok'), cancelButton: $('#cancel'),
            initContent: function () {
                $("#editor").jqxTextArea({ height: 320, width: '100%', minLength: 1 }).val(cellvalue);
                $('#ok').jqxButton({
                    width: '65px',
                    theme: 'energyblue'
                });
                $('#cancel').jqxButton({
                    width: '65px',
                    theme: 'energyblue'
                });
                $('#editor').jqxTextArea('focus');
            }
        });
        //$('#window').keypress(function (eventObject) {
        //    alert(eventObject.which);
        //    //if (eventObject.which == 13)
        //    //    $('#ok').click();
        //});
        //$('#window').on('close', function (event) {
        //    if (event.args.dialogResult.OK) {
        //        var rowindex = $('#grid').jqxGrid('getselectedrowindex');
        //        $('#grid').jqxGrid('beginupdate');

        //        //var body = new Object();
        //        //body.class_name = cObj.class_name;
        //        //var changedObject = { id: $('#grid').jqxGrid('getrowid', rowindex) };
        //        //changedObject[editedObject.DataField] = $('#editor').jqxTextArea('val');
        //        //body.json_object = JSON.stringify(changedObject);
        //        //var xhr = new XMLHttpRequest();
        //        //xhr.open('POST', '/Object/Update', false);
        //        //xhr.setRequestHeader('Content-Type', 'application/json');
        //        //xhr.send(JSON.stringify(body));

        //        $('#grid').jqxGrid('endupdate');
        //    }
        //});
    }
            // prepare the data
    data.fields.forEach(function (item) {
        if (item.exteditor) {
            item.createeditor = createEditor;
            item.initeditor = initEditor;
            item.geteditorvalue = getEditorValue;
        }
    });
    console.log(data);

            var source =
            {
                localdata: data.tabledata,
                datatype: "local",
                addrow: function (rowid, rowdata, position, commit) {
                    // synchronize with the server - send insert command
                    // call commit with parameter true if the synchronization with the server is successful 
                    //and with parameter false if the synchronization failed.
                    // you can pass additional argument to the commit callback which represents the new ID if it is generated from a DB.
                    commit(true);
                },
                deleterow: function (rowid, commit) {
                    // synchronize with the server - send delete command
                    // call commit with parameter true if the synchronization with the server is successful 
                    //and with parameter false if the synchronization failed.
                    commit(true);
                },
                updaterow: function (rowid, newdata, commit) {
                    // synchronize with the server - send update command
                    // call commit with parameter true if the synchronization with the server is successful 
                    // and with parameter false if the synchronization failed.
                    commit(true);
                }
            };
            var dataAdapter = new $.jqx.dataAdapter(source);
            // initialize jqxGrid
            $("#grid").jqxGrid(
            {
                width: getWidth('Grid'),
                height: 350,
                //rowsheight:60,
                source: dataAdapter,
                editable: true,
                editmode: 'dblclick',
                enabletooltips: true,
                    showtoolbar: true,
                    columnsresize: true,
                rendertoolbar: function (toolbar) {
                    var me = this;
                    var container = $("<div style='margin: 5px;'></div>");
                    toolbar.append(container);
                    container.append('<input id="addrowbutton" type="button" value="Add Row" />');
                    container.append('<input style="margin-left: 5px;" id="deleterowbutton" type="button" value="Delete Row" />');
                    container.append('<input style="margin-left: 5px;" id="savebutton" type="button" value="Save" />');
                    $("#addrowbutton").jqxButton();
                    $("#deleterowbutton").jqxButton();
                    $("#savebutton").jqxButton();
                    // update row.
                    $("#savebutton").on('click', function () {
                        console.log(JSON.stringify(dataAdapter.records));
                        var body = new Object();
                        body.entity = data.entity;
                        body.prop_name = 'JsonContent';
                        body.prop_value = JSON.stringify(dataAdapter.records);
                        var xhr = new XMLHttpRequest();
                        xhr.open('POST', '/Node/SetTextProperty', false);
                        xhr.setRequestHeader('Content-Type', 'application/json');
                        xhr.send(JSON.stringify(body));
                    });
                    // create new row.
                    $("#addrowbutton").on('click', function () {
                        var datarow = {};
                        var commit = $("#grid").jqxGrid('addrow', null, datarow);
                    });
                    // delete row.
                    $("#deleterowbutton").on('click', function () {
                        var selectedrowindex = $("#grid").jqxGrid('getselectedrowindex');
                        if (selectedrowindex >= 0) {
                            var id = $("#grid").jqxGrid('getrowid', selectedrowindex);
                            var commit = $("#grid").jqxGrid('deleterow', id);
                        }
                    });
                },
                columns: data.fields
            });    
        });
