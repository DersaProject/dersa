Vue.component('drdiv', {
    mounted: function () {
        $('#' + this.id).draggable({
          stop: function(ev) {
            if(this.__vue__ && this.__vue__.app_index >= 0){
	            appDiagram.ctrls[this.__vue__.app_index].left = $('#'+this.id).position().left; 
	            appDiagram.ctrls[this.__vue__.app_index].top = $('#'+this.id).position().top; 
	    }
         }
	});
	$('#'+this.id).resizable({
          stop: function(ev) {
            if(this.__vue__ && this.__vue__.app_index >= 0){
				let w = $('#'+this.id).width();
				let h = $('#'+this.id).height();
	            appDiagram.ctrls[this.__vue__.app_index].width = w;
	            appDiagram.ctrls[this.__vue__.app_index].height = h;
				appDiagram.DisplayDimensions(w, h);
			}
        }
	});
},

methods:{
    DoSelect: function(event){
        appDiagram.SelectByIndex(this.app_index);
    }
},

computed: {
  styleD: function () {
    return {
        position: 'absolute',
        background: 'blue',
        color: 'yellow',
        fontSize: '6pt',
        width: this.width + 'px',
        height: this.height + 'px',
        left: this.left + 'px',
        top: this.top + 'px'
    }
  }
},
props: ['displayed_name','id', 'app_index', 'left', 'top', 'width', 'height', 'is_selected', 'is_visible'],

    template: '<div v-if="is_visible" class="drcomp" v-on:click="DoSelect" v-bind:class="{ sel: is_selected }" v-bind:id="id" v-bind:style="styleD"><div style="position:relative;top:-15px;left:0px;color:black;font-size:8pt;">{{ this.displayed_name }}</div><i>{{ this.left }} : {{ this.top }}</i></div>'
})

function dragStart(ev) {
   ev.dataTransfer.effectAllowed='move';
   ev.dataTransfer.setData("Text", ev.target.getAttribute('id'));
   ev.dataTransfer.setDragImage(ev.target,10,10);
   return true;
}
function dragEnter(ev) {
   event.preventDefault();
   return false;
}
function dragOver(ev) {
    if(ev.target.id == "diag")
		event.preventDefault();
}
function dragDrop(ev) {
    //console.log(ev);
    let xhr = new XMLHttpRequest();
    xhr.open('GET', '/Node/GetCoords?id=' + ev.data.id, false);
    xhr.send();
    console.log(xhr.responseText);
    let coordsObj = new Object();
    let coordsObjFromServer = xhr.responseText == "" ? new Object() : JSON.parse(xhr.responseText);
    coordsObj.X = coordsObjFromServer.X ? coordsObjFromServer.X : ev.offsetX;
    coordsObj.Y = coordsObjFromServer.Y ? coordsObjFromServer.Y : ev.offsetY;
    coordsObj.X = coordsObjFromServer.X ? coordsObjFromServer.X : ev.offsetX;
    coordsObj.Width = coordsObjFromServer.Width ? coordsObjFromServer.Width : 100;
    coordsObj.Height = coordsObjFromServer.Height ? coordsObjFromServer.Height : 25;
    
	let N = appDiagram.ctrls.length;
    appDiagram.ctrls.push({ displayed_name: ev.data.name, id: 'n_' + ev.data.id + '_' + N, app_index: N, left: coordsObj.X, top: coordsObj.Y, width: coordsObj.Width, height: coordsObj.Height, is_selected: false, is_visible: true });
	ev.stopPropagation();
	return false;
}

