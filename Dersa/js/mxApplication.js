/*
 * Copyright (c) 2006-2013, JGraph Ltd
 *
 * Defines the startup sequence of the application.
 */
{

	/**
	 * Constructs a new application (returns an mxEditor instance)
	 */
	function createEditor(config)
	{
		var editor = null;
		
		var hideSplash = function()
		{
			// Fades-out the splash screen
			var splash = document.getElementById('splash');
			
			if (splash != null)
			{
				try
				{
					mxEvent.release(splash);
					mxEffects.fadeOut(splash, 100, true);
				}
				catch (e)
				{
					splash.parentNode.removeChild(splash);
				}
			}
		};
		
		try
		{
			if (!mxClient.isBrowserSupported())
			{
				mxUtils.error('Browser is not supported!', 200, false);
			}
			else
			{
				mxObjectCodec.allowEval = true;
				var node = mxUtils.load(config).getDocumentElement();
				editor = new mxEditor(node);
				mxObjectCodec.allowEval = false;
				
				// Adds active border for panning inside the container
				editor.graph.createPanningManager = function()
				{
					var pm = new mxPanningManager(this);
					pm.border = 30;
					
					return pm;
				};
				
				editor.graph.allowAutoPanning = true;
				editor.graph.timerAutoScroll = true;
				
				// Updates the window title after opening new files
				var title = document.title;
				var funct = function(sender)
				{
					document.title = title + ' - ' + sender.getTitle();
				};
				
				editor.addListener(mxEvent.OPEN, funct);
				
				// Prints the current root in the window title if the
				// current root of the graph changes (drilling).
				editor.addListener(mxEvent.ROOT, funct);
				funct(editor);
				
				// Displays version in statusbar
				editor.setStatus('mxGraph '+mxClient.VERSION);

				// Shows the application
				hideSplash();
			}
		}
		catch (e)
		{
			hideSplash();

			// Shows an error message if the editor cannot start
			mxUtils.alert('Cannot start application: ' + e.message);
			throw e; // for debugging
		}

		return editor;
	}

}

