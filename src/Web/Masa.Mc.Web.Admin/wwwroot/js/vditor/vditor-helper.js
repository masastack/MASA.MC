const defaultOptions = {
    tab: '\t',
    mode: 'sv',
    preview: {
        actions: []
    },
    outline: true,
    cache: {
        enable: false,
    },
    icon: 'material'
}


export function init(domRef, obj, value, options, isUploadHandler) {
    let vditorOptions = {
        ...defaultOptions,
        ...options,
    }
    if (vditorOptions.hasOwnProperty('toolbar')) {
        vditorOptions.toolbar.forEach(btn => {
            if (typeof btn == 'object') {
                btn.click = () => {
                    obj.invokeMethodAsync('HandleToolbarButtonClickAsync', btn.name);
                }
            }
        })
    }
    if (isUploadHandler) {
        vditorOptions.upload = {
            handler: (files) => {
                obj.invokeMethodAsync('HandleFileChanged');
            }
        }
    }
    SetDefaultFileNameHandle(vditorOptions);
    domRef.Vditor = new Vditor(domRef, {
        ...vditorOptions,
        value,
        after: () => {
            obj.invokeMethodAsync('HandleRenderedAsync', value);
        },
        input: (value) => {
            obj.invokeMethodAsync('HandleInputAsync', value);
        },
        focus: (value) => {
            obj.invokeMethodAsync('HandleFocusAsync', value);
        },
        blur: (value) => {
            obj.invokeMethodAsync('HandleBlurAsync', value);
        },
        esc: (value) => {
            obj.invokeMethodAsync('HandleEscPressAsync', value);
        },
        ctrlEnter: (value) => {
            obj.invokeMethodAsync('HandleCtrlEnterPressAsync', value);
        },
        select: (value) => {
            obj.invokeMethodAsync('HandleSelectAsync', value);
        },
    })
}
export function getValue(domRef) {
    return domRef.Vditor.getValue();
}
export function getHtml(domRef) {
    return domRef.Vditor.getHTML();
}
export function setValue(domRef, value, clearStack = false) {
    domRef.Vditor.setValue(value, clearStack);
}
export function insertValue(domRef, value, render = true) {
    domRef.Vditor.insertValue(value, render);
}
export function destroy(domRef) {
    domRef.Vditor.destroy();
}
export function disabled(domRef) {
    domRef.Vditor.disabled();
    let obj = domRef.querySelector('[data-type=preview]');
    obj.click();
}
export function enable(domRef) {
    domRef.Vditor.enable();
}
function SetDefaultFileNameHandle(vditorOptions) {
    let { upload } = vditorOptions;
    if (upload) upload.filename || (upload.filename = (name) => name);
}


export function upload(element, ossParamter, index = 0) {
    let _that = this;
    let vditor = element.Vditor;
    let fileInput = element.querySelector('input[type=file]')
    let files = fileInput.files;

    ossUpload(files[index], ossParamter).then(fileUrl => {
        let succFileText = "";
        if (vditor && vditor.vditor.currentMode === "wysiwyg") {
            succFileText += `\n <img alt=${fileUrl} src="${fileUrl}">`;
        } else {
            succFileText += `\n![${fileUrl}](${fileUrl})`;
        }
        document.execCommand("insertHTML", false, succFileText);
        index += 1;
        if (index < files.length) {
            _that.upload(element, ossParamter, index);
        }
        else {
            fileInput.value = '';
        }
    });
}