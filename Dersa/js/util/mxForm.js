/**
 * Copyright (c) 2006-2015, JGraph Ltd
 * Copyright (c) 2006-2015, Gaudenz Alder
 */
/**
 * Class: mxForm
 * 
 * A simple class for creating HTML forms.
 * 
 * Constructor: mxForm
 * 
 * Creates a HTML table using the specified classname.
 */
function mxForm(className, owner)
{
	this.table = document.createElement('table');
	this.table.className = className;
	this.body = document.createElement('tbody');
	
	this.table.appendChild(this.body);

    this.window = owner;

};

mxForm.prototype.window = null;
mxForm.prototype.Result = null;

/**
 * Variable: table
 * 
 * Holds the DOM node that represents the table.
 */
mxForm.prototype.table = null;

/**
 * Variable: body
 * 
 * Holds the DOM node that represents the tbody (table body). New rows
 * can be added to this object using DOM API.
 */
mxForm.prototype.body = false;

/**
 * Function: getTable
 * 
 * Returns the table that contains this form.
 */
mxForm.prototype.getTable = function()
{
	return this.table;
};

/**
 * Function: addButtons
 * 
 * Helper method to add an OK and Cancel button using the respective
 * functions.
 */
mxForm.prototype.addButtons = function(okFunct, cancelFunct)
{
	var tr = document.createElement('tr');
	var td = document.createElement('td');
	tr.appendChild(td);
	td = document.createElement('td');

	// Adds the ok button
	var button = document.createElement('button');
	mxUtils.write(button, mxResources.get('ok') || 'OK');
	td.appendChild(button);

	mxEvent.addListener(button, 'click', function()
	{
		okFunct();
	});
	
	// Adds the cancel button
	button = document.createElement('button');
	mxUtils.write(button, mxResources.get('cancel') || 'Cancel');
	td.appendChild(button);
	
	mxEvent.addListener(button, 'click', function()
	{
		cancelFunct();
	});
	
	tr.appendChild(td);
	this.body.appendChild(tr);
};

/**
 * Function: addText
 * 
 * Adds an input for the given name, type and value and returns it.
 */
mxForm.prototype.addText = function(name, value, type, childFormAttrs, ParentForm, Width)
{
    var input = document.createElement('input');
    var owner = this;

    mxEvent.addListener(input, 'keydown', function (event) {
        if (event.key == "Enter") {
                if (owner.okClick)
                    owner.okClick();
                else
                    alert('okClick undefined');
            return;
        }
        if (event.key == "Escape") {
            if (owner.cancelclick)
                owner.cancelclick();
            else
                alert('cancelclick undefined');
            return;
        }
    });

    input.setAttribute('type', type || 'text');
    if(Width)
        input.setAttribute('style', 'width:' + Width + "px");
	if (childFormAttrs)
	{
	    input.value = childFormAttrs.DisplayValue;
	    var inputForValue = document.createElement('input');
	    inputForValue.setAttribute('type', 'hidden');
        inputForValue.value = value;
        if (type == 'button')
        {
            if (childFormAttrs.InfoLink == "")
                input.setAttribute('onclick', childFormAttrs.OnClick);
            else
                mxEvent.addListener(input, 'click', function () {
                    ChildForm(name, inputForValue, childFormAttrs.InfoLink, childFormAttrs.SaveLink, childFormAttrs.Height, childFormAttrs.Width, childFormAttrs.ActionAfterExec, ParentForm, childFormAttrs.CancelLink);
                });
        }
        this.addField(name, input);
	    return this.addField('', inputForValue);
	}
    input.value = value;
    if (type == 'hidden')
        name = '';
	return this.addField(name, input);
};

