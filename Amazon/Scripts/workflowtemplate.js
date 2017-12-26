var WorkFlowTemplate = function () {
    var route_selected_color = "#7030A0";
    var wf_create = function (username) {
        var _jm = null;
        var _allsteps = [];
        var route_color = '#9545D0';
        var wfnodeComplete = function (type){
            $('#wf-node').autoComplete({
                minChars: 0,
                source: function (term, suggest) {
                    term = term.toLowerCase();
                    $.post('/WorkFlowTemplate/RegistedWorkFlowStepList',
                    {
                        type: type
                    }, function (output) {
                        var choices = output;

                        _allsteps = [];
                        $.each(output, function (i, value) {
                            _allsteps.push(value[0])
                        })

                        var suggestions = [];
                        for (i = 0; i < choices.length; i++) {
                            if (~(choices[i] + ' ' + choices[i]).toLowerCase().indexOf(term))
                                suggestions.push(choices[i]);
                        }
                        suggest(suggestions);
                    })
                },
                renderItem: function (item, search) {
                    search = search.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
                    var re = new RegExp("(" + search.split(' ').join('|') + ")", "gi");
                    return '<div class="autocomplete-suggestion" data-value="' + item[0] + '" data-type="' + item[1] + '"> ' + item[0].replace(re, "<b>$1</b>") + '</div>';
                },
                onSelect: function (e, term, item) {
                    $('#wf-node').val(item.data('value'));
                    $('#wf-node').attr('data-value', item.data('value'));
                    $('#wf-node').attr('data-type', item.data('type'));
                    if (item.data('type') == 1) {
                        $('.route-list').removeClass('hidden');
                    }
                    else {
                        $('.route-list').addClass('hidden');
                    }
                }
            });
        }

        if (username == '') {
            open_empty();

            $('#wf-type').autoComplete({
                minChars: 0,
                source: function (term, suggest) {
                    term = term.toLowerCase();
                    $.post('/WorkFlowTemplate/RegistedWorkFlowStepTypeList', {}, function (output) {
                        var choices = output;
                        var suggestions = [];
                        for (i = 0; i < choices.length; i++) {
                            if (~(choices[i] + ' ' + choices[i]).toLowerCase().indexOf(term))
                                suggestions.push(choices[i]);
                        }
                        suggest(suggestions);
                    })
                },
                onSelect: function (e, term, item) {

                    var old_term = $('#wf-type').attr('old_data');
                    if (old_term != term) {
                        if (_jm.mind != null && _jm.mind.nodes != null) {
                            if (confirm("Warning: changing workflow template type will clean current workflow template!")) {
                                $('#wf-node').autoComplete('destroy');

                                var mind = {
                                    "format": "node_array",
                                    "data": []
                                }
                                _jm.show(mind);
                                $('#wf-node').val('');
                            }
                            else {
                                $('#wf-type').val(old_term);
                                return false;
                            }
                        }
                    }
                    $('#wf-type').attr('old_data', term);

                    wfnodeComplete(term);
                }
            });
        }
        else {
            var options = {
                container: 'jsmind_container',
                theme: 'greensea',
                editable: true,
                mode: 'side',
                view: {
                    hmargin: 10,
                    vmargin: 10,
                    line_width: 2,
                    line_color: '#555'
                },
                layout: {
                    hspace: 20,
                    vspace: 10,
                    pspace: 0
                },
            }

            $.post('/WorkFlowTemplate/CachedWorkflowTemplateByName',
             {
                 username: username
             }, function (output) {
                 if (output.success) {
                     $('#wf-type').val(output.workflowtype);
                     $('#nwf-name').val(output.workflowname);
                     
                     $('#wf-node').autoComplete('destroy');
                     wfnodeComplete(output.workflowtype);
                     
                     var mind = {
                         "format": "node_array",
                         "data": JSON.parse(output.data)
                     };
                     _jm = jsMind.show(options, mind);
                     _jm.view.setZoom(0.6);
                     if (output.route != null && output.route != '')
                     {
                         var routeobj = JSON.parse(output.route);
                         $.each(routeobj, function (i,value) {
                             var appendstring = '<span id="' + value.node_id + '" value="' + value.node_name + '">' + value.routelists + '</span>';
                             $(appendstring).appendTo($('#node-route-list'));
                         })
                     }

                 }
                 else {
                     alert(output.msg);
                 }
             })
        }

        $('#m-route').autoComplete({
            minChars: 0,
            source: function (term, suggest) {
                term = term.toLowerCase();

                var choices = _allsteps;
                var suggestions = [];
                for (i = 0; i < choices.length; i++) {
                    if (~(choices[i]).toLowerCase().indexOf(term))
                        suggestions.push(choices[i]);
                }
                suggest(suggestions);

            },
            onSelect: function (e, term, item) {
                $('#m-route').val('');

                var currentroutestr = $('#m-selected-routes').html();
                if (currentroutestr == '') {
                    currentroutestr = term;
                }
                else {
                    var existarray = currentroutestr.split(';');
                    if (existarray.indexOf(term) == -1)
                    {
                        currentroutestr = currentroutestr + ";" + term;
                    }
                }
                $('#m-selected-routes').html(currentroutestr);
            }

        });

        $('body').on('click', '.m-btn-cancel', function () {
            $('#m-selected-routes').html('');
            $('#modal-route-list').modal('hide');
        })

        $('body').on('click', '.m-btn-add', function () {
            var node = $('wf-node').attr('data-value');

            var routelists = $('#m-selected-routes').html();
            if (routelists == null || routelists == '') {
                alert("Please select rollback/jump node");
                return false
            }
            $('#route-list').attr('data-node', node);
            $('#route-list').val(routelists);
            $('#modal-route-list').modal('hide');
        })

        //$('body').on('click', '#m-selected-routes option', function () {
        //    alert('come to here');
        //    $(this).remove();
        //})

        $('body').on('click', '#route-list', function () {
            $('#modal-route-list').modal('show');
        })
        
        $('body').on('click', 'jmnode', function (event) {
            var node_id = $(this).attr('nodeid');
            $('.r-list-group').empty();
            $('#jmnode-panel').remove();
            if ($('#' + node_id).length > 0) {
                $(this).css('background-color', route_selected_color);
                var routelists = $('#' + node_id).html().split(';');
                var appendStrtmp = "";
                $.each(routelists, function (i, info) {
                    appendStrtmp += '<li class="list-group-item">' + info + '</li > ';
                })
                var appendStr = '<div id="jmnode-panel" class="panel panel-info"> ' +
                        '<div class="triangle_border_up" ><span></span></div>' +
                        '<div class="panel-heading c-panel-headding">RouteLists</div> ' +
                        '<ul class="list-group r-list-group r-list-group-l">' + appendStrtmp +'</ul>' +
                    '</div>';
                $(appendStr).appendTo($('#jsmind-content'));
                $('#jmnode-panel').attr('style', 'left: ' + (event.clientX - 50) + 'px; top: ' + (event.clientY + document.documentElement.scrollTop + 20) + 'px;');
            }
        })

        $('body').click(function (event) {
            var target = $(event.target);
            if (target.closest("#jmnode-panel").length == 0 && target.closest("jmnode").length == 0) {
                $("#jmnode-panel").hide();
            }
            else {
                $("#jmnode-panel").show();
            }
        })

        $('body').on('click', '#add-wf-node', function(){
            var node_id = jsMind.util.uuid.newid();
            var node_val = $('#wf-node').val();
            if(node_val == ""){
                alert("Please select one step");
                return false;
            }

            if (_allsteps.indexOf(node_val) == -1)
            {
                alert('Step ' + node_val + ' is not exist in step list');
                $('#wf-node').val('');
                return false;
            }

            if (_jm.mind == null || _jm.mind.root == null) {

                var node_type = $('#wf-node').attr('data-type');
                if (node_type == '1') {
                    var node_routelist = $('#route-list').val();
                    if (node_routelist == '') {
                        alert("Please set route");
                        return false;
                    }
                    else {
                        var appendStr = '<span id="' + node_id + '" value="' + node_val + '">' + $('#route-list').val() + '</span>';
                        $(appendStr).appendTo($('#node-route-list'));
                        $('#m-selected-routes').html('');
                        $('#route-list').val('');
                    }
                }

                var mind = {
                    "format":"node_array",
                    "data":[
                        { "id": node_id, "isroot": true, "topic": node_val, "background_color": (node_type == "1") ? route_color :""}
                    ]
                };
                _jm.show(mind);
                _jm.select_node(node_id);
                $('#wf-node').val('');
            }
            else{
                var selected_node = _jm.get_selected_node();
                if(!selected_node){
                    alert('please select one step first.');
                    return;
                }
                if(selected_node.data['status'] != null && selected_node.data['status'] == 0){
                    return false;
                }

                var node_type = $('#wf-node').attr('data-type');
                if (node_type == '1') {
                    var node_routelist = $('#route-list').val();
                    if (node_routelist == '') {
                        alert("Please set route");
                        return false;
                    }
                    else {
                        var appendStr = '<span id="' + node_id + '" value="' + node_val + '">' + $('#route-list').val() + '</span>';
                        $(appendStr).appendTo($('#node-route-list'));
                        $('#m-selected-routes').html('');
                        $('#route-list').val('');
                    }
                }

                var node_before = [];
                node_before.push(selected_node);
                get_node_before(selected_node, node_before);
                var exist_flg = false;
                for(var i = 0; i < node_before.length; i++){
                    if(node_val == node_before[i].topic){
                        alert('Step ' + node_val + ' has already exist  in same sub-flow');
                        $('#wf-node').val('');
                        return false;
                    }
                }
                if(selected_node.children != null){
                    for(var i = 0; i< selected_node.children.length; i++){
                        if(node_val == selected_node.children[i].topic){
                            alert('Step ' + node_val + ' has already exist in its brother step');
                            $('#wf-node').val('');
                            return false;
                        }
                    }
                }

                _jm.add_node(selected_node, node_id, node_val, { "background_color": (node_type == "1") ? route_color : ""});
                _jm.select_node(node_id);
                $('#wf-node').val('');
            }
        })

        $('body').on('click', '#del-wf-node', function(){
            var selected_id = get_selected_nodeid();
            if(selected_id == _jm.mind.root.id){
                var mind = {
                    "format":"node_array",
                    "data":[]
                }
                _jm.show(mind);
                return false;
            }
            if(!selected_id){
                alert('please select one step first.');
                return;
            }
            if(confirm("If you delete this step, all of next steps will be delete, Please confirm to delete this step？")){
                _jm.remove_node(selected_id);
            }
        })

        $('body').on('click', '#save-nwf', function(){
             var edit_type = 'cache';
             get_jm_data(edit_type);

        })

        $('body').on('click', '#sumbit-nwf', function(){
            var edit_type = 'store';
            get_jm_data(edit_type);
        })

        $('body').on('click', '.span-zoom-in', function(){
            if(_jm.mind != null && _jm.mind.root != null){
                if (_jm.view.zoomIn()) {
                    $('.span-zoom').css('color', '#7f7f7f');
                } else {
                    $(this).css('color', '#cfcfcf');
                };
            }
        })

        $('body').on('click', '.span-zoom-out', function(){
            if(_jm.mind != null && _jm.mind.root != null){
                if (_jm.view.zoomOut()) {
                    $('.span-zoom').css('color', '#7f7f7f');
                } else {
                    $(this).css('color', '#cfcfcf');
                };
            }
        })

        function get_jm_data(edit_type){
            if(_jm.mind == null || _jm.mind.root == null){
                alert('Please add one step at least');
                return false;
            }
            var jm_data = _jm.mind.nodes;
            var wf_type = $.trim($('#wf-type').val());
            if(wf_type == ""){
                alert("Please select Workflow type");
                return false;
            }
            var wf_name = $.trim($('#nwf-name').val());
            if(wf_name == ''){
                alert('Please input your workflow name');
                return false;
            }
            var data = [];
            $.each(jm_data, function () {
                var str_tmp = {
                    id: this.id,
                    topic: this.topic,
                    isroot: this.isroot,
                    parentid: (this.parent != null) ? this.parent.id : "",
                    background_color: this.data.background_color
                };
                data.push(str_tmp);
            })

            var route = [];
            $.each($('#node-route-list span'), function () {
                var str_tmp = {
                    node_id: $(this).attr('id'),
                    node_name: $(this).attr('value'),
                    routelists: $(this).html()
                }
                route.push(str_tmp);
            })
            var wf_id = jsMind.util.uuid.newid();
            $.post('/WorkFlowTemplate/StortNewWorkFlow',
             {
                 edit_type:edit_type,
                 wf_id: wf_id,
                 wf_type: wf_type,
                 wf_name: wf_name,
                 data: JSON.stringify(data),
                 route: JSON.stringify(route)
             }, function(output){
                 if (output.success) {
                     if (edit_type == 'store') {
                         window.location.href = '/WorkFlowTemplate/AllWorkFlowTemplate';
                     }
                     else {
                         alert("Cache Workflow template successfully!");
                     }
                 }
                 else{
                     alert(output.msg);
                 }
             })
        }

        function get_selected_nodeid(){
            var selected_node = _jm.get_selected_node();
            if(!!selected_node){
                return selected_node.id;
            }else{
                return null;
            }
        }

        function get_new_node_id(new_node_id, node, node_before){
            var all_nodes = _jm.mind.nodes;
            var tmp_node;
            $.each(all_nodes, function(){
                if($.grep(node_before, function(e){ return e.id == this.id; }) == 0
                    && this.id == new_node_id){
                    new_node_id = node.id + '_' + new_node_id;
                    return false;
                }
            });
            return new_node_id;
        }

        function get_node_before(node, node_before){
            if(node.parent != null){
                if(node.parent.isroot != true){
                    node_before.push(node.parent);
                    get_node_before(node.parent, node_before)
                }
                else if(node.parent.isroot){
                    node_before.push(node.parent);
                }
            }
        }

        function open_empty(){
            var options = {
                container:'jsmind_container',
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
            _jm = jsMind.createJMObj(options);
        }

        function open_ajax(){
            var mind_url = 'data_example.json';
            jsMind.util.ajax.get(mind_url,function(mind){
                _jm.show(mind);
            });
        }
    }
    var wf_show = function(){
        var _jm = {};
        $.each($('.jsmind_container'), function(){
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
             var wf_id = $(this).attr('data-id');
             $.post('/WorkFlowTemplate/WorkFlowTemplateByID',
             {
                 wf_id: wf_id
             }, function (output) {
                 if (output.sucess)
                 {
                    var mind = {
                        "format":"node_array",
                        "data": JSON.parse(output.data)
                    };
                    var _jm_tmp = jsMind.show(options, mind);
                    _jm_tmp.view.setZoom(0.6);
                    _jm[wf_id] = _jm_tmp;
                 }
             })
        })

        $('body').on('change', '#wf-type', function(){
            var wf_type = $(this).val();
            //window.location.reload();
            window.location.href = '/WorkFlowTemplate/AllWorkFlowTemplate?&templatetype=' + wf_type;
        })

        ////edit workflow template name
        //$('body').on('click', '.edit-swf-op', function(){
        //    if($(this).val() == 'Edit'){
        //        $(this).parent().prev().find('span').eq(0).hide();
        //        $(this).parent().prev().find('input').removeClass('hidden');
        //        $(this).val('Save');
        //    }
        //    else{
        //        var nwf_name = $(this).parent().prev().find('input').val();
        //        var nwf_id = $(this).parent().attr('data-id');
        //        // $.post('/',
        //        // {
        //        //     nwf_name: nwf_name,
        //        //     nwf_id: nwf_id
        //        // }, function(output){
        //        //     if(output.success){
        //                $(this).parent().prev().find('span').eq(0).html(nwf_name).show();
        //                $(this).parent().prev().find('input').addClass('hidden');
        //                $(this).val('Edit');
        //        //     }
        //        // })
        //    }
        //})

        //delete workflow template
        $('body').on('click', '.del-swf-op', function () {
            if (confirm('Do you really want to remove this workflow template ?'))
            {
                var nwf_id = $(this).parent().attr('data-id');
                $.post('/WorkFlowTemplate/RemoveWorkFlowTemplateByID',
                 {
                     nwf_id: nwf_id
                 }, function(output){
                     if (output.sucess) {
                         window.location.href = '/WorkFlowTemplate/AllWorkFlowTemplate';
                     }
                 })
            }
        })
        $('body').on('click', '.span-zoom-in', function(){
            var idx = $(this).parent().find('.jc').attr('data-id');
            if (_jm[idx].view.zoomIn()) {
                $('.span-zoom').css('color', '#7f7f7f');
            } else {
                $(this).css('color', '#cfcfcf');
            };
        })
        $('body').on('click', '.span-zoom-out', function(){
            var idx = $(this).parent().find('.jc').attr('data-id');
            if (_jm[idx].view.zoomOut()) {
                $('.span-zoom').css('color', '#7f7f7f');
            } else {
                $(this).css('color', '#cfcfcf');
            };
        })
        $('body').on('click', 'jmnode', function (event) {
            var wft_id = $(this).parent().parent().parent().attr('data-id');
            var node_id = $(this).attr('nodeid');
            if ($('#hidden-nodeid').val() == node_id) {
                $('#jmnode-panel').attr('style', 'left: ' + (event.clientX - 50) + 'px; top: ' + (event.clientY + document.documentElement.scrollTop + 20) + 'px;');
                $('#jmnode-panel').show();
            }
            else {
                $.post('/WorkFlowTemplate/LogicRouteOfNode',
                {
                    wf_id: wft_id,
                    node_id: node_id
                }, function (output) {
                    $('.r-list-group').empty();
                    if (output.success) {
                        $('#jmnode-panel').remove();
                        $('#hidden-nodeid').val(node_id);
                        var routelists = output.data;
                        if (routelists.length > 0) {
                            $(this).css('background-color', route_selected_color);
                            var appendStrtmp = "";
                            $.each(routelists, function (i, info) {
                                var direction = info[1] == '1' ? 'backward' : 'forward';
                                var title = info[1] == '1' ? 'Rollback' : 'Jump';
                                appendStrtmp += '<li class="list-group-item">' +
                                        '<span class="route-type glyphicon glyphicon-' +
                                        direction + '" title="' + title + '" data-toggle="tooltip">' +
                                        '</span>' +
                                        '<span class="">' + info[0] + '</span>' +
                                    '</li > ';
                            })
                            var appendStr = '<div id="jmnode-panel" class="panel panel-info"> ' +
                                    '<div class="triangle_border_up" ><span></span></div>' +
                                    '<div class="panel-heading c-panel-headding">RouteLists</div> ' +
                                    '<ul class="list-group r-list-group r-list-group-l">' + appendStrtmp + '</ul>' +
                                '</div>';
                            $(appendStr).appendTo($('.wf-list'));
                            $('#jmnode-panel').attr('style', 'left: ' + (event.clientX - 50) + 'px; top: ' + (event.clientY + document.documentElement.scrollTop + 20) + 'px;');
                        }
                    }
                    else {
                        $('#jmnode-panel').remove();
                    }
                });
            }
        })
        $('body').click(function (event) {
            var target = $(event.target);
            if (target.closest("#jmnode-panel").length == 0 && target.closest("jmnode").length == 0) {
                $("#jmnode-panel").hide();
            }
            else {
                $("#jmnode-panel").show();
            }
        })
    }
    return {
        init: function(username){
            wf_create(username);
        },
        show: function(){
            wf_show();
        }
    }
}();