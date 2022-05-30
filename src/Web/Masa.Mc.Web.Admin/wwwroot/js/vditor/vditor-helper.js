export function upload(element, ossParamter, index = 0) {
    let _that = this;
    let vditor = element.Vditor;
    let fileInput = element.querySelector('input[type=file]')
    let files = fileInput.files;

    ossUploadImage(files[index], ossParamter).then(imageUrl => {
        let succFileText = "";
        if (vditor && vditor.vditor.currentMode === "wysiwyg") {
            succFileText += `\n <img alt=${imageUrl} src="${imageUrl}">`;
        } else {
            succFileText += `\n![${imageUrl}](${imageUrl})`;
        }
        console.log("ossUploadImage", imageUrl);
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