function CreateProperties(form, attrs, urlOK, ActionAfterExec, ClassName, callBackFunction, parentForm, urlCancel) {
    var texts = [];
    for (var i = 0; i < attrs.length; i++) {
        var fControl = null;
        if (attrs[i].ControlType == "custom_button") {
            var prev_i = i - 1;
            fControl = texts[i] = form.addButton(attrs[i], (text) => {
                texts[prev_i].value = text;
            });
        }
        else if (attrs[i].ControlType == "textarea")
            fControl = texts[i] = form.addTextarea(attrs[i]);
        else if (attrs[i].ControlType == "combo") {
            var items = new Array();
            if (attrs[i].Value)
                items = JSON.parse(attrs[i].Value);
            fControl = texts[i] = form.addCombo(attrs[i].Name, false, 1, items);
        }
        else
            fControl = texts[i] = form.addText(attrs[i].Name, attrs[i].Value, attrs[i].ControlType, attrs[i].ChildFormAttrs, form, attrs[i].Width);
        if (attrs[i].ReadOnly)
            fControl.setAttribute('readonly', 'true');
    }

    // Adds an OK and Cancel button to the dialog
    // contents and implements the respective
    // actions below

    // Defines the function to be executed when the
    // OK button is pressed in the dialog
    var okFunction = mxUtils.bind(this, function () {
        if (form.CheckAttrs && !form.CheckAttrs(attrs, texts))
                return;
        // Hides the dialog
        form.window.setVisible(false);
        var sendResult = false;
        var results = [];
        var j = 0;
        for (var i = 0; i < attrs.length; i++) {
            if (attrs[i].Skip)
                continue;
            var iValue = attrs[i].Value;
            if (iValue == null)
                iValue = "";
            if ((iValue != texts[i].value && !attrs[i].ReadOnly) || attrs[i].WriteUnchanged)
            {
                saveText = texts[i].value.replace(new RegExp("<", 'g'), "$lt$").replace(new RegExp(">", 'g'), "$gt$");
                results[j++] = { Name: attrs[i].Name, Value: saveText};
                if (form.Result) {
                    form.Result.value = texts[i].value;
                }
                sendResult = true;
            }
        }
        if (sendResult) {
            if (urlOK) {
                if (ActionAfterExec == "goto")
                {
                    window.open(urlOK + "?json_params=" + encodeURIComponent(JSON.stringify(results)));
                    return;
                }
                if (ActionAfterExec == "goto_byId") {
                    var xhr = new XMLHttpRequest();
                    var body = "json_params=" + encodeURIComponent(JSON.stringify(results));
                    xhr.open('POST', urlOK, false);
                    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                    xhr.send(body);
                    window.open(xhr.responseText);
                    return;
                }
                var xhr = new XMLHttpRequest();
                var body = "json_params=" + encodeURIComponent(JSON.stringify(results));
                if (ClassName)
                    body += "&class_name=" + ClassName;
                xhr.open('POST', urlOK, false);
                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send(body);
                var actionObject = null;
                var resultAction = null;
                try {
                    actionObject = JSON.parse(xhr.responseText);
                    console.log(actionObject);
                    resultAction = actionObject.action;
                }
                catch (err) { }
                if (ActionAfterExec == "alert" && !resultAction) {
                    if (xhr.responseText != "") {
                        alert(xhr.responseText);
                    }
                }
                else if (ActionAfterExec == "close") {
                    if (callBackFunction) {
                        callBackFunction(parentForm);
                    }
                }
                else if (ActionAfterExec == "exec" || resultAction) {
                    if (actionObject && resultAction && actionObject.arg_name && actionObject.arg) {
                        var f = new Function(actionObject.arg_name, resultAction);
                        var arg = actionObject.arg;
                        arg = arg.constructor === Array ? arg : [arg];      //если передали массив - так и оставляем, а если одиночный объект - преобразуем в массив (нужно для передачи более одного аргумента)
                        var R = f.apply(this, arg);
                        if (ActionAfterExec == "alert")
                            alert(R);
                    }
                    else if (resultAction) {
                        try {
                            eval(resultAction);
                        }
                        catch (err) {
                            let errWin = window.open('', 'newwin', 'width=700,height=700,status=1,menubar=1');
                            errWin.document.open();
                            errWin.document.write(xhr.responseText);
                            errWin.document.close();
                        }
                     }
                }
                else {
                    console.log('ActionAfterExec = ' + ActionAfterExec);
                }
            }
        }

    });
    // Defines the function to be executed when the
    // Cancel button is pressed in the dialog
    var cancelFunction = mxUtils.bind(this, function () {
        // Hides the dialog
        form.window.setVisible(false);
        if(urlCancel){
            var xhr = new XMLHttpRequest();
            xhr.open('GET', urlCancel, false);
            xhr.send();
        }
    });

    form.addButtons(okFunction, cancelFunction);
    form.okClick = okFunction;
    form.cancelclick = cancelFunction;
    return form.table;
};

function CloseForm(form) {
    if (form.okClick)
        form.okClick();
}

function ChildForm(Name, Input, UrlIn, UrlOut, Height, Width, ActionAfterExec, ParentForm, UrlCancel) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', UrlIn, false);
    xhr.send();
    var formAttrs = JSON.parse(xhr.responseText);
    
    var form = new mxForm(Name);
    form.Result = Input;
    var Props = CreateProperties(form, formAttrs, UrlOut, ActionAfterExec, "", this.CloseForm, ParentForm, UrlCancel);
    var wnd = new mxWindow(Name,
        Props, 100, +$(window).scrollTop() + 200, Width || 250, Height || 400, false, true);
    form.window = wnd;
    wnd.setVisible(true);
}
/**
 * Function: addCheckbox
 * 
 * Adds a checkbox for the given name and value and returns the textfield.
 */
mxForm.prototype.addCheckbox = function(name, value)
{
	var input = document.createElement('input');
	
	input.setAttribute('type', 'checkbox');
	this.addField(name, input);

	// IE can only change the checked value if the input is inside the DOM
	if (value)
	{
		input.checked = true;
	}

	return input;
};

