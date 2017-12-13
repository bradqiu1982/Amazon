var WorkFlow = function(){
    var wf_show = function(){
        $('#wf-list').autoComplete({
            minChars: 0,
            source: function(term, suggest){
                term = term.toLowerCase();
                // $.post('url', {data}, function (output){
                    var choices = [["1","WorkFlow"], ["2","WorkFlow1"], ["3","WorkFlow2"], ["4","WorkFlow3"], ["5","WorkFlow4"]];
                    var suggestions = [];
                    for (i=0;i<choices.length;i++){
                        if (~(choices[i][0]+' '+choices[i][1]).toLowerCase().indexOf(term))
                            suggestions.push(choices[i]);
                    }
                    suggest(suggestions);
                // }
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
            // $.post('/',
            // {
            //     wf_id: wf_id,
            //     wfe_name: wfe_name
            // }, function(output){
            //     if(output.success){
                    window.location.reload();
            //     }
            //     else{
            //         alert('Failed');
            //     }
            // });
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
            // var wfe_id = $(this).attr('data-id');
            // $.post('/',
            // {
            //     wfe_id: wfe_id
            // }, function(output){
                var mind = {
                    "format":"node_array",
                    "data":[
                        {"id":"sub1", "isroot":true, "topic":"sub1"},
                        {"id":"sub11", "parentid":"sub1", "topic":"sub11", "line-color": "#aaa", "status":"0"},
                        {"id":"sub12", "parentid":"sub1", "topic":"sub12", "line-color": "#aaa", "status":"0"},
                        {"id":"sub13", "parentid":"sub1", "topic":"sub13", "line-color": "#aaa", "status":"0"},
                    ]
                };
                jsMind.show(options, mind);
            // })
        })
        var _jm = [];
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
            // var wfe_id = $(this).attr('data-id');
            // $.post('/',
            // {
            //     wfe_id: wfe_id
            // }, function(output){
                var mind = {
                    "format":"node_array",
                    "data":[
                        {"id":"root", "isroot":true, "topic":"jsMind"},

                        {"id":"sub1", "parentid":"root", "topic":"sub1"},
                        {"id":"sub11", "parentid":"sub1", "topic":"sub11", "line-color": "#aaa", "status":"0"},
                        {"id":"sub12", "parentid":"sub1", "topic":"sub12", "line-color": "#aaa", "status":"0"},
                        {"id":"sub13", "parentid":"sub1", "topic":"sub13", "line-color": "#aaa", "status":"0"},
                    ]
                };
                var _jm_tmp = jsMind.show(options, mind);
                _jm_tmp.view.setZoom(0.8);
                _jm.push(_jm_tmp);
            // })
        })

        $('body').on('click', '.view-wfe-op', function(){
            var wfe_id = $(this).parent().attr('data-id');
            window.location.reload();
            // window.location.href = '/WorkFlow/Show?id='+wfe_id;
        })

        //delete workflow
        $('body').on('click', '.del-wfe-op', function(){
            var wfe_id = $(this).parent().attr('data-id');
            // $.post('/', 
            // {
            //     wfe_id: wfe_id
            // }, function(output){
            //     if(output.success){
                $(this).parent().parent().parent().remove();
            //     }
            // })
        })
        $('body').on('click', '.d-span-zoom-in', function(){
            var idx = $(this).parent().find('.jc_detail').attr('id').split('_')[2];
            if (_jm[idx].view.zoomIn()) {
                $('.span-zoom').css('color', '#7f7f7f');
            } else {
                $(this).css('color', '#cfcfcf');
            };
        })
        $('body').on('click', '.span-zoom-out', function(){
            var idx = $(this).parent().find('.jc_detail').attr('id').split('_')[2];
            if (_jm[idx].view.zoomOut()) {
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