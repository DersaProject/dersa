Vue.component('fltdiv', {

methods:{
    DoSelect: function(event){
        objFilter.SelectByIndex(this.app_index);
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

    template: '<div v-if="is_visible" v-on:click="DoSelect" v-bind:class="{ sel: is_selected }" v-bind:id="id" v-bind:style="styleD"><div style="position:relative;top:-15px;left:0px;color:black;font-size:8pt;">{{ this.displayed_name }}</div><i>{{ this.left }} : {{ this.top }}</i></div>'
})


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//if (!keydown_assigned) {
//	$(document).keydown(function (e) { objFilter.ProcessKey(e.key, e.altKey, e.ctrlKey, e.shiftKey) });
//	keydown_assigned = true;
//}
	

//var filterFormContainer = document.createElement('div');
//filterFormContainer.id = "filterForm";
//filterFormContainer.innerHTML = '<fltdiv v-for="ctrl in ctrls" v-bind:displayed_name="ctrl.displayed_name" v-bind:app_index="ctrl.app_index" v-bind:left="ctrl.left" v-bind:top="ctrl.top" v-bind:width="ctrl.width" v-bind:height="ctrl.height" v-bind:id="ctrl.id" v-bind:key="ctrl.id"_selected="ctrl.is_selected"></fltdiv>';

//filterFormContainer = document.getElementById('filterForm');
//var wnd = new mxWindow('Filter form', filterFormContainer, 120, +$(window).scrollTop() + 100, 600, 600, false, true);
//    wnd.setClosable(true);
//    wnd.setResizable(true);
//    wnd.setVisible(true);
//    wnd.addListener("close", function () {
//		objFilter.ProcessKey = null;
//        objFilter = null;
//    });
    

var initArray = 
[
    { displayed_name: 'entity1', id: 'e1', app_index: 0, left: 98, top: 100, width: 100, height: 25, is_selected: false, is_visible: true },
    { displayed_name: 'entity2', id: 'e2', app_index: 1, left: 200, top: 0, width: 100, height: 25, is_selected: false, is_visible: true },
    { displayed_name: 'entity3', id: 'e3', app_index: 2, left: 302, top: 150, width: 100, height: 25, is_selected: false, is_visible: true }
];
objFilter = new Vue({
	el: '#filterForm', 
data: {
  selected_index: -1,
  ctrls: initArray
},
methods:{
	ProcessKey: function(key, altKey, ctrlKey, shiftKey){
		if(key == "Control")
			return;
        console.log(key);
	}
}
});

