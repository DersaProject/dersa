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
        left: this.left + 'px',
        top: this.top + 'px',
    }
  },
  styleI: function () {
    return {
        width: this.width + 'px',
        height: this.height + 'px',
    }
  }
},
props: ['caption', 'data_field', 'filter_value', 'id', 'app_index', 'left', 'top', 'width', 'height', 'is_selected', 'is_visible'],

template: '<div v-bind:id="id" v-bind:style="styleD"><input type="text"  v-bind:style="styleI" v-model="filter_value" /><div style="position:relative;top:-40px;left:0px;color:black;font-size:10pt;">{{ this.caption ? this.caption : this.data_field }}</div></div>'
});


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////    

var initArray = 
[
    { caption: 'entity1', data_field: 'f1', filter_value: 'v1', id: 'e1', app_index: 0, left: 98, top: 100, width: 100, height: 25, is_selected: false, is_visible: true },
    { caption: null, data_field: 'f2', filter_value: 'v2', id: 'e2', app_index: 1, left: 200, top: 30, width: 150, height: 25, is_selected: false, is_visible: true },
    { caption: 'entity3', data_field: 'f3', filter_value: 'v3', id: 'e3', app_index: 2, left: 302, top: 150, width: 60, height: 25, is_selected: false, is_visible: true }
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
	},
	GetFilter: function(){
		let result = new Array();
		this.$children.forEach(function(item, i, arr) {
			result[i] = item.filter_value;
		});
		return JSON.stringify(result);
	}
}
});