/**
 * Function: addButton
 * 
 * Adds a button for the given name and click handler and returns the button.
 */
mxForm.prototype.addButton = function (elem, clickHandler) {
    var input = document.createElement('input');

    input.setAttribute('type', 'file');
    //input.setAttribute('style', 'width:' + elem.Width + "px");
    input.setAttribute('style', 'width:200px');
    input.onchange = e => {
        var file = e.target.files[0];
        var reader = new FileReader();
        reader.readAsText(file, 'UTF-8');
        reader.onload = readerEvent => {
            var content = readerEvent.target.result; // this is the content!
            clickHandler(content);
        }
    }
    //input.addEventListener('click', clickHandler);
    var caption = elem.Name;
    //input.value = caption;
    return this.addField('', input);
};

/**
 * Function: addTextarea
 * 
 * Adds a textarea for the given name and value and returns the textarea.
 */
mxForm.prototype.addTextarea = function(elem)
{
    var input = document.createElement('textarea');

    var rows = elem.Height / 10;
	if (mxClient.IS_NS)
	{
		rows--;
	}
	
    input.setAttribute('style', 'width:' + elem.Width + "px");
    input.setAttribute('rows', rows || 2);
    input.value = elem.Value;

    var caption = elem.Name;
    //if (elem.NotNull)
    //    caption += " *";
    this.addField(caption, input);

    if (!elem.NoButtons) {
        var file_input = document.createElement('input');
        file_input.type = 'file';
        file_input.setAttribute('style', 'width:' + elem.Width + "px");
        //input.setAttribute('style', 'width:200px');
        file_input.onchange = e => {
            var file = e.target.files[0];
            var reader = new FileReader();
            reader.readAsText(file, 'windows-1251');
            reader.onload = readerEvent => {
                input.value = readerEvent.target.result; // this is the content!
            }
        }
        this.addField('', file_input);
    }

    return input;
};

/**
 * Function: addCombo
 * 
 * Adds a combo for the given name and returns the combo.
 */
mxForm.prototype.addCombo = function(name, isMultiSelect, size, items)
{
    var select = document.createElement('select');
    if(items)
    items.forEach(function (item) {
        var selectItem = document.createElement('option');
        selectItem.innerHTML = item.DisplayedValue;
        selectItem.setAttribute("value", item.Value);
        select.appendChild(selectItem);
    });
	
	if (size != null)
	{
		select.setAttribute('size', size);
	}
	
	if (isMultiSelect)
	{
		select.setAttribute('multiple', 'true');
	}
	
	return this.addField(name, select);
};

/**
 * Function: addOption
 * 
 * Adds an option for the given label to the specified combo.
 */
mxForm.prototype.addOption = function(combo, label, value, isSelected)
{
	var option = document.createElement('option');
	
	mxUtils.writeln(option, label);
	option.setAttribute('value', value);
	
	if (isSelected)
	{
		option.setAttribute('selected', isSelected);
	}
	
	combo.appendChild(option);
};

/**
 * Function: addField
 * 
 * Adds a new row with the name and the input field in two columns and
 * returns the given input.
 */
mxForm.prototype.addField = function(name, input)
{
    if (this.SingleColumn)
        return this.addFieldSingleColumn(name, input);

	var tr = document.createElement('tr');
	var td = document.createElement('td');
	mxUtils.write(td, name);
	tr.appendChild(td);
	
	td = document.createElement('td');
	td.appendChild(input);
	tr.appendChild(td);
	this.body.appendChild(tr);
	
	return input;
};

/**
 * Function: addField
 * 
 * Adds 2 new rows with the name and the input field in two columns (first is eempty) and
 * returns the given input.
 */
mxForm.prototype.addFieldSingleColumn = function (name, input) {
    var tr = document.createElement('tr');
    var td = document.createElement('td');
    mxUtils.write(td, '');
    tr.appendChild(td);
    td = document.createElement('td');
    mxUtils.write(td, name);
    tr.appendChild(td);
    this.body.appendChild(tr);

    tr = document.createElement('tr');
    td = document.createElement('td');
    mxUtils.write(td, '');
    tr.appendChild(td);
    td = document.createElement('td');
    td.appendChild(input);
    tr.appendChild(td);
    this.body.appendChild(tr);

    return input;
};

/**
 * Function: addTexButton
 * 
 * Helper method to add button for edit Textarea
 */
mxForm.prototype.addTexButton = function (textContainer) {
    console.log('addTexButton');

    //var tr = document.createElement('tr');
    //var td = document.createElement('td');
    //tr.appendChild(td);
    //td = document.createElement('td');

    //// Adds the ok button
    //var button = document.createElement('button');
    //mxUtils.write(button, '...');
    //td.appendChild(button);

    //mxEvent.addListener(button, 'click', function () {
    //    alert('Edit text');
    //});

    //tr.appendChild(td);
    //this.body.appendChild(tr);
};
