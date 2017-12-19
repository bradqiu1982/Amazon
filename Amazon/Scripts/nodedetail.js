var NodeDetail = function(){
    var nd_show = function(){
        var _jm = null;
        var options = {
            container: 'wfn-jc-detai',
            theme:'greensea',
            editable:true,
            mode: 'side',
            view:{
               hmargin:10,
               vmargin:10,
               line_width:2,
               line_color:'#555'
           },
           layout:{
               hspace:20,
               vspace:10,
               pspace:0
           },
        }
        var wfe_id = $('#wfn-jc-detai').attr('data-id');
         $.post('/WorkFlow/WorkFlowByID',
         {
             wfe_id: wfe_id
         }, function(output){
            var mind = {
                "format":"node_array",
                "data": output
            };
            _jm = jsMind.show(options, mind);
         })
    }
    return {
        init: function(){
            nd_show();
        }
    }
}();