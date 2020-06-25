Vue.component('tabtip', {
    computed: {
        styleTab: function () {
            return {
                position: 'absolute',
                background: 'cyan',
                color: 'red',
                fontSize: '6pt',
                width: this.tipwidth + 'px',
                height: '20px',
                left: this.index * this.tipwidth + 'px',
                top: '-30px'
            }
        },
        tipwidth: {
            get: function () {
                return 60;
            }
        }
    },
    props: ['displayed_name', 'id', 'index', 'is_selected'],
    template: '<div v-bind:style="styleTab" v-on:click="$emit(\'click\', index)"> {{ this.displayed_name }} selected = {{ this.is_selected }} </div>'
});

Vue.component('tabpage', {

methods:{
    Select: function () {
        this.$root.Select(this.index);
        alert('selected ' + this.index);
    }
},

props: ['displayed_name','id', 'index', 'is_selected', 'is_visible'],

    template: '<div v-bind:id="id" style="position:relative; left:0px; top: 20px; width: 400px; height: 200px"><tabtip v-bind:is_selected="is_selected" v-bind:displayed_name="displayed_name" v-bind:index="index" v-on:click="Select"></tabtip><i>{{ this.index }}</i></div>'
})