if (!keydown_assigned) {
    $(document).keydown(function (e) { if (appDiagram) appDiagram.ProcessKey(e.key, e.altKey, e.ctrlKey, e.shiftKey) });
    keydown_assigned = true;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function initDiag(diagramId){

var diagContainer = document.createElement('div');
diagContainer.id = "diag";
diagContainer.innerHTML = '<drdiv v-for="ctrl in ctrls" v-bind:displayed_name="ctrl.displayed_name" v-bind:app_index="ctrl.app_index" v-bind:left="ctrl.left" v-bind:top="ctrl.top" v-bind:width="ctrl.width" v-bind:height="ctrl.height" v-bind:id="ctrl.id" v-bind:key="ctrl.id"_selected="ctrl.is_selected" v-bind:is_visible="ctrl.is_visible"	></drdiv><div class="statusbar">{{status_message}}</div>';
diagContainer.setAttribute("ondrop", "return dragDrop(event)"); 
document.addEventListener("dragover", dragOver); 

//var diagContainer = document.getElementById('diag');
    var wnd = new mxWindow('Diagram', diagContainer, 120, +$(window).scrollTop() + 100, 600, 600, false, true);
    wnd.setClosable(true);
    wnd.setResizable(true);
    wnd.setVisible(true);
    wnd.addListener("close", function () {
        if (confirm("Сохранить диаграмму?")) {
            var body = new Object();
            body.id = diagramId;
            body.jsonObject = JSON.stringify(appDiagram.ctrls);
            let xhr = new XMLHttpRequest();
            xhr.open('POST', '/Diagram/SaveDiagram', false);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.send(JSON.stringify(body));
        }
        appDiagram = null;
    });
    

var xhr=new XMLHttpRequest();
xhr.open('GET', '/Diagram/GetJson?id=' + diagramId, false);
xhr.send();
var initArray = JSON.parse(xhr.responseText);

appDiagram = new Vue({ el: '#diag', 
data: {
  selected_index: -1,
  status_message: 'hello',
  ctrls: initArray
},
methods:{
	ProcessKey: function(key, altKey, ctrlKey, shiftKey){
		if(key == "Control")
			return;
        console.log(key);
		if(key == "Escape"){
			this.SelectByIndex(-1);
		}
		else if(key == "Delete"){
			if(confirm('Удалить?')){
				this.ctrls[this.selected_index].is_visible = false;
				this.ctrls[this.selected_index].is_selected = false;
				this.selected_index = -1;
			}
		}
		else if(key == "Tab" && ctrlKey){
			this.SelectNext();
		}
		else if(key == "ArrowUp" && ctrlKey && this.selected_index >= 0){
			
			this.MoveNode(this.selected_index, 0, -1);
		}
		else if(key == "ArrowDown" && ctrlKey && this.selected_index >= 0){
			
			this.MoveNode(this.selected_index, 0, 1);
		}
		else if(key == "ArrowLeft" && ctrlKey && this.selected_index >= 0){
			
			this.MoveNode(this.selected_index, -1, 0);
		}
		else if(key == "ArrowRight" && ctrlKey && this.selected_index >= 0){
			
			this.MoveNode(this.selected_index, 1, 0);
		}
		else if(key == "ArrowUp" && shiftKey && this.selected_index >= 0){
			
			this.ResizeNode(this.selected_index, 0, -1);
		}
		else if(key == "ArrowDown" && shiftKey && this.selected_index >= 0){
			
			this.ResizeNode(this.selected_index, 0, 1);
		}
		else if(key == "ArrowLeft" && shiftKey && this.selected_index >= 0){
			
			this.ResizeNode(this.selected_index, -1, 0);
		}
		else if(key == "ArrowRight" && shiftKey && this.selected_index >= 0){
			
			this.ResizeNode(this.selected_index, 1, 0);
		}
	},
	SelectNext: function(recursionLevel){
		if(this.selected_index < 0)
			return;
		if(!recursionLevel)
			recursionLevel = 1;
		if(recursionLevel > this.ctrls.length)
			return;
		this.ctrls[this.selected_index].is_selected = false;
		this.selected_index++;
		if(this.selected_index >= this.ctrls.length)
			this.selected_index = 0;
		if(!this.ctrls[this.selected_index].is_visible)
			this.SelectNext(++recursionLevel);
		this.ctrls[this.selected_index].is_selected = true;
		//this.$forceUpdate();
	},
	SelectByIndex: function(index){
		this.DisplayStatus('');
		this.selected_index = -1;
		for(let i=0; i < this.ctrls.length;i++)
		{
			this.ctrls[i].is_selected = false;
		}
		if(index >= 0){
			this.ctrls[index].is_selected = true;
			this.DisplayDimensions(this.ctrls[index].width, this.ctrls[index].height);
			this.selected_index = index;
		}
		//this.$forceUpdate(); необходимость этого вызова пропала, когда в методе стали изменять свойство самого app (до того при выборе динамически добавленных компонентов не происходила перерисовка)
	},
	MoveNode: function(index, diffX, diffY){
			this.ctrls[index].left += diffX;
			this.ctrls[index].top += diffY;
	},
	ResizeNode: function(index, diffX, diffY){
			this.ctrls[index].width += diffX;
			this.ctrls[index].height += diffY;
			this.DisplayDimensions(this.ctrls[index].width, this.ctrls[index].height);
	},
	DisplayDimensions(width, height)
	{
		this.status_message = 'w:' + width + ' ' + 'h:' + height;
	},
	DisplayStatus(status)
	{
		this.status_message = status;
	}
}
});

}