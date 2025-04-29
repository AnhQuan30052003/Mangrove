function EditWithCkEditor(idEditor) {
	const editor = document.querySelector(idEditor);
	if (editor) {
		ClassicEditor
		.create(editor, {
			ckfinder: {
				uploadUrl: '/CkEditor/Upload'
			},
			image: {
				toolbar: [
					'imageTextAlternative',
					'toggleImageCaption'
				],
			},
			toolbar: {
				items: [
					'undo', 'redo', '|',
					'heading', '|',
					'bold', 'italic', 'underline', '|',
					'link', 'imageUpload', 'insertTable', '|',
					'alignment',
					'blockQuote', 'numberedList', 'bulletedList', 'outdent', 'indent'
				]
			},
			alignment: {
				options: ['left', 'center', 'right', 'justify']
			}
		})
		.then(editor => {

		})
		.catch(error => {
			console.error(error);
		});
	}
}

EditWithCkEditor("#editor");