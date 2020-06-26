Vue.component('tabtip', {
    data: function () {
        return {
            tipwidth: 80
        };
    },
    computed: {
        styleTab: function () {
            return {
                position: 'absolute',
                borderTop: (this.is_selected ? '2px inset black' : '1px inset gray'),
                borderBottom: (this.is_selected ? 'none' : '2px inset black'),
                borderLeft: (this.is_selected ? '2px inset black' : '1px inset gray'),
                borderRight: (this.is_selected ? '2px inset black' : '1px inset gray'),
                zIndex: (this.is_selected ? '200' : '100'),
                color: (this.is_selected ? 'black' : 'gray'),
                //fontSize: '6pt',
                width: this.tipwidth + 'px',
                height: '20px',
                left: this.index * (this.tipwidth + 1) + 'px',
                top: '-40px'
            }
        }
    },
    props: ['displayed_name', 'id', 'index', 'is_selected'],
    template: '<div v-bind:style="styleTab" v-on:click="$emit(\'click\')"> {{ this.displayed_name }} </div>'
});

Vue.component('tabpage', {

    computed: {
        stylePage: function () {
            return {
                display: (this.is_selected ? 'block' : 'none'),
                //background: '#e0e0e0',
                width: this.$root.width + 'px',
                height: this.$root.height + 'px'
            }
        },
        stylePageAndTab: function () {
            return {
                position: 'absolute',
                left: this.$root.left + 'px',
                top: this.$root.top + 'px',
                //width: this.$root.width + 'px',
                //height: this.$root.height + 'px'
            }
        }
    },
    methods: {
        Select: function () {
            this.$root.UnselectItems();
            this.is_selected = true;
            this.$root.selected_index = this.index;
        }
    },
    props: ['displayed_name', 'id', 'index', 'is_selected', 'is_visible', 'width', 'height'],
    template: '<div v-bind:id="id" v-bind:style="stylePageAndTab">'
        + '<tabtip v-bind:is_selected="is_selected" v-bind:displayed_name="displayed_name" v-bind:index="index" v-on:click="Select"></tabtip>'
        + '<div v-bind:style="stylePage"><div v-bind:id="\'userpage\'+this.index"></div></div></div>'
});

    (function ($) {
        $.fn.dersaTabControl = function (position, size, initArray) {
            var innerComponents = $("<tabpage></tabpage>");
            innerComponents.attr("v-for", "item in pages");
            innerComponents.attr("v-bind:displayed_name", "item.displayed_name");
            innerComponents.attr("v-bind:index", "item.index");
            innerComponents.attr("v-bind:is_selected", "item.is_selected");
            innerComponents.attr("v-bind:id", "item.id");
            this.append(innerComponents);

            return new Vue({
                el: this.selector,
                data: {
                    selected_index: -1,
                    width: size.width,
                    height: size.height,
                    left: position.X,
                    top: position.Y,
                    pages: initArray
                },
                methods: {
                    UnselectItems() {
                        this.$children.forEach(function (item) {
                            item.is_selected = false;
                        });
                        this.selected_index = -1;
                    },
                    GetPage(pageindex) {
                        return $('#userpage' + pageindex);
                    }
                }
            });
        };
    })(jQuery);

