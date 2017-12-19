var WorkFlow = function () {
    var wf_show = function(){
        $('#wf-list').autoComplete({
            minChars: 0,
            source: function(term, suggest){
                term = term.toLowerCase();
                $.post('/WorkFlowTemplate/WorkFlowTemplateKeyValueArray', { }, function (output) {
                    var choices = output;
                    var suggestions = [];
                    for (i=0;i<choices.length;i++){
                        if (~(choices[i][0]+' '+choices[i][1]).toLowerCase().indexOf(term))
                            suggestions.push(choices[i]);
                    }
                    suggest(suggestions);
                 })
            },
            renderItem: function (item, search){
                search = search.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
                var re = new RegExp("(" + search.split(' ').join('|') + ")", "gi");
                return '<div class="autocomplete-suggestion" data-value="'+item[1]+'" data-id="'+item[0]+'" data-val="'+search+'"> '+item[1].replace(re, "<b>$1</b>")+'</div>';
            },
            onSelect: function(e, term, item){
                $('#wf-list').val(item.data('value'));
                $('#wf-list').attr('data-id', item.data('id'));
            }
        });
        $('body').on('click', '#btn-add-wfe', function(){
            var wf_id = $('#wf-list').attr('data-id');
            var wfe_name = $.trim($('#wfe-name').val());
            if(wf_id == undefined){
                alert('Please choose workflow template');
                return false;
            }
            if(wfe_name == ''){
                alert('Please input your workflow name');
                return false;
            }
            $.post('/WorkFlow/CreateNewWorkFlow',
             {
                 wf_id: wf_id,
                 wfe_name: wfe_name
             }, function(output){
                 if(output.success){
                    window.location.reload();
                 }
                 else{
                     alert(output.msg);
                 }
             });
        });
        $.each($('.jc_view'), function(){
            var options = {
                container: $(this).attr('id'),
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
             var wfe_id = $(this).attr('data-id');
             $.post('/WorkFlow/WorkFlowStatusByID',
             {
                 wfe_id: wfe_id
             }, function(output){
                var mind = {
                    "format":"node_array",
                    "data":output
                };
                var tempjm = jsMind.show(options, mind);
                tempjm.view.setZoom(0.8);
             })
        })
        var _jm = {};
        $.each($('.jc_detail'), function(){
            var options = {
                container: $(this).attr('id'),
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
            
            var wfe_id = $(this).attr('data-id');
            $.post('/WorkFlow/WorkFlowByID',
            {
                 wfe_id: wfe_id
            }, function(output){
                var mind = {
                    "format":"node_array",
                    "data":output
                };
                var _jm_tmp = jsMind.show(options, mind);
                _jm_tmp.view.setZoom(0.8);
                _jm[wfe_id] = _jm_tmp;
             })
        })

        $('body').on('click', '.view-wfe-op', function(){
            var wfe_id = $(this).parent().attr('data-id');
            window.location.reload();
            // window.location.href = '/WorkFlow/Show?id='+wfe_id;
        })

        //delete workflow
        $('body').on('click', '.del-wfe-op', function(){
            var wfe_id = $(this).parent().attr('data-id');
            $.post('/WorkFlow/RemoveWorkFlow',
             {
                 wfe_id: wfe_id
             }, function(output){
                 if(output.success){
                     window.location.reload();
                 }
             })
        })
        $('body').on('click', '.d-span-zoom-in', function(){
            var wfe_id = $(this).parent().find('.jc_detail').attr('data-id');
            if (_jm[wfe_id].view.zoomIn()) {
                $('.span-zoom').css('color', '#7f7f7f');
            } else {
                $(this).css('color', '#cfcfcf');
            };
        })
        $('body').on('click', '.span-zoom-out', function(){
            var wfe_id = $(this).parent().find('.jc_detail').attr('data-id');
            if (_jm[wfe_id].view.zoomOut()) {
                $('.span-zoom').css('color', '#7f7f7f');
            } else {
                $(this).css('color', '#cfcfcf');
            };
        })
    }
    return {
        init: function(){
            wf_show();
        }
    }
}();