// Program starts here. The document.onLoad executes the
// createEditor function with a given configuration.
// In the config file, the mxEditor.onInit method is
// overridden to invoke this global function as the
// last step in the editor constructor.
function onInit(editor) {
    Editor = editor;
    // Enables rotation handle
    mxVertexHandler.prototype.rotationEnabled = true;

    // Enables guides
    mxGraphHandler.prototype.guidesEnabled = true;

    // Alt disables guides
    mxGuide.prototype.isEnabledForEvent = function (evt) {
        return !mxEvent.isAltDown(evt);
    };

    // Enables snapping waypoints to terminals
    mxEdgeHandler.prototype.snapToTerminals = true;

    // Defines an icon for creating new connections in the connection handler.
    // This will automatically disable the highlighting of the source vertex.
    mxConnectionHandler.prototype.connectImage = new mxImage('images/connector.gif', 16, 16);

    // Enables connections in the graph and disables
    // reset of zoom and translate on root change
    // (ie. switch between XML and graphical mode).
    editor.graph.setConnectable(true);

    //******LABELS!!!!!!!!*******
    editor.graph.getLabel = function (cell) {
        return cell.value.label ? cell.value.label : cell.value;
    };


    editor.createProperties = function (cell) {
        var model = this.graph.getModel();
        var value = model.getValue(cell);
        //console.log(value);

        if (/*mxUtils.isNode(value) ||*/ value.entity) {
            // Creates a form for the user object inside
            // the cell
            var form = new mxForm('properties');

            // Adds a readonly field for the cell id
            var id = form.addText('ID', value.entity/*cell.getId()*/);
            id.setAttribute('readonly', 'true');

            var geo = null;
            var yField = null;
            var xField = null;
            var widthField = null;
            var heightField = null;

            var xhr = new XMLHttpRequest();
            args = "id=" + value.entity;//cell.getId();
            xhr.open('GET', "node/properties?" + args, false);
            xhr.send();
            var attrs = JSON.parse(xhr.responseText);
            var texts = [];

            //// Creates textareas for each attribute of the
            //// user object within the cell

            for (var i = 0; i < attrs.length; i++) {
                // Creates a textarea with more lines for
                // the cell label
                var val = attrs[i].Value;
                texts[i] = form.addText(attrs[i].Name, val, 2);
            }

            // Adds an OK and Cancel button to the dialog
            // contents and implements the respective
            // actions below

            // Defines the function to be executed when the
            // OK button is pressed in the dialog
            var okFunction = mxUtils.bind(this, function () {
                // Hides the dialog
                this.hideProperties();

                // Supports undo for the changes on the underlying
                // XML structure / XML node attribute changes.
                model.beginUpdate();
                try {

                    // Creates an undoable change for each
                    // attribute and executes it using the
                    // model, which will also make the change
                    // part of the current transaction
                    for (var i = 0; i < attrs.length; i++) {
                        var edit = new mxCellAttributeChange(
                            cell, attrs[i].nodeName,
                            texts[i].value);
                        model.execute(edit);
                    }

                    // Checks if the graph wants cells to
                    // be automatically sized and updates
                    // the size as an undoable step if
                    // the feature is enabled
                    if (this.graph.isAutoSizeCell(cell)) {
                        this.graph.updateCellSize(cell);
                    }
                }
                finally {
                    model.endUpdate();
                }
            });
            // Defines the function to be executed when the
            // Cancel button is pressed in the dialog
            var cancelFunction = mxUtils.bind(this, function () {
                // Hides the dialog
                this.hideProperties();
            });

            form.addButtons(okFunction, cancelFunction);

            return form.table;
        }

        return null;
    };



    // Defines a new action to switch between
    // XML and graphical display
    var diagNode = document.getElementById('userpage0');//('diagramPresentation');
    var diagTCNode = document.getElementById('diagramPresentation');
    var textNode = document.getElementById('xml');
    var graphNode = editor.graph.container;
    var sourceInput = document.getElementById('source');
    sourceInput.checked = false;

    var showSaved = function (editor, diaghtml) {
        //Editor.execute("clearDiagram");
        dID = document.getElementById("DiagramId");
        diagramXml = GetText(dID.value, "DiagramXml");
        drawGraph(diagramXml);

        var diagText = graphNode.innerHTML;
        var newText = diagText.replace(new RegExp("c3d9ff-1-white", 'g'), "c3d9ff-1-black").replace(new RegExp("cdeb8b-1-white", 'g'), "cdeb8b-1-black")

        if (newText == "")
            newText = diaghtml;
        diagNode.innerHTML = newText;//diaghtml;

        diagTCNode.style.display = '';
        graphNode.style.display = 'none';
    }

    editor.addAction('showSavedDiagram', showSaved);

    var clearDiag = function (editor) {
        //console.log('clearDiag');
        //diagNode.innerHTML = "";
        diagTCNode.style.display = 'none';
        graphNode.style.display = '';

        if (textNode.value != textNode.originalValue) {
            var doc = mxUtils.parseXml("<mxGraphModel><root><mxCell id=\"0\"/><mxCell id=\"1\" parent=\"0\"/></root></mxGraphModel>");
            var dec = new mxCodec(doc);
            dec.decode(doc.documentElement, editor.graph.getModel());
        }

        textNode.originalValue = null;

        // Makes sure nothing is selected in IE
        if (mxClient.IS_IE) {
            mxUtils.clearSelection();
        }

        textNode.style.display = 'none';

        // Moves the focus back to the graph
        editor.graph.container.focus();
    }

    editor.addAction('clearDiagram', clearDiag);

    var funct = function (editor) {
        if (sourceInput.checked) {
            graphNode.style.display = 'none';
            textNode.style.display = 'inline';

            var enc = new mxCodec();
            var node = enc.encode(editor.graph.getModel());

            textNode.value = mxUtils.getPrettyXml(node);
            textNode.originalValue = textNode.value;
            textNode.focus();
        }
        else {
            graphNode.style.display = '';

            if (textNode.value != textNode.originalValue) {
                var doc = mxUtils.parseXml(textNode.value);
                var dec = new mxCodec(doc);
                dec.decode(doc.documentElement, editor.graph.getModel());
            }

            textNode.originalValue = null;

            // Makes sure nothing is selected in IE
            if (mxClient.IS_IE) {
                mxUtils.clearSelection();
            }

            textNode.style.display = 'none';

            // Moves the focus back to the graph
            editor.graph.container.focus();
        }
    };

    editor.addAction('switchView', funct);

    // Defines a new action to switch between
    // XML and graphical display
    mxEvent.addListener(sourceInput, 'click', function () {
        editor.execute('switchView');
    });

    var insNode = function (editor, id, text, x, y) {
        editor.graph.getModel().beginUpdate();
        try {
            var vObj = new Object();
            vObj.entity = id;
            vObj.label = text;
            var v1 = editor.graph.insertVertex(null, null/*id*/, vObj, x, y, 120, 25);
        }
        finally {
            editor.graph.getModel().endUpdate();
        }
    }

    editor.addAction('insertNode', insNode);

    var initGraph = function (editor, xml) {
        editor.graph.getModel().beginUpdate();
        try {
            var doc = mxUtils.parseXml(xml);
            var codec = new mxCodec(doc);
            codec.decode(doc.documentElement, editor.graph.getModel());
        }
        finally {
            editor.graph.getModel().endUpdate();
        }
    }

    editor.addAction('initG', initGraph);

}
