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