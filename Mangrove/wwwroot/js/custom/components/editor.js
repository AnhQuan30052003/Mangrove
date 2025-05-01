function editWithCkEditor(idEditor) {
    const editor = document.querySelector(idEditor);
    if (editor) {
        ClassicEditor
            .create(editor, {
                ckfinder: {
                    uploadUrl: '/SettingWebsite/Upload'
                },
                image: {
                    toolbar: [
                        'imageTextAlternative',
                        'imageStyleFull',
                        'imageStyleSide',
                        'toggleImageCaption',
                    ],
                },
                toolbar: {
                    items: [
                        'undo', 'redo', '|',
                        'heading', '|',
                        'bold', 'italic', '|',
                        'link', 'imageUpload', 'insertTable', '|',
                        'alignment',
                        'blockQuote', 'numberedList', 'bulletedList', 'outdent', 'indent'
                    ]
                },
            })
    }
}

editWithCkEditor("#InforVi");
editWithCkEditor("#InforEn");