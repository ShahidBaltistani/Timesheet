﻿/*
 Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 For licensing, see LICENSE.md or https://ckeditor.com/legal/ckeditor-oss-license
*/
CKEDITOR.dialog.add("anchor",
    function(c) {
        function d(b, a) {
            return b.createFakeElement(b.document.createElement("a", { attributes: a }), "cke_anchor", "anchor");
        }

        return{
            title: c.lang.link.anchor.title,
            minWidth: 300,
            minHeight: 60,
            getModel: function(b) {
                var a = b.getSelection();
                b = a.getRanges()[0];
                a = a.getSelectedElement();
                b.shrink(CKEDITOR.SHRINK_ELEMENT);
                (a = b.getEnclosedNode()) && a.type === CKEDITOR.NODE_TEXT && (a = a.getParent());
                b = a && a.type === CKEDITOR.NODE_ELEMENT && ("anchor" === a.data("cke-real-element-type") || a.is("a"))
                    ? a
                    : void 0;
                return b || null;
            },
            onOk: function() {
                var b = CKEDITOR.tools.trim(this.getValueOf("info", "txtName")),
                    b = { id: b, name: b, "data-cke-saved-name": b },
                    a = this.getModel(c);
                a
                    ? a.data("cke-realelement")
                    ? (b = d(c, b), b.replace(a), CKEDITOR.env.ie && c.getSelection().selectElement(b))
                    : a.setAttributes(b)
                    : (a = (a = c.getSelection()) && a.getRanges()[0], a.collapsed
                        ? (b = d(c, b), a.insertNode(b))
                        : (CKEDITOR.env.ie && 9 > CKEDITOR.env.version && (b["class"] = "cke_anchor"), b =
                                new CKEDITOR.style({ element: "a", attributes: b }), b.type = CKEDITOR.STYLE_INLINE,
                            b.applyToRange(a)));
            },
            onShow: function() {
                var b = c.getSelection(), a = this.getModel(c), d = a && a.data("cke-realelement");
                if (a =
                    d ? CKEDITOR.plugins.link.tryRestoreFakeAnchor(c, a) : CKEDITOR.plugins.link.getSelectedLink(c)) {
                    var e = a.data("cke-saved-name");
                    this.setValueOf("info", "txtName", e || "");
                    !d && b.selectElement(a);
                }
                this.getContentElement("info", "txtName").focus();
            },
            contents: [
                {
                    id: "info",
                    label: c.lang.link.anchor.title,
                    accessKey: "I",
                    elements: [
                        {
                            type: "text",
                            id: "txtName",
                            label: c.lang.link.anchor.name,
                            required: !0,
                            validate: function() {
                                return this.getValue() ? !0 : (alert(c.lang.link.anchor.errorName), !1);
                            }
                        }
                    ]
                }
            ]
        };
    });