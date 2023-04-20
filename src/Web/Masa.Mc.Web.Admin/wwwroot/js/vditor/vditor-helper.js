export function upload(element, ossParamter, index = 0) {
    let _that = this;
    let vditor = element.Vditor;
    let fileInput = element.querySelector('input[type=file]')
    let files = fileInput.files;

    UploadImage([files[index]], ossParamter).then(fileUrls => {
        let fileUrl = fileUrls[0];
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