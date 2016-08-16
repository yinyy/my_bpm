<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="WasherSetting.aspx.cs" Inherits="BPM.Admin.Washer.WasherSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../scripts/Colorpicker/spectrum.css" rel="stylesheet" />
    <link href="../scripts/fine-uploader/fine-uploader-new.min.css" rel="stylesheet" />
    <style type="text/css">
        div.c ul:first-child li label:first-child{
            text-align: right;
            width: 100px;

            display: inline-block;
        }

        div.c ul:first-child li img#Logo{
            width: 48px;
            height: 48px;

            border: 1px solid #ffffff;
        }

        .full-spectrum .sp-palette {
            max-width: 200px;
        }

        div.c ul:first-child li ul {
            display: inline-block;
            vertical-align: middle;
        }

        div.c ul:first-child li ul li label:first-child{
            width: 60px;
            text-align: right;
        }

        div.c ul:first-child li ul li input{
            width: 60px;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="sysconfig" style="margin:10px;">
        <h1>微信参数设置</h1>
        <div class="c">
            <ul>
                <li><label>appid：</label><input type="text" id="txt_Appid" name="Appid" style="width: 500px;"/></li>
                <li><label>secret：</label><input type="text" id="txt_Secret" name="Secret" style="width: 500px;"/></li>
                <li><label>aeskey：</label><input type="text" id="txt_Aeskey" name="Aeskey" style="width: 500px;"/></li>
                <li><label>Token：</label><input type="text" id="txt_Token" name="Token" style="width: 500px;"/></li>
           </ul>
        </div>
        <h1>短信接口参数设置</h1>
        <div class="c">
            <ul>
                <li><label>cid：</label><input type="text" id="txt_SmsCid" name="SmsCid" style="width: 500px;"/></li>
                <li><label>uid：</label><input type="text" id="txt_SmsUid" name="SmsUid" style="width: 500px;"/></li>
                <li><label>pas：</label><input type="text" id="txt_SmsPas" name="SmsPas" style="width: 500px;"/></li>
                <li><label>url：</label><input type="text" id="txt_SmsUrl" name="SmsUrl" style="width: 500px;"/></li>
           </ul>
        </div>
        <h1>品牌</h1>
        <div class="c">
            <ul>
                <li><label>名称：</label><input type="text" id="txt_Brand" name="Brand" style="width: 300px;"/></li>
                <li><label style="display: inline-block; vertical-align: middle;">商标：</label><img id="Logo" src="/images/PublicPlatform/default_logo.png" alt="Logo" style="display: inline-block; vertical-align: middle;"/><input type="hidden" id="txt_Logo" name="Logo"/><a href="#" id="Upload" class="alertify-button alertify-button-ok" style="margin: 0px 0px 0px 15px; display: inline-block; vertical-align: middle;">上传</a><label style="display: inline-block; vertical-align: middle;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;建议使用48x48像素的png图片</label></li>
           </ul>
        </div>
        <h1>洗车卡</h1>
        <div class="c">
            <ul>
                <li>
                    <label>购买洗车卡：</label>
                    <ul>
                        <li><label>50元&nbsp;卖&nbsp;</label><input type="text" id="txt_Card50" name="Card50"/><label>&nbsp;元，有效期&nbsp;</label><input type="text" id="txt_Day50" name="Day50"/><label>&nbsp;天，送&nbsp;</label><input type="text" id="txt_Score50" /><label>&nbsp;积分。</label></li>
                        <li><label>100元&nbsp;卖&nbsp;</label><input type="text" id="txt_Card100" name="Card100"/><label>&nbsp;元，有效期&nbsp;</label><input type="text" id="txt_Day100" name="Day100"/><label>&nbsp;天，送&nbsp;</label><input type="text" id="txt_Score100" /><label>&nbsp;积分。</label></li>
                        <li><label>200元&nbsp;卖&nbsp;</label><input type="text" id="txt_Card200" name="Card200"/><label>&nbsp;元，有效期&nbsp;</label><input type="text" id="txt_Day200" name="Day200"/><label>&nbsp;天，送&nbsp;</label><input type="text" id="txt_Score200" /><label>&nbsp;积分。</label></li>
                        <li><label>300元&nbsp;卖&nbsp;</label><input type="text" id="txt_Card300" name="Card300"/><label>&nbsp;元，有效期&nbsp;</label><input type="text" id="txt_Day300" name="Day300"/><label>&nbsp;天，送&nbsp;</label><input type="text" id="txt_Score300" /><label>&nbsp;积分。</label></li>
                    </ul>
                </li>
           </ul>
        </div>
        <h1>洗车币</h1>
        <div class="c">
            <ul>
                <li>
                    <label>兑换：</label>
                    <ul>
                        <li><label>1元=&nbsp;</label><input type="text" id="txt_Exchange" name="Exchange"/><label>&nbsp;&nbsp;洗车币</label></li>
                    </ul>
                </li>
                <li>
                    <label>充值送币：</label>
                    <ul>
                        <li><label>50元&nbsp;&nbsp;</label><input type="text" id="txt_Coin50" name="Coin50"/><label>&nbsp;&nbsp;洗车币</label></li>
                        <li><label>100元&nbsp;&nbsp;</label><input type="text" id="txt_Coin100" name="Coin100"/><label>&nbsp;&nbsp;洗车币</label></li>
                        <li><label>200元&nbsp;&nbsp;</label><input type="text" id="txt_Coin200" name="Coin200"/><label>&nbsp;&nbsp;洗车币</label></li>
                    </ul>
                </li>
           </ul>
        </div>
        <h1>优惠券</h1>
        <div class="c">
            <ul>
                <li>
                    <label>注册个人信息：</label>
                    <ul>
                        <li><label>送&nbsp;&nbsp;</label><input type="text" id="txt_Coupon" name="Coupon"/><label>&nbsp;&nbsp;洗车币的优惠价，有效期&nbsp;&nbsp;</label><input type="text" id="txt_CouponTime" name="CouponTime" />&nbsp;&nbsp;天。</li>
                    </ul>
                </li>
           </ul>
        </div>
        <h1>积分</h1>
        <div class="c">
            <ul>                
                <li>
                    <label>洗车：</label>
                    <ul>
                        <li><label>&nbsp;&nbsp;</label><input type="text" id="txt_WashCar" name="WashCar"/><label>&nbsp;&nbsp;积分</label></li>
                    </ul>
                </li>
                <li>
                    <label>注册个人信息：</label>
                    <ul>
                        <li><label>&nbsp;&nbsp;</label><input type="text" id="txt_Subscribe" name="Subscribe"/><label>&nbsp;&nbsp;积分</label></li>
                    </ul>
                </li>
                <li>
                    <label>充值：</label>
                    <ul>
                        <li><label>50元&nbsp;&nbsp;</label><input type="text" id="txt_Point50" name="Point50"/><label>&nbsp;&nbsp;积分</label></li>
                        <li><label>100元&nbsp;&nbsp;</label><input type="text" id="txt_Point100" name="Point100"/><label>&nbsp;&nbsp;积分</label></li>
                        <li><label>200元&nbsp;&nbsp;</label><input type="text" id="txt_Point200" name="Point200"/><label>&nbsp;&nbsp;积分</label></li>
                    </ul>
                </li>
                <li>
                    <label>推荐奖励：</label>
                    <ul style="width: 210px;">
                        <li><label>一级&nbsp;&nbsp;</label><input type="text" id="txt_Level1" name="Level1"/><label>&nbsp;&nbsp;积分</label></li>
                        <li><label>二级&nbsp;&nbsp;</label><input type="text" id="txt_Level2" name="Level2"/><label>&nbsp;&nbsp;积分</label></li>
                        <li><label>三级&nbsp;&nbsp;</label><input type="text" id="txt_Level3" name="Level3"/><label>&nbsp;&nbsp;积分</label></li>
                        <li><label>四级&nbsp;&nbsp;</label><input type="text" id="txt_Level4" name="Level4"/><label>&nbsp;&nbsp;积分</label></li>
                        <li><label>五级&nbsp;&nbsp;</label><input type="text" id="txt_Level5" name="Level5"/><label>&nbsp;&nbsp;积分</label></li>
                    </ul>
                    <ul>
                        <li><input name="Point_Kind" id="rb_Kind_Point" type="radio" style="width: auto; margin-left: 20px;" checked="checked"/><label for="rb_Kind_Point" style="text-align: left;">送积分</label></li>
                        <li><input name="Point_Kind" id="rb_Kind_Percent" type="radio" style="width: auto; margin-left: 20px;" /><label for="rb_Kind_Percent" style="text-align: left;">送充值金额的百分比</label></li>
                    </ul>
                </li>
            </ul>
        </div>
        <!--h1>会员卡</!--h1>
        <div class="c">
            <ul>
                <li><label style="display:block; float:left;">背景色：</label><input type="text" id="txt_Color" name="Color"/></li>
                <li style="margin-top: 10px;"><label style="vertical-align: middle; display: inline-block;">用卡须知：</label><textarea id="txt_Intro" name="Intro" cols="100" rows="30" style="vertical-align: middle;"></textarea></li>
           </ul>
        </div-->
    </div>


    <div style="margin:140px;width:160px; margin-top:40px; font-family:'Microsoft YaHei'">
        <a id="btnok" href="javascript:;" class="alertify-button alertify-button-ok">保存设置</a>
        <%--<a id="btnmenu" href="javascript:;" class="alertify-button alertify-button-ok">创建菜单</a>--%>
    </div>

    <div id="fine-uploader" style="display: none;">
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


    
    <script src="ashx/WasherSettingHandler.ashx?action=js" type="text/javascript"></script>
    <!-- 引入js文件 -->
    <script src="../scripts/Colorpicker/spectrum.js?d=<%=DateTime.Now %>"></script>
    <script src="../scripts/fine-uploader/fine-uploader.min.js"></script>
    <script src="js/WasherSetting.js?d=<%=DateTime.Now %>"></script>
    <script src="../Editor/xhEditor/xheditor-1.2.1.min.js"></script>
    <script src="../Editor/xhEditor/xheditor_lang/zh-cn.js"></script>
</asp:Content>


