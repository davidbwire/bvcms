﻿$(function() {
    $('#Name').focus();
    $("#search").click(function() {
        $.getTable();
        return false;
    });
    $.gotoPage = function(e, pg) {
        $("#Page").val(pg);
        return $.getTable();
    }
    $.setPageSize = function(e) {
        $('#Page').val(1);
        $("#PageSize").val($(e).val());
        return $.getTable();
    }
    $.getTable = function() {
        var f = $('#results').closest('form');
        var q = f.serialize();
        $.post($('#search').attr('href'), q, function(ret) {
            $('#results').html(ret).ready(function() {
                $('.addrcol').cluetip({
                    splitTitle: '|',
                    hoverIntent: {
                        sensitivity: 3,
                        interval: 50,
                        timeout: 0
                    }
                });
                $('#results > tbody > tr:even').addClass('alt');
            });
        });
        return false;
    }
    $('#results > thead a.sortable').live('click', function() {
        var newsort = $(this).text();
        var sort = $("#Sort");
        var dir = $("#Direction");
        if ($(sort).val() == newsort && $(dir).val() == 'asc')
            $(dir).val('desc');
        else
            $(dir).val('asc');
        $(sort).val(newsort);
        $.getTable();
        return false;
    });
    $('a.clear').click(function() {
        var f = $(this).closest('form');
        $(f).find(':input').each(function() {
            $(this).val('');
        });
        $(f).find('select').each(function() {
            $(this).val("0");
        });
        $('#DivisionId').html('<option value="0">(select a program)</option>');
        return $.getTable();
    });
    $.maxZIndex = $.fn.maxZIndex = function(opt) {
        var def = { inc: 10, group: "*" };
        $.extend(def, opt);
        var zmax = 0;
        $(def.group).each(function() {
            var cur = parseInt($(this).css('z-index'));
            zmax = cur > zmax ? cur : zmax;
        });
        if (!this.jquery)
            return zmax;
        return this.each(function() {
            zmax += def.inc;
            $(this).css("z-index", zmax);
        });
    }
    $(".datepicker").datepicker({
        showOn: 'button',
        buttonImageOnly: true,
        buttonImage: '/Content/images/calendar.gif',
        dateFormat: 'm/d/yy',
        beforeShow: function() { $('#ui-datepicker-div').maxZIndex(); },
        changeMonth: true,
        changeYear: true
    });
    $('#ProgramId').change(function() {
        $.post('/OrgSearch/DivisionIds/' + $('#ProgramId').val(), null, function(ret) {
            $('#DivisionId').replaceWith(ret);
        });
    });
    $("form input").live("keypress", function(e) {
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            $('a.default').click();
            return false;
        }
        return true;
    });
    $('div.dialog').dialog({ autoOpen: false });
    $('#Rollsheet').click(function() {
        $.post('/OrgSearch/DefaultMeetingDate/' + $('#ScheduleId').val(), null, function(ret) {
            $('#MeetingDate').val(ret.date);
            $('#MeetingTime').val(ret.time);
            $('#PanelRollsheet').dialog('open');
        }, "json");
        return false;
    });
    $('#AttDetail').click(function() {
        $('#PanelAttDetail').dialog('open');
        return false;
    });
    $('#Rollsheet').click(function() {
        $('div.dialog').dialog('close');
        var did = $('#DivisionId').val();
        if (did == '0') {
            alert('must choose division');
            return false;
        }
        var args = "?div=" + did +
               "&schedule=" + $('#ScheduleId').val() +
               "&name=" + $('#Name').val() +
               "&dt=" + $('#MeetingDate').val() + " " + $('#MeetingTime').val();
        window.open("/Reports/Rollsheet/" + args);
    });
    $('#ExportExcel').click(function() {
        $('div.dialog').dialog('close');
        var args = "?prog=" + $('#ProgramId').val() +
               "&div=" + $('#DivisionId').val() +
               "&schedule=" + $('#ScheduleId').val() +
               "&status=" + $('#StatusId').val() +
               "&campus=" + $('#CampusId').val() +
               "&name=" + $('#Name').val();
        window.open("/OrgSearch/ExportExcel/" + args);
    });
    $('#Meetings').click(function() {
        $('div.dialog').dialog('close');
        var args = "?progid=" + $('#ProgramId').val() +
               "&divid=" + $('#DivisionId').val() +
               "&schedid=" + $('#ScheduleId').val() +
               "&campusid=" + $('#CampusId').val() +
               "&name=" + $('#Name').val();
        window.open("/Meetings.aspx" + args);
    });
    $('#AttDetail').click(function() {
        $('div.dialog').dialog('close');
        var did = $('#DivisionId').val();
        if (did == '0') {
            alert('must choose division');
            return false;
        }
        var args = "?divid=" + did +
               "&schedid=" + $('#ScheduleId').val() +
               "&name=" + $('#Name').val() +
               "&dt1=" + $('#MeetingDate1').val() +
               "&dt2=" + $('#MeetingDate2').val();
        window.open("/Report/AttendanceDetail.aspx" + args);
    });
    $('#Roster').click(function() {
        var did = $('#DivisionId').val();
        if (did == '0') {
            alert('must choose division');
            return false;
        }
        var args = "?div=" + did + "&schedule=" + $('#ScheduleId').val();
        window.open("/Reports/Roster/" + args);
    });
    $('a.ViewReport').click(function() {
        var did = $('#DivisionId').val();
        if (did == '0') {
            alert('must choose division');
            return false;
        }
        var args = "?div=" + did +
            "&schedule=" + $('#ScheduleId').val() +
            "&name=" + $('#Name').val();
        window.open($(this).attr("href") + args);
    });
});

function ToggleTagCallback(e) {
    if (e == "")
        return;
    var result = eval('(' + e + ')');
    $('#' + result.ControlId).text(result.HasTag ? "Remove" : "Add");
}
