<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ExamExam.aspx.cs" Inherits="BPM.Admin.Exam.ExamExam" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="layout">
        <div region="west" iconCls="icon-chart_organisation" split="true" style="width:450px; overflow: hidden;" collapsible="false">
            <div id="examGridToolbar" class="toolbar" style="background:#efefef;border-bottom:1px solid #ccc">
                <a id="a_addExam" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-add" title="添加考试信息">添加</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_editExam" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-edit" title="修改考试信息">修改</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_delExam" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-delete" title="删除考试信息">删除</a>
                <div class="datagrid-btn-separator"></div>
                <%= base.BuildToolbar()%>
            </div>
            <table id="examGrid"></table>
        </div>
        <div region="center" iconCls="icon-users" style="overflow: hidden">
            <div id="examSectionGridToolbar" class="toolbar" style="background:#efefef;border-bottom:1px solid #ccc">
                <a id="a_addExamSection" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-add" title="添加考试场次信息">添加</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_editExamSection" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-edit" title="修改考试场次信息">修改</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_delExamSection" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-delete" title="删除考试场次信息">删除</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_autoArrange" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-database_copy" title="自动安排监考">自动安排监考</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_fillArrange" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-database_copy" title="自动补全监考">自动补全监考</a>
            </div>
            <table id="examSectionGrid"></table>
        </div>
        <div region="east" iconCls="icon-users"split="true" style="width:700px; overflow: hidden;" collapsible="false">
            <div id="examSectionItemGridToolbar" class="toolbar" style="background:#efefef;border-bottom:1px solid #ccc">
                <a id="a_addExamSectionItem" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-add" title="添加考场信息">添加</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_editExamSectionItem" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-edit" title="修改考场信息">修改</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_delExamSectionItem" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-delete" title="删除考场信息">删除</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_importExamSectionItem" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-database_copy" title="导入考场信息">导入</a>
                <div class="datagrid-btn-separator"></div>
                <a id="a_manualArrange" style="float:left" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-database_copy" title="手动安排监考">手动安排监考</a>
            </div>
            <table id="examSectionItemGrid"></table>
        </div>
    </div>
    
    <script type="text/javascript" src="js/ExamExam.js?v=<%=DateTime.Now.Ticks %>"></script>
</asp:Content>
