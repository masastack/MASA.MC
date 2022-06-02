export function uploadImage(quillElement, element, ossParamter, index = 0) {
    let _that = this;
    let quill = quillElement.__quill;//get quill editor
    let fileInput = element.querySelector('input.ql-image[type=file]')//get fileInput
    let files = fileInput.files;

    ossUpload(files[index], ossParamter).then(fileUrl => {
        let length = quill.getSelection().index;
        quill.insertEmbed(length, 'image', fileUrl);//Insert the uploaded picture into the editor
        quill.setSelection(length + 1);
        index += 1;
        if (index < files.length) {
            _that.uploadImage(quillElement, element, ossParamter, index);
        }
        else {
            fileInput.value = '';
        }
    });
}