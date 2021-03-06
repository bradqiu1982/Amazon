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
            _jm.view.setZoom(0.6);
         })

         $('body').on('click', '#btn-node-submit', function () {
             var wfid = $('#HWFID').val();
             var nodeid = $('#HNODEID').val();
             $.post('/WorkFlow/WorkFlowMoveNext',
              {
                  wfid: wfid,
                  nodeid: nodeid
              }, function (output) {
                  window.location.href = '/WorkFlow/WorkFlowNodeDetail?wfid='+wfid+'&nodeid='+output.nodeid;
              })
         })

         //$('body').on('click', '#btn-node-startnext', function () {
         //    var wfid = $('#HWFID').val();
         //    var nodeids = JSON.stringify($('#nextnodes').val());
         //    $.post('/WorkFlow/ActiveMChildNode',
         //     {
         //         wfe_id: wfid,
         //         nodeid: nodeids
         //     }, function (output) {
         //         if (output.success)
         //         {
         //             window.location.href = '/WorkFlow/WorkFlowNodeDetail?wfid=' + wfid + '&nodeid=' + output.nodeid;
         //         }
         //     })
         //})

         $('body').on('click', '#btn-next-start', function () {
             if ($('#logicnext').val() == null || $('#logicnext').val() == '') {
                 alert('Please Choose Next Step');
                 return false;
             }
             
             var jumpnodeid = $('#logicnext option:selected').attr('data-id');
             var isrollback = $('#logicnext option:selected').attr('isrollback');
             var logiccommet = $('#logicroutecomment').html();
             if (isrollback == '1' && logiccommet == '')
             {
                 alert('Please input jump comment');
                 return false;
             }

            var wfid = $('#HWFID').val();
            var nodeid = $('#HNODEID').val();
            $.post('/WorkFlow/LogicJumpNext',
             {
                 wfe_id: wfid,
                 nodeid:nodeid,
                 jumpnodeid: jumpnodeid,
                 logiccommet: logiccommet
             }, function (output) {
                 if (output.success)
                 {
                     window.location.href = '/WorkFlow/WorkFlowNodeDetail?wfid=' + wfid + '&nodeid=' + output.nodeid;
                 }
             })
        })

         $('body').on('click', '.status-op', function () {
             var wf_id = $('#HWFID').val();
             var node_id = $(this).attr('data-node-id');
             var node_name = $(this).attr('data-node-name');

             if (confirm('you will start sub-flow ' + node_name))
             {
                 $.post('/WorkFlow/ActiveSChildNode',
                 {
                     wfid: wf_id,
                     nodeid: node_id
                 }, function (output) {
                     if (output.success) {
                         window.location.reload();
                     }
                     else {
                         alert('Failed Start');
                     }
                 })
             }
         })

         $('body').on('click', '.d-span-zoom-in', function () {
             var wfe_id = $(this).parent().find('.jc_detail').attr('data-id');
             if (_jm.view.zoomIn()) {
                 $('.span-zoom').css('color', '#7f7f7f');
             } else {
                 $(this).css('color', '#cfcfcf');
             };
         })
         $('body').on('click', '.span-zoom-out', function () {
             var wfe_id = $(this).parent().find('.jc_detail').attr('data-id');
             if (_jm.view.zoomOut()) {
                 $('.span-zoom').css('color', '#7f7f7f');
             } else {
                 $(this).css('color', '#cfcfcf');
             };
         })
    }
    return {
        init: function(){
            nd_show();
        }
    }
}();