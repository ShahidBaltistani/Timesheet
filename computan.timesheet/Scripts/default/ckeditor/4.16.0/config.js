/**
 * @license Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function(config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    //config.uiColor = '#F0B95E';
    config.removePlugins = "easyimage, cloudservices";
    config.toolbar = [
        {
            items: [
                "Source", "Save", "NewPage", "ExportPdf", "Preview", "Print", "Templates", "Cut",
                "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Undo", "Redo", "Find", "Replace",
                "SelectAll", "-", "Scayt", "Form", "Link", "Unlink", "Anchor", "Maximize", "ShowBlocks", "-",
                "BidiLtr", "BidiRtl", "Language", "CreateDiv", "Blockquote", "Bold", "Italic", "Underline",
                "Strike", "Subscript", "Superscript", "TextColor", "BGColor", "-", "CopyFormatting", "RemoveFormat",
                "NumberedList", "BulletedList", "-", "Outdent", "Indent", "JustifyLeft", "JustifyCenter",
                "JustifyRight", "JustifyBlock", "base64image", "Flash", "Table", "HorizontalRule",
                "Smiley", "SpecialChar", "PageBreak", "Iframe", "Styles", "Format", "Font", "FontSize"
            ]
        }
    ];

    //config.toolbar = [
    //	{ name: 'document', items: ['Source', '-', 'Save', 'NewPage', 'ExportPdf', 'Preview', 'Print', '-', 'Templates'] },
    //	{ name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
    //	{ name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'Scayt'] },
    //	{ name: 'forms', items: ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton', 'HiddenField'] },
    //	'/',
    //	{ name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'CopyFormatting', 'RemoveFormat'] },
    //	{ name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl', 'Language'] },
    //	{ name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
    //	{ name: 'insert', items: ['EasyImageUpload', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Iframe'] },
    //	'/',
    //	{ name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
    //	{ name: 'colors', items: ['TextColor', 'BGColor'] },
    //	{ name: 'tools', items: ['Maximize', 'ShowBlocks'] },
    //	{ name: 'about', items: ['About'] }
    //];
};