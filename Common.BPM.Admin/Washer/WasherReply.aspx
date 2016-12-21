<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="WasherReply.aspx.cs" Inherits="BPM.Admin.Washer.WasherReply" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../scripts/fine-uploader/fine-uploader.min.css" rel="stylesheet" />

    <style type="text/css">
        div.setting{
            width: 400px; 
            margin: 20px auto;
        }

        div.setting div.big{
            position: relative;
            height: 200px;
        }

        div.setting div.big img{
            width: 400px;
            height: 200px;
        }

        div.setting div.big div.title{
            width: 380px; 
            background-color: rgba(0,0,0,0.2);
            line-height: 24px;
            position: absolute;
            bottom: 0px;
            color: #ffffff;
            padding: 10px 10px;
            font-family: 'Microsoft YaHei UI';
            font-size: 1.3em;
        }

        div.url, div.desc{
            display: none;
        }

        div.setting hr{
            border: none;
            border-top: 1px solid #cfcfcf;
        }

        div.setting div.little table{
            padding: 0px;
            margin: 0px;
            width: 400px;
            border-spacing: 0px;
            border-collapse: collapse;
            font-family: 'Microsoft YaHei UI';
            font-size: 1.3em;
        }

        div.setting div.little table img{
            width: 70px;
            height: 70px;
        }

        div.setting div.control{
            text-align: center;
            padding: 50px 0px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="fine-uploader" style="display:none">
    </div>


    <script type="text/template" id="qq-template">
        <div class="qq-uploader-selector qq-uploader" qq-drop-area-text="Drop files here">
            <div class="qq-total-progress-bar-container-selector qq-total-progress-bar-container">
                <div role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" class="qq-total-progress-bar-selector qq-progress-bar qq-total-progress-bar"></div>
            </div>
            <div class="qq-upload-drop-area-selector qq-upload-drop-area" qq-hide-dropzone>
                <span class="qq-upload-drop-area-text-selector"></span>
            </div>
            <div class="qq-upload-button-selector qq-upload-button">
                <div>Upload a file</div>
            </div>
            <span class="qq-drop-processing-selector qq-drop-processing">
                <span>Processing dropped files...</span>
                <span class="qq-drop-processing-spinner-selector qq-drop-processing-spinner"></span>
            </span>
            <ul class="qq-upload-list-selector qq-upload-list" aria-live="polite" aria-relevant="additions removals">
                <li>
                    <div class="qq-progress-bar-container-selector">
                        <div role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" class="qq-progress-bar-selector qq-progress-bar"></div>
                    </div>
                    <span class="qq-upload-spinner-selector qq-upload-spinner"></span>
                    <img class="qq-thumbnail-selector" qq-max-size="100" qq-server-scale>
                    <span class="qq-upload-file-selector qq-upload-file"></span>
                    <span class="qq-edit-filename-icon-selector qq-edit-filename-icon" aria-label="Edit filename"></span>
                    <input class="qq-edit-filename-selector qq-edit-filename" tabindex="0" type="text">
                    <span class="qq-upload-size-selector qq-upload-size"></span>
                    <button type="button" class="qq-btn qq-upload-cancel-selector qq-upload-cancel">Cancel</button>
                    <button type="button" class="qq-btn qq-upload-retry-selector qq-upload-retry">Retry</button>
                    <button type="button" class="qq-btn qq-upload-delete-selector qq-upload-delete">Delete</button>
                    <span role="status" class="qq-upload-status-text-selector qq-upload-status-text"></span>
                </li>
            </ul>

            <dialog class="qq-alert-dialog-selector">
                <div class="qq-dialog-message-selector"></div>
                <div class="qq-dialog-buttons">
                    <button type="button" class="qq-cancel-button-selector">Close</button>
                </div>
            </dialog>

            <dialog class="qq-confirm-dialog-selector">
                <div class="qq-dialog-message-selector"></div>
                <div class="qq-dialog-buttons">
                    <button type="button" class="qq-cancel-button-selector">No</button>
                    <button type="button" class="qq-ok-button-selector">Yes</button>
                </div>
            </dialog>

            <dialog class="qq-prompt-dialog-selector">
                <div class="qq-dialog-message-selector"></div>
                <input type="text">
                <div class="qq-dialog-buttons">
                    <button type="button" class="qq-cancel-button-selector">Cancel</button>
                    <button type="button" class="qq-ok-button-selector">Ok</button>
                </div>
            </dialog>
        </div>
    </script>

    <div id="tabs">  
        <div title="扫码洗车图文消息" data-options="iconCls: 'icon-arrow_left', closable: false" style="padding:20px;">  
               
        </div>
        <div title="关注图文消息" data-options="iconCls: 'icon-eye', closable:false" style="padding:20px;">  
               
        </div>  
        <div title="其它图文消息" data-options="iconCls:'icon-reload',closable:false" style="padding:20px;">  
               
        </div>  
    </div>  

    <script src="../scripts/fine-uploader/fine-uploader.min.js"></script>
    <!-- 引入js文件 -->
    <script src="js/WasherReply.js?d=<%=DateTime.Now %>"></script>
</asp:Content>
