new Vue({
    el: "#hubs",
    data: {
        object: [],
        username: '',
        password: '',
        arm0: '',
        
        hasError: false,
        isValid: false,
    },
    


    beforeCreate:
    function () {
        // console.log(112312312321123123);
        this.$http.get('/workplace/list').then(response => {
            var body = response.body;
            if (body.response_status !== -1) {
                response_body = JSON.parse(body.response_body);
               
                console.log("this.obj",this.object);
                var arm = [];
                for (var i = 0; i < response_body.length; i++) {

                    var name = response_body[i].__name;
                    //console.log(response_body[i]);
                    var isLeft = i % 2 == 0;
                        if (response_body[i].__current_cuks_user) {
                            var isActive = false;
                            var isUsed = true;
                            var used_text = "Used by " + response_body[i].__user_full_name;
                        } else {
                            var isActive = true;
                            var isUsed = false;
                            var used_text = ''
                        }
                    
                        arm.push({name:name, isLeft:isLeft, isActive:isActive, isUsed:isUsed, used_text:used_text});
                    this.arm0 = response_body[0].__name;

                }
               
                this.object = arm;

            }
        }, response => {

        });
    }

